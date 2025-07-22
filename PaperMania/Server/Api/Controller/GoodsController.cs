using Microsoft.AspNetCore.Mvc;
using Server.Application.Port;

namespace Server.Api.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<DataController> _logger;

        public GoodsController(IDataService dataService, ILogger<DataController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }
        
        private bool CheckSessionId(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                _logger.LogWarning("세션 Id가 없습니다.");
                return false;
            }
            return true;
        }
        
        [HttpGet("action-point/{id}")]
        public async Task<IActionResult> GetPlayerCurrentActionPointById(
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromRoute(Name = "id")] int userId)
        {
            _logger.LogInformation($"플레이어 AP 조회 시도 : Id : {userId}");
            
            if (!CheckSessionId(sessionId))
                return Unauthorized(new { message = "세션 ID가 없습니다." });

            try
            {
                var data = await _dataService.GetPlayerGoodsDataByUserIdAsync(userId, sessionId);
                var currentActionPoint = data.ActionPoint;

                _logger.LogInformation($"플레이어 AP 조회 성공 : Id : {userId}");
                return Ok(new
                {
                    currentActionPoint
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 AP 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
