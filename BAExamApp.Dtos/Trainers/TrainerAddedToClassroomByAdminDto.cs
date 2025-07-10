using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BAExamApp.Dtos.Trainers;
public class TrainerAddedToClassroomByAdminDto
{
    public List<SelectListItem> Classrooms { get; set; } = new();
    public List<Guid> SelectedTClassroomIds { get; set; } = new();
    public Guid TrainerId { get; set; } = new();
    public List<string>? AppointedTrainersId { get; set; }
}
