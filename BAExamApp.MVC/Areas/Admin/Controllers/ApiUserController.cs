using AutoMapper;
using BAExamApp.Dtos.ApiUsers;
using BAExamApp.Dtos.Attributes;
using BAExamApp.MVC.Areas.Admin.Models.ApiUserVMs;
using X.PagedList;

namespace BAExamApp.MVC.Areas.Admin.Controllers
{
    [BreadcrumbName("ApiUsers")]
    public class ApiUserController : AdminBaseController
    {
        private readonly IApiUserService _apiUserService;
        private readonly IMapper _mapper;

        public ApiUserController(IApiUserService apiUserService, IMapper mapper)
        {
            _apiUserService = apiUserService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index(string firstName, string lastName, string email, int? page, int pageSize = 10)
        {
            int pageNumber = page ?? 1;

            var apiUserResult = await _apiUserService.GetAllAsync();
            var apiUserList = _mapper.Map<List<AdminApiUserListVM>>(apiUserResult.Data);

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                firstName = firstName.Trim();
                apiUserList = apiUserList
                    .Where(x => x.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                lastName = lastName.Trim();
                apiUserList = apiUserList
                    .Where(x => x.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim();
                apiUserList = apiUserList
                    .Where(x => x.Email.Contains(email, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var pagedList = apiUserList.ToPagedList(pageNumber, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.Email = email;

            return View(pagedList);
        }

        [HttpGet]
        public async Task<IActionResult> SearchApiUserFirstNameAutocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var apiUsers = await _apiUserService.GetAllAsync();

            var matched = apiUsers.Data
                .Where(x => x.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(x =>  x.FirstName)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(matched);
        }

        [HttpGet]
        public async Task<IActionResult> SearchApiUserLastNameAutocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var apiUsers = await _apiUserService.GetAllAsync();

            var matched = apiUsers.Data
                .Where(x => x.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.LastName)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(matched);
        }

        [HttpGet]
        public async Task<IActionResult> SearchApiUserEmailAutocomplete(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            var apiUsers = await _apiUserService.GetAllAsync();

            var matched = apiUsers.Data
                .Where(x => x.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Email)
                .Distinct()
                .Take(10)
                .ToList();

            return Json(matched);
        }
        public async Task<List<AdminApiUserListVM>> Search(string typeOfApiUser)
        {
            var apiUserResult = await _apiUserService.GetAllAsync();
            var apiUserList = _mapper.Map<List<AdminApiUserListVM>>(apiUserResult.Data);

            var searchList = apiUserList
                .Where(s =>
                    s.FirstName.StartsWith(typeOfApiUser, StringComparison.OrdinalIgnoreCase) ||
                    s.LastName.StartsWith(typeOfApiUser, StringComparison.OrdinalIgnoreCase) ||
                    $"{s.FirstName} {s.LastName}".StartsWith(typeOfApiUser, StringComparison.OrdinalIgnoreCase))
                .OrderBy(o => o.FirstName)
                .ToList();

            return searchList;
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminApiUserCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var createApiUserDto = _mapper.Map<CreateApiUserDto>(model);
            var result = await _apiUserService.AddAsync(createApiUserDto);

            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
                return View(model);
            }

            NotifySuccess($"{model.FirstName} {model.LastName} başarıyla eklendi.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var result = await _apiUserService.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
                return RedirectToAction(nameof(Index));
            }

            var model = _mapper.Map<AdminApiUserUpdateVM>(result.Data);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AdminApiUserUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var updateApiUserDto = _mapper.Map<UpdateApiUserDto>(model);


            var result = await _apiUserService.UpdateAsync(updateApiUserDto);

            if (!result.IsSuccess)
            {
                NotifyErrorLocalized(result.Message);
            }
            else
            {
                NotifySuccessLocalized(result.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var apiUserDeleteResponse = await _apiUserService.DeleteAsync(id);

            if (apiUserDeleteResponse.IsSuccess)
                NotifySuccessLocalized(apiUserDeleteResponse.Message);
            else
                NotifyErrorLocalized(apiUserDeleteResponse.Message);
            return Json(apiUserDeleteResponse);
        }
        [BreadcrumbName("ApiUser_Details")]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var result = await _apiUserService.GetByIdAsync(id);
            if (result.IsSuccess)
            {
                var apiUsersDetailsVM = _mapper.Map<AdminApiUserDetailsVM>(result.Data);
                return View(apiUsersDetailsVM);
            }

            NotifyError(result.Message);
            return RedirectToAction(nameof(Index));
        }
        public async Task<AdminApiUserUpdateVM> GetApiUser(Guid apiUserId)
        {
            var getApiUserResult = await _apiUserService.GetByIdAsync(apiUserId);
            var apiUserDto = getApiUserResult.Data;
            var apiUserUpdateVM = _mapper.Map<AdminApiUserUpdateVM>(apiUserDto);
            return apiUserUpdateVM;
        }
    }
}