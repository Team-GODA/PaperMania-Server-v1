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
    [ServiceFilter(typeof(SessionValidationFilter))]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ICharacterService characterService, ILogger<CharacterController> logger,
            ISessionService sessionService)
        {
            _characterService = characterService;
            _sessionService = sessionService;
            _logger = logger;
        }
        
        /// <summary>
        /// 특정 캐릭터 정보를 조회합니다.
        /// </summary>
        /// <param name="id">조회할 캐릭터의 ID</param>
        /// <returns>캐릭터 정보</returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetAllPlayerCharactersResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GetAllPlayerCharactersResponse>> GetAllPlayerCharacters()
        {
            var sessionId = HttpContext.Items["SessionId"] as string;
            var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId!);
            
            _logger.LogInformation($"플레이어 보유 캐릭터 데이터 조회 시도: ID: {userId}");
            
            try
            {
                var data = await _characterService.GetPlayerCharacterDataByUserIdAsync(userId);
                var response = new GetAllPlayerCharactersResponse
                {
                    PlayerCharacters = data
                };
                
                _logger.LogInformation($"플레이어 보유 캐릭터 데이터 조회 성공: ID: {userId}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 보유 캐릭터 조회 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }

        /// <summary>
        /// 유저의 보유 캐릭터를 추가합니다.
        /// </summary>
        /// <param name="request">추가할 캐릭터 정보</param>
        /// <returns>추가된 캐릭터 정보</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AddPlayerCharacterResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<AddPlayerCharacterResponse>> AddPlayerCharacter(
            [FromBody] AddPlayerCharacterRequest request)
        {
            _logger.LogInformation($"플레이어 보유 캐릭터 추가 시도: Id: {request.Id}, CharacterId: {request.CharacterId}");
            var sessionId = HttpContext.Items["SessionId"] as string;
            
            try
            {
                var data = new PlayerCharacterData
                {
                    Id = request.Id,
                    CharacterId = request.CharacterId
                };

                var addedCharacter = await _characterService.AddPlayerCharacterDataByUserIdAsync(data);
                var response = new AddPlayerCharacterResponse
                {
                    Id = addedCharacter.Id,
                    CharacterId = addedCharacter.CharacterId
                };
                
                _logger.LogInformation($"플레이어 보유 캐릭터 추가 성공: Id: {request.Id}, CharacterId: {request.CharacterId}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "플레이어 캐릭터 추가 중 오류 발생");
                return StatusCode(500, new { message = "서버 오류가 발생했습니다." });
            }
        }
    }
}
