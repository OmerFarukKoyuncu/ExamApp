using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.SendMails;
public class StudentMakeUpExamMailDto
{
    public string EmailAdress { get; set; }
    public string Url { get; set; }
    public Guid StudentExamId { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan ExamDuration { get; set; }
    public string ExamName { get; set; }
}
