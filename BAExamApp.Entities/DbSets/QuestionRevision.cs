namespace BAExamApp.Entities.DbSets;

public class QuestionRevision : AuditableEntity
{
    public string? RequestDescription { get; set; }
    public string? RevisionConclusion { get; set; }
    public string? Comment { get; set; }

    public QuestionRevisionType RevisionType { get; set; }

    //Navigation Prop.
    public Guid QuestionId { get; set; }
    public virtual Question? Question { get; set; }
    public Guid RequesterAdminId { get; set; }
    public virtual Admin? RequesterAdmin { get; set; }
    public Guid? RequestedTrainerId { get; set; }
    public virtual Trainer? RequestedTrainer { get; set; }
}