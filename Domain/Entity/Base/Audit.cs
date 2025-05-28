namespace Domain.Base;

public class Audit
{
    public required string CreatedBy { get; set; }
    public required DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Active { get; set; }
}
