using BAExamApp.Dtos.ApiDtos.ExamRuleSubtopicApiDtos;
using BAExamApp.Dtos.ExamRuleSubtopics;
using BAExamApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.ExamRuleApiDtos;
public class ExamRuleApiDto
{

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ExamCreationType ExamCreationType { get; set; }
    public Guid ProductId { get; set; }
    public List<ExamRuleSubtopicApiDto> ExamRuleSubtopics { get; set; }
}
