using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.Dtos.Subtopics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BAExamApp.Business.ApiServices.Interfaces;

public interface ISubtopicApiService
{
    /// <summary>
    /// Alt konuyu günceller.
    /// </summary>
    /// <param name="entity">Güncellenecek alt konu bilgileri</param>
    Task<IDataResult<SubtopicApiDto>> UpdateAsync(SubtopicUpdateApiDto entity);

    /// <summary>
    /// Alt konunun aktiflik durumunu değiştirir.
    /// </summary>
    /// <param name="subtopicId">Durumu değiştirilecek alt konunun ID'si</param>
    Task<IResult> ChangeRuleStatusAsync(Guid subtopicId);

    /// <summary>
    /// Belirtilen ID'ye sahip alt konuyu getirir.
    /// </summary>
    /// <param name="id">Getirilecek alt konunun ID'si</param>
    Task<IDataResult<SubtopicApiDto>> GetSubtopicById(Guid id);

    /// <summary>
    /// Subtopic için db ekleme işlemi yapar.
    /// </summary>
    /// <param name="subtopicCreateApiDto"></param>
    /// <returns>Data result döner.</returns>
    Task<IDataResult<SubtopicDto>> AddAsync(SubtopicCreateApiDto subtopicCreateApiDto);

    /// <summary>
    /// Tüm alt konuları Dto olarak asenkron getirir. 
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<SubtopicListDto>>> GetAllAsync();
    Task<IResult> DeleteAsync(Guid subtopicId);
}
