namespace Bloggy.Models.Dto
{
  public class CompletionRequestDto
  {
    public string? eventId { get; set; }
    public string? challengeId { get; set; }
    public required string key { get; set; }
  }

  public interface ICompletionResponseDto
  {
    public string? name { get; set; }
    public int? pointsAwarded { get; }
  }

  public class SuccessfulCompletionDto : ICompletionResponseDto
  {
    public required string? name { get; set; }
    public required DateTime? time { get; set; }
    public required int? pointsAwarded { get; set; }
  }

  public class IncompleteDto : ICompletionResponseDto
  {
    public required string? name { get; set; }
    public required bool isExpired { get; set; }
    public int? pointsAwarded { get; set; } = 0;
  }
}