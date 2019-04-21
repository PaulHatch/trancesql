
# TranceSQL

[![NuGet version (TranceSql)](https://img.shields.io/nuget/v/TranceSql.svg?style=flat-square)](https://www.nuget.org/packages/TranceSql/)

TranceSQL provides an easy to use, high performance data access interface for SQL databases.
TranceSQL is an alternative to using an ORM which provides an API for creating and executing
commands and mapping query results back to objects. You can think of it as sitting in between
an ORM like Entity Framework in which queries are generated from object-based DSLs such as LINQ
and lightweight clients like Dapper that map results but require the query be provided as a string.

TranceSQL provides a command API for modeling SQL queries and an extensible translation layer
for converting results to usable objects. The command definition and command "rendering" are
separated into two different steps, allowing for differences between different dialects of SQL
to be accounted for in the resulting SQL command. For example SQL Server uses the `TOP`
keyword to indicate the maximum number of rows to return from a `SELECT`, whereas other vendors
use `LIMIT` or require specialized `WHERE` clauses. TranceSQL provides platform specific drivers
to render compatible SQL code for several dialects:

 - Postgres
 - Microsoft SQL Server
 - Sqlite
 - MySql
 - Oracle

 For more information, see documentation in the [wiki](https://github.com/PaulHatch/trancesql/wiki)

## Basic Example

TranceSQL commands leverage collection initialization and implicit type conversion extensively to make code
generation simple.

```csharp
public string GetUsername(int id)
{
	var database = new SqlServerDatabase(connectionString);
	var command = new Command(database)
	{
		new Select
		{
			Column = { "FirstName" }
			From = "Users"
			Where = Condition.Equal("UserID", id)
		}
	};

	// This executes as "SELECT [FirstName] FROM [Users] WHERE [UserID] = @P1"
	var user = command.Fetch<User>();

	return user.FirstName;
}
```

## Features

### Automatic Query Parameterization

Values can be included in various query building methods. These values are automatically converted to parameters
when the query is executed. In addition, parameter converters may be added to provide application-specific conversion
of values to parameters.

To create a automatic parameter, use the `Value` class.


### Deferred Execution

Deferred execution provides a way to execute multiple queries within a single command and maps the results back to each request.
This can be useful for keeping your code simple when building components that compose. For example if you need some user specific
information along with some page specific queries.

