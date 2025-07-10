using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.TrainerClassrooms;
public class TrainerClassroomDeleteDto
{
    public Guid TrainerId { get; set; }
    public Guid ClassroomId { get; set; }
    public DateTime? ResignedDate { get; set; } 
}
