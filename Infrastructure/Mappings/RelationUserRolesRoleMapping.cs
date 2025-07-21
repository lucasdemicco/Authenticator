using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Mappings;

public class RelationUserRolesRoleMapping : IEntityTypeConfiguration<RelationUsersRoles>
{
    public void Configure(EntityTypeBuilder<RelationUsersRoles> builder)
    {
        builder.ToTable("Users_UsersRoles");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.UserRoleId).IsRequired();
    }
}
