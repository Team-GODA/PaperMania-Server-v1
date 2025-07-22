using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Filter;
using Server.Application.Port;

namespace Server.Api.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SessionValidationFilter))]
    public class GoodsController : ControllerBase
    {
        private readonly IGoodsService _goodsService;
        private readonly ILogger<GoodsController> _logger;

        public GoodsController(IGoodsService goodsService, ILogger<GoodsController> logger)
        {
            _goodsService = goodsService;
            _logger = logger;
        }
        
        [HttpGet("action-point/{id}")]
        public async Task<IActionResult> GetPlayerCurrentActionPointById(
            [FromRoute(Name = "id")] int userId)
        {
            _logger.LogInformation($"플레이어 AP 조회 시도 : Id : {userId}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                var currentActionPoint = await _goodsService.GetPlayerActionPointAsync(userId, sessionId);
                
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

        [HttpPost("action-point/max")]
        public async Task<IActionResult> UpdatePlayerMaxActionPoint(
            [FromBody] UpdatePlayerMaxActionPointRequest request)
        {
            _logger.LogInformation($"플레이어 최대 AP 갱신 시도");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                var newMaxActionPoint = await _goodsService.UpdatePlayerMaxActionPoint(request.Id, request.NewMaxActionPoint, sessionId);
                
                _logger.LogInformation($"플레이어 최대 AP 갱신 성공 : Id : {request.Id}");
                return Ok(new
                {
                    newMaxActionPoint
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 최대 AP 갱신 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPost("action-point")]
        public async Task<IActionResult> UpdatePlayerActionPoint(
            [FromBody] UsePlayerActionPointRequest request)
        {
            _logger.LogInformation($"플레이어 AP 갱신 시도 : Id : {request.Id}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                await _goodsService.UsePlayerActionPointAsync(request.Id, request.UsedActionPoint, sessionId);
                var currentActionPoint = await _goodsService.GetPlayerActionPointAsync(request.Id, sessionId);

                _logger.LogInformation($"플레이어 AP 갱신 성공 : Id : {request.Id}");
                return Ok(new
                {
                    CurrentActionPoint = currentActionPoint
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 최대 AP 갱신 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
