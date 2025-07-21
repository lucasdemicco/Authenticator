using Domain.Base;

namespace Domain.Entity;

public class User : Audit
{
    public int Id { get; set; }
    public string? UserHash { get; set; }
    public string? UserName { get; set; }
    public string? PasswordHash { get; set; }
    public string? Phone { get; set; }
    public string? Name { get; set; }
}
