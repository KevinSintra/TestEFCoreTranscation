// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using TestEFCoreTranscation;


//InsertData();
//PrintData();
SelectUsingTransaction();
Console.WriteLine("Hello, World!");

void InsertData()
{
    using (var context = new TestDBContext())
    {
        // Creates the database if not exists
        context.Database.EnsureCreated();

        // Adds some books
        context.TestModel.Add(new TestModel
        {
            ISBN = "978-0544003420",
            Title = "The Lord of the Rings",
            IsEnable = true,
        });

        // Saves changes
        context.SaveChanges();
    }
}

void PrintData()
{
    using (var context = new TestDBContext())
    {
        var data = context.TestModel.FirstOrDefault();
        var sb = new StringBuilder();
        sb.AppendLine($"ISBN: {data.ISBN}");
        sb.AppendLine($"Title: {data.Title}");
        sb.AppendLine($"isEnabled: {data.IsEnable}");
        Console.WriteLine(sb.ToString());
    }
}

void SelectUsingTransaction()
{
    using (var context = new TestDBContext())
    {
        //using (var tran = context.Database.BeginTransaction(IsolationLevel.Serializable))
        using (var tran = context.Database.BeginTransaction(IsolationLevel.Serializable))
        {
            Console.WriteLine("start");

            // transcation 查詢不管如何都不會去管
            var data = context.TestModel.FirstOrDefault(x => x.ISBN == "978-0544003419" && x.IsEnable == true);
            var sb = new StringBuilder();
            sb.AppendLine($"ISBN: {data?.ISBN ?? "not found"}");
            sb.AppendLine($"Title: {data?.Title ?? "not found"}");
            sb.AppendLine($"isEnabled: {(data == null ? "not found" : data.IsEnable)}");
            Console.WriteLine(sb.ToString());

            if (data != null)
            {
                data.IsEnable = false; // 可以查詢(值無變化) || 無法查詢(值必須有變化)
                context.SaveChanges();
            }
            tran.Commit();
        }
    }
}
