using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Filter;
using Server.Application.Port;

namespace Server.Api.Controller
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SessionValidationFilter))]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ICurrencyService currencyService,ISessionService sessionService, ILogger<CurrencyController> logger)
        {
            _currencyService = currencyService;
            _sessionService = sessionService;
            _logger = logger;
        }
        
        [HttpGet("action-point")]
        public async Task<IActionResult> GetPlayerActionPointById()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            int userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 AP 조회 시도 : Id : {userId}");
            
            try
            {
                var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(userId, sessionId!);
                
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

        [HttpPatch("action-point/max")]
        public async Task<IActionResult> UpdatePlayerMaxActionPoint(
            [FromBody] UpdatePlayerMaxActionPointRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 최대 AP 갱신 시도");
            
            try
            {
                var newMaxActionPoint = await _currencyService.UpdatePlayerMaxActionPoint(userId, request.NewMaxActionPoint, sessionId!);
                
                _logger.LogInformation($"플레이어 최대 AP 갱신 성공 : Id : {userId}");
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
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 AP 사용 시도 : Id : {userId}");
            
            try
            {
                await _currencyService.UsePlayerActionPointAsync(userId, request.UsedActionPoint, sessionId!);
                var currentActionPoint = await _currencyService.GetPlayerActionPointAsync(userId, sessionId!);

                _logger.LogInformation($"플레이어 AP 사용 성공 : Id : {userId}");
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

        [HttpGet("gold")]
        public async Task<IActionResult> GetPlayerGold()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 골드 조회 시도 : Id : {userId}");
            
            try
            {
                var gold = await _currencyService.GetPlayerGoldAsync(userId, sessionId!);
                
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

        [HttpPatch("gold")]
        public async Task<IActionResult> UpdatePlayerGold(
            [FromBody] ModifyGoldRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 골드 갱신 시도: Id={userId}, Amount={request.Amount}");

            try
            {
                if (request.Amount >= 0)
                {
                    _logger.LogInformation($"플레이어 골드 추가 성공 :  {userId}, Amount : {request.Amount}");
                    await _currencyService.AddPlayerGoldAsync(userId, request.Amount, sessionId!);
                }
                else
                {
                    _logger.LogInformation($"플레이어 골드 사용 성공 : Id : {userId}, Amount : {request.Amount}");
                    await _currencyService.UsePlayerGoldAsync(userId, -request.Amount, sessionId!);
                }
                
                var currentGold = await _currencyService.GetPlayerGoldAsync(userId, sessionId!);
                return Ok(new
                {
                    CurrentGold = currentGold
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 골드 업데이트 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpGet("paper-piece")]
        public async Task<IActionResult> GetPlayerPaperPiece()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 종이조각 조회 시도 : Id : {userId}");

            try
            {
                var currentPaperPiece = await _currencyService.GetPlayerPaperPieceAsync(userId, sessionId!);

                _logger.LogInformation($"플레이어 종이조각 조회 성공 : Id : {userId}");
                return Ok(new
                {
                    currentPaperPiece = currentPaperPiece
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 종이조각 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPatch("paper-piece")]
        public async Task<IActionResult> UpdatePlayerPaperPiece(
            [FromBody] ModifyPaperPieceRequest request)
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 종이 조각 갱신 시도 : Id : {userId}");
            
            try
            {
                if (request.Amount >= 0)
                {
                    _logger.LogInformation($"플레이어 종이 조각 추가 성공 : Id : {userId}");
                    await _currencyService.AddPlayerPaperPieceAsync(userId, request.Amount, sessionId!);
                }
                else
                {
                    _logger.LogInformation($"플레이어 종이 조각 사용 성공 : Id : {userId}");
                    await _currencyService.UsePlayerPaperPieceAsync(userId, -request.Amount, sessionId!);
                }

                var currentPaperPiece = await _currencyService.GetPlayerPaperPieceAsync(userId, sessionId!);
                return Ok(new
                {
                    currentPaperPiece = currentPaperPiece
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 종이 조각 업데이트 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
