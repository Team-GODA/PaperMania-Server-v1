using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Filter;
using Server.Application.Port;

namespace Server.Api.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SessionValidationFilter))]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ICurrencyService currencyService, ILogger<CurrencyController> logger)
        {
            _currencyService = currencyService;
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
                var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(userId, sessionId);
                
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
                var newMaxActionPoint = await _currencyService.UpdatePlayerMaxActionPoint(request.Id, request.NewMaxActionPoint, sessionId);
                
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

        [HttpPatch("action-point")]
        public async Task<IActionResult> UsePlayerActionPoint(
            [FromBody] UsePlayerActionPointRequest request)
        {
            _logger.LogInformation($"플레이어 AP 사용 시도 : Id : {request.Id}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                await _currencyService.UsePlayerActionPointAsync(request.Id, request.UsedActionPoint, sessionId);
                var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(request.Id, sessionId);

                _logger.LogInformation($"플레이어 AP 사용 성공 : Id : {request.Id}");
                return Ok(new
                {
                    CurrentActionPoint = currentActionPoint
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 AP 사용 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpGet("gold/{id}")]
        public async Task<IActionResult> GetPlayerGold(
            [FromRoute(Name = "id")] int userId)
        {
            _logger.LogInformation($"플레이어 골드 조회 시도 : Id : {userId}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                var gold = await _currencyService.GetPlayerGoldAsync(userId, sessionId);
                
                _logger.LogInformation($"플레이어 골드 조회 성공 : Id {userId}");
                return Ok(new
                {
                    gold
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 골드 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPatch("gold/use")]
        public async Task<IActionResult> UsePlayerGold(
            [FromBody] UsePlayerGoldRequest request)
        {
            _logger.LogInformation($"플레이어 골드 사용 시도 : Id : {request.Id}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                await _currencyService.UsePlayerGoldAsync(request.Id, request.UsedGold, sessionId);
                var currentGold = await _currencyService.GetPlayerGoldAsync(request.Id, sessionId);
                
                _logger.LogInformation($"플레이어 골드 사용 성공: Id : {request.Id} 사용 골드 : {request.UsedGold}");
                return Ok(new
                {
                    CurrentGold = currentGold
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 골드 사용 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPatch("gold/add")]
        public Task<IActionResult> AddPlayerGold(
            [FromBody] UsePlayerGoldRequest request)
        {
            _logger.LogInformation($"플레이어 골드 추가 시도 : Id {request.Id}");
        }
    }
}
