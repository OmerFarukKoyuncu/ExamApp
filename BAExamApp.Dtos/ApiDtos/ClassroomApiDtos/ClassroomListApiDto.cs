using BAExamApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.ClassroomApiDtos;
public class ClassroomListApiDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime OpeningDate { get; set; }
    public DateTime ClosedDate { get; set; }
    public int StudentCount { get; set; }
    public string BranchName { get; set; }
    public bool IsActive { get; set; }
    public Status Status { get; set; }
    public int ClassroomExamCount { get; set; }
    public int ClassroomAppointedExamCount { get; set; }
}
