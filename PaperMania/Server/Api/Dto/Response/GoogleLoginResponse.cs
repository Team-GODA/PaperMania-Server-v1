namespace Server.Api.Dto.Response
{
    /// <summary>
    /// 구글 로그인 응답 DTO
    /// </summary>
    public class GoogleLoginResponse
    {
        /// <summary>
        /// 로그인 결과 메시지
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 사용자 세션 ID
        /// </summary>
        public string SessionId { get; set; } = "";
    }
}