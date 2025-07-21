namespace Domain.Dto;

public class UserDto
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Phone { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
    public List<int> RoleTypeCodes { get; set; } = [];
}
