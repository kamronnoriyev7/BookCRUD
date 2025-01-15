namespace BookCRUD.DateAccess.Entity;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    public DateTime PublishDate { get; set; }
}