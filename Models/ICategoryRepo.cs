using Microsoft.AspNetCore.JsonPatch;

namespace Library.Models
{
    public interface ICategoryRepo
    {
        IAsyncEnumerable<Category> GetAllCategories();

        Task <Category?> GetByIdAsync (long id);

        Task<Category?> PatchChangesAsync (long id, JsonPatchDocument<Category> patchDocument);

        
    }
}
