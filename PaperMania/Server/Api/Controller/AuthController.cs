using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Api.Filter;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Api.Controller
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAccountService accountService, ISessionService sessionService, 
            ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _sessionService =  sessionService;
            _logger = logger;
        }

        
        /// <summary>
        /// 신규 회원가입을 처리합니다.
        /// </summary>
        /// <param name="request">회원가입에 필요한 이메일, 비밀번호, PlayerId 등의 정보</param>
        /// <returns>회원가입 성공 시 생성된 사용자 ID</returns>
        /// <response code="201">회원가입이 성공적으로 완료됨</response>
        /// <response code="409">중복된 이메일 또는 PlayerId</response>
        /// <response code="500">서버 내부 오류</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterResponse), 201)]
        [ProducesResponseType(typeof(object), 409)]
        [ProducesResponseType(typeof(object), 500)]
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

        /// <summary>
        /// 로그인 요청을 처리합니다.
        /// </summary>
        /// <param name="request">로그인에 필요한 PlayerId와 비밀번호</param>
        /// <returns>로그인 결과</returns>
        /// <response code="200">로그인 성공</response>
        /// <response code="401">인증 실패 (잘못된 PlayerId 또는 비밀번호)</response>
        /// <response code="500">서버 내부 오류</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        [ProducesResponseType(typeof(object), 401)]
        [ProducesResponseType(typeof(object), 500)]
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

                var data = await _accountService.GetByPlayerIdAsync(request.PlayerId);
                
                var response = new LoginResponse
                {
                    Message = "로그인 성공",
                    Id = data!.Id,
                    SessionId = sessionId
                };

                _logger.LogInformation("로그인 성공: PlayerId={PlayerId}, SessionId={SessionId}", request.PlayerId, sessionId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류: 로그인 중 예외 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        /// <summary>
        /// 구글 로그인 API
        /// </summary>
        /// <param name="request">구글 로그인 요청 정보</param>
        /// <returns>로그인 결과</returns>
        [HttpPost("login/google")]
        [ProducesResponseType(typeof(GoogleLoginResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<GoogleLoginResponse>> LoginByGoogle([FromBody] GoogleLoginRequest request)
        {
            _logger.LogInformation("구글 로그인 시도");

            try
            {
                var sessionId = await _accountService.LoginByGoogleAsync(request.IdToken);
                if (string.IsNullOrEmpty(sessionId))
                {
                    _logger.LogWarning("구글 로그인 실패");
                    return Unauthorized(new { message = "구글 로그인 실패." });
                }

                var response = new GoogleLoginResponse
                {
                    SessionId = sessionId,
                    Message = $"구글 로그인 성공"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류: 구글 로그인 중 예외 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        /// <summary>
        /// 로그아웃 API
        /// </summary>
        /// <returns>로그아웃 결과</returns>
        [HttpPost("logout")]
        [ServiceFilter(typeof(SessionValidationFilter))]
        [ProducesResponseType(typeof(LogoutResponse), 200)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        [ServiceFilter(typeof(SessionValidationFilter))]
        public async Task<ActionResult<LogoutResponse>> Logout()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId =  await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
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

                var response = new LogoutResponse
                {
                    Id = userId,
                    Message = "로그아웃 성공"
                };

                _logger.LogInformation("로그아웃 성공: SessionId={SessionId}", sessionId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 오류 - 로그아웃 중 예외 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
