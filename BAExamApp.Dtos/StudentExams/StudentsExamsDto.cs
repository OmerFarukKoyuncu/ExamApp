using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.StudentExams
{
    public class StudentsExamsDto
    {
        public Guid Id { get; set; }

        public string ExamName { get; set; }

        public DateTime ExamDateTime { get; set; }

        public TimeSpan ExamDuration { get; set; }

        public decimal? Score { get; set; }

        public int AnsweredQuestionCount { get; set; } = 0;

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
