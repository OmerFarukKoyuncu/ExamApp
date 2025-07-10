using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.ExamRuleSubtopicApiDtos;
public class ExamRuleSubtopicApiDto
{
    public Guid Id { get; set; }
    public int ExamType { get; set; }
    public int QuestionType { get; set; }
    public int QuestionCount { get; set; }
    public Guid QuestionDifficultyId { get; set; }
    public Guid ExamRuleId { get; set; }
    public Guid SubtopicId { get; set; }
    public Guid SubjectId { get; set; }
}
