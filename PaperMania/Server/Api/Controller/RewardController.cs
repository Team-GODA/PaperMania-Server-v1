using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Filter;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [ApiVersion("1.0")]
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
                
                _logger.LogInformation($"스테이지 보상 조회 성공 : StageNum = {stageNum}, StageSubNum = {stageSubNum}");
                return Ok(reward);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "스테이지 보상 조회 중 오류 발생");
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        [HttpPatch("stage")]
        [ServiceFilter(typeof(SessionValidationFilter))]
        public async Task<IActionResult> ClaimStageReward(
            [FromBody] ClaimStageRewardRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);
            
            _logger.LogInformation($"플레이어 스테이지 보상 수령 시도 : Id : {userId}");

            try
            {
                var stageData = new PlayerStageData
                {
                    Id = userId,
                    StageNum = request.StageNum,
                    SubStageNum = request.SubStageNum
                };
                
                var stageReward = await _rewardService.GetStageRewardAsync(request.StageNum, request.SubStageNum);
                if (stageReward == null)
                    return NotFound("해당 스테이지 보상이 없습니다.");
                
                await _rewardService.ClaimStageRewardByUserIdAsync(userId, stageReward, stageData);
                
                _logger.LogInformation($"플레이어 스테이지 보상 수령 성공 : Id : {userId}");

                return Ok(new
                {
                    message = "스테이지 보상 지급 완료",
                    id = userId,
                    stageReward = stageReward
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "스테이지 보상 수령 중 오류 발생");
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }
    }
}
