using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.ExamRuleApiDtos;
using BAExamApp.Dtos.ExamRules;
using BAExamApp.Dtos.StudentClassrooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Concrete;
public class ExamRuleApiService : IExamRuleApiService
{
    private readonly IMapper _mapper;
    private readonly IExamRuleRepository _examRuleRepository;
   


    public ExamRuleApiService(IExamRuleRepository examRuleRepository, IMapper mapper)
    {
       
        _examRuleRepository = examRuleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Id'ye göre ExamRule döndürür.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IDataResult<ExamRuleApiDto>> GetByIdAsync(Guid id)
    {
        var examRule = await _examRuleRepository.GetByIdAsync(id);

        if (examRule is null)
        {
            return new ErrorDataResult<ExamRuleApiDto>(Messages.ExamRuleNotFound);
    }

        return new SuccessDataResult<ExamRuleApiDto>(_mapper.Map<ExamRuleApiDto>(examRule), Messages.FoundSuccess);
    }
    /// <summary>
    /// sınav kurallarının hepsini listeler
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<ExamRuleApiDto>>> GetAllExamRule()
    {
        var examRules = await _examRuleRepository.GetAllAsync();

        if (examRules == null || !examRules.Any())
            return new ErrorDataResult<List<ExamRuleApiDto>>(Messages.ListNotFound);

        var examRuleDtos = _mapper.Map<List<ExamRuleApiDto>>(examRules);

        return new SuccessDataResult<List<ExamRuleApiDto>>(examRuleDtos, Messages.FoundSuccess);
    }
}
