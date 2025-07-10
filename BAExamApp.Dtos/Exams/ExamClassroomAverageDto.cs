using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.Exams;
public class ExamClassroomAverageDto
{
    public Guid ClassroomId { get; set; }
    public string? ClassroomName { get; set; }
    public TimeSpan AverageCompletionTime { get; set; }
}
