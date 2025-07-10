using AutoMapper;
using BAExamApp.Dtos.Notes;
using BAExamApp.MVC.Areas.Admin.Models.BranchVMs;
using BAExamApp.MVC.Areas.Admin.Models.DashboardVMs;
using BAExamApp.MVC.Areas.Admin.Models.NoteVMs;
using System.Security.Claims;

namespace BAExamApp.MVC.Areas.Admin.Controllers;
public class NoteController : AdminBaseController
{
    private readonly INoteService _noteService;
    private readonly IMapper _mapper;
    public NoteController(INoteService noteService, IMapper mapper)
    {
        _noteService = noteService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new AdminNoteCreateVM();
        return PartialView("CreateNotePartial", model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminNoteCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            string errorsMessages = null!;
            foreach (var error in errors)
            {
                errorsMessages += " ," + error.ErrorMessage;
            }
            NotifyError(errorsMessages);
            return RedirectToAction("Index", "Home");
        }
        var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Oturum açmış kullanıcının ID'sini al
        if (string.IsNullOrEmpty(adminId))
        {
            return Unauthorized(); // Eğer kullanıcı oturum açmamışsa yetkisiz hata döndür
        }

        var noteDto = _mapper.Map<NoteCreateDto>(model);
        noteDto.AdminId = Guid.Parse(adminId); // Oturum açmış kullanıcı ID'sini ekle


        var addResult = await _noteService.AddAsync(noteDto);

        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return RedirectToAction("Index", "Home");
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var getNoteResponse = await _noteService.GetByIdAsync(id);

        if (getNoteResponse.IsSuccess)
        {

            var noteDetails = _mapper.Map<DashboardNoteVM>(getNoteResponse.Data);
            return Json(noteDetails);
        }

        // Hata durumunda da mesaj dönebiliriz
        return Json(new { error = "Not bulunamadı" });
    }


    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleteNoteResponse = await _noteService.DeleteAsync(id);

        if (deleteNoteResponse.IsSuccess)
        {
            return Json(deleteNoteResponse.Message);
        }

        // Hata durumunda da mesaj dönebiliriz
        return Json(new { error = "Not bulunamadı" });
    }




}
