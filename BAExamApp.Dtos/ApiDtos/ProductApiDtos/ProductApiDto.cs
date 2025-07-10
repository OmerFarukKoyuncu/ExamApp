using BAExamApp.Dtos.ApiDtos.ProductSubjectApiDtos;
using BAExamApp.Dtos.ProductSubjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.ProductApiDtos;
public class ProductApiDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool? IsActive { get; set; }
    public Guid TechnicalUnitId { get; set; }
    public List<ProductSubjectApiDto>? ProductSubjects { get; set; }
}
