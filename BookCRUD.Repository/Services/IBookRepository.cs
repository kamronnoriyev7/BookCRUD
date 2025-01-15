using BookCRUD.DateAccess.Entity;

namespace BookCRUD.Repository.Services;

public interface IBookRepository
{
    Guid AddBook(Book book);
    void UpdateBook(Book book);
    void DeleteBook(Guid id);
    Book GetBookById(Guid id);
    List<Book> GetAllBooks();
    
    
}