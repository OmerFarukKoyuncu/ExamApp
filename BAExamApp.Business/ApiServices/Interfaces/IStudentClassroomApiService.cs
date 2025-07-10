using BAExamApp.Dtos.ApiDtos.StudentClassroomApiDtos;
using BAExamApp.Dtos.StudentClassrooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IStudentClassroomApiService
{
    /// <summary>
    /// Öğrenci id, adı ve sınıfının id ve adını listeler
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<StudentClassroomListApiDto>>> GetAllStudentsWithClassroomAsync();
}
