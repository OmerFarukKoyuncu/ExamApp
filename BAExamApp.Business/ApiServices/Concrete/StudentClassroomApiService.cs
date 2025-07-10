using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.StudentClassroomApiDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAExamApp.Entities;
using BAExamApp.Dtos.ApiDtos.StudentExamApiDtos;
using BAExamApp.Dtos.StudentExams;

namespace BAExamApp.Business.ApiServices.Concrete
{
    public class StudentClassroomApiService : IStudentClassroomApiService
    {
        private readonly IMapper _mapper;
        private readonly IStudentClassroomRepository _studentClassroomRepository;

        public StudentClassroomApiService(IMapper mapper, IStudentClassroomRepository studentClassroomRepository)
        {
            _mapper = mapper;
            _studentClassroomRepository = studentClassroomRepository;
        }
        /// <summary>
        /// Öğrenci id, adı ve sınıfının id ve adını listeler
        /// </summary>
        /// <returns></returns>
        public async Task<IDataResult<List<StudentClassroomListApiDto>>> GetAllStudentsWithClassroomAsync()
        {
            var studentClassrooms = await _studentClassroomRepository.GetAllAsync();

            if (studentClassrooms == null || !studentClassrooms.Any())
                return new ErrorDataResult<List<StudentClassroomListApiDto>>(Messages.ListNotFound);

            var studentClassroomDtos = _mapper.Map<List<StudentClassroomListApiDto>>(studentClassrooms);

            return new SuccessDataResult<List<StudentClassroomListApiDto>>(studentClassroomDtos, Messages.FoundSuccess);
        }
    }
}
