using AutoMapper;
using BAExamApp.Dtos.TrainerClassrooms;
using BAExamApp.Dtos.Trainers;
using BAExamApp.MVC.Areas.Admin.Models.ClassroomVMs;
using BAExamApp.MVC.Areas.Trainer.Models.ClassroomVMs;
using BAExamApp.MVC.Areas.Trainer.Models.TrainerVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BAExamApp.MVC.Areas.Trainer.Controllers;

public class TrainerController : TrainerBaseController
{
    private readonly ITrainerService _trainerService;
    private readonly IMapper _mapper;
    private readonly IExamEvaluatorService _examsEvaluatorsService;
    private readonly ITrainerClassroomService _trainerClassroomService;
    private readonly IClassroomService _classroomService;

    public TrainerController(ITrainerService trainerService, IMapper mapper, IExamEvaluatorService examsEvaluatorsService,ITrainerClassroomService trainerClassroomService,IClassroomService classroomService)
    {
        _trainerService = trainerService;
        _mapper = mapper;
        _examsEvaluatorsService = examsEvaluatorsService;
        _trainerClassroomService = trainerClassroomService;
        _classroomService = classroomService;
    }


    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var trainerResult = await _trainerService.GetDetailsByIdentityIdForTrainerAsync(UserIdentityId);

        if (trainerResult.IsSuccess)
        {
            return View(_mapper.Map<TrainerTrainerDetailVM>(trainerResult.Data));
        }

        NotifyErrorLocalized(trainerResult.Message);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Update()
    {
        var trainerFoundResult = await _trainerService.GetByIdentityIdAsync(UserIdentityId);

        if (!trainerFoundResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerFoundResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var trainerUpdateVM = _mapper.Map<TrainerTrainerUpdateVM>(trainerFoundResult.Data);


        return View(trainerUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(TrainerTrainerUpdateVM trainerUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            return View(trainerUpdateVM);
        }

        var trainerUpdateDto = _mapper.Map<TrainerUpdateDto>(trainerUpdateVM);
        //if (trainerUpdateVM.ImageFile is not null)
        //{
        //    trainerUpdateDto.Image = await trainerUpdateVM.ImageFile.FileToStringAsync();
        //}

        var result = await _trainerService.UpdateAsync(trainerUpdateDto);
        if (result.IsSuccess)
        {
            NotifySuccessLocalized(result.Message);
            return RedirectToAction("Index", "Home");
        }

        NotifyErrorLocalized(result.Message);
        return View(trainerUpdateVM);
    }



    //Bu Action ExamRule'a sınav tipi eklendikten sonra düzeltilecek. Bu Action'ın amacı otomatik puanlanmayacak sınavlara atanan Evaluator'ların okumayı tamamladığı sınavları listelemektir.
    //[HttpGet]
    //public async Task<IActionResult> GetAssignedExamsForTrainer()
    //{
    //    var trainerId = (await _trainerService.GetByIdentityIdAsync(UserIdentityId)).Data.Id;

    //    var result = await _examsEvaluatorsService.GetAllByTrainerIdAsync(trainerId);

    //    return View(_mapper.Map<List<TrainerExamEvaluatorListVM>>(result.Data));
    //}

    //Bu Action ExamRule'a sınav tipi eklendikten sonra düzeltilecek. Bu Action'ın amacı otomatik puanlanmayacak sınavlara atanan Evaluator'ların henüz okumadığı sınavları listelemektir.
    //[HttpGet]
    //public async Task<IActionResult> GetUnfinishedAssignedExamsForTrainer(int itemsPerPage = 10, int page = 1)
    //{
    //    var trainerId = (await _trainerService.GetByIdentityIdAsync(UserIdentityId)).Data.Id;

    //    var result = (await _examsEvaluatorsService.GetAllByTrainerIdAsync(trainerId)).Data.Where(x => x.ExamDateTime < DateTime.Now);

    //    return View(_mapper.Map<List<TrainerExamEvaluatorListVM>>(result));
    //}
    [HttpGet]
    public async Task<IActionResult> AddClassroom(Guid id)
    {
        TrainerAddedToClassromByAdminVM viewModel = new()
        {
            TrainerId = id,
            Classrooms = await GetClassroomsAsync(id)
        };
        //try
        //{
        //    viewModel.AppointedTrainersId = (await _trainerClassroomService.GetTrainersWithSpesificClassroomIdAsync(id))
        //        .Data
        //        .Select(x => x.Id.ToString())
        //        .ToList();
        //}
        //catch (Exception)
        //{
        //    viewModel.AppointedTrainersId = new List<string>();
        //}
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddClassroom(TrainerAddedToClassromByAdminVM viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.Classrooms = await GetTrainersAsync(viewModel.TrainerId);
            return View(viewModel);
        }

        var addTrainerResponse = await _trainerClassroomService.AddClassRoomsToTrainers(_mapper.Map<TrainerAddedToClassroomByAdminDto>(viewModel));
        if (addTrainerResponse.IsSuccess)
        {
            NotifySuccessLocalized(addTrainerResponse.Message);
        }
        else
        {
            NotifyErrorLocalized(addTrainerResponse.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetTrainersAsync(Guid classroomId)
    {
        var getFreeTrainersResponse = await _trainerService.GetAllActiveAsync();
        if (getFreeTrainersResponse.IsSuccess)
        {
            var trainerList = getFreeTrainersResponse.Data.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName,
            }).ToList();

            return trainerList;
        }
        return new List<SelectListItem>();
    }
    private async Task<List<SelectListItem>> GetClassroomsAsync(Guid trainerId)
    {
        var alLClassrooms = await _classroomService.GetAllAsync();
        if (alLClassrooms.IsSuccess)
        {
            var classList = alLClassrooms.Data.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToList();

            return classList;
        }
        return new List<SelectListItem>();
    }
}