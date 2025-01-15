using BookCRUD.DateAccess.Entity;
using BookCRUD.Repository.Services;
using BookCRUD.Service.DTOs;

namespace BookCRUD.Service.Service;

public class BookService: IBookService
{
    private readonly IBookRepository _bookRepository;
    private IBookService _bookServiceImplementation;

    public BookService()
    {
        _bookRepository = new BookRepository();
    }

    public Guid AddBookService(BookDto bookDto)
    {
        var bookEntity = ConvertToBookEntity(bookDto);
        var idBook = _bookRepository.AddBook(bookEntity);
        return idBook;
    }

    public void UpdateBookService(BookDto bookDto)
    {
        var bookEntity = ConvertToBookEntity(bookDto);
        _bookRepository.UpdateBook(bookEntity);
    }
    
    public void DeleteBookService(BookDto bookDto)
    {
        var bookEntity = ConvertToBookEntity(bookDto);
        _bookRepository.DeleteBook(bookEntity.Id);
    }

    public Book GetBookByIdService(Guid bookId)
    {
        return _bookRepository.GetBookById(bookId);
    }

    public List<BookDto> GetAllBooksService()
    {
        var books = _bookRepository.GetAllBooks();
        var booksDto = books.Select(book => ConvertToBookDto(book)).ToList();
        return booksDto;
    }

    private Book ConvertToBookEntity(BookDto bookDto)
    {
        return new Book()
        {
            Id = bookDto.Id ,
            Title = bookDto.Title,
            Author = bookDto.Author,
            Publisher = bookDto.Publisher,
            PublishDate = bookDto.PublishDate,
        };
    }

    private BookDto ConvertToBookDto(Book book)
    {
        return new BookDto()
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            PublishDate = book.PublishDate,
        };
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
}