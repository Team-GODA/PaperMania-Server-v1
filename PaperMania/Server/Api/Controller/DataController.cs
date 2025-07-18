using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Application.Port;
using System.Linq;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ILogger<DataController> _logger;

        public DataController(IDataService dataService, ILogger<DataController> logger)
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

        [HttpPost("name")]
        public async Task<IActionResult> AddPlayerName(
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromBody] AddPlayerNameRequest request)
        {
            _logger.LogInformation($"플레이어 이름 등록 시도: PlayerName = {request.PlayerName}");
            
            if (!CheckSessionId(sessionId))
                return Unauthorized(new { message = "세션 ID가 없습니다." });

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
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromRoute(Name = "id")]int userId)
        {
            _logger.LogInformation($"플레이어 이름 조회 시도: Id: {userId}");

            if (!CheckSessionId(sessionId))
                return Unauthorized(new { message = "세션 ID가 없습니다." });

            try
            {
                 var id = userId;
                var playerName = await _dataService.GetPlayerNameByUserIdAsync(userId, sessionId);
                
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

        [HttpGet("level/{id}")]
        public async Task<IActionResult> GetPlayerLevelById(
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromRoute(Name = "id")] int userId)
        {
            _logger.LogInformation($"플레이어 레벨 조회 시도: Id: {userId}");

            if (!CheckSessionId(sessionId))
                return Unauthorized(new { message = "세션 ID가 없습니다." });

            try
            {
                var id = userId;
                var level = await _dataService.GetPlayerLevelByUserIdAsync(userId, sessionId);
                var exp = await _dataService.GetPlayerExpByUserIdAsync(userId, sessionId);
                
                _logger.LogInformation($"플레이어 레벨 조회 성공: PlayerLevel: {level}");
                return Ok(new
                {
                    id,
                    level,
                    exp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 이름 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPost("level/{id}")]
        public async Task<IActionResult> UpdatePlayerLevel(
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromRoute(Name = "id")] int userId,
            [FromBody] UpdatePlayerLevelRequest request)
        {
            _logger.LogInformation($"플레이어 레벨 갱신 시도: Id: {userId}, 갱신 레벨: {request.NewLevel}, 갱신 Exp: {request.NewExp}");

            if (!CheckSessionId(sessionId))
                return Unauthorized(new { message = "세션 ID가 없습니다." });
            
            try
            {
                var data =
                    await _dataService.UpdatePlayerLevelAsync(userId, request.NewLevel, request.NewExp, sessionId);

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
                _logger.LogError(ex, "플레이어 이름 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpGet("character/{id}")]
        public async Task<IActionResult> GetPlayerCharacterById(
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromRoute(Name = "id")] int userId)
        {
            _logger.LogInformation($"플레이어 보유 캐릭터 데이터 조회 시도: ID: {userId}");

            if (!CheckSessionId(sessionId))
                return Unauthorized(new { message = "세션 ID가 없습니다." });
            
            try
            {
                var data = await _dataService.GetPlayerCharacterDataByUserIdAsync(userId, sessionId);

                _logger.LogInformation($"플레이어 보유 캐릭터 데이터 조회 성공: ID: {userId}");
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 보유 캐릭터 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPost("character")]
        public async Task<IActionResult> AddPlayerCharacterById(
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromBody] AddPlayerCharacterRequest request)
        {
            _logger.LogInformation($"플레이어 보유 캐릭터 추가 시도: Id: {request.Id}, CharacterId: {request.CharacterId}");

            if (!CheckSessionId(sessionId))
                return Unauthorized(new { message = "세션 ID가 없습니다." });
            
            try
            {
                var data = new PlayerCharacterData
                {
                    Id = request.Id,
                    CharacterId = request.CharacterId
                };

                var addedCharacter = await _dataService.AddPlayerCharacterDataByUserIdAsync(data, sessionId);
                
                _logger.LogInformation($"플레이어 보유 캐릭터 추가 성공: Id: {request.Id}, CharacterId: {request.CharacterId}");
                return Ok(addedCharacter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 캐릭터 추가 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
