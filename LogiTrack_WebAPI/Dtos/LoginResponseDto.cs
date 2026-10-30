namespace LogiTrack_WebAPI.Dtos;

public class LoginResponseDto
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
