
# SQLite-Interface-Maker

This is a simple program used to make a simple C# interface of a provided `.db` file using `Dapper.Net` and `System.Data.SQLite`

## Setup Instructions
- Make sure your data base has one field named "`id`"(all lowercase) that is unique, an integer, and auto incrementing (You set these when you create your table)
- Install `Dapper.NET` and `System.Data.SQLite` Nuget packages into the project were your interface class will be located.
- Download the `InterfaceMaker.cs` file.
- Open `InterfaceMaker.cs` file and locate the 3 main variables that need to be changed.
- Replace "`{COMPLETE PATH OF DESTINATION FOLDER}`" located on the top variable `Folder` with the complete file path of your destination folder. (Example :  `C:\Temp\DestinationFolder`).
- Replace "`{RELATIVE PATH OF DATABASE EXAMPLE: ./database.db}`" with the relative path of your database (Example(if your file was located at `C:\ParentDir` and your database is located at `C:\ParentDir\DatabaseLocation`): `./DatabaseLocation/DatabaseName.db`).
- Replace "`{NAMESPACE OF DATABASE}`" with the namespace you want for your Interface class.

## Usage Instructions
### Add a model to a table
- Create your model by creating a class "`var model = new {Namespace}.{Table Name}.Model();`" (Example: `var model = new DatabaseInterface.User.Model();`)
- Assign the fields a value individually using "`{the variable you just made}.{Field Name} = {Value};` " (Example: `model.FirstName = "Jack";` or, for an integer value `model.Age = 32;`)
- Add the model to your table using "`{Namespace}.{Table Name}.Access.Add( {model variable} );`" (Example: `DatabaseInterface.User.Access.Add(model);`)

## Usage Instructions

## Created Functions per table in DB
- `GetItems()` -- Returns a complete list of every model in a table
- `Add(   {TableName}.Model model   )` -- Adds a model to a table
- `RemoveById(   int ItemId   )` -- Removes item from specified table based on its id
- `RepaceById(   {TableName}.Model model   )` -- Replaces a model in the specified table with a new model (uses the specified id in the new model to find replacement target)
