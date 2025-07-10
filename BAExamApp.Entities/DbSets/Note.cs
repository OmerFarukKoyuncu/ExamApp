namespace BAExamApp.Entities.DbSets;
public class Note: AuditableEntity
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public Guid AdminId { get; set; }
    public virtual Admin? Admin { get; set; }
}
