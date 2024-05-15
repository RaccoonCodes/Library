namespace Library.Models
{
    public class AddBooksModel
    {
        public Book Book { get; set;} = new Book();

        public string? NewCategory { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
