using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
    public static class SeedData
    {
        public static void SeedDatabase(DataContext context)
        {
            context.Database.Migrate();
            if(context.Books.Count() == 0 && context.Categories.Count() == 0 )
            {
                Category c1 = new Category { Name = "ASP.NET Development" };
                Category c2 = new Category { Name = "Programming Languages" };

                context.Books.AddRange(
                 new Book
                {
                    Title = "Pro ASP.NET Core 6",
                    Author = "Adam Freeman",
                    Edition = 9,
                    Year =  2022,
                    Category = c1
                },

                new Book
                {
                    Title = "Pro Entity Framework Core 2 for ASP.NET Core MVC",
                    Author = "Adam Freeman",
                    Edition = 1,
                    Year = 2022,
                    Category = c1
                },
                 new Book
                {
                    Title = "C++ Concurrency in Action",
                    Author = "Anthony Williams",
                    Edition = 1,
                    Year = 2012,
                    Category = c2
                }
                );
                context.SaveChanges();
            }
        }
    }
}
