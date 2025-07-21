using Domain.Dto;
using Domain.Entity;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

public class AuthenticationService(ApplicationDbContext context, ILogger<AuthenticationService> logger, 
    IOptions<JwtSettings> jwtSettings) : IAuthenticationService
{
    public async Task<JwtSettingsDto> LoginAsync(LoginDto user)
    {
        var userDb = await context.Users.FirstOrDefaultAsync(x => x.UserName == user.UserName)
            ?? throw new InvalidOperationException($"Usuário {user.UserName} não encontrado na base de dados.");

        var validatePassword = ValidatePasswordHash(user.Password!, userDb.PasswordHash!);
        if (!validatePassword)
            return new();

        var token = await GenerateJwtTokenAsync(user);
        return new JwtSettingsDto
        {
            AccessToken = token,
            Expiration = DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpireMinutes)
        };
    }

    public async Task RegisterAsync(UserDto user)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(x => x.UserName!.Equals(user.UserName));
            if (existingUser is not null)
                throw new InvalidOperationException($"Usuário {user.UserName} existente na base de dados.");

            var userEntity = await AddNewUserAsync(user);

            var userRoles = await context.UserRoles
                .Where(r => user.RoleTypeCodes.Contains(r.RoleTypeCode))
                .ToListAsync();

            List<RelationUsersRoles> userRolesList = [];
            foreach (var role in userRoles)
            {
                userRolesList.Add(new RelationUsersRoles
                {
                    UserId = userEntity.Id,
                    UserRoleId = role.Id
                });
            }

            await context.AddRangeAsync(userRolesList);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            logger.LogInformation("Usuário criado.");
        }
        catch 
        {
            logger.LogError("Realizando rollback.");
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<User> AddNewUserAsync(UserDto user)
    {
        User userEntity = new()
        {
            UserName = user.UserName,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.UserName!,
            Active = true,
            Name = user.Name,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Phone = user.Phone,
            UserHash = Guid.NewGuid().ToString()
        };

        await context.AddAsync(userEntity);
        await context.SaveChangesAsync();
        return userEntity;
    }

    private static bool ValidatePasswordHash(string password, string passwordHashDatabase)
        => BCrypt.Net.BCrypt.Verify(password, passwordHashDatabase);

    private async Task<string> GenerateJwtTokenAsync(LoginDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings.Value.SecretKey);

        var claimsList = await ProcessingClaimsAsync(user);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claimsList),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.Value.ExpireMinutes),
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<List<Claim>> ProcessingClaimsAsync(LoginDto user)
    {
        List<Claim> claimsList = [];
        List<UserRole> roles = [];

        //Busca entidade do usuário
        var userEntity = await context.Users.FirstOrDefaultAsync(u => u.UserName!.Equals(user.UserName));

        //Busca od Ids de Claims para aquele usuário
        var userClaims = await context.RelationUsersRoles.Where(x => x.UserId == userEntity!.Id).ToListAsync();

        //Busca Todas as Roles do usuário
        var userRoles = await context.UserRoles.Where(x => x.Id == userClaims.FirstOrDefault()!.UserRoleId).ToListAsync();
        userRoles.ForEach(claims => claimsList.Add(new Claim(ClaimTypes.Role, claims.Description!)));
        return claimsList;
    }
}
