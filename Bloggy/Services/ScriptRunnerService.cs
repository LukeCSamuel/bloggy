using System.Text.Json;
using Bloggy.Models;

namespace Bloggy.Services
{
  public class ScriptRunnerService
  {
    private readonly CosmosService cosmos;
    private readonly bool isEnabled;
    public Task Job { get; private set; }

    public ScriptRunnerService(CosmosService cosmos, EnvService env)
    {
      this.cosmos = cosmos;
      isEnabled = env.AppEnvironment == AppEnvironment.Development;
      if (isEnabled)
      {
        Job = StartJob();
      }
      else
      {
        Job = Task.CompletedTask;
      }
    }

    public async Task StartJob(CancellationToken cancellationToken = default)
    {
      using PeriodicTimer timer = new(TimeSpan.FromMinutes(5));
      await Task.Delay(1, CancellationToken.None);
      while (true)
      {
        try
        {
          await CheckEventsAfterTick();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex);
        }
        await timer.WaitForNextTickAsync(cancellationToken);
      }
    }

    public async Task CheckEventsAfterCompletion(Completion completion)
    {
      if (!isEnabled)
      {
        return;
      }

      var now = DateTime.Now;
      var events = await cosmos.GetAllAsync<Event>();
      events = events.Where(ev => ev.hasTriggered is not true && ev.afterEventId == completion.eventId && (ev.afterTime is null || ev.afterTime < now));
      await TriggerEvents(events);
    }

    public async Task CheckEventsAfterTick()
    {
      if (!isEnabled)
      {
        return;
      }

      var now = DateTime.Now;
      var events = await cosmos.GetAllAsync<Event>();
      events = events.Where(ev => ev.hasTriggered is not true && (ev.afterTime is null || ev.afterTime < now));
      var qualifiedEvents = new List<Event>();
      foreach (var ev in events)
      {
        if (ev.afterEventId is null)
        {
          qualifiedEvents.Add(ev);
        }
        else
        {
          var completions = await cosmos.GetAllAsync<Completion>(c => c.eventId == ev.id);
          if (completions.Any())
          {
            qualifiedEvents.Add(ev);
          }
        }
      }
      await TriggerEvents(qualifiedEvents);
    }

    private async Task TriggerEvents(IEnumerable<Event> events)
    {
      Console.WriteLine($"Updating {events.Count()} events");

      foreach (var ev in events)
      {
        // Add trending post
        if (ev.trending is Post post)
        {
          post.created = DateTime.Now;
          post.isTrending = true;
          post.id = post.id is "" ? Guid.NewGuid().ToString() : post.id;

          await cosmos.UpdateAsync(post, force: true);
        }

        foreach (var entity in ev.entities ?? [])
        {
          switch (entity.kind)
          {
            case "Post":
              await HandleDataAsType<Post>(entity);
              break;
            case "User":
              await HandleDataAsType<User>(entity);
              break;
            case "Comment":
              await HandleDataAsType<Comment>(entity);
              break;
          }
        }

        ev.hasTriggered = true;
        await cosmos.UpdateAsync(ev);
      }
    }

    private async Task HandleDataAsType<T>(EventEntity entity) where T : CosmosModel
    {
      try
      {
        entity.data = entity.data?.Replace("%%NOW%%", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        if (entity.data is not null)
        {
          var model = JsonSerializer.Deserialize<T>(entity.data);
          if (model is not null)
          {
            await cosmos.UpdateAsync(model, force: true);
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error while processing entities.");
        Console.WriteLine(ex);
      }
    }
  }
}