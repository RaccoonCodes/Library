# Library
# Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [How it was Developed](#how-it-was-developed)
4. [Installation](#installation)
5. [Behind the Code](#behind-the-code)
    - [Model](#model)
        - [Database Context](#database-context)
            - [Category](#category)
            - [Books](#books)
            - [DataContext](#datacontext)
        - [BookRepo Interface and Implementation](#bookrepo-interface-and-implementation)
            - [Interface](#interface)
            - [Implementation](#implementation)
        - [Category Interface and Implementation](#category-interface-and-implementation)
            - [Interface](#interface-1)
            - [Implementation](#implementation-1)
        - [BookBinding](#bookbinding)
    - [Controllers](#controllers)
        - [BooksController](#bookscontroller)
        - [CategoriesController](#categoriescontroller)
        - [HomeController](#homecontroller)
            - [Index](#index)
            - [AddBook](#addbook)
            - [Delete and Edit Methods](#delete-and-edit-methods)
    - [Middleware](#middleware)
6. [Using Swagger](#using-swagger)
7. [Conclusion](#conclusion)
    - [Future Enhancements](#future-enhancements)


## Overview

This Project is about users storing their own books into this virtual bookself. Hence, your very own Library!  This project includes three controllers: BooksController, CategoriesController, and HomeController, which handle various operations such as retrieving, adding, updating, and deleting books and categories.

This web project focuses heavily on backend work.

**Note**: The books that is currently in the bookself are my own books that I have. Feel free to look them up! They are great books for various different subjects that I have 
been studying for and practicing.

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
     
     `dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.0`
     
     `dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.0`
     
   -  **Installing Swashbuckle:**
     
      `dotnet add package Swashbuckle.AspNetCore --version 6.2.3`
      
3) Connection String in appsettings.json is set to local machine, you may change the connection string to your own database.
4) As mentioned before, there is seeded Data to begin with and I personally added some of my own personal books. If update is needed use the follwoing command on Terminal: "dotnet ef database update"
5) If you need to Drop the tables use : "dotnet ef database drop --force". Then Update using previous command

## Behind the code
The following section will describe parts of the code on how they play the role on the web project

### Model

#### Database Context
There are three Models that connect to each other to build a database set: Category, Books, and DataContext

#### Category 
```csharp
public class Category
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<Book>? Books { get; set; }
}
```
#### Books
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
- In Category: `public IEnumerable<Book>? Books { get; set; } `
- In Books: `public Category? Category { get; set; }`

#### DataContext
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

#### Interface
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

#### Implementation
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
- `GetBookByIdAsync`: Retrieves a book by its ID.
- `GetAllBooksAsync`: Retrieves all books, ordered by title, author, edition, and year.
- `GetBooksByCategoryIdAsync`: Retrieves books by a specific category ID, ordered by title, author, edition, and year.
- `AddBookAsync`: Adds a new book to the database and saves the changes.
- `UpdateBookAsync`: Updates an existing book in the database and saves the changes.
- `DeleteBookAsync`: Deletes a book by its ID from the database and saves the changes.

The EBookRepo class is injected into Homecontrollers via dependency injection. 

#### Category Interface and Implementation
#### Interface
```csharp
 public interface ICategoryRepo
 {
     IAsyncEnumerable<Category> GetAllCategories();
     Task <Category?> GetByIdAsync (long id);
     Task<Category?> PatchChangesAsync (long id, JsonPatchDocument<Category> patchDocument);
 }
```
The interface defines the category repository. It includes methods for retrieving categories, getting a category by its ID, and applying JSON Patch updates to a category.

#### Implementation
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
- `GetAllCategories`: Retrieves all categories.
- `GetByIdAsync`: Retrieves a category by its ID, including its related books. To avoid 
 circular references, it sets the Category property of each book to null.
- `PatchChangesAsync`: Applies a JSON Patch document to a category and saves the changes.

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
The following will describe the REST APIs and HomeController.

#### BooksController
The BooksController provides various endpoints to manage books using interface BookRepo via dependency Injection. Its function is described in Implementation of BookRepo section
```csharp
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookRepo _bookRepo;
    public BooksController(IBookRepo bookRepo, ICategoryRepo categoryRepo) => _bookRepo = bookRepo;

    //Get All Books
    [HttpGet]
    public IAsyncEnumerable<Book> GetBooks()
    {
        return _bookRepo.GetAllBooksAsync();
    }

    //Get book by ID
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBook(long id)
    {
        var book = await _bookRepo.GetBookByIdAsync(id);

        if (book == null)
        {
            return NotFound();
        }
        
        return Ok(book);

    }

    //retrives all books based on categoryID
    [HttpGet("GetBooks/{categoryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public IActionResult GetBooksByCategory(long categoryId)
    {
        
        var books =  _bookRepo.GetBooksByCategoryIdAsync(categoryId);

        if (books == null)
        {
            return NotFound();
        }

        return Ok(books);
    }

    //Update Book
    [HttpPost]
    public async Task<IActionResult> SaveBook(BookBinding target)
    {
        Book book = target.ToBook();
        await _bookRepo.AddBookAsync(book);
        return Ok(book);
    }

    //Create New Book
    [HttpPut]
    public async Task<IActionResult> UpdateBook(Book book)
    {
        await _bookRepo.UpdateBookAsync(book);
        return Ok();
    }


    //Delete Book
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteBook(long id)
    {

        await _bookRepo.DeleteBookAsync(id);
        return NoContent();
    }

    //testing redirect
    [HttpGet("redirect")]
    public IActionResult Redirect()
    {
        return RedirectToAction(nameof(GetBook), new { id = 1 });
    }
}
```

#### CategoriesController
This API provides endpoints for managing categories using the Interface CategoryRepo via Dependency Injection
```csharp
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepo _categoryRepo;
    public CategoriesController(ICategoryRepo categoryRepo) => _categoryRepo = categoryRepo;

    //GetAll
    [HttpGet]
    public IAsyncEnumerable<Category> GetCategories()=> _categoryRepo.GetAllCategories();

    //GetByID
    [HttpGet("{id}")]
    public async Task <Category?> GetCategory(long id) => await _categoryRepo.GetByIdAsync(id);
    
    //Save changes
    [HttpPatch("{id}")]
    public async Task <Category?> PatchCategory(long id, 
        JsonPatchDocument<Category> patchDocument) => await _categoryRepo.PatchChangesAsync(id, patchDocument);
}
```

#### HomeController
This Controller has a lot of coding so I will provide short explanation and show parts of the code in the homecontroller instead of displaying the whole code.

**Instances**
```csharp
 public class HomeController : Controller {
private readonly IHttpClientFactory _httpClientFactory;
private FinalView? _finalView;
private AddBooksModel? _addBooksModel;
public const int pageSize = 5;

// Other Methods

}
```
To start off, These are instances to the Home Class where: 
- "_httpClientFactory" is used to create HTTP clients for making API requests.
- "_finalView" and "_addBooksModel" hold the data models for the views to use.
- "pageSize" is used to determine how many items to display per page in paginated views.

#### Index
```csharp
 [HttpGet]
 public async Task<IActionResult> Index(long selectedCategory, int page = 1)
 {
var httpClient = _httpClientFactory.CreateClient();

//Category API
string catApi = "http://localhost:5000/api/categories";

//Book API
string bookApi = selectedCategory != 0 ?
   $"http://localhost:5000/api/Books/GetBooks/{selectedCategory}" :
   "http://localhost:5000/api/Books";

HttpResponseMessage responseCategory = await httpClient.GetAsync(catApi);
HttpResponseMessage responseBookApi = await httpClient.GetAsync(bookApi);

if (responseCategory.IsSuccessStatusCode && responseBookApi.IsSuccessStatusCode){
// [Omitted for brevity]
}
 else
     return Content($"Error {responseCategory.StatusCode}");
}
```
In this segment, both API, books and Category, are used to retrive info for the view to render. The HttpResponseMesssage, is used to check if the connection can process the request and return a status code. If the the code is sucesssful, then it proceeds to do the necessary operations, otherwise it will display the error code when rendering the view.

```csharp
if (responseCategory.IsSuccessStatusCode && responseBookApi.IsSuccessStatusCode)
{
    string jsonCatValues = await responseCategory.Content.ReadAsStringAsync();
    List<Category>? categories = JsonConvert.DeserializeObject<List<Category>>(jsonCatValues);

    string jsonBookValues = await responseBookApi.Content.ReadAsStringAsync();
    List<Book>? books = JsonConvert.DeserializeObject<List<Book>>(jsonBookValues);

    //Paginations
    int totalItems = books.Count;          
    int skip = (page - 1) * pageSize;
    List<Book> pagedBooks = books.Skip(skip).Take(pageSize).ToList();

    _finalView = new FinalView
    {
        Categories = categories,
        Books = pagedBooks,
        SelectedCategory = selectedCategory,
        PagingInfo = new PagingInfo
        {
            TotalItems = totalItems,
            ItemPerPage = pageSize,
            CurrentPage = page
        }
    };
    return View(_finalView);
}
```
As mentioned before, after sucessful status code for both api connection, it starts to read into JSON strings and store them, respectively, in jsonCatValues and jsonBookValues.
Then, it gets deserialized into list of objects categories and books. Pagination is also prepared and set for the view. Finally, it gets stored and prepared in an instance of FinalView and populated with the list of categories, paginated books, the selected category, and pagination information for the view to render.

#### AddBook
```csharp
public async Task<IActionResult> AddBook()
{
  // [Omitted for brevity]
    if (responseCategory.IsSuccessStatusCode)
    {
        string jsonCatValues = await responseCategory.Content.ReadAsStringAsync();
        List<Category>? categories = JsonConvert.DeserializeObject<List<Category>>(jsonCatValues);
        _addBooksModel = new AddBooksModel
        {
            Categories = categories
        };
    }
    return View(_addBooksModel);
}
```
Similiar to the previous code, a response is created. If it is sucessful, then it begins to do its operation and store it in an instance of addBooksModel. This part prepares for the selection for a dropdown menu so it can display your available categories and an option to add new categories.

```csharp
[HttpPost]
public async Task<IActionResult> AddBook(AddBooksModel addBooksModel)
{

    if (addBooksModel.Book.CategoryId == 0 && addBooksModel.NewCategory !=null)
    {
        Category c1 = new Category { Name = addBooksModel.NewCategory };
        addBooksModel.Book.Category = c1;
    }


    var httpClient = _httpClientFactory.CreateClient();
    string bookApi = "http://localhost:5000/api/Books";
    var content = new StringContent(JsonConvert.SerializeObject(addBooksModel.Book),Encoding.UTF8, "application/json");
    HttpResponseMessage response = await httpClient.PutAsync(bookApi, content);

    if (response.IsSuccessStatusCode)
    {
        return RedirectToAction(nameof(Index));
    }
    else
    {
        return Content($"Error {response.StatusCode}");
    }
}
```
This part is the submition for addBooks. If the new categories is selected, then a new object for category is created and added to the Book object as well. After the http client is prepared and serialized, it gets sent and await response. if sucessful, then redirects to the homepage (which in Index in this case). otherwise display the error code. 

#### Delete and edit Methods
```csharp
[HttpGet]public async Task<IActionResult> DeleteBook(long id){// [Omitted for brevity]}
[HttpPost]public async Task<IActionResult> DeleteConfirmed(long id){// [Omitted for brevity]}
[HttpGet]public async Task<IActionResult> EditBook(long id){// [Omitted for brevity]}
[HttpPost]public async Task<IActionResult> EditBook(Book book){// [Omitted for brevity]}
```
Similar to AddBooks Methods, each method does Delete and edit operations respectively. After each method finishes their operations, it redirects the user to the home page. 

### MiddleWare
```csharp
 public class TestMiddleware
 {
     private readonly RequestDelegate requestDelegate;

     public TestMiddleware(RequestDelegate requestDelegate) 
         => this.requestDelegate = requestDelegate;

     public async Task Invoke(HttpContext context, DataContext dataContext)
     {
         if(context.Request.Path == "/test")
         {
             await context.Response.WriteAsync($"There are {dataContext.Books.Count()} Books\n");
             await context.Response.WriteAsync($"There are {dataContext.Categories.Count()} categories\n");
            
         }
         else
         {
             //will let the next middleware handle the request
             await requestDelegate(context);
         }
     }
 }
```

A Middleware is created to see simple information on total book and categories count. To see this, search: 

"http://localhost:5000/test" 

after running the program and it will display the information mentioned prior.

### Using Swagger
To test out the API Connections and see if they are communicating to the application properly, I used swagger. On Program.cs Ive added: 

```csharp
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library");
});
```

To use the swagger UI, after running the program, search: 

`http://localhost:5000/swagger/index.html` 

Documentation is possible with Swagger, However I did not include them.

### Conclusion
this project implements a comprehensive virtual bookshelf where users can store and manage their own book collections. The application leverages ASP.NET Core MVC for robust backend operations, integrating Entity Framework Core for database management and Microsoft SQL Server for data storage. The frontend utilizes HTML, CSS, Bootstrap 5, and JavaScript, ensuring a responsive and user-friendly interface.

#### Future Enhancements
Potential improvements and additional features could include:

User Authentication: Adding user roles and permissions to enhance security and provide "sharing bookself" style.
Advanced Search: Implementing more sophisticated search capabilities.
Analytics: Providing more detailed statistics and insights about the user's book collection.
UI Improvements: Enhancing the user interface for better usability and aesthetics.





   
      
