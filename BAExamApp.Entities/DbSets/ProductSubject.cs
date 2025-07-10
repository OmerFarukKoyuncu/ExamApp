namespace BAExamApp.Entities.DbSets;

public class ProductSubject : AuditableEntity
{
    //Navigation Prop.
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public Guid SubjectId { get; set; }
    public virtual Subject? Subject { get; set; }
}