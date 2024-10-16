using System;
using System.Collections;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        ILibrary libraryProxy = new LibraryProxy(); // LibraryProxy initialized

        var book1 = new RegularBook("Naruto");
        var book2 = new PremiumBook("Dragon Ball Z");

        // Add books to the library collection
        libraryProxy.AddBook(book1);
        libraryProxy.AddBook(book2);

        Console.WriteLine("\nInitial Library Book Collection:\n");
        foreach (var book in libraryProxy.GetBooks())
        {
            Console.WriteLine($"- {book.Title} (State: {book.CurrentState.GetType().Name})");
        }

        var user1 = new User("Naldo", false); // Non-premium user
        var user2 = new User("Sam", true);    // Premium user

        // User actions
        libraryProxy.BorrowBook(book1, user1); // Should succeed
        libraryProxy.BorrowBook(book2, user1); // Should fail
        libraryProxy.BorrowBook(book2, user2); // Should succeed

        Console.WriteLine("\nAfter Borrowing Books:");
        foreach (var book in libraryProxy.GetBooks())
        {
            Console.WriteLine($"- {book.Title} (State: {book.CurrentState.GetType().Name})");
        }

        // Return books
        book1.CurrentState.Return(book1);
        book2.CurrentState.Return(book2);

        Console.WriteLine("\nAfter Returning Books:");
        foreach (var book in libraryProxy.GetBooks())
        {
            Console.WriteLine($"- {book.Title} (State: {book.CurrentState.GetType().Name})");
        }
        // Reserve a book
        Console.WriteLine("\nReserving a Book:");
        book1.CurrentState.Reserve(book1, user1); // Reserve Regular Book 1

        Console.WriteLine("\nAfter Reserving the Book:");
        foreach (var book in libraryProxy.GetBooks())
        {
            Console.WriteLine($"- {book.Title} (State: {book.CurrentState.GetType().Name})");
        }

        // Attempt to borrow a reserved book
        Console.WriteLine("\nAttempting to Borrow a Reserved Book:");
        libraryProxy.BorrowBook(book1, user2); // Should fail due to reservation

        // Returning the reserved book to make it available again
        book1.CurrentState.Return(book1);

        Console.WriteLine("\nAfter Returning the Reserved Book:");
        foreach (var book in libraryProxy.GetBooks())
        {
            Console.WriteLine($"- {book.Title} (State: {book.CurrentState.GetType().Name})");
        }

        Console.ReadLine();
    }
}

// State Pattern
public interface IBookState
{
    void Borrow(Book book, User user);
    void Return(Book book);
    void Reserve(Book book, User user);
}

public class AvailableState : IBookState
{
    public void Borrow(Book book, User user)
    {
        if (user.premiumMember || !book.premiumBook)
        {
            book.CurrentState = new BorrowedState();
            Console.WriteLine($"'{book.Title}' is now borrowed by {user.Name}.");
        }
        else
        {
            Console.WriteLine($"'{book.Title}' is a premium book. Only premium members can borrow it.");
        }
    }

    public void Return(Book book)
    {
        Console.WriteLine($"'{book.Title}' is already available.");
    }

    public void Reserve(Book book, User user)
    {
        book.CurrentState = new ReservedState();
        Console.WriteLine($"'{book.Title}' is now reserved by {user.Name}.");
    }
}

public class BorrowedState : IBookState
{
    public void Borrow(Book book, User user)
    {
        Console.WriteLine($"'{book.Title}' is currently borrowed.");
    }

    public void Return(Book book)
    {
        book.CurrentState = new AvailableState();
        Console.WriteLine($"'{book.Title}' has been returned and is now available.");
    }

    public void Reserve(Book book, User user)
    {
        Console.WriteLine($"'{book.Title}' is currently borrowed and cannot be reserved.");
    }
}

public class ReservedState : IBookState
{
    public void Borrow(Book book, User user)
    {
        Console.WriteLine($"'{book.Title}' is reserved and cannot be borrowed.");
    }

    public void Return(Book book)
    {
        book.CurrentState = new AvailableState();
        Console.WriteLine($"'{book.Title}' is now available after being returned.");
    }

    public void Reserve(Book book, User user)
    {
        Console.WriteLine($"'{book.Title}' is already reserved.");
    }
}

// User Class
public class User
{
    public string Name { get; }
    public bool premiumMember { get; }

    public User(string name, bool PremiumMember)
    {
        Name = name;
        premiumMember = PremiumMember;
    }
}

// Book Class Hierarchy
public abstract class Book
{
    public string Title { get; }
    public bool premiumBook { get; }
    public IBookState CurrentState { get; set; }

    protected Book(string title, bool PremiumBook)
    {
        Title = title;
        premiumBook = PremiumBook;
        CurrentState = new AvailableState(); // Initial state
    }
}

public class RegularBook : Book
{
    public RegularBook(string title) : base(title, false) { }
}

public class PremiumBook : Book
{
    public PremiumBook(string title) : base(title, true) { }
}

// Proxy Class
public interface ILibrary
{
    void AddBook(Book book);
    void BorrowBook(Book book, User user);
    List<Book> GetBooks();
}

public class LibraryProxy : ILibrary
{
    private Library _realLibrary;

    // Lazy initialization of the real library
    private void EnsureRealLibraryCreated()
    {
        if (_realLibrary == null)
        {
            _realLibrary = new Library();
            Console.WriteLine("Library initialized.");
        }
    }

    public void AddBook(Book book)
    {
        EnsureRealLibraryCreated();
        _realLibrary.AddBook(book);
    }

    public void BorrowBook(Book book, User user)
    {
        EnsureRealLibraryCreated();
        _realLibrary.BorrowBook(book, user);
    }

    public List<Book> GetBooks()
    {
        EnsureRealLibraryCreated();
        return _realLibrary.GetBooks();
    }
}

// Library Class (RealSubject)
public class Library : ILibrary
{
    private readonly List<Book> _books = new List<Book>();

    public void AddBook(Book book)
    {
        _books.Add(book);
    }

    public void BorrowBook(Book book, User user)
    {
        book.CurrentState.Borrow(book, user);
    }

    public List<Book> GetBooks()
    {
        return _books;
    }
}
//Iterator Pattern
//Iterator Base
public interface IBookIterator 
{
    bool HasNext();
    Book Next();
}
//Concrete Iterator
public class BookIterator : IBookIterator
{
    private readonly List<Book> _books;
    private int _position = 0;

    public BookIterator(List<Book> books)
    {
        _books = books;
    }

    public bool HasNext()
    {
        return _position < _books.Count;
    }

    public Book Next()
    {
        return _books[_position++];
    }
}
