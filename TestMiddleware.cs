using Library.Models;

namespace Library
{
    public class TestMiddleware
    {
        private readonly RequestDelegate requestDelegate;

        public TestMiddleware(RequestDelegate requestDelegate) 
            => this.requestDelegate = requestDelegate;

        public async Task Invoke(HttpContext context, DataContext dataContext)
        {
            if(context.Request.Path == "/test")
            {
                await context.Response.WriteAsync($"There are {dataContext.Books.Count()} Books\n");
                await context.Response.WriteAsync($"There are {dataContext.Categories.Count()} categories\n");
               
            }
            else
            {
                //will let the next middleware handle the request
                await requestDelegate(context);
            }
        }
    }
}
