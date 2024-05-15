namespace Library.Models
{
    public class FinalView
    {
        public PagingInfo PagingInfo { get; set; } = new PagingInfo();
        public List<Book>? Books { get; set; } 
        public List<Category>? Categories { get; set; }

        public long? SelectedCategory { get; set; }
    }
}
