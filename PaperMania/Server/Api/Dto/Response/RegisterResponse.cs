/// <summary>
/// 회원가입 응답 DTO
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// 응답 메시지
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// 등록된 유저의 고유 ID
    /// </summary>
    public int Id { get; set; }
}