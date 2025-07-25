using System.Net;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Application.Port;
using Asp.Versioning;
using Server.Api.Filter;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(SessionValidationFilter))]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<DataController> _logger;

        public DataController(IDataService dataService, ISessionService sessionService ,ILogger<DataController> logger)
        {
            _dataService = dataService;
            _sessionService = sessionService;
            _logger = logger;
        }

        /// <summary>
        /// 플레이어 이름을 등록합니다.
        /// </summary>
        /// <param name="request">플레이어 이름 등록 요청 객체</param>
        /// <returns>등록 성공 여부에 대한 응답</returns>
        [HttpPost("player")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddPlayerData([FromBody] AddPlayerDataRequest request)
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

        /// <summary>
        /// 현재 플레이어의 이름을 조회합니다.
        /// </summary>
        /// <returns>플레이어 이름 정보</returns>
        [HttpGet("name")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPlayerName()
        {
            var sessionId =  HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 이름 조회 시도: Id: {userId}");

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

        /// <summary>
        /// 플레이어 이름을 변경합니다.
        /// </summary>
        /// <param name="request">변경할 새 플레이어 이름 정보</param>
        /// <returns>변경된 이름 반환</returns>
        [HttpPatch("name")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RenamePlayerName([FromBody] RenamePlayerNameRequest request)
        {
            var sessionId =  HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 이름 재설정 시도: Id: {userId}");

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

        /// <summary>
        /// 플레이어의 현재 레벨과 경험치를 조회합니다.
        /// </summary>
        /// <returns>레벨 및 경험치 정보</returns>
        [HttpGet("level")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPlayerLevel()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 레벨 조회 시도: Id: {userId}");

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

        /// <summary>
        /// 플레이어의 경험치를 추가하고 레벨을 갱신합니다.
        /// </summary>
        /// <param name="request">추가할 경험치 정보</param>
        /// <returns>갱신된 레벨 및 경험치 정보</returns>
        [HttpPatch("level/exp")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdatePlayerLevelByExp([FromBody] AddPlayerExpRequest request)
        {
            var sessionId =  HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);

            _logger.LogInformation($"플레이어 레벨 갱신 시도: Id: {userId}");

            try
            {
                var data = await _dataService.UpdatePlayerLevelByExpAsync(userId, request.NewExp);

                var newLevel = data.PlayerLevel;
                var newExp = data.PlayerExp;

                _logger.LogInformation($"플레이어 레벨 갱신 성공: Id: {userId}");
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
