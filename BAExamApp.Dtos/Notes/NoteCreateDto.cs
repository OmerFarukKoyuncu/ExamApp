namespace BAExamApp.Dtos.Notes;
public class NoteCreateDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public Guid AdminId { get; set; }
}
