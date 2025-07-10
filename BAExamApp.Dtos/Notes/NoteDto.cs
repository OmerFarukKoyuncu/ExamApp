namespace BAExamApp.Dtos.Notes;
public class NoteDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public Guid AdminId { get; set; }
}
