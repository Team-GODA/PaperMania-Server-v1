using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Application.Port;
using System.Linq;
using Server.Api.Filter;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SessionValidationFilter))]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<DataController> _logger;

        public DataController(IDataService dataService, ILogger<DataController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [HttpPost("player")]
        public async Task<IActionResult> AddPlayerData(
            [FromBody] AddPlayerDataRequest request)
        {
            _logger.LogInformation($"플레이어 이름 등록 시도: PlayerName = {request.PlayerName}");
            var sessionId = HttpContext.Items["SessionId"] as string;

            try
            {
                 var result = await _dataService.AddPlayerDataAsync(request.PlayerName, sessionId);
                
                _logger.LogInformation("플레이어 이름 등록 성공: PlayerName = {PlayerName}", request.PlayerName);
                return Created(string.Empty, new { message = "이름이 성공적으로 설정되었습니다." });
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("플레이어 이름 등록 실패: 유효하지 않은 세션");
                return Conflict(new { message = "유효하지 않은 세션입니다." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("플레이어 이름 등록 실패: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류 발생: 플레이어 이름 등록 중 예외");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
        
        [HttpGet("name/{id}")]
        public async Task<IActionResult> GetPlayerNameById(
            [FromRoute(Name = "id")] int userId)
        {
            _logger.LogInformation($"플레이어 이름 조회 시도: Id: {userId}");
            var sessionId = HttpContext.Items["SessionId"] as string;

            try
            {
                 var id = userId;
                var playerName = await _dataService.GetPlayerNameByUserIdAsync(userId);
                
                _logger.LogInformation($"플레이어 이름 조회 성공: PlayerName: {playerName}");
                return Ok(new
                {
                    id,
                    playerName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 이름 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPatch("name/{id}")]
        public async Task<IActionResult> RenamePlayerName(
            [FromRoute(Name = "id")] int userId, 
            [FromBody] RenamePlayerNameRequest request)
        {
            _logger.LogInformation($"플레이어 이름 재설정 시도: Id: {userId}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                if (request.NewName == null)
                    return Conflict(new { message = "플레이어 이름 재설정 실패 : NewName 누락 오류" });
                
                await _dataService.RenamePlayerNameAsync(userId, request.NewName);
                
                _logger.LogInformation($"플레이어 이름 재설정 성공: Id: {userId}, NewName: {request.NewName}");
                return Ok(new { newName = request.NewName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 이름 재설정 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpGet("level/{id}")]
        public async Task<IActionResult> GetPlayerLevelById(
            [FromRoute(Name = "id")] int userId)
        {
            _logger.LogInformation($"플레이어 레벨 조회 시도: Id: {userId}");
            var sessionId = HttpContext.Items["SessionId"] as string;

            try
            {
                var level = await _dataService.GetPlayerLevelByUserIdAsync(userId);
                var exp = await _dataService.GetPlayerExpByUserIdAsync(userId);
                
                _logger.LogInformation($"플레이어 레벨 조회 성공: PlayerLevel: {level}");
                return Ok(new
                {
                    userId,
                    level,
                    exp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 레벨 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPost("level/{id}")]
        public async Task<IActionResult> UpdatePlayerLevel(
            [FromRoute(Name = "id")] int userId,
            [FromBody] UpdatePlayerLevelRequest request)
        {
            _logger.LogInformation($"플레이어 레벨 갱신 시도: Id: {userId}, 갱신 레벨: {request.NewLevel}, 갱신 Exp: {request.NewExp}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                var data =
                    await _dataService.UpdatePlayerLevelAsync(userId, request.NewLevel, request.NewExp);

                var newLevel = data.PlayerLevel;
                var newExp = data.PlayerExp;
                
                _logger.LogInformation($"플레이어 레벨 갱신 성공: Id: {userId}, 갱신 레벨: {request.NewLevel}, 갱신 Exp: {request.NewExp}");
                return Ok(new
                {
                    userId,
                    newLevel,
                    newExp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 레벨 갱신 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
