using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Domain.Entity;
using Server.Infrastructure.Service.Interface;

namespace Server.Api.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAccountService accountService, ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("회원가입 시도: Email={Email}, PlayerId={PlayerId}", request.Email, request.PlayerId);
            
            try
            {
                var existByEmail = await _accountService.GetByEmailAsync(request.Email);
                if (existByEmail != null)
                {
                    _logger.LogWarning("회원가입 실패: 이메일 중복: {Email}", request.Email);
                    return Conflict(new { message = "이미 사용 중인 이메일입니다." });
                }

                var existByPlayerId = await _accountService.GetByPlayerIdAsync(request.PlayerId);
                if (existByPlayerId != null)
                {
                    _logger.LogWarning("회원가입 실패: PlayerId 중복: {PlayerId}", request.PlayerId);
                    return Conflict(new { message = "이미 사용 중인 PlayerId 입니다." });
                }

                var newUser = new PlayerAccountData
                {
                    PlayerId = request.PlayerId,
                    Email = request.Email
                };
                
                await _accountService.RegisterAsync(newUser, request.Password);

                var response = new RegisterResponse
                {
                    Message = "회원가입 성공",
                    Id = newUser.Id
                };

                _logger.LogInformation("회원가입 성공: Id={Id}, PlayerId={PlayerId}", response.Id, request.PlayerId);
                return Created(string.Empty, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류:  회원가입 중 예외 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("로그인 시도: PlayerId={PlayerId}", request.PlayerId);
            
            try
            {
                var sessionId = await _accountService.LoginAsync(request.PlayerId, request.Password);
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("로그인 실패: 아이디 또는 비밀번호 불일치: PlayerId={PlayerId}", request.PlayerId);
                    return Unauthorized(new { message = "아이디 또는 비밀번호가 올바르지 않습니다." });
                }

                var response = new LoginResponse
                {
                    Message = "로그인 성공",
                    SessionId = sessionId
                };

                _logger.LogInformation("로그인 성공: PlayerId={PlayerId}, SessionId={SessionId}", request.PlayerId, sessionId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류:  로그인 중 예외 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Session-Id")] string sessionId)
        {
            _logger.LogInformation("로그아웃 시도: SessionId={SessionId}", sessionId);
            
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("로그아웃 실패: 세션 ID 없음");
                    return Conflict(new { message = "세션 ID가 없습니다." });
                }

                var success = await _accountService.LogoutAsync(sessionId);
                if (!success)
                {
                    _logger.LogWarning("로그아웃 실패:  유효하지 않은 세션: SessionId={SessionId}", sessionId);
                    return Conflict(new { message = "유효하지 않은 세션입니다." });
                }

                _logger.LogInformation("로그아웃 성공: SessionId={SessionId}", sessionId);
                return Ok(new { message = "로그아웃 성공" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류 - 로그아웃 중 예외 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
