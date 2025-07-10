using System.ComponentModel.DataAnnotations;

public class StudentStudentDetailVM
{
    public Guid Id { get; set; }  // ← BU SATIR ŞART!

    [Display(Name = "First_Name")]
    public string FirstName { get; set; }

    [Display(Name = "Last_Name")]
    public string LastName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Gender")]
    public bool Gender { get; set; }

    [Display(Name = "Profile_Image")]
    public string Image { get; set; }

    [Display(Name = "Graduation_Date")]
    [DataType(DataType.Date, ErrorMessage = "Lütfen geçerli bir tarih giriniz.")]
    public DateTime? GraduatedDate { get; set; }
}
