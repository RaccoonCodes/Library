using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
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
}
