using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.ApiDtos.SubjectApiDtos;
public class SubjectUpdateRequestApiDto
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "At least one product must be selected")]
    [MinLength(1, ErrorMessage = "At least one product must be selected")]
    public Guid[] ProductIds { get; set; }
}
