using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.StudentClassroomApiDtos;
public class StudentClassroomListApiDto
{
    public Guid StudentId { get; set; }
    public Guid ClassroomId { get; set; }
    public string StudentName { get; set; }
    public string ClassroomName { get; set; }
}
