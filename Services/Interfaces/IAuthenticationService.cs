using Domain.Dto;

namespace Services.Interfaces;

public interface IAuthenticationService
{
    Task RegisterAsync(UserDto user);
    Task<JwtSettingsDto> LoginAsync(LoginDto user);
}
