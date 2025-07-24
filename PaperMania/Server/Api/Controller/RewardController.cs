using Microsoft.AspNetCore.Mvc;
using Server.Api.Filter;
using Server.Application.Port;

namespace Server.Api.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<RewardController> _logger;

        public RewardController(IRewardService rewardService, ISessionService sessionService,ILogger<RewardController> logger)
        {
            _rewardService = rewardService;
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet("stage")]
        public async Task<IActionResult> GetStageReward(
            [FromQuery] int stageNum,
            [FromQuery] int stageSubNum)
        {
            _logger.LogInformation($"스테이지 보상 조회 시도");

            try
            {
                var reward = await _rewardService.GetStageRewardAsync(stageNum, stageSubNum);
                if (reward == null)
                    return NotFound("해당 스테이지 보상이 없습니다.");
                
                _logger.LogInformation($"스테이지 보상 조회 성공 : Id");
                return Ok(reward);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "스테이지 보상 조회 중 오류 발생");
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }
    }
}
