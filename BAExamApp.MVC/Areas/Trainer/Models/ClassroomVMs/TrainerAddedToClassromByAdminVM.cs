namespace BAExamApp.MVC.Areas.Trainer.Models.ClassroomVMs;

public class TrainerAddedToClassromByAdminVM
{
    public List<SelectListItem> Classrooms { get; set; } = new();
    public List<Guid> SelectedTClassroomIds { get; set; } = new();
    public Guid TrainerId { get; set; } = new();
    public List<string>? AppointedTrainersId { get; set; }
}
