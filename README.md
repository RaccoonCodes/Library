# Library

## Overview

This Project is about users storing their own books into this virtual bookself. Hence, your very own Library!  This project includes three controllers: BooksController, CategoriesController, and HomeController, which handle various operations such as retrieving, adding, updating, and deleting books and categories.

This web project focuses heavily on backend work.

## Features
1) CRUD Operations: You can Add, edit, or delete Books from your Library.
2) Pagination: To keep the webpage clean and avoid overflowing with information.
3) Category Filter: Filter Books based on the genre of books you want to look at.
4) REST API: The API endpoints manage user's request and make the changes on local database
5) Swashbuckle: To test API Connection and Operations

## How it was Developed
- ASP.NET Core MVC for Back-end
- HTML, CSS, Bootstrap 5 and javascript for front-end
- Entity Framework Core for database management.
- Microsoft SQL Server for database storage.
- REST API for handling user requests efficiently and return appropriate responses, ensuring smooth interaction between the client and the server.

## Installation
1) Clone Repository into your machine
2) Open your Visual Studio and Download any necessary Nuget Packages or Download them via Command Terminal using the following:
   - **Installing Entity Framework and SQL:**
     
     "dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.0"
     
     "dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.0"
     
   -  **Installing Swashbuckle:**
     
      "dotnet add package Swashbuckle.AspNetCore --version 6.2.3"
      
3) Connection String in appsettings.json is set to local machine, you may change the connection string to your own database.
4) As mentioned before, there is seeded Data to begin with and I personally added some of my own personal books. If update is needed use the follwoing command on Terminal: "dotnet ef database update"
5) If you need to Drop the tables use : "dotnet ef database drop --force". Then Update using previous command

## Behind the code
The following section will describe parts of the code on how they play the role on the web project

**README IN PROGRESS**

   
      
