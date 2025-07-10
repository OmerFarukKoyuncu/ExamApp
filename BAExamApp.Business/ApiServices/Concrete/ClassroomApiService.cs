using BAExamApp.Dtos.ApiDtos.ClassroomApiDtos;
using BAExamApp.Dtos.Classrooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Concrete;
public class ClassroomApiService : IClassroomApiService
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly IMapper _mapper;
    private readonly IStudentExamRepository _studentExamRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentClassroomRepository _studentClassroomRepository;

    public ClassroomApiService(IClassroomRepository classroomRepository, IMapper mapper, IStudentExamRepository studentExamRepository, IStudentRepository studentRepository, IStudentClassroomRepository studentClassroomRepository)
    {
        _studentExamRepository = studentExamRepository;
        _classroomRepository = classroomRepository;
        _mapper = mapper;
        _studentRepository = studentRepository;
        _studentClassroomRepository = studentClassroomRepository;
    }

    public async Task<IDataResult<List<ClassroomListApiDto>>> GetAllAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        return new SuccessDataResult<List<ClassroomListApiDto>>(classrooms.Adapt<List<ClassroomListApiDto>>(), Messages.ListedSuccess);
    }
}
