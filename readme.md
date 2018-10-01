
# TranceSQL

TranceSQL provides an easy to use, high performance data access interface for SQL databases.

It is minimally abstract compared to ORMs such as Entity Framework. When using TranceSQL, queries are
constructing using a structured API that closely resembles the final SQL query. Platform specific dialects
provide compatible SQL code for several databases:

 - Postgres
 - Microsoft SQL Server
 - Sqlite
 - MySql
 - Oracle 

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


