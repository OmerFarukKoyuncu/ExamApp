using System.ComponentModel.DataAnnotations;

namespace BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
public class SubtopicUpdateApiDto
{
    [Required]
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
    public Guid? SubjectId { get; set; }
}
