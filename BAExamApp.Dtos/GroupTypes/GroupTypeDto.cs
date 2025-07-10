using BAExamApp.Dtos.Classrooms;
using BAExamApp.Entities.DbSets;

namespace BAExamApp.Dtos.GroupTypes;

public class GroupTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Information { get; set; }
    public List<ClassroomDetailsForAdminDto> Classrooms { get; set; } = new List<ClassroomDetailsForAdminDto>();
}