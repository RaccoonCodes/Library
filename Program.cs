using Microsoft.EntityFrameworkCore;
using Library.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
//using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:BookShelveConnection"]);
    opts.EnableSensitiveDataLogging(true);
});

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBookRepo,EBookRepo>();
builder.Services.AddScoped<ICategoryRepo,ECategoryRepo>();

//Omit Null Values
builder.Services.Configure<MvcNewtonsoftJsonOptions>(options =>{ 
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library", Version = "v1" });
});
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapControllers();
app.MapDefaultControllerRoute();
//app.MapControllerRoute("Default", "{controller = Home}/{action=Index}/{id?}");

app.UseMiddleware<Library.TestMiddleware>();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library");
});



//app.MapGet("/", () => "Hello World!");

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();
