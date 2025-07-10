using BAExamApp.Dtos.QuestionRevisions;
using X.PagedList;

namespace BAExamApp.MVC.Areas.Admin.Models.QuestionVMs;

public class AdminQuestionPageListVM
{
    public AdminQuestionApprovedVM? AdminQuestionApprovedVM { get; set; }
    public IPagedList<QuestionRevisionListDto>? PagedList { get; set; }
}
