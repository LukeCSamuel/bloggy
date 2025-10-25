using System.Security.Claims;
using Bloggy.Models;
using Bloggy.Models.Dto;

namespace Bloggy.Services
{
  public class CompletionService(CosmosService cosmos, ScriptRunnerService runner)
  {
    public async Task<ICompletionResponseDto> Create(CompletionRequestDto dto, ClaimsPrincipal user)
    {
      var completion = new Completion(Guid.NewGuid().ToString(), user)
      {
        eventId = dto.eventId,
        challengeId = dto.challengeId,
        time = DateTime.Now
      };

      // Check if the user already has a completion for this event
      var existing = await cosmos.GetAllAsync<Completion>(c =>
        c.ownerId == completion.ownerId
        && (c.challengeId == completion.challengeId || c.eventId == completion.eventId)
      );
      if (existing.FirstOrDefault() is Completion _existing)
      {
        return new SuccessfulCompletionDto()
        {
          name = "You already completed this!",
          pointsAwarded = 0,
          time = _existing.time,
        };
      }

      var @event = completion.eventId is not null ? await cosmos.GetByIdAsync<Event>(completion.eventId) : null;
      var challenge = completion.challengeId is not null ? await cosmos.GetByIdAsync<Challenge>(completion.challengeId) : null;

      // Validate the provided key
      if (@event is not null)
      {
        if (@event.key != dto.key)
        {
          return new IncompleteDto()
          {
            name = @event.name,
            isExpired = false,
          };
        }
      }
      else if (challenge is not null)
      {
        if (challenge.key != dto.key)
        {
          return new IncompleteDto()
          {
            name = challenge.name,
            isExpired = false,
          };
        }
      }
      else
      {
        return new IncompleteDto()
        {
          name = "Does not exist",
          isExpired = false,
        };
      }

      // Calculate number of points to award
      completion.pointsAwarded = @event is not null
          ? await ComputePoints(@event, (max, _, first) =>
          {
            if (first is DateTime _first)
            {
              var sinceFirst = (DateTime.UtcNow - TimeSpan.FromMinutes(3) - _first).TotalMilliseconds;
              var fullDuration = TimeSpan.FromHours(1).TotalMilliseconds;

              // Decay points for events exponentially over the course of 1 hour since 3 minutes after it was first completed
              return (int)(Math.Pow(Math.Clamp((fullDuration - sinceFirst) / fullDuration, 0, 1), 2) * max);
            }
            else
            {
              return max;
            }
          })
          : challenge is not null
          ? await ComputePoints(challenge, (max, count, _) =>
          {
            if (count == 0)
            {
              return max;
            }
            else
            {
              // Decay points for challenges exponentially over the number of times the challenge has been completed
              return (int)(Math.Pow(Math.Clamp((10.0 - count) / 10, 0, 1), 2) * max);
            }
          })
          : 0;

      Console.WriteLine($"Awarded points: {completion.pointsAwarded}");

      if (completion.pointsAwarded == 0)
      {
        return new IncompleteDto()
        {
          name = @event?.name ?? challenge?.name,
          isExpired = true,
        };
      }

      await cosmos.UpdateAsync(completion, user);
      _ = runner.CheckEventsAfterCompletion(completion);
      return new SuccessfulCompletionDto()
      {
        name = @event?.name ?? challenge?.name,
        time = completion.time,
        pointsAwarded = completion.pointsAwarded,
      };
    }

    private async Task<int> ComputePoints<T>(T model, Func<int, int, DateTime?, int> decayFunction) where T : CosmosModel, IAwardsPoints
    {
      var previousCompletions = typeof(T).Name switch
      {
        "Event" => await cosmos.GetAllAsync<Completion>(c => c.eventId == model.id),
        _ => await cosmos.GetAllAsync<Completion>(c => c.challengeId == model.id),
      };
      var first = previousCompletions.OrderBy(completion => completion.time).Select(completion => completion.time).FirstOrDefault();
      var count = previousCompletions.Count();
      var maxPoints = model.maxPoints;

      return decayFunction(maxPoints ?? 0, count, first);
    }
  }
}