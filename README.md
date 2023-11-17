![alt text](https://banwo-ighodalo.com/assets/grey-matter/90f0860e3476ab3b271fd8d608253ef4.jpg)


# The Bank of Dreams

This C# console application simulates basic bank/atm functionalities. Users can log in to their accounts, view account details, create new accounts, withdraw, deposit, and transfer money between accounts. Accounts supports six different currencies with automatic conversion between accounts when transfering money. Additionally, there is an admin menu for managing users in the app.

## Features
* User authentication
* Account management (view, create, remove)
* Money transactions (withdraw, deposit, transfer, currency convertion)


## Table of contents
[Introduction](https://github.com/Adrozs/The_Bank/tree/master#the-bank-of-dreams)

[Features](https://github.com/Adrozs/The_Bank/tree/master#features)

[Getting started](https://github.com/Adrozs/The_Bank/tree/master#getting-started)

[Prerequisites](https://github.com/Adrozs/The_Bank/tree/master#prerequisites)

[Installation](https://github.com/Adrozs/The_Bank/tree/master#installing-the-project)

[Configure database](https://github.com/Adrozs/The_Bank/tree/master#configure-database)

[Structure](https://github.com/Adrozs/The_Bank/tree/master#structure)

[Built with](https://github.com/Adrozs/The_Bank/tree/master#built-with)

[Made by](https://github.com/Adrozs/The_Bank/tree/master#made-by-dreamteam)



## Getting started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Make sure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download) (version .NET 6)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (version 15.0.4153.1)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/):
  * EntityFrameworkCore (version 6.0.24)
  * EntityFrameworkCore.SqlServer (version 6.0.24)
  * EntityFrameworkCore.Tools (version 6.0.24)

> [!NOTE]
> _The project was developed using Visual Studio, but you can use your preferred IDE.
> Similarly, you can use your own SQL Server instance, ensuring compatibility with Entity Framework Core._


## Installing the project

A step by step series of examples that tell you how to get the app upp and running.

1. Clone the repository (either with git bash or using the repository link).

```bash
git clone https://github.com/adrozs/the_bank.git

```

2. Navigate to the project directory (if using git bash)

```bash
cd The_Bank
```

3. Build the project (if using git bash)

```bash
dotnet build
```

## Configure database

Ensure that you have A SQL Server instance running. Then update the database connection string located in **DbContext** inside the **Data** folder.

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseSqlServer("your-connection-string");
}
```

An example of the connection string format you can use:

```csharp
"Data Source=(localdb)\\.;Initial Catalog=Bank;Integrated Security=True;Pooling=False"
```


When connected to your database run the Update-Database command in the NuGet Package Manager Console to populate the database with the correct tables and columns.

```nuget
Update-Database
```

## Structure
The code is organized into the following main components:

* Models: Contains the data models for the application.
  * Consists of the 3 tables User, Account & Admin in the database.
* Data: Handles database interactions using Entity Framework.
* Utilities: Contains all classes that help the main program with menues, logic and calculations. 
* Program.cs: Entry point of the application and consists of the base of the program that calls all other methods 


## Built with

* [C#](http://www.dropwizard.io/1.0.2/docs/](https://learn.microsoft.com/en-us/dotnet/csharp/)) - Programming language
* [MS SQL](https://maven.apache.org/](https://learn.microsoft.com/en-us/sql/?view=sql-server-ver16)) - Database
* [Entity Framework](https://rometools.github.io/rome/](https://learn.microsoft.com/en-us/ef/)) - ORM


## Made by Dreamteam

* **Adrian Rozsahegyi** - [Adrozs](https://github.com/Adrozs)
* **Fady Hatta** - [Manhattaa](https://github.com/Manhattaa)
* **Malin Nyberg** - [MalinNyberg](https://github.com/MalinNyberg)
* **Fredrich Benedetti** - [Shakejelly](https://github.com/Shakejelly)
* **Amanda Olving** - [Skokartong](https://github.com/Skokartong)
