namespace BAExamApp.Entities.DbSets;

public class TrainerProduct : AuditableEntity
{
    //Navigation Prop.
    public Guid TrainerId { get; set; }
    public virtual Trainer? Trainer { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product? Product { get; set; }
}
