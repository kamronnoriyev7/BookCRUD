using BookCRUD.DateAccess.Entity;
using BookCRUD.Service.DTOs;

namespace BookCRUD.Service.Service;

public interface IBookService
{
    Guid AddBookService(BookDto book);
    void UpdateBookService(BookDto book);
    void DeleteBookService(Guid bookId);
    Book GetBookByIdService(Guid bookId);
    List<BookDto> GetAllBooksService();
    
}