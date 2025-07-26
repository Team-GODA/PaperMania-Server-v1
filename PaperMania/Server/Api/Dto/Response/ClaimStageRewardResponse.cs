using Server.Domain.Entity;

namespace Server.Api.Dto.Response;

public class ClaimStageRewardResponse
{
    public string Message  { get; set; }
    public int? Id   { get; set; }
    public StageReward? StageReward { get; set; }
}