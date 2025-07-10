using BAExamApp.Core.Enums;
using BAExamApp.Dtos.StudentClassrooms;
using BAExamApp.Dtos.Students;
using BAExamApp.Entities.DbSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;

namespace BAExamApp.Business.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentClassroomRepository _studentClassroomRepository;
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;

    public StudentService(IStudentRepository studentRepository, IAccountService accountService, IMapper mapper, IStudentClassroomRepository studentClassroomRepository)
    {
        _studentRepository = studentRepository;
        _accountService = accountService;
        _mapper = mapper;
        _studentClassroomRepository = studentClassroomRepository;
    }
    public async Task<IDataResult<StudentDto>> GetByIdAsync(Guid id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
        {
            return new ErrorDataResult<StudentDto>(Messages.UserNotFound);
        }

        return new SuccessDataResult<StudentDto>(_mapper.Map<StudentDto>(student), Messages.FoundSuccess);
    }

    public async Task<IDataResult<StudentDto>> GetByIdentityIdAsync(string identityId)
    {
        var student = await _studentRepository.GetAsync(x => x.IdentityId == identityId);

        if (student is null)
            return new ErrorDataResult<StudentDto>(Messages.UserNotFound);

        return new SuccessDataResult<StudentDto>(_mapper.Map<StudentDto>(student), Messages.FoundSuccess);
    }

    public async Task<IDataResult<StudentDetailsForTrainerDto>> GetDetailsByIdForTrainerAsync(Guid id)
    {
        var student = await _studentRepository.GetAsync(x => x.Id == id);

        if (student is null)
            return new ErrorDataResult<StudentDetailsForTrainerDto>(Messages.UserNotFound);

        return new SuccessDataResult<StudentDetailsForTrainerDto>(_mapper.Map<StudentDetailsForTrainerDto>(student), Messages.FoundSuccess);
    }

    public async Task<IDataResult<List<StudentListDto>>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();

        return new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(students), Messages.ListedSuccess);
    }

    public async Task<IDataResult<List<StudentListDto>>> GetAllByClassroomIdAsync(Guid id)
    {
        var students = await _studentRepository.GetAllAsync(x => x.StudentClassrooms.Select(x => x.ClassroomId).Any(x => x == id));

        return new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(students), Messages.ListedSuccess);
    }

    public async Task<IDataResult<List<StudentListDto>>> GetStudentsWithoutSpesificClassroomIdAsync(Guid classroomId)
    {
        var freeStudents = await _studentRepository.GetAllAsync(x => x.Status != Status.Deleted);

        if (freeStudents.Any())
        {
            return new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(freeStudents), Messages.ListedSuccess);
        }

        return new ErrorDataResult<List<StudentListDto>>(Messages.NoAvailableStudent);
    }

    public async Task<IDataResult<List<StudentListDto>>> GetStudentsWithSpesificClassroomIdAsync(Guid classroomId)
    {

        var students = await _studentRepository.GetAllAsync(x => x.StudentClassrooms.Any(x => x.ClassroomId == classroomId && string.IsNullOrEmpty(x.DeletedBy)));

        return students.Any()
            ? new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(students), Messages.ListedSuccess)
            : new ErrorDataResult<List<StudentListDto>>(Messages.NoStudentFoundInClassroom);
    }

    /// <summary>
    /// Students tablosundaki silinmemiş tüm öğrencilerin listesini döndüren metot.
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<StudentListDto>>> GetActiveStudentsAsync()
    {
        List<Student> activeStudents = new List<Student>();

        var students = await _studentRepository.GetAllAsync();

        foreach (var item in students)
        {
            if (item.DeletedDate == null)
            {
                activeStudents.Add(item);
            }
        }

        return activeStudents.Any()
            ? new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(activeStudents), Messages.ListedSuccess)
            : new ErrorDataResult<List<StudentListDto>>(Messages.StudentNotFound);
    }

    /// <summary>
    /// Parametre olarak girilen ad, soyad veya mail adresine göre öğrenci listesindeki öğrencileri filtreleyen metot.
    /// </summary>
    /// <param name="name">Öğrenci adına karşılık gelen değişken</param>
    /// <param name="surname">Öğrenci soyadına karşılık gelen değişken</param>
    /// <param name="mailAdress">Öğrenci mail adresine karşılık gelen değişken</param>
    /// <returns></returns>
    public async Task<IDataResult<List<StudentListDto>>> GetStudentsByNameOrSurnameOrMailAdressAsync(string? name, string? surname, string? mailAdress)
    {
        var studentsByName = await _studentRepository.GetAllAsync(x => x.FirstName.Contains(name));
        var studentsBySurname = await _studentRepository.GetAllAsync(x => x.LastName.Contains(surname));
        var studentsByMailAdress = await _studentRepository.GetAllAsync(x => x.Email.Contains(mailAdress));

        var filteredStudents = IntersectNonEmpty(studentsByName, studentsBySurname, studentsByMailAdress);

        return filteredStudents.Any()
            ? new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(filteredStudents), Messages.ListedSuccess)
            : new ErrorDataResult<List<StudentListDto>>(Messages.StudentNotFound);
    }

    /// <summary>
    /// Parametre olarak girilen listelerin içindeki ortak elemanlarını, listeler boşsa boş liste döndürür.
    /// </summary>
    /// <typeparam name="T">Listenin tipine karşılık gelen parametre</typeparam>
    /// <param name="lists">Listelere karşılık gelen parametre</param>
    /// <returns></returns>
    private static IEnumerable<T> IntersectNonEmpty<T>(params IEnumerable<T>[] lists)
    {
        var nonEmptyLists = lists.Where(list => list != null && list.Any()).ToList();

        if (nonEmptyLists.Count == 0)
        {
            return new List<T>();
        }

        IEnumerable<T> result = nonEmptyLists[0];

        for (int i = 1; i < nonEmptyLists.Count; i++)
        {
            result = result.Intersect(nonEmptyLists[i]).ToList();

            if (!result.Any())
            {
                break;
            }
        }

        return result;
    }

    public async Task<IDataResult<StudentDetailsDto>> GetStudentDetailsByIdAsync(Guid id)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student is null)
        {
            return new ErrorDataResult<StudentDetailsDto>(Messages.StudentNotFound);
        }
        var studentDetailsDto = new SuccessDataResult<StudentDetailsDto>(_mapper.Map<StudentDetailsDto>(student), Messages.StudentFoundSuccess);

        return studentDetailsDto;
    }

    public async Task<IDataResult<StudentDto>> AddAsync(StudentCreateDto studentCreateDto)
    {
        if (await _studentRepository.AnyAsync(x => x.Email == studentCreateDto.Email))
        {
            return new ErrorDataResult<StudentDto>(Messages.EmailDuplicate);
        }

        //Student student = _mapper.Map<Student>(studentCreateDto);
        Student student = studentCreateDto.Adapt<Student>();
        if (studentCreateDto.ClassroomIds.Count() >= 1)
        {
            //var studentResult = await _studentRepository.AddAsync(student);
            foreach (var item in studentCreateDto.ClassroomIds)
            {
                StudentClassroomDto studentClassroomDto = new StudentClassroomDto()
                {
                    StudentId = student.Id,
                    ClassroomId = item
                };
                student.StudentClassrooms.Add(studentClassroomDto.Adapt<StudentClassroom>());
            }

        }

        IdentityUser identityUser = new()
        {
            Email = studentCreateDto.Email,
            EmailConfirmed = true,
            UserName = studentCreateDto.Email
        };
        DataResult<StudentDto> result = new ErrorDataResult<StudentDto>();
        var strategy = await _studentRepository.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transactionScope = await _studentRepository.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var identityResult = await _accountService.CreateUserAsync(identityUser, Roles.Student);
                if (!identityResult.Succeeded)
                {
                    string customErrorMessage = null;
                    if (customErrorMessage != null)
                    {
                        result = new ErrorDataResult<StudentDto>(customErrorMessage);
                    }
                    else
                    {
                        result = new ErrorDataResult<StudentDto>(identityResult.ToString());
                    }
                    transactionScope.Rollback();
                    return;
                }
                if (!identityResult.Succeeded)
                {
                    result = new ErrorDataResult<StudentDto>(identityResult.ToString());
                    transactionScope.Rollback();
                    return;
                }
                Student student = _mapper.Map<Student>(studentCreateDto);
                student.IdentityId = identityUser.Id;
                await _studentRepository.AddAsync(student);
                result = new SuccessDataResult<StudentDto>(_mapper.Map<StudentDto>(student),
                    Messages.AddSuccess);
                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                result = new ErrorDataResult<StudentDto>($"{Messages.AddFail} - {ex.Message}");
                transactionScope.Rollback();
            }
            finally
            {
                transactionScope.Dispose();
            }
        });
        return result;

    }
   
    public async Task<IDataResult<StudentDto>> UpdateAsync(StudentUpdateDto studentUpdateDto)
    {
        var student = await _studentRepository.GetByIdAsync(studentUpdateDto.Id);

        if (student is null)
        {
            return new ErrorDataResult<StudentDto>(Messages.StudentNotFound);
        }

        var updatedStudent = _mapper.Map(studentUpdateDto, student);

        await _studentRepository.UpdateAsync(updatedStudent);
        await _studentRepository.SaveChangesAsync();

        return new SuccessDataResult<StudentDto>(_mapper.Map<StudentDto>(updatedStudent), Messages.UpdateSuccess);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student is null)
        {
            return new ErrorResult(Messages.UserNotFound);
        }

        await _studentRepository.DeleteAsync(student);
        await _studentRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccessRedirect);
    }

    public async Task<bool> IsGraduatedAsync(string identityId)
    {
        var student = await _studentRepository.GetAsync(x => x.IdentityId == identityId);

        return student?.GraduatedDate?.Date < DateTime.UtcNow.Date ? true : false;
    }

    public async Task<IDataResult<StudentDto>> GetByEmailAsync(string email)
    {
        // Öğrenci veritabanında var mı kontrol et
        var student = await _studentRepository.GetAsync(s => s.Email == email);
        if (student == null)
        {
            return new ErrorDataResult<StudentDto>("Öğrenci bulunamadı.");
        }

        // Öğrenci DTO'suna map et ve başarılı sonucu dön
        var studentDto = _mapper.Map<StudentDto>(student);
        return new SuccessDataResult<StudentDto>(studentDto, "Öğrenci başarıyla bulundu.");
    }


    public async Task<IDataResult<List<StudentListDto>>> GetActiveStudentsByClassroomIdAsync(Guid id)
    {
        var students = await _studentRepository.GetAllAsync(x => x.StudentClassrooms
        .Select(x => x.ClassroomId)
        .Any(x => x == id));

        var classroomStudents = await _studentClassroomRepository.GetActiveStudentsByClassroomIdAsync(id);
        List<Student> activeStudents = new List<Student>();

        foreach (var student in students)
        {
            if (classroomStudents.Any(x => x.StudentId == student.Id) && student.DeletedBy == null)
            {
                activeStudents.Add(student);
            }
        }

        return activeStudents.Any()
            ? new SuccessDataResult<List<StudentListDto>>(_mapper.Map<List<StudentListDto>>(activeStudents), Messages.ListedSuccess)
            : new ErrorDataResult<List<StudentListDto>>(Messages.StudentNotFound);
    }

    public async Task<IResult> ChangeProfilePicture(Guid id, string base64Image)
    {
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null)
        {
            return new ErrorResult();
        }

        student.Image = base64Image;

        await _studentRepository.UpdateAsync(student);
        await _studentRepository.SaveChangesAsync();

        return new SuccessResult();
    }

    public async Task RemoveProfilePictureAsync(Guid studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student != null)
        {
            student.Image = null;
            await _studentRepository.UpdateAsync(student);
            await _studentRepository.SaveChangesAsync();
        }
    }
}
