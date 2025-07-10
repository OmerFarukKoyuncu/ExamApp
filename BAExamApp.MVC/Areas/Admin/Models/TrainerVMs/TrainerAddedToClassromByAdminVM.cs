namespace BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;

public class TrainerAddedToClassromByAdminVM
{
    public List<SelectListItem> Classrooms { get; set; } = new();
    public List<Guid> SelectedTClassroomIds { get; set; } = new();
    public Guid TrainerId { get; set; } = new();
}
