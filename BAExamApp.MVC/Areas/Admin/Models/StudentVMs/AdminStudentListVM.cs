using BAExamApp.Entities.DbSets;
using System.ComponentModel.DataAnnotations;


namespace BAExamApp.MVC.Areas.Admin.Models.StudentVMs;

public class AdminStudentListVM
{
    public Guid Id { get; set; }
    private string _firstName;
    private string _lastName;

    [Display(Name = "FirstName")]
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _firstName = string.Join(" ", value.Split(' ')
                                                   .Select(n => char.ToUpper(n[0]) + n.Substring(1).ToLower()));
            }
            else
            {
                _firstName = value;
            }
        }
    }
    [Display(Name = "LastName")]
    public string LastName
    {
        get => _lastName;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _lastName = string.Join(" ", value.Split(' ')
                                                   .Select(n => char.ToUpper(n[0]) + n.Substring(1).ToLower()));
            }
            else
            {
                _lastName = value;
            }
        }
    }
    [Display(Name = "Email")]
    public string Email { get; set; }
    public byte[]? NewImage { get; set; }


}