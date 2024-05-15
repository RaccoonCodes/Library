# Library

## Overview

This Project is about users storing their own books into this virtual bookself. Hence, your very own Library!  This project includes three controllers: BooksController, CategoriesController, and HomeController, which handle various operations such as retrieving, adding, updating, and deleting books and categories.

This web project focuses heavily on backend work.

## Features
1) CRUD Operations: You can Add, edit, or delete Books from your Library.
2) Pagination: To keep the webpage clean and avoid overflowing with information.
3) Category Filter: Filter Books based on the genre of books you want to look at.
4) REST API: The API endpoints manage user's request and make the changes on local database
5) Swashbuckle: To test API Connection and Operations

## How it was Developed
- ASP.NET Core MVC for Back-end
- HTML, CSS, Bootstrap 5 and javascript for front-end
- Entity Framework Core for database management.
- Microsoft SQL Server for database storage.
- REST API for handling user requests efficiently and return appropriate responses, ensuring smooth interaction between the client and the server.

## Installation
1) Clone Repository into your machine
2) Open your Visual Studio and Download any necessary Nuget Packages or Download them via Command Terminal using the following:
   - **Installing Entity Framework and SQL:**
     
     "dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.0"
     
     "dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.0"
     
   -  **Installing Swashbuckle:**
     
      "dotnet add package Swashbuckle.AspNetCore --version 6.2.3"
      
3) Connection String in appsettings.json is set to local machine, you may change the connection string to your own database.
4) As mentioned before, there is seeded Data to begin with and I personally added some of my own personal books. If update is needed use the follwoing command on Terminal: "dotnet ef database update"
5) If you need to Drop the tables use : "dotnet ef database drop --force". Then Update using previous command

## Behind the code
The following section will describe parts of the code on how they play the role on the web project

### Model

#### Database Context
There are three Models that connect to each other to build a database set: Category, Books, and DataContext

**Category** 
```csharp
public class Category
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<Book>? Books { get; set; }
}
```
**Books**
```csharp
public class Book
{
    public long BookId { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int Edition { get; set; }
    public int Year { get; set; }
    public long CategoryId { get; set; }
    public Category? Category { get; set; }
}
```
Respectively, both define key property in which it will need in the database when new objects, or books, are created and stored.  There are also navigation properties that can be used to querry specific categories, however, I wanted to demontrate the use of using two API for this function. The Navigation Properties are : 
- In Category: public IEnumerable<Book>? Books { get; set; } 
- In Books: public Category? Category { get; set; }

**DataContext**
```csharp
public class DataContext : DbContext 
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
}
```
In here we build the database set and its properties for both Books and Categories.

#### BookRepo Interface and Implementation

**Interface**
```csharp
public interface IBookRepo
{
    Task<Book?> GetBookByIdAsync(long id);
    public IAsyncEnumerable<Book> GetBooksByCategoryIdAsync(long categoryId);
    IAsyncEnumerable<Book> GetAllBooksAsync();
    Task AddBookAsync(Book book);
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(long id);
}
```
This is an interface for Book repository operations for CRUD

**Implementation**
```csharp
public class EBookRepo : IBookRepo
{
    private readonly DataContext _dataContext;

    public EBookRepo(DataContext dataContext) => _dataContext = dataContext;
    
    public async Task <Book?> GetBookByIdAsync(long id) => await _dataContext.Books.FindAsync(id);
    public IAsyncEnumerable<Book> GetAllBooksAsync() =>
        _dataContext.Books.OrderBy(b=>b.Title)
        .OrderBy(b => b.Title)
        .ThenBy(b => b.Author)
        .ThenBy(b => b.Edition)
        .ThenBy(b => b.Year)
        .AsAsyncEnumerable();
    public IAsyncEnumerable<Book> GetBooksByCategoryIdAsync(long categoryId) =>
        _dataContext.Books
        .Where(book => book.CategoryId == categoryId)
        .OrderBy(b => b.Title)
        .ThenBy(b => b.Author)
        .ThenBy(b => b.Edition)
        .ThenBy(b => b.Year)
        .AsAsyncEnumerable();
    public async Task AddBookAsync(Book book)
    {
        await _dataContext.Books.AddAsync(book);
        await _dataContext.SaveChangesAsync();
    }
    public async Task UpdateBookAsync(Book book)
    {
        _dataContext.Books.Update(book);
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(long id)
    {
        _dataContext.Books.Remove(new Book() { BookId = id });
        await _dataContext.SaveChangesAsync();
    }
}
```
- GetBookByIdAsync: Retrieves a book by its ID.
- GetAllBooksAsync: Retrieves all books, ordered by title, author, edition, and year.
- GetBooksByCategoryIdAsync: Retrieves books by a specific category ID, ordered by title, author, edition, and year.
- AddBookAsync: Adds a new book to the database and saves the changes.
- UpdateBookAsync: Updates an existing book in the database and saves the changes.
- DeleteBookAsync: Deletes a book by its ID from the database and saves the changes.

The EBookRepo class is injected into Homecontrollers via dependency injection. 

#### Category Interface and Implementation
**Interface**
```csharp
 public interface ICategoryRepo
 {
     IAsyncEnumerable<Category> GetAllCategories();
     Task <Category?> GetByIdAsync (long id);
     Task<Category?> PatchChangesAsync (long id, JsonPatchDocument<Category> patchDocument);
 }
```
The interface defines the category repository. It includes methods for retrieving categories, getting a category by its ID, and applying JSON Patch updates to a category.

**Implementation**
```csharp
 public class ECategoryRepo : ICategoryRepo
 {
     private readonly DataContext _context;

     public ECategoryRepo(DataContext context) => _context = context;

     public IAsyncEnumerable<Category> GetAllCategories()
     {
         return _context.Categories.AsAsyncEnumerable();
     }

     public async Task<Category?> GetByIdAsync(long id)
     {
         Category category = await _context.Categories
            .Include(c => c.Books)
            .FirstAsync(c => c.CategoryId == id);

         //Breaking Circular References in Related Data
         if (category.Books != null)
         {
             foreach (Book b in category.Books)
             {
                 b.Category = null;
             };
         }
         return category;
     }

     public async Task <Category?> PatchChangesAsync(long id, JsonPatchDocument<Category> patchDocument)
     {
         Category? c = await _context.Categories.FindAsync(id);

         if (c != null)
         {
             patchDocument.ApplyTo(c);
             await _context.SaveChangesAsync();
         }
         return c;
     }
 }
```
- GetAllCategories: Retrieves all categories.
- GetByIdAsync: Retrieves a category by its ID, including its related books. To avoid 
 circular references, it sets the Category property of each book to null.
- PatchChangesAsync: Applies a JSON Patch document to a category and saves the changes.

#### BookBinding
```csharp
public class BookBinding
{
    [Required]
    public string Title { get; set; } = "";
    
    [Required]
    public string Author { get; set; } = "";
    
    [Range(1,int.MaxValue)]
    public int Edition { get; set; }
    
    [Range(1,int.MaxValue)]
    public int Year { get; set; }

    [Range(1,long.MaxValue)]
    public long CategoryId { get; set; }

    public Book ToBook() => new Book()
    {
        Title = this.Title,
        Author = this.Author,
        Edition = this.Edition,
        Year = this.Year,
        CategoryId = this.CategoryId
    };
}
```
This Model is created to prevent Over-binding when unexpected values are entered such as its primary key. Entity Framework Core configures the database to assign primary key values when new objects are stored. This means the application doesn’t have to worry about keeping track of which key values have already been assigned and allows multiple applications to share the same database without the need to coordinate key allocation.

The safest way to prevent over-binding is to create separate data model classes that are used only for receiving data through the model binding process

### Controllers
The following will describe the APIs and HomeController.



**README IN PROGRESS**


   
      
