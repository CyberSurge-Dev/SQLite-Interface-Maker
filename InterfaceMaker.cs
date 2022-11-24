using System.CodeDom.Compiler;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.IO.Enumeration;
using System.Runtime.InteropServices;
using System.Text;

public class Program
{
    
    public static string Folder = @"{COMPLETE PATH OF DESTINATION FOLDER}";
    public static string Database = @"URI=file: {RELATIVE PATH OF DATABASE EXAMPLE: ./database.db} ";
    public static string Namespace = @"{NAMESPACE OF DATABASE}";
    public static DataTable tList;
    
    public static string[] WrapItems(string recs, string[] suite)
    {
        List<string> output = new List<string>();

        output.Add($"{recs} {{\n");
        foreach (string x in suite)
        {
            output.Add($"   {x}\n");
        }
        output.Add("}");

        return output.ToArray();
    }
    public static string WrapToString(string[] items)
    {
        string output = "";

        foreach (string x in items)
        {
            output += x;
        }

        return output;
    }

    public static ArrayList GetTables()
    {
        ArrayList list = new ArrayList();

        // executes query that select names of all tables in master table of the database
        String query = "SELECT name FROM sqlite_master " +
                "WHERE type = 'table'" +
                "ORDER BY 1";
        try
        {

            DataTable table = GetDataTable(query);

            // Return all table names in the ArrayList

            foreach (DataRow row in table.Rows)
            {
                list.Add(row.ItemArray[0].ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return list;
    }

    public static DataTable GetDataTable(string sql)
    {
        try
        {
            DataTable dt = new DataTable();
            using (var c = new SQLiteConnection(Database))
            {
                c.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, c))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                        return dt;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }

    public static void Main()
    {
        
        List<string> tables = new List<string>(); 
        foreach (var tableName in GetTables())
        {
            string name = GetDataTable("select * from " + tableName).ToString();
            if (name != "sqlite_sequence")
            {
                tables.Add(name);
            }
        }

        // $"namespace {Namespace} {{ \n   internal class CommonData {{ \n      public static string db = @\"URI=file:{Database}\"; \n   }}\n}}";
        string[] CommonDataCLass = WrapItems(recs: "internal class CommonData", suite: new string[] { $"public static string db = @\"{Database}\";" });
        string[] CommonData = WrapItems(recs: $"namespace {Namespace}", suite: CommonDataCLass);
        using (FileStream fs = File.Create(Folder+"\\CommonData.cs"))
        {
            // Add some text to file    
            Byte[] title = new UTF8Encoding(true).GetBytes(WrapToString(CommonData));
            fs.Write(title, 0, title.Length);
        }

        foreach (string TableName in tables)
        {
            if (!Directory.Exists(Folder + $@"{TableName}"))
            {
                Directory.CreateDirectory(Folder + $@"{TableName}");
            }

            using (var con = new SQLiteConnection(Database))
            {

                con.Open();
                var cmd = new SQLiteCommand($"select * from {TableName}", con);

                List<string> fields = new List<string>();

                var dr = cmd.ExecuteReader();
                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string FieldName = dr.GetName(i);
                    Type FieldType = dr.GetFieldType(i);

                    Console.WriteLine(FieldType.ToString().ToLower());

                    if (FieldType.ToString().ToLower() == "system.int64")
                    {
                        fields.Add("public int " + FieldName + " { get; set; }");
                        Console.WriteLine("INT");
                    } 
                    else
                    {
                        fields.Add("public string " + FieldName + " { get; set; }");
                        Console.WriteLine("STRING");
                    }

                    string[] clas = WrapItems(recs: "public class Model", suite: fields.ToArray());
                    string[] final = WrapItems(recs: $"namespace {Namespace}.{TableName}", suite: clas);

                    using (FileStream fs = File.Create(Folder + TableName + @$"\Model.cs"))
                    {
                        // Add some text to file    
                        Byte[] title = new UTF8Encoding(true).GetBytes(WrapToString(final));
                        fs.Write(title, 0, title.Length);
                    }
                    Console.WriteLine($"Wrote to \"{Folder + TableName + @$"\Model.cs"}\":");
                    Console.WriteLine(WrapToString(final));
                }


                // Code for GetItems Function
                List<string> AccessFile = new List<string>();

                string[] usingStatments = new string[] {
                        $"var output = con.Query<{TableName}.Model>(\"select * from {TableName}\", new DynamicParameters());",
                        "return output.ToList();"
                    };

                string[] tempStats = WrapItems(recs: "using (IDbConnection con = new SQLiteConnection(CommonData.db))", suite: usingStatments);
                string[] method = WrapItems(recs: $"public static List<{TableName}.Model> GetItems()", suite: tempStats);

                foreach (string x in method)
                {
                    AccessFile.Add(x);
                }

                // Code for Add Function

                string RegVars = "";
                string AtVars = "";

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    if (dr.GetName(i) != "id")
                    {
                        if (i == dr.FieldCount-1)
                        {
                            RegVars += $"{dr.GetName(i)}";
                            AtVars += $"@{dr.GetName(i)}";
                            Console.WriteLine($"DR MAX RegVars: {RegVars}");
                            Console.WriteLine($"DR MAX AtVars: {AtVars}");
                        }
                        else
                        {
                            RegVars += $"{dr.GetName(i)}, ";
                            AtVars += $"@{dr.GetName(i)}, ";
                            Console.WriteLine($"RegVars: {RegVars}");
                            Console.WriteLine($"AtVars: {AtVars}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("BYPASS \"id\" ");
                    }
                        
                }

                usingStatments = new string[] {
                    $"con.Execute(\"insert into {TableName} ({RegVars}) values ({AtVars})\", model);"
                };
                

                tempStats = WrapItems(recs: "using (IDbConnection con = new SQLiteConnection(CommonData.db))", suite: usingStatments);
                method = WrapItems(recs: $"public static void Add({TableName}.Model model)", suite: tempStats);



                foreach (string x in method)
                {
                    AccessFile.Add(x);
                }

                //Code for RemoveById Function
                usingStatments = new string[] {
                    $"var model = new {TableName}.Model();",
                    $"model.id = ItemId;",
                    $"con.Execute(\"DELETE FROM {TableName} WHERE id = @id\", model);"
                };


                tempStats = WrapItems(recs: "using (IDbConnection con = new SQLiteConnection(CommonData.db))", suite: usingStatments);
                method = WrapItems(recs: $"public static void RemoveById(int ItemId)", suite: tempStats);



                foreach (string x in method)
                {
                    AccessFile.Add(x);
                }

                //Code for ReplaceById Function

                RegVars = "";
                AtVars = "";

                for (var i = 0; i < dr.FieldCount; i++)
                {

                    if (i == dr.FieldCount - 1)
                    {
                        RegVars += $"{dr.GetName(i)}";
                        AtVars += $"@{dr.GetName(i)}";
                        Console.WriteLine($"DR MAX RegVars: {RegVars}");
                        Console.WriteLine($"DR MAX AtVars: {AtVars}");
                    }
                    else
                    {
                        RegVars += $"{dr.GetName(i)}, ";
                        AtVars += $"@{dr.GetName(i)}, ";
                        Console.WriteLine($"RegVars: {RegVars}");
                        Console.WriteLine($"AtVars: {AtVars}");
                    }
                }

                usingStatments = new string[] {
                    $"con.Execute(\"REPLACE INTO {TableName} ({RegVars}) VALUES ({AtVars})\", model);",
                };


                tempStats = WrapItems(recs: "using (IDbConnection con = new SQLiteConnection(CommonData.db))", suite: usingStatments);
                method = WrapItems(recs: $"public static void ReplaceById({TableName}.Model model)", suite: tempStats);



                foreach (string x in method)
                {
                    AccessFile.Add(x);
                }


                string[] WrapClass = WrapItems(recs: $"public class Access", suite: AccessFile.ToArray());
                string[] WrapFinal = WrapItems(recs: $"namespace {Namespace}.{TableName}", suite: WrapClass);
                string finalAccess = "using System.Data.SQLite; using System; using Dapper; using System.Data;\n" + WrapToString(WrapFinal);
                using (FileStream fs = File.Create(Folder + TableName + @$"\Access.cs"))
                {
                    // Add some text to file    
                    Byte[] title = new UTF8Encoding(true).GetBytes(finalAccess);
                    fs.Write(title, 0, title.Length);
                }

                Console.WriteLine($"Wrote to file \"{Folder + TableName + @$"\Access.cs"}\":");
                Console.WriteLine(finalAccess);

            }
        } 

    }
    

    /*
    public static void Main()
    {
        var model = new Database.Tags.Model();

        model.Name = "Tag name 3";

        Database.Tags.Access.Add(model);

        foreach (var x in Database.Tags.Access.GetItems())
        {
            Console.WriteLine(x.Name);
        }
    } */
}

