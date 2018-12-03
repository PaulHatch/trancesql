
# TranceSQL

[![NuGet version (TranceSql)](https://img.shields.io/nuget/v/TranceSql.svg?style=flat-square)](https://www.nuget.org/packages/TranceSql/)

TranceSQL provides an easy to use, high performance data access interface for SQL databases.
TranceSQL is an alternative to using an ORM which provides an API for creating and executing
commands and mapping query results back to objects. You can think of it as sitting in between
an ORM like Entity Framework in which queries are generated from object-based DSLs such as LINQ
and lightweight clients like Dapper that map results but require the query be provided as a string.

```
┌──────────────────┐  ╔═════════════════╗   ┌──────────────┐   ┌───────────────┐
│ Entity Framework │  ║    TranceSQL    ║   │    Dapper    │   │    ADO.NET    │
└────────┬─────────┘  ╚════════╦════════╝   └──────┬───────┘   └───────┬───────┘
         │                     │                   │                   │
         │                     │                   │                   │
◀────────●─────────────────────●───────────────────●───────────────────●────────▶
Abstract                                                               Bare Metal
```

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

 TranceSQL supports [OpenTracing].

 [OpenTracing]: https://opentracing.io/

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



## SQL Execution, Fetching & Mapping

Once a query command is ready to be run, the results of the query can be obtained by calling one of the fetch methods. Execution
methods come in four flavors, _synchronous_, _asynchronous_, _cached_ and _deferred_.

## Results Mapping Type

Each flavor of execution provides several mapping options for obtaining query results. The goal of these options is to provide
results in a convenient way, minimizing the amount of code required for transformation to keep data access code clean and easy
to read.

### Conventions-based Mapping

TraceSQL provides conversions based mapping to data types using simple rules, result columns are matched to constructor parameters
or settable property names.

### Multiple Results

There are several options for handling queries which return multiple results. If the results are relate to the same root entity,
for example you are fetching a single `customer` record with a one-to-many set of `orders`, you can simply provide the properties
these are to be mapped to. The first result set will be mapped to the customer, the second result set will automatically be mapped
and assigned to the resulting `Customer.Orders` property.

### Dictionary Mapping

TranceSQL provides two types of mapping results to dictionaries, one for results with two columns, and one for single row
results where each column is mapped to a key.

For a row-keyed dictionary, the first column selected is used as a the dictionary key, the second as the value. This means you
generally want to select a column which is either the primary key or a unique value, or in some other way constrain the query to
ensure that the first column is unique, otherwise an exception will be thrown.

A column-keyed dictionary supports any number of columns (again, column names must be unique), but only maps a single row. This
method can be useful for dynamically generated queries as it does not require a predefined schema.

## Streaming Results

For iterating over large results, the `Command.FetchStream<T>()` method will create an enumerable result
which when enumerated will iterate over the results of the query. Unlike the `Command.FetchList<T>()`
method which loads the entire result into memory, the enumerable returned by `FetchStream` will keep an
open database connection.

No connection is attempted until the result is enumerated, and each enumeration will execute the command.


