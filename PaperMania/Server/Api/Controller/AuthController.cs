using Microsoft.AspNetCore.Mvc;
using Server.Api.Dto.Request;
using Server.Api.Dto.Response;
using Server.Domain.Entity;
using Server.Infrastructure.Service.Interface;

namespace Server.Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existByEmail = await _accountService.GetByEmailAsync(request.Email);
                if (existByEmail != null)
                    return Conflict(new { message = "이미 사용 중인 이메일입니다." });

                var existByPlayerId = await _accountService.GetByPlayerIdAsync(request.PlayerId);
                if (existByPlayerId != null)
                    return Conflict(new { message = "이미 사용 중인 PlayerId 입니다." });

                var newUser = new PlayerAccountData
                {
                    PlayerId = request.PlayerId,
                    Email = request.Email
                };

                var response = new RegisterResponse
                {
                    Message = "회원가입 성공",
                    Id = newUser.Id
                };

                await _accountService.RegisterAsync(newUser, request.Password);
                return Created(string.Empty, response);
            }
            catch
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var sessionId = await _accountService.LoginAsync(request.PlayerId, request.Password);
                if (string.IsNullOrEmpty(sessionId))
                    return Unauthorized(new { message = "아이디 또는 비밀번호가 올바르지 않습니다." });

                var response = new LoginResponse
                {
                    Message = "로그인 성공",
                    SessionId = sessionId
                };

                return Ok(response);
            }
            catch
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        [HttpDelete("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Session-Id")] string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return Conflict(new { message = "세션 ID가 없습니다." });


                var success = await _accountService.LogoutAsync(sessionId);
                if (!success)
                    return Conflict(new { message = "유효하지 않은 세션입니다." });

                return Ok(new { message = "로그아웃 성공" });
            }
            catch
            {
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
