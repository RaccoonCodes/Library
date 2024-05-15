using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
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
}
