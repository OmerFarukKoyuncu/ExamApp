using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.SubjectApiDtos;
public class SubjectCreateApiDto
{
    public List<Guid> ProductIdList { get; set; }
    public string Name { get; set; }
}
