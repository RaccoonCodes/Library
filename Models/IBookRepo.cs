namespace Library.Models
{
    public interface IBookRepo
    {
        Task<Book?> GetBookByIdAsync(long id);
        public IAsyncEnumerable<Book> GetBooksByCategoryIdAsync(long categoryId);
        IAsyncEnumerable<Book> GetAllBooksAsync();
        Task AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(long id);
    }
}
