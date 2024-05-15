using Library.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private FinalView? _finalView;
        private AddBooksModel? _addBooksModel;
        public const int pageSize = 5;

        //Finish clean up,overview everything and document

        public HomeController(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

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
            else
                return Content($"Error {responseCategory.StatusCode}");
        }

        public async Task<IActionResult> AddBook()
        {
            var httpClient = _httpClientFactory.CreateClient();
            string catApi = "http://localhost:5000/api/Categories";

            HttpResponseMessage responseCategory = await httpClient.GetAsync(catApi);

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

        [HttpGet]
        public async Task<IActionResult> DeleteBook(long id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            string bookApi = $"http://localhost:5000/api/Books/{id}";

            HttpResponseMessage response = await httpClient.GetAsync(bookApi);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                Book? book = JsonConvert.DeserializeObject<Book>(jsonString);
                return View(book);
            }
            else
            {
                return Content($"Error {response.StatusCode}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            string deleteUrl = $"http://localhost:5000/api/Books/{id}";
            HttpResponseMessage response = await httpClient.DeleteAsync(deleteUrl);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Content($"Error {response.StatusCode}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditBook(long id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            string bookApi = $"http://localhost:5000/api/Books/{id}";

            HttpResponseMessage response = await httpClient.GetAsync(bookApi);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                Book? book = JsonConvert.DeserializeObject<Book>(jsonString);
                return View(book);
            }
            else
            {
                return Content($"Error {response.StatusCode}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(Book book)
        {
            var httpClient = _httpClientFactory.CreateClient();
            string bookApi = $"http://localhost:5000/api/Books";

            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
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
    }
}
