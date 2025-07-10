using BAExamApp.Entities.Enums;

namespace BAExamApp.Dtos.QuestionRevisions;
public class QuestionRevisionCreateDto
{
    public string? RequestDescription { get; set; }
    public string? Comment { get; set; }
    public QuestionRevisionType RevisionType { get; set; }
    public Guid QuestionId { get; set; }
    public Guid RequesterAdminId { get; set; }
    public Guid? RequestedTrainerId { get; set; }
}
