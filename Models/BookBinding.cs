//prevent over-binding is to separate data model classes that are used only for 
//receiving data through the model binding process.
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class BookBinding
    {
        [Required]
        public string Title { get; set; } = "";
        
        [Required]
        public string Author { get; set; } = "";
        
        [Range(1,int.MaxValue)]
        public int Edition { get; set; }
        
        [Range(1,int.MaxValue)]
        public int Year { get; set; }

        [Range(1,long.MaxValue)]
        public long CategoryId { get; set; }

        public Book ToBook() => new Book()
        {
            Title = this.Title,
            Author = this.Author,
            Edition = this.Edition,
            Year = this.Year,
            CategoryId = this.CategoryId
        };
    }
}
