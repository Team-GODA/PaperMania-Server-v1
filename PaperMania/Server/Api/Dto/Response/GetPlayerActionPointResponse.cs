namespace Server.Api.Dto.Response
{
    /// <summary>
    /// 플레이어 현재 행동력 조회 응답 DTO
    /// </summary>
    public class GetPlayerActionPointResponse
    {
        /// <summary>
        /// 현재 행동력 값
        /// </summary>
        public int CurrentActionPoint { get; set; }
    }
}