namespace Domain.Entity;

public class RelationUsersRoles
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int UserRoleId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual UserRole UserRole { get; set; } = null!;
}
