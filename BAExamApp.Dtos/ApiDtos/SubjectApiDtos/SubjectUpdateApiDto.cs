
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAExamApp.Dtos.ApiDtos.ProductSubjectApiDtos;
using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.Entities.DbSets;

namespace BAExamApp.Dtos.ApiDtos.SubjectApiDtos;
public class SubjectUpdateApiDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public virtual List<ProductSubjectApiDto> ProductSubjects { get; set; } = new List<ProductSubjectApiDto>();
    public virtual List<Subtopic>? Subtopics { get; set; } = new List<Subtopic>();

}
