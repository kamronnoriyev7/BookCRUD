using System.Text.Json;
using BookCRUD.DateAccess.Entity;

namespace BookCRUD.Repository.Services;

public class BookRepository: IBookRepository
{
    private readonly string _path;
    private readonly string _fileName;
    private List<Book> _books;

    public BookRepository()
    {
        _path = Path.Combine(Directory.GetCurrentDirectory() ,"Books.json");
        _fileName = Path.Combine(Directory.GetCurrentDirectory(), "Books.json");
        _books = new List<Book>();

        if (!File.Exists(_fileName))
        {
            File.WriteAllText(_fileName, "{}");
        }

        _books = GetAllBooks();

    }

    public Guid AddBook(Book book)
    {
        book.Id = Guid.NewGuid();
        _books.Add(book);
        SaveData();
        return book.Id;
    }

    public Book GetBookById(Guid id)
    {
        var book = _books.FirstOrDefault(x => x.Id == id);
        if (book == null)
        {
            throw new KeyNotFoundException("Book not found");
        }
        return book;
    }

    public List<Book> GetAllBooks()
    {
        var booksJson = File.ReadAllText(_path);
        var bookList = JsonSerializer.Deserialize<List<Book>>(booksJson);
        return bookList;
    }

    public void UpdateBook(Book book)
    {
        var books = GetBookById(book.Id);
        var index = _books.IndexOf(books);
        _books[index] = book;
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