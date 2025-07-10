using BAExamApp.Dtos.ApiDtos.ClassroomApiDtos;
using BAExamApp.Dtos.Classrooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IClassroomApiService
{
    /// <summary>
    /// Classroom listeleme işlemini yapar.
    /// </summary>
    /// <returns>List<ClassroomListApiDto> döndürür.</returns>
    Task<IDataResult<List<ClassroomListApiDto>>> GetAllAsync();
}
