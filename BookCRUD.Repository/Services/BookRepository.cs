using System.Text.Json;
using BookCRUD.DateAccess.Entity;

namespace BookCRUD.Repository.Services;

public class BookRepository: IBookRepository
{
    private readonly string _path;
    private List<Book> _books;

    public BookRepository()
    {
        _path = Path.Combine(Directory.GetCurrentDirectory(), "Books.json");

        if (!File.Exists(_path))
        {
            File.WriteAllText(_path, "{}");
        }

        _books = GetAllBooks();
    }

    public Guid AddBook(Book book)
    {
        _books.Add(book);
        SaveData();
        return book.Id;
    }

    public Book GetBookById(Guid id)
    {
        return _books.FirstOrDefault(x => x.Id == id);
    }

    public List<Book> GetAllBooks()
    {
        var books = File.ReadAllText(_path);
        var bookList = JsonSerializer.Deserialize<List<Book>>(books);
        return bookList;
    }

    public void UpdateBook(Book book)
    {
        var books = GetBookById(book.Id);
        var bookIndex = _books.FindIndex(x => x.Id == book.Id);
        _books[bookIndex] = book;
        SaveData();
    }

    public void DeleteBook(Guid id)
    {
        var book = GetBookById(id);
        _books.Remove(book);
        SaveData();
    }

    private void SaveData()
    {
        var bookJson = JsonSerializer.Serialize(_books);
        File.WriteAllText(_path, bookJson);
    }
}