using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
public class SubtopicApiDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid SubjectId { get; set; }
}
