using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.Dtos.Subtopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
﻿using BAExamApp.Core.Enums;
using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.Dtos.Subtopics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Concrete;

/// <summary>
/// Alt konu (subtopic) yönetimi için servis sınıfı.
/// </summary>
public class SubtopicApiService : ISubtopicApiService
{
    private readonly ISubtopicRepository _subtopicRepository;
    private readonly IMapper _mapper;
    private readonly IQuestionSubtopicsRepository _questionSubtopicsRepository;
    private readonly IExamRuleSubtopicRepository _examRuleSubtopicRepository;

    /// <summary>
    /// SubtopicApiService sınıfının yapıcı metodu.
    /// </summary>
    /// <param name="subtopicRepository">Alt konu işlemleri için repository</param>
    /// <param name="mapper">Nesne dönüşümleri için AutoMapper örneği</param>
    public SubtopicApiService(ISubtopicRepository subtopicRepository, IMapper mapper, IQuestionSubtopicsRepository questionSubtopicsRepository, IExamRuleSubtopicRepository examRuleSubtopicRepository)
    {
        _subtopicRepository = subtopicRepository;
        _mapper = mapper;
        //Açılacak
        _questionSubtopicsRepository = questionSubtopicsRepository;
        _examRuleSubtopicRepository = examRuleSubtopicRepository;
    }

    /// <summary>
    /// Yeni bir alt konu ekler.
    /// </summary>
    /// <param name="subtopicCreateApiDto"></param>
    /// <returns></returns>
    public async Task<IDataResult<SubtopicDto>> AddAsync(SubtopicCreateApiDto subtopicCreateApiDto)
    {
        if (await _subtopicRepository.AnyAsync(x => x.Name.ToLower().Equals(subtopicCreateApiDto.Name.Trim().ToLower()) && x.SubjectId == subtopicCreateApiDto.SubjectId))
        {
            return new ErrorDataResult<SubtopicDto>(Messages.SubtopicAlreadyExist);
        }
        var subtopic = _mapper.Map<Subtopic>(subtopicCreateApiDto);
        await _subtopicRepository.AddAsync(subtopic);
        await _subtopicRepository.SaveChangesAsync();
        return new SuccessDataResult<SubtopicDto>(_mapper.Map<SubtopicDto>(subtopic), Messages.AddSuccess);
    }


