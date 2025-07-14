using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Application.Port;

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

        [HttpPost("name")]
        public async Task<IActionResult> AddPlayerNameAsync(
            [FromHeader(Name = "Session-Id")] string sessionId,
            [FromBody] AddPlayerNameRequest request)
        {
            _logger.LogInformation($"플레이어 이름 등록 시도: PlayerName = {request.PlayerName}");
            
            try
            {
                var result = await _dataService.AddPlayerNameAsync(request.PlayerName, sessionId);
                
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
    }
}
