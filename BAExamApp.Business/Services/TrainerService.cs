using BAExamApp.Core.Enums;
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.Classrooms;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Entities.DbSets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BAExamApp.Business.Services;

public class TrainerService : ITrainerService
{
    private readonly ITrainerRepository _trainerRepository;
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly ITrainerClassroomRepository _trainerClassroomRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly IExamRepository _examRepository;
    private readonly IClassroomService _classroomService;
    private readonly ITrainerProductRepository _trainerProductRepository;
    private readonly IExamEvaluatorRepository _examEvaluatorRepository;
    public TrainerService(ITrainerRepository trainerRepository, IAccountService accountService, IMapper mapper, IEmailService emailService, IClassroomRepository classroomRepository, ITrainerClassroomRepository trainerClassroomRepository, IExamRepository examRepository, IClassroomService classroomService, ITrainerProductRepository trainerProductRepository, IExamEvaluatorRepository examEvaluatorRepository)
    {
        _trainerRepository = trainerRepository;
        _accountService = accountService;
        _mapper = mapper;
        _emailService = emailService;
        _classroomRepository = classroomRepository;
        _trainerClassroomRepository = trainerClassroomRepository;
        _examRepository = examRepository;
        _classroomService = classroomService;
        _trainerProductRepository = trainerProductRepository;
        _examEvaluatorRepository = examEvaluatorRepository;
    }
    /// <summary>
    /// Eğitmen Ekler
    /// </summary>
    /// <param name="trainerCreateDto">Eklenecek eğitmen bilgilerini içeren DTO nesnesi.</param>
    public async Task<IDataResult<TrainerDto>> AddAsync(TrainerCreateDto trainerCreateDto)
    {
        if (await _accountService.AnyAsync(x => x.Email == trainerCreateDto.Email))
        {
            return new ErrorDataResult<TrainerDto>(Messages.EmailDuplicate);
        }

        if (trainerCreateDto.OtherEmails.Count != 0)
        {
            foreach (var email in trainerCreateDto.OtherEmails)
            {
                if (await _accountService.AnyAsync(x => x.Email == email) || await _emailService.AnyAsync(x => x.EmailAddress == email))
                {
                    return new ErrorDataResult<TrainerDto>(Messages.EmailDuplicate);
                }
                ;
            }
        }

        IdentityUser identityUser = new()
        {
            Email = trainerCreateDto.Email,
            EmailConfirmed = true, // TODO: Email confirmation yapılırsa burası değiştirilmeli.
            UserName = trainerCreateDto.Email
        };

        DataResult<TrainerDto> result = new ErrorDataResult<TrainerDto>();

        var strategy = await _trainerRepository.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transactionScope = await _trainerRepository.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var identityResult = await _accountService.CreateUserAsync(identityUser, Roles.Trainer);
                if (!identityResult.Succeeded)
                {
                    string customErrorMessage = null;

                    if (customErrorMessage != null)
                    {
                        result = new ErrorDataResult<TrainerDto>(customErrorMessage);
                    }
                    else
                    {
                        result = new ErrorDataResult<TrainerDto>(identityResult.ToString());
                    }

                    transactionScope.Rollback();
                    return;
                }

                if (!identityResult.Succeeded)
                {
                    result = new ErrorDataResult<TrainerDto>(identityResult.ToString());
                    transactionScope.Rollback();
                    return;
                }

                Trainer trainer = _mapper.Map<Trainer>(trainerCreateDto);
                trainer.IdentityId = identityUser.Id;
                AddTrainerProducts(trainer, trainerCreateDto);
                await _trainerRepository.AddAsync(trainer);

                result = new SuccessDataResult<TrainerDto>(_mapper.Map<TrainerDto>(trainer), Messages.AddSuccess);
                transactionScope.Commit();
            }
            catch (Exception ex)
            {
                result = new ErrorDataResult<TrainerDto>($"{Messages.AddFail} - {ex.Message}");
                transactionScope.Rollback();
            }
            finally
            {
                transactionScope.Dispose();
            }
        });

        return result;
    }

    /// <summary>
    /// Admin Rol değişimindeki Eğitmen Ekleme İşlemi. Aksi Halde ASLA!!!!!!!! Kullanmayın
    /// </summary>
    /// <param name="trainerCreateDto">Eklenecek eğitmen bilgilerini içeren DTO nesnesi.</param>
    public async Task<IDataResult<TrainerDto>> RoleAddAsync(TrainerCreateDto trainerCreateDto)
    {
        DataResult<TrainerDto> result = new ErrorDataResult<TrainerDto>();

        var strategy = await _trainerRepository.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transactionScope = await _trainerRepository.BeginTransactionAsync().ConfigureAwait(false);
            try
            {

                Trainer trainer = _mapper.Map<Trainer>(trainerCreateDto);
                trainer.IdentityId = trainerCreateDto.IdentityId;
                trainer.Status = Status.Active;
                //AddTrainerProducts(trainer, trainerCreateDto);
                await _trainerRepository.AddAsync(trainer);
                await _trainerRepository.SaveChangesAsync();
                transactionScope.CommitAsync();
                result = new SuccessDataResult<TrainerDto>(_mapper.Map<TrainerDto>(trainer), Messages.AddSuccess);

            }
            catch (Exception ex)
            {
                transactionScope.RollbackAsync();
                result = new ErrorDataResult<TrainerDto>($"{Messages.AddFail} - {ex.Message}");
            }
            finally
            {
                transactionScope.DisposeAsync();
            }
        });

        return result;
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);

        if (trainer is null)
        {
            return new ErrorResult(Messages.UserNotFound);
        }

        var classroomUsingTrainer = await IsTrainerUsedClassroomAsync(trainer.Id);
        var productUsingTrainer = await IsTrainerUsedProductAsync(trainer.Id);
        var examEvaluatorUsingTrainer = await IsTrainerUsedExamEvaluatorAsync((trainer.Id));

        if (classroomUsingTrainer)
        {
            var classroomTrainers = await _trainerClassroomRepository.GetAllAsync(x => x.TrainerId == trainer.Id);
            foreach (var classroomTrainer in classroomTrainers)
            {
                await _trainerClassroomRepository.DeleteAsync(classroomTrainer);
            }
            await _trainerClassroomRepository.SaveChangesAsync();
        }

        if (productUsingTrainer)
        {
            var productTrainers = await _trainerProductRepository.GetAllAsync(x => x.TrainerId == trainer.Id);
            foreach (var productTrainer in productTrainers)
            {
                await _trainerProductRepository.DeleteAsync(productTrainer);
            }
            await _trainerProductRepository.SaveChangesAsync();
        }

        if (examEvaluatorUsingTrainer)
        {
            var examEvaluators = await _examEvaluatorRepository.GetAllAsync(x => x.TrainerId == trainer.Id);
            foreach (var examEvaluator in examEvaluators)
            {
                await _examEvaluatorRepository.DeleteAsync(examEvaluator);
            }
            await _examEvaluatorRepository.SaveChangesAsync();
        }

        var identityDeleteResult = await _accountService.DeleteUserAsync(trainer.IdentityId!);
        if (!identityDeleteResult.Succeeded)
        {
            return new ErrorResult(identityDeleteResult.ToString());
        }

        await _trainerRepository.DeleteAsync(trainer);
        await _trainerRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    public async Task<bool> IsTrainerUsedClassroomAsync(Guid trainerId)
    {
        var classroomUsingTrainer = await _trainerClassroomRepository.AnyAsync(x => x.TrainerId == trainerId);
        return classroomUsingTrainer;
    }

    public async Task<bool> IsTrainerUsedProductAsync(Guid trainerId)
    {
        var productUsingTrainer = await _trainerProductRepository.AnyAsync(x => x.TrainerId == trainerId);
        return productUsingTrainer;
    }

    public async Task<bool> IsTrainerUsedExamEvaluatorAsync(Guid trainerId)
    {
        var examEvaluatorUsingTrainer = await _examEvaluatorRepository.AnyAsync(x => x.TrainerId == trainerId);
        return examEvaluatorUsingTrainer;
    }

    public async Task<IDataResult<List<TrainerListDto>>> GetAllAsync()
    {
        var trainers = await _trainerRepository.GetAllTrainersNonDeleted();
        return new SuccessDataResult<List<TrainerListDto>>(_mapper.Map<List<TrainerListDto>>(trainers), Messages.ListedSuccess);
    }

    public async Task<IDataResult<TrainerDto>> GetByIdAsync(Guid id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);
        if (trainer is null)
        {
            return new ErrorDataResult<TrainerDto>(Messages.UserNotFound);
        }

        var trainerDto = new SuccessDataResult<TrainerDto>(_mapper.Map<TrainerDto>(trainer), Messages.FoundSuccess);

        return trainerDto;
    }

    public async Task<IDataResult<TrainerDto>> GetByIdentityIdAsync(string identityId)
    {
        var trainer = await _trainerRepository.GetByIdentityIdAsync(identityId);

        if (trainer is null) return new ErrorDataResult<TrainerDto>(Messages.UserNotFound);

        return new SuccessDataResult<TrainerDto>(_mapper.Map<TrainerDto>(trainer), Messages.FoundSuccess);
    }

    /// <summary>
    /// Bu metod, sağlanan kimlik ID'si kullanılarak eğitmeni depodan alır. Ardından durumu "Deleted" olan sınıfları filtreler
    /// ve kalan sınıfları nesnelerinin bir listesine dönüştürür.
    /// </summary>
    public async Task<IDataResult<List<ClassroomListDto>>> GetClassroomsByIdentityId(string identityId)
    {
        var trainer = await _trainerRepository.GetAsync(x => x.IdentityId == identityId);
        if (trainer is null)
            return new ErrorDataResult<List<ClassroomListDto>>(Messages.UserNotFound);
        var classroomList = trainer.TrainerClassrooms
                               .Select(x => x.Classroom)
                               .Where(c => c.Status != Status.Deleted)
                               .ToList();

        return new SuccessDataResult<List<ClassroomListDto>>(_mapper.Map<List<ClassroomListDto>>(classroomList), Messages.ListedSuccess);

    }

    public async Task<IDataResult<List<TrainerDto>>> GetTrainersWithoutSpesificClassroomIdAsync(Guid classroomId)
    {
        var freeTrainers = await _trainerRepository.GetAllAsync(x => x.TrainerClassrooms.Count == 0 || x.TrainerClassrooms.Any(cs => cs.ClassroomId != classroomId));

        if (freeTrainers.Any())
        {
            return new SuccessDataResult<List<TrainerDto>>(_mapper.Map<List<TrainerDto>>(freeTrainers), Messages.ListedSuccess);
        }
        return new ErrorDataResult<List<TrainerDto>>(Messages.NoAvailableTrainer);
    }

    public async Task<IDataResult<TrainerDto>> UpdateAsync(TrainerUpdateDto trainerUpdateDto)
    {
        var trainer = await _trainerRepository.GetByIdAsync(trainerUpdateDto.Id);

        if (trainer is null) return new ErrorDataResult<TrainerDto>(Messages.UserNotFound);

        var updatedTrainer = _mapper.Map(trainerUpdateDto, trainer);

        await _trainerRepository.UpdateAsync(updatedTrainer);

        if (trainerUpdateDto.ProductIds != null)
        {
            UpdateTrainerProducts(trainer, trainerUpdateDto);
        }

        await _trainerRepository.SaveChangesAsync();

        return new SuccessDataResult<TrainerDto>(_mapper.Map<TrainerDto>(updatedTrainer), Messages.UpdateSuccess);
    }


    /// Guid ProductId ye Göre Sınıfa Kayıtlı Eğitmen Var İse Eğitmenleri Getirir.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    public async Task<IDataResult<List<TrainerDto>>> GetTrainersWithSpesificProductIdAsync(Guid productId)
    {

        var trainers = await _trainerRepository.GetAllAsync(x => x.TrainerProducts.Any(x => x.ProductId == productId));

        return trainers.Any()
            ? new SuccessDataResult<List<TrainerDto>>(_mapper.Map<List<TrainerDto>>(trainers), Messages.ListedSuccess)
            : new ErrorDataResult<List<TrainerDto>>(Messages.NoTrainerFoundInClassroom);
    }

    /// Eğer eğitmen mevcutsa eğitmen detaylarını döner.
    /// Eğitmen detayları şunları içerir; FirstName, LastName, Email, IdentificationNumber, Address, DateOfBirth, Gender, Image, Classrooms
    /// </summary>
    /// <param name="id">İlgili sınıf ve detaylarını bulmak için TrainerId gereklidir.</param>
    /// <returns>DataResult<TrainerDetailsDto></returns>
    public async Task<IDataResult<TrainerDetailsDto>> GetTrainerDetailsByIdAsync(Guid id)
    {
        var trainer = await _trainerRepository.GetByIdAsync(id);

        if (trainer is null)
        {
            return new ErrorDataResult<TrainerDetailsDto>(Messages.TrainerNotFound);
        }

        var trainerDetailsDto = new SuccessDataResult<TrainerDetailsDto>(_mapper.Map<TrainerDetailsDto>(trainer), Messages.TrainerFoundSuccess);

        return trainerDetailsDto;
    }

    public async Task<IDataResult<TrainerDetailsForTrainerDto>> GetDetailsByIdentityIdForTrainerAsync(string id)
    {
        var trainer = await _trainerRepository.GetByIdentityIdAsync(id);

        if (trainer is null)
        {
            return new ErrorDataResult<TrainerDetailsForTrainerDto>(Messages.TrainerNotFound);
        }

        return new SuccessDataResult<TrainerDetailsForTrainerDto>(_mapper.Map<TrainerDetailsForTrainerDto>(trainer), Messages.TrainerFoundSuccess);
    }

    ///// <summary>
    ///// Verilen eğitmene ilişkin eğitimleri günceller.
    ///// </summary>
    ///// <param name="trainer">Eğitimleri güncellenecek eğitmen.</param>
    ///// <param name="trainerUpdateDto">Güncellenecek eğitmen bilgilerini içeren DTO nesnesi.</param>
    private void UpdateTrainerProducts(Trainer trainer, TrainerUpdateDto trainerUpdateDto)
    {
        foreach (var productId in trainerUpdateDto.ProductIds)
        {
            if (!trainer.TrainerProducts.Any(x => x.ProductId == productId))
            {
                var trainerProduct = new TrainerProduct
                {
                    ProductId = productId,
                    TrainerId = trainer.Id
                };
                trainer.TrainerProducts.Add(trainerProduct);
            }
        }
        foreach (var product in trainer.TrainerProducts)
        {
            if (!trainerUpdateDto.ProductIds.Any(x => x.Equals(product.ProductId)))
            {
                trainer.TrainerProducts.Remove(product);
            }
        }
    }
    /// <summary>
    /// Verilen eğitmene ilişkin eğitimleri ekler.
    /// </summary>
    /// <param name="trainer">Eğitimleri eklencek eğitmen.</param>
    /// <param name="trainerCreateDto">Eklenecek eğitmen bilgilerini içeren DTO nesnesi.</param>
    private void AddTrainerProducts(Trainer trainer, TrainerCreateDto trainerCreateDto)
    {
        List<TrainerProduct> trainerProducts = new();
        foreach (var productId in trainerCreateDto.ProductIds)
        {
            var trainerProduct = new TrainerProduct
            {
                ProductId = productId,
                TrainerId = trainer.Id
            };
            trainerProducts.Add(trainerProduct);
        }
        trainer.TrainerProducts = trainerProducts;
    }

    public async Task<IResult> AddTrainerClassroomAndProductAsync(List<Guid> trainerIds, Guid? classroomId, List<Guid>? productIds)
    {
        if ((classroomId == null || classroomId == Guid.Empty) && (productIds == null || !productIds.Any()))
        {
            return new ErrorResult("Hem ClassroomId hem ProductIds boş olamaz.");
        }

        if (classroomId.HasValue && classroomId != Guid.Empty)
        {
            foreach (var trainerId in trainerIds)
            {
                var entity = new TrainerClassroom
                {
                    TrainerId = trainerId,
                    ClassroomId = classroomId.Value,
                    CreatedDate = DateTime.UtcNow,
                    ResignedDate= DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,

                };

                await _trainerClassroomRepository.AddAsync(entity);
            }
        }

        if (productIds != null && productIds.Any())
        {
            foreach (var trainerId in trainerIds)
            {
                foreach (var productId in productIds)
                {
                    var entity = new TrainerProduct
                    {
                        TrainerId = trainerId,
                        ProductId = productId,
                        ModifiedDate= DateTime.UtcNow,
                    };

                    await _trainerProductRepository.AddAsync(entity);
                }
            }
        }

        return new SuccessResult("Eğitmenler başarıyla sınıfa ve/veya ürünlere eklendi.");
    }




    public async Task<IDataResult<List<TrainerListDto>>> GetAllActiveAsync()
    {
        var trainers = await _trainerRepository.GetAllAsync(x => x.Status == Status.Active);
        return new SuccessDataResult<List<TrainerListDto>>(_mapper.Map<List<TrainerListDto>>(trainers), Messages.ListedSuccess);
    }


    public async Task<IDataResult<List<TrainerListDto>>> GetAllWithClassroomCountsAsync()
    {
        var trainers = await _trainerRepository.GetAllTrainersNonDeleted();
        var trainerDtos = _mapper.Map<List<TrainerListDto>>(trainers);

        if (trainers == null || trainers.Count == 0)
        {
            return new SuccessDataResult<List<TrainerListDto>>(new List<TrainerListDto>(), Messages.TrainerNotFound);
        }
        else
        {
            foreach (var trainer in trainerDtos)
            {
                var trainerEntity = trainers.First(t => t.Id == trainer.Id);

                var classRooms = trainerEntity.TrainerClassrooms;

                var count = 0;

                if (classRooms != null && classRooms.Count != 0)
                {
                    foreach (var item in classRooms)
                    {
                        if (item.Status != Status.Deleted)
                        {
                            count += 1;
                        }
                    }
                }

                trainer.ClassroomCount = count;
            }

            return new SuccessDataResult<List<TrainerListDto>>(trainerDtos, Messages.ListedSuccess);

        }


    }

    /// <summary>
    /// ROLE DEĞİŞTİRME İŞLEMİ İÇİN KULLANILIR STATUS DEĞİŞTİRME İŞLEMİ
    /// </summary>
    /// <param name="identityId">IdentityID</param>
    /// <param name="make">True veya False yapma durumuna göre Status değiştirme yapar</param>
    /// <returns>AdminDto Nesnesi ve Message</returns>
    public async Task<IDataResult<TrainerDto>> UpdateRoleAsync(string identityId, bool make)
    {
        var trainer = await _trainerRepository.GetByIdentityIdAsync(identityId);
        if (trainer == null)
            return new ErrorDataResult<TrainerDto>(Messages.TrainerNotFound);

        if (make == false)
            trainer.Status = Status.Deleted;
        if (make == true)
            trainer.Status = Status.Active;
        await _trainerRepository.UpdateAsync(trainer);
        await _trainerRepository.SaveChangesAsync();


        return new SuccessDataResult<TrainerDto>(_mapper.Map<TrainerDto>(trainer), Messages.UpdateSuccess);
    }
}

