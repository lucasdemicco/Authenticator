using Domain.Dto;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ic_api_authenticator.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthenticatorController(IAuthenticationService authenticationService) : ControllerBase
    {
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Realiza login do usuário",
            Description = "Autentica o usuário com nome de usuário e senha, e retorna um token JWT."
        )]
        [ProducesResponseType(typeof(JwtSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost()]
        public async Task<IActionResult> Login(LoginDto user)
        {
            try
            {
                var result = await authenticationService.LoginAsync(user);
                return !string.IsNullOrEmpty(result.AccessToken)
                    ? Ok(new ApiResult(result!, StatusCodes.Status200OK, true, []))
                    : Unauthorized(new ApiResult(result!, StatusCodes.Status404NotFound, false, ["Usuário não autenticado"]));
            }
            catch (Exception ex)
            {
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new ApiResult("Erro interno do Servidor", 500, false, [ex.Message]));
                }
            }
        }

        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Realiza registro de um novo usuário",
            Description = "Autentica o usuário com nome de usuário e senha, e retorna um token JWT."
        )]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost()]
        public async Task<IActionResult> Register(UserDto user)
        {
            try
            {
                await authenticationService.RegisterAsync(user);
                return Ok(new ApiResult("Usuário cadastrado com sucesso.", StatusCodes.Status200OK, true, []));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResult("Erro interno do Servidor", 500, false, [ex.Message]));
            }
        }
    }
}
