namespace Library.Models
{
    //Category for Books
    public class Category
    {
        public long CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;

        public IEnumerable<Book>? Books { get; set; }
    }
}