    /// Varolan bir alt konuyu günceller. Null veya boş gelen alanlarda mevcut değerler korunur.
    /// </summary>
    /// <param name="entity">Güncellenecek bilgileri içeren DTO nesnesi</param>
    /// <returns>Güncellenmiş alt konu bilgilerini veya hata mesajını içeren sonuç nesnesi</returns>
    public async Task<IDataResult<SubtopicApiDto>> UpdateAsync(SubtopicUpdateApiDto entity)
    {
        try
        {
            var existingSubtopic = await _subtopicRepository.GetByIdAsync(entity.Id);
            if (existingSubtopic is null)
            {
                return new ErrorDataResult<SubtopicApiDto>(Messages.SubtopicNotFound);
        }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.Name = existingSubtopic.Name;
            }
            if (entity.SubjectId is null)
            {
                entity.SubjectId = existingSubtopic.SubjectId;
            }

            var updatedSubtopic = _mapper.Map(entity, existingSubtopic);
            await _subtopicRepository.UpdateAsync(updatedSubtopic);
            await _subtopicRepository.SaveChangesAsync();
            var mappedResult = _mapper.Map<SubtopicApiDto>(updatedSubtopic);
            return new SuccessDataResult<SubtopicApiDto>(mappedResult, Messages.UpdateSuccess);
        }
        catch (Exception)
        {
            return new ErrorDataResult<SubtopicApiDto>(Messages.UpdateFail);
        }
    }

    /// <summary>
    /// Alt konunun aktiflik durumunu değiştirir.
    /// </summary>
    /// <param name="subtopicId">Durumu değiştirilecek alt konunun ID'si</param>
    /// <returns>İşlem sonucunu içeren IResult nesnesi</returns>
    public async Task<IResult> ChangeRuleStatusAsync(Guid subtopicId)
    {
        var subtopic = await _subtopicRepository.GetByIdAsync(subtopicId);
        if (subtopic == null)
        {
            return new ErrorResult(Messages.SubtopicNotFound);
        }
        subtopic.Status = subtopic.Status == Status.Active ? Status.Passive : Status.Active;
        try
        {
            subtopic.Status = Core.Enums.Status.Passive;
            await _subtopicRepository.SaveChangesAsync();
        }
        catch (Exception)
        {
            return new ErrorResult(Messages.ChangeStatusFail);
        }
            return new SuccessResult(Messages.ChangeStatusSuccess);
        }

    /// <summary>
    /// Belirtilen ID'ye sahip alt konuyu getirir.
    /// </summary>
    /// <param name="id">Getirilecek alt konunun ID'si</param>
    /// <returns>Alt konu bilgilerini veya hata mesajını içeren sonuç nesnesi</returns>
    public async Task<IDataResult<SubtopicApiDto>> GetSubtopicById(Guid id)
    {
        var subtopic = await _subtopicRepository.GetByIdAsync(id);
        if (subtopic is null)
    {
            return new ErrorDataResult<SubtopicApiDto>(Messages.SubtopicNotFound);
        }
        var subtopicDto = _mapper.Map<SubtopicApiDto>(subtopic);
        return new SuccessDataResult<SubtopicApiDto>(subtopicDto, Messages.FoundSuccess);
    }

    /// <summary>
    /// Alt konuları asenkron alır.
    /// Alt konuları SubtopicListDto nesnesine dönüştürür. 
    /// Son olarak başarılı sonuç ve mesaj döner. 
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<SubtopicListDto>>> GetAllAsync()
    {
        var subtopics = await _subtopicRepository.GetAllAsync();
        return new SuccessDataResult<List<SubtopicListDto>>(_mapper.Map<List<SubtopicListDto>>(subtopics), Messages.ListedSuccess);
    }

    /// <summary>
    /// İşaretlenen alt konuyu bularak db silme işlemini yapmaya çalışır, lakin alt konu kullanılıyorsa status modified işlemini gerçekleştirir. 
    /// </summary>
    /// <param name="subtopicId"></param>
    /// <returns></returns>
    public async Task<IResult> DeleteAsync(Guid subtopicId)
    {
        var subtopic = await _subtopicRepository.GetByIdAsync(subtopicId);
        if (subtopic is null)
        {
            return new ErrorDataResult<SubtopicApiDto>(Messages.ProductNotFound);
        }

        var examRuleUsingSuptopic = await IsRuleUsedSubtopicAsync(subtopic.Id);

        var questionUsingSuptopic = await IsQuestionUsedSubtopicAsync(subtopic.Id);

        if (examRuleUsingSuptopic || questionUsingSuptopic)
        {
            subtopic.Status = Core.Enums.Status.Passive;
            await _subtopicRepository.SaveChangesAsync();
            return new SuccessResult(Messages.ChangeStatusSuccess);
        }



        await _subtopicRepository.DeleteAsync(subtopic);
        await _subtopicRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }
    /// <summary>
    /// Alt konuya ait bir sorunun olup olmadığını döner
    /// </summary>
    /// <param name="subtopicId"></param>
    /// <returns></returns>
    public async Task<bool> IsQuestionUsedSubtopicAsync(Guid subtopicId)
    {
        var questionUsingSubtopic = await _questionSubtopicsRepository.AnyAsync(e => e.SubtopicId == subtopicId);
        return questionUsingSubtopic;
    }
    /// <summary>
    /// Alt konuya ait sınav kuralı olup olmadığını döner.
    /// </summary>
    /// <param name="subtopicId"></param>
    /// <returns></returns>
    public async Task<bool> IsRuleUsedSubtopicAsync(Guid subtopicId)
    {
        var ruleUsingSubtopic = await _examRuleSubtopicRepository.AnyAsync(e => e.SubtopicId == subtopicId);
        return ruleUsingSubtopic;
    }
}

