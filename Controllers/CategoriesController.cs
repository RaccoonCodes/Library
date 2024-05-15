using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Http;

namespace Library.Controllers
{
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
}
