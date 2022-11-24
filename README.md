
# SQLite C# Interface Maker

This is a simple program used to make a simple C# interface of a provided `.db` file using `Dapper.Net` and `System.Data.SQLite`
Note: The code for this project is extremely hard to read, and might be the most "built for one purpose, and one purpose only" kind of code ever written. But if you wish to help with adding to this project, it would be greatly appreciated. 

# Setup Instructions
- Make sure your data base has one field named "`id`"(all lowercase) that is unique, an integer, and auto incrementing (You set these when you create your table)
- Install `Dapper.NET` and `System.Data.SQLite` Nuget packages into the project were your interface class will be located.
- Download the `InterfaceMaker.cs` file.
- Open `InterfaceMaker.cs` file and locate the 3 main variables that need to be changed.
- Replace "`{COMPLETE PATH OF DESTINATION FOLDER}`" located on the top variable `Folder` with the complete file path of your destination folder. (Example :  `C:\Temp\DestinationFolder`).
- Replace "`{RELATIVE PATH OF DATABASE EXAMPLE: ./database.db}`" with the relative path of your database (Example(if your file was located at `C:\ParentDir` and your database is located at `C:\ParentDir\DatabaseLocation`): `./DatabaseLocation/DatabaseName.db`).
- Replace "`{NAMESPACE OF DATABASE}`" with the namespace you want for your Interface class.

# Usage Instructions
### Add a row to a table
- Create your model by creating a class: "`var model = new {Namespace}.{Table Name}.Model();`" (Example: `var model = new DatabaseInterface.User.Model();`)
- Assign the fields a value individually using "`{the variable you just made}.{Field Name} = {Value};` " (Example: `model.FirstName = "Jack";` or, for an integer value `model.Age = 32;`)
- Add the model to your table using "`{Namespace}.{Table Name}.Access.Add( {model variable} );`" (Example: `DatabaseInterface.User.Access.Add(model);`)

### Get list of rows in a table
- Create a list of your items using "`List < {Namespace}.{Table Name}.Model > {List Name} = {Namespace}.{Table Name}.Access.GetItems();`" (Example: `List < DatabaseInterface.User.Model > items = DatabaseInterface.User.Access.GetItems();`).
- Access values within the list using "`{List Name}[ {Index of row} ].{Field Name}`" (Example: `items[0].FirstName` Usually, these statements are assigned to variables).
- You can cycle through these items using a foreach loop: "`foreach (var variable in list) { suite; }`" (Example: `foreach (var x in items) { Console.WriteLine(x.FirstName); }`).
- You can also convert the list to an array if it better suites you needs.
### Remove rows from a table
- Using `{Namespace}.{Table Name}.Access.RemoveById();` You can remove an item from a table by using its integer ID. (Example: `DatabaseInterface.User.Access.RemoveById(3);`).

### Replace rows in a table
- Create the model you wish to replace something with using the same instructions found above in "**Add a row to a table**".
- Assign the id field of the model to the id of the row you want to replace using: "`{model variable name}.id = {id of row you want to replace}`" (Example: `model.id = 23;`).
- Pass the new model into the "`{Namespace}.{Table Name}.Access.ReplaceById({ model variable} );`"  function (Example: `DatabaseInterface.User.Access.ReplaceById(model);`).


# Functions created per table in the database
- `Access.GetItems()` -- Returns a complete list of every model in a table
- `Access.Add(   {TableName}.Model model   )` -- Adds a model to a table
- `Access.RemoveById(   int ItemId   )` -- Removes item from specified table based on its id
- `Access.RepaceById(   {TableName}.Model model   )` -- Replaces a model in the specified table with a new model (uses the specified id in the new model to find replacement target)
- `Model` Class -- A basic class that contains all field names as variables. (Used for most functions)
