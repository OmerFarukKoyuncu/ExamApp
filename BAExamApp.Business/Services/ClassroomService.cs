using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Core.Enums;
using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.Classrooms;
using BAExamApp.Dtos.Products;
using BAExamApp.Dtos.StudentExams;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Entities.DbSets;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BAExamApp.Business.Services;

public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepository;
    private readonly IMapper _mapper;
    private readonly IStudentExamRepository _studentExamRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentClassroomRepository _studentClassroomRepository;
    private readonly IClassroomProductRepository _classroomProductRepository;
    private readonly IExamClassroomsRepository _examClassroomRepository;
    private readonly ITrainerClassroomRepository _trainerClassroomRepository;
    public ClassroomService(IClassroomRepository classroomRepository, IMapper mapper, IStudentExamRepository studentExamRepository, IStudentRepository studentRepository, IStudentClassroomRepository studentClassroomRepository, IClassroomProductRepository classroomProductRepository, IExamClassroomsRepository examClassroomRepository, ITrainerClassroomRepository trainerClassroomRepository)
    {
        _studentExamRepository = studentExamRepository;
        _classroomRepository = classroomRepository;
        _mapper = mapper;
        _studentRepository = studentRepository;
        _studentClassroomRepository = studentClassroomRepository;
        _classroomProductRepository = classroomProductRepository;
        _examClassroomRepository = examClassroomRepository;
        _trainerClassroomRepository = trainerClassroomRepository;
    }
    
    public async Task<IDataResult<ClassroomDto>> GetByIdAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom != null)
        {
            return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(classroom), Messages.ClassroomFoundSuccess);
        }

        return new ErrorDataResult<ClassroomDto>(Messages.ClassroomNotFound);
    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetAllAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        return new SuccessDataResult<List<ClassroomListDto>>(classrooms.Adapt<List<ClassroomListDto>>(), Messages.ListedSuccess);
    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetAllByIdentityIdAsync(string id)
    {
        var classrooms = await _classroomRepository.GetAllAsync(
            x => x.TrainerClassrooms.Any(tc => tc.Trainer.IdentityId == id),
            true, // assuming tracking is enabled
            query => query.Include(x => x.TrainerClassrooms).ThenInclude(tc => tc.Trainer),
            query => query.Include(x => x.ExamClassrooms).ThenInclude(ec => ec.Exam)
            );

        List<ClassroomListDto> classroomListDtos = new List<ClassroomListDto>();

        foreach (var classroom in classrooms)
        {
            ClassroomListDto classroomListDto = new ClassroomListDto();

            classroomListDto.BranchName = classroom.Branch.Name;
            classroomListDto.ClassroomExamCount = classroom.ExamClassrooms.Count;
            classroomListDto.ClassroomAppointedExamCount = classroom.ExamClassrooms.Count(x => x.Exam.IsStarted == true);
            classroomListDto.StudentCount = classroom.StudentClassrooms.Count(c => c.Status != Core.Enums.Status.Deleted);

            classroom.Adapt(classroomListDto);

            classroomListDtos.Add(classroomListDto);
        }

        return new SuccessDataResult<List<ClassroomListDto>>(classroomListDtos, Messages.ListedSuccess);
    }

    /// <summary>
    /// Admin panelinde sınıfları; sınıf ID'sine, şube ID'sine, grup türüne, açılış ve kapanış tarihine göre filtreleyerek getiren metottur.
    /// Yalnızca silinmemiş (Status != Deleted) sınıflar döndürülür.
    /// </summary>
    /// <param name="name">Filtrelenecek sınıfın ID'si (opsiyonel).</param>
    /// <param name="branchName">Filtrelenecek şubenin ID'si (opsiyonel).</param>
    /// <param name="groupType">Filtrelenecek grup türünün ID'si (opsiyonel).</param>
    /// <param name="openingDate">Başlangıç tarihi filtresi (>=) (opsiyonel).</param>
    /// <param name="closedDate">Bitiş tarihi filtresi (<=) (opsiyonel).</param>
    /// <param name="takeCount">Getirilecek maksimum kayıt sayısı.</param>
    /// <returns>Filtrelenmiş sınıf listesini içeren veri sonucu.</returns>
    public async Task<IDataResult<List<ClassroomListDto>>> GetFilteredByNameOrBranchNameOrGroupTypeOrOpeningDateOrClosedDateAsync(
      string name, string branchName, string groupType, DateTime openingDate, DateTime closedDate, int takeCount)
    {
        if (string.IsNullOrEmpty(name) &&
            string.IsNullOrEmpty(branchName) &&
            string.IsNullOrEmpty(groupType) &&
            openingDate == default &&
            closedDate == default)
        {
            return new SuccessDataResult<List<ClassroomListDto>>(new List<ClassroomListDto>(), Messages.ListedSuccess);
        }

        Expression<Func<Classroom, bool>> filter = x =>
        (string.IsNullOrEmpty(name) || x.Id.ToString() == name) &&
        (string.IsNullOrEmpty(branchName) || x.Branch.Id.ToString() == branchName) &&
        (string.IsNullOrEmpty(groupType) || x.GroupType.Id.ToString() == groupType) &&
        (openingDate == default || x.OpeningDate.Date >= openingDate.Date) &&
        (closedDate == default || x.ClosedDate.Date <= closedDate.Date) && (x.Status != Status.Deleted);

        Expression<Func<Classroom, DateTime>> orderby = x => x.CreatedDate;

        var getList = await _classroomRepository.GetAllAsync(
        expression: filter,
        orderby: orderby,
        skip: 0,
        take: 10,
        orderDesc: false,
        tracking: true
        );

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(getList), Messages.ListedSuccess);

    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetActiveAsync()
    {
        List<Classroom> activeClassrooms = new List<Classroom>();

        var classrooms = await _classroomRepository.GetAllAsync();

        foreach (var item in classrooms)
        {
            if (item.IsActive)
            {
                activeClassrooms.Add(item);
            }
        }

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(activeClassrooms), Messages.ListedSuccess);
    }

    public async Task<IDataResult<ClassroomDetailsDto>> GetDetailsByIdAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDetailsDto>(Messages.ClassroomNotFound);
        }

        return new SuccessDataResult<ClassroomDetailsDto>(_mapper.Map<ClassroomDetailsDto>(classroom), Messages.ClassroomFoundSuccess);
    }

    public async Task<IDataResult<ClassroomDetailsForAdminDto>> GetDetailsByIdForAdminAsync(Guid id)
    {
        decimal? studentExamAvg;
        var classroom = await _classroomRepository.GetByIdAsync(id);
        if (classroom is null)
            return new ErrorDataResult<ClassroomDetailsForAdminDto>(Messages.ClassroomNotFound);

        var classroomDetailsForAdminDto = _mapper.Map<ClassroomDetailsForAdminDto>(classroom);
        foreach (var student in classroomDetailsForAdminDto.StudentClassrooms)
        {
            var studentExams = await _studentExamRepository.GetAllAsync(x => x.StudentId == student.Id);
            var studentExamsDetails = _mapper.Map<List<StudentExamsDetailsDto>>(studentExams);
            var studentExamsCount = studentExamsDetails.Count();
            decimal? totalStudentExamScore = 0;


            foreach (var studentExamDetail in studentExamsDetails)
            {
                if (studentExamDetail.Score is null)
                    studentExamDetail.Score = 0;
                totalStudentExamScore += studentExamDetail.Score;
            }
            studentExamAvg = studentExamsCount == 0 ? studentExamAvg = 0 : (totalStudentExamScore / studentExamsCount);
            student.StudentExamAvg = studentExamAvg;
        }

        return new SuccessDataResult<ClassroomDetailsForAdminDto>(classroomDetailsForAdminDto, Messages.ClassroomFoundSuccess);
    }

    public async Task<IDataResult<ClassroomDto>> AddAsync(ClassroomCreateDto classroomCreateDto)
    {
        var hasClassroom = await _classroomRepository.AnyAsync(classroom => classroom.Name.ToLower() == classroomCreateDto.Name.ToLower());

        if (hasClassroom)
        {
            return new ErrorDataResult<ClassroomDto>(Messages.AddFailAlreadyExists);
        }

        var classroom = _mapper.Map<Classroom>(classroomCreateDto);

        await _classroomRepository.AddAsync(classroom);
        await _classroomRepository.SaveChangesAsync();

        return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(classroom), Messages.AddSuccess);
    }

    public async Task<IDataResult<ClassroomDto>> UpdateAsync(ClassroomUpdateDto classroomUpdateDto)
    {
        var classroom = await _classroomRepository.GetByIdAsync(classroomUpdateDto.Id);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDto>(Messages.ClassroomNotFound);
        }

        var updatedClassroom = _mapper.Map(classroomUpdateDto, classroom);


        if (classroomUpdateDto.ProductIds != null)
        {
            UpdateClassroomProducts(classroom, classroomUpdateDto);
        }


        await _classroomRepository.UpdateAsync(updatedClassroom);
        await _classroomRepository.SaveChangesAsync();

        return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(updatedClassroom), Messages.UpdateSuccess);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom is null)
        {
            return new ErrorResult(Messages.ClassroomNotFound);
        }

        var productUsingClassroom = await IsClassroomUsedProductAsync(classroom.Id);
        var examUsingClassroom = await IsClassroomUsedExamAsync(classroom.Id);
        var studentUsingClassroom = await IsClassroomUsedStudentAsync(classroom.Id);
        var trainerUsingClassroom = await IsClassroomUsedTrainerAsync(classroom.Id);

        if (productUsingClassroom)
        {
            var clasroomProducts = await _classroomProductRepository.GetAllAsync(x => x.ClassroomId == classroom.Id);
            foreach (var clasroomProduct in clasroomProducts)
            {
                await _classroomProductRepository.DeleteAsync(clasroomProduct);
            }

            await _classroomProductRepository.SaveChangesAsync();
        }

        if (examUsingClassroom)
        {
            var examClassrooms = await _examClassroomRepository.GetAllAsync(x => x.ClassroomId == classroom.Id);
            foreach (var examClassroom in examClassrooms)
            {
                await _examClassroomRepository.DeleteAsync(examClassroom);
            }
            await _examClassroomRepository.SaveChangesAsync();
        }

        if (studentUsingClassroom)
        {
            var studentClassrooms = await _studentClassroomRepository.GetAllAsync(x => x.ClassroomId == classroom.Id);
            foreach (var studentClassroom in studentClassrooms)
            {
                await _studentClassroomRepository.DeleteAsync(studentClassroom);
            }
            await _studentClassroomRepository.SaveChangesAsync();
        }

        if (trainerUsingClassroom)
        {
            var trainerClassrooms = await _trainerClassroomRepository.GetAllAsync(x => x.ClassroomId == classroom.Id);
            foreach (var trainerClassroom in trainerClassrooms)
            {
                await _trainerClassroomRepository.DeleteAsync(trainerClassroom);
            }
            await _studentClassroomRepository.SaveChangesAsync();
        }

        await _classroomRepository.DeleteAsync(classroom);
        await _classroomRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    public async Task<bool> IsClassroomUsedProductAsync(Guid classroomId)
    {
        var productUsingClassroom = await _classroomProductRepository.AnyAsync(e => e.ClassroomId == classroomId);
        return productUsingClassroom;
    }

    public async Task<bool> IsClassroomUsedExamAsync(Guid classroomId)
    {
        var examUsingClassroom = await _examClassroomRepository.AnyAsync(e => e.ClassroomId == classroomId);
        return examUsingClassroom;
    }

    public async Task<bool> IsClassroomUsedStudentAsync(Guid classroomId)
    {
        var studentUsingClassroom = await _studentClassroomRepository.AnyAsync(e => e.ClassroomId == classroomId);
        return studentUsingClassroom;
    }

    public async Task<bool> IsClassroomUsedTrainerAsync(Guid classroomId)
    {
        var trainerUsingClassroom = await _trainerClassroomRepository.AnyAsync(e => e.ClassroomId == classroomId);
        return trainerUsingClassroom;
    }
    private void UpdateClassroomProducts(Classroom classroom, ClassroomUpdateDto classroomUpdateDto)
    {
        foreach (var productId in classroomUpdateDto.ProductIds)
        {
            if (!classroom.ClassroomProducts.Any(x => x.ProductId == productId))
            {
                var classroomProduct = new ClassroomProduct
                {
                    ProductId = productId,
                    ClassroomId = classroom.Id
                };
                classroom.ClassroomProducts.Add(classroomProduct);
            }
        }
        foreach (var product in classroom.ClassroomProducts)
        {
            if (!classroomUpdateDto.ProductIds.Any(x => x.Equals(product.ProductId)))
            {
                classroom.ClassroomProducts.Remove(product);
            }
        }
    }

    public async Task<IDataResult<ClassroomDto>> GetAsync(Expression<Func<Classroom, bool>> expression)
    {
        var classroom = await _classroomRepository.GetAsync(expression);

        if (classroom is null)
        {
            return new ErrorDataResult<ClassroomDto>(Messages.ClassroomNotFound);
        }

        return new SuccessDataResult<ClassroomDto>(_mapper.Map<ClassroomDto>(classroom), Messages.ClassroomFoundSuccess);
    }


    /// <summary>
    /// Filtreyi doldurmak için tüm sınıfları isim ve id'sine göre getirir.
    /// </summary>
    /// <returns>Liste tipinde ClassroomFilterDto döndürür</returns>
    public async Task<IDataResult<List<ClassroomFilterDto>>> GetAllClassroomByFilter()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        return new SuccessDataResult<List<ClassroomFilterDto>>(_mapper.Map<List<ClassroomFilterDto>>(classrooms), Messages.ListedSuccess);
    }

    public async Task<bool> HasRelationship(Guid id)
    {
        bool hasRelationship = await _classroomRepository.HasRelate(id);
        return hasRelationship;
    }

    /// <summary>
    /// Sınıf kapanış tarihi tamamlanmamış, aktif olan sınıfları getiren metoddur.
    /// </summary>
    /// <returns>Aktif sınıfları temsil eden bir <see cref="ClassroomListDto"/> listesi döndürür.</returns>
    public async Task<IDataResult<List<ClassroomListDto>>> GetActiveClassroomsAsync()
    {
        List<Classroom> activeClassrooms = new List<Classroom>();

        var classrooms = await _classroomRepository.GetAllActiveClassrooms(x => x.IsActive);

        foreach (var item in classrooms)
        {
            if (item.IsActive)
            {
                activeClassrooms.Add(item);
            }
        }

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(activeClassrooms), Messages.ListedSuccess);
    }
    /// <summary>
    /// Bir öğrencinin dahil olduğu eğitimlerin tamamı bittiği durumda öğrencinin durumunu pasife çeken metotdur. Hangfire ile background job olarak çalışacaktır.
    /// </summary>
    /// <returns></returns>
    public async Task UpdateStudentStatusIfClassroomClosedAsync()
    {
        // Aktif olmayan sınıfları bir IEnumerable listeye atar.
        var closedClassrooms = await _classroomRepository.GetAllAsync(x => x.ClosedDate < DateTime.Now);

        var studentsToUpdate = closedClassrooms
            .SelectMany(classroom => classroom.StudentClassrooms)
            .Where(sc => sc.Status == Status.Active)
            .ToList();

        if (studentsToUpdate.Any())
        {
            // statüs ü güncellenmesi gereken öğrenciler için hashset oluşturur.
            var studentsToUpdateStatus = new HashSet<Student>();

            // Güncellenmesi gereken öğrencileri listeye ekler.
            foreach (var studentClassroom in studentsToUpdate)
            {
                studentClassroom.Status = Status.Passive;
                studentClassroom.ModifiedDate = DateTime.UtcNow;

                studentsToUpdateStatus.Add(studentClassroom.Student);
            }

            await _studentClassroomRepository.UpdateRangeAsync(studentsToUpdate);

            // Eğer öğrencinin dahil olduğu bütün sınıflar kapanmışsa statüsünü pasife çeker
            foreach (var student in studentsToUpdateStatus)
            {
                // Öğrencinin dahil olduğu bütün sınıfların pasif olup olmadığının kontrolünü yapar. Eğer hepsi pasifse true döner.
                bool allClassroomsPassive = student.StudentClassrooms.All(sc => sc.Status == Status.Passive);

                if (allClassroomsPassive)
                {
                    student.Status = Status.Passive;
                    student.ModifiedDate = DateTime.UtcNow.AddDays(1);
                    student.GraduatedDate = DateTime.Now.AddDays(1);
                    await _studentRepository.UpdateAsync(student);
                }
            }

            await _studentRepository.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Belirtilen sınıfın durumunu 'Passive' (Pasif) olarak günceller ve kapanış tarihini güncel tarih olarak ayarlar.
    /// Eğer sınıf zaten pasif durumda ise işlem gerçekleştirilmez.
    /// </summary>
    public async Task<IResult> MakePassiveAsync(Guid id)
    {
        var classroom = await _classroomRepository.GetByIdAsync(id);

        if (classroom == null || classroom.Status == Status.Passive)
            return new ErrorResult("Sınıf bulunamadı veya zaten pasif.");

        classroom.Status = Status.Passive;
        classroom.ClosedDate = DateTime.Now;

        await _classroomRepository.UpdateAsync(classroom);

        // İlgili Eğitmen-Sınıf ilişkilerini bul ve ResignedDate'i güncelle
        var trainerClassroomsToUpdate = await _trainerClassroomRepository.GetAllAsync(tc => tc.ClassroomId == id && tc.ResignedDate == null);

        if (trainerClassroomsToUpdate.Any())
        {
            foreach (var trainerClassroom in trainerClassroomsToUpdate)
            {
                trainerClassroom.ResignedDate = DateTime.Now;
                await _trainerClassroomRepository.UpdateAsync(trainerClassroom);
            }
        }

        // Değişiklikleri kaydet (Classroom ve TrainerClassroom için)
        await _classroomRepository.SaveChangesAsync(); // SaveChangesAsync tüm değişiklikleri kaydeder.

        return new SuccessResult("Sınıf pasife alındı ve eğitmen ilişkileri güncellendi.");
    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetActivelassroomsByStudentIdAsync(Guid studentId)
    {
        List<Classroom> activeClassrooms = new List<Classroom>();

        var classrooms = await _classroomRepository.GetAllActiveClassrooms(x => x.IsActive);

        foreach (var item in classrooms)
        {
            if (item.StudentClassrooms.Any(sc => sc.StudentId == studentId))
            {
                activeClassrooms.Add(item);
            }
        }

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(activeClassrooms), Messages.ListedSuccess);
    }

    /// <summary>
    /// Admin paneli sınıf listeleme (index) sayfası için gerekli olan tüm sınıf kayıtlarının ID'sini ve İsimlerini getirir.
    /// Silinmiş sınıflar hariç tutulur.
    /// </summary>
    /// <returns>Başarılı listeleme sonucu ile birlikte sınıf filtreleme DTO'larının listesi.</returns>
    public async Task<IDataResult<List<ClassroomFilterDto>>> GetAllForIndexAsync()
    {
        var classrooms = await _classroomRepository.GetAllAsync();

        var result = classrooms
            .Select(c => new
            {
                c.Id,
                c.Name,
            })
            .Select(c => new ClassroomFilterDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToList();

        return new SuccessDataResult<List<ClassroomFilterDto>>(result, Messages.ListedSuccess);
    }

    public async Task<IDataResult<List<ClassroomListDto>>> GetClassroomsForAutoComplete(string term)
    {
        //var classrooms = await _classroomRepository.GetAllAsync(x=>x.Name.ToLower().Contains(term.ToLower()), x=>x.Name, 0, 10);
        var classrooms = await _classroomRepository.GetClassroomsForAutoCompleteAsync(term);
        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(classrooms), Messages.ListedSuccess);
    }
}
