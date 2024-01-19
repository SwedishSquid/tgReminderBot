
using System;
using System.Text.Json;
using Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain;

class Program
{
    public static void Main()
    {
        Console.WriteLine("domain expansion!");



        //var remData = new ReminderData(DateTime.Now, 223498, "pls rember");

        //var dict = new Dictionary<long, Entity<ReminderData>>();

        //dict[823] = Entity.Create(823, remData);

        //var s = JsonSerializer.Serialize(dict);

        //Console.WriteLine(s);

        //var des = JsonSerializer.Deserialize<Dictionary<long, Entity<ReminderData>>>(s);

        //Console.WriteLine(des);


        Console.WriteLine(Directory.GetCurrentDirectory());

        var relPath = "../../../../Infrastructure/AppData/testDataBase";

        var path = Path.GetFullPath(relPath);

        Console.WriteLine(path);
        Console.WriteLine(Directory.Exists(path));

    }
}

