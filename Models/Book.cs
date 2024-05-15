namespace Library.Models
{
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
}
