namespace Server.Api.Dto.Response
{
    /// <summary>
    /// 로그인 응답 DTO
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// 로그인 결과 메시지
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 로그인한 사용자 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 사용자 세션 ID
        /// </summary>
        public string SessionId { get; set; } = "";
    }
}