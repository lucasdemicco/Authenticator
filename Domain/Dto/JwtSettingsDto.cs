namespace Domain.Dto;

public class JwtSettingsDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}
