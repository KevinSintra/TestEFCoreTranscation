// See https://aka.ms/new-console-template for more information
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Text;
using TestEFCoreTranscation;


//InsertData();
//PrintData();
//SelectUsingTransaction();
BatchUpdate();
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

// 測試 Transaction 情境為: LB 下是否能起到互相等待的效果
// 如何測? 開啟兩個 vs 2022 並且將其中一個中斷點放在 tran.Commit() 上，接者另外一個 IDE 執行從 console 會看到指跑到 Start 
void SelectUsingTransaction()
{
    using (var context = new TestDBContext())
    {
        //using (var tran = context.Database.BeginTransaction())
        using (var tran = context.Database.BeginTransaction(IsolationLevel.Serializable))
        {
            Console.WriteLine("Start");

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

void BatchUpdate()
{
    var toDb = new List<Employee>();

    for (int i = 0; i < 10000; i++)
    {
        toDb.Add(new Employee
        {
            Id = Guid.NewGuid(),
            Age = 10,
            CreateBy = "yao",
            CreateAt = DateTimeOffset.Now,
            Name = "yao",
            Remark = "等待更新"
        });
    }

    var update = new Employee
    {
        Id = Guid.NewGuid(),
        Age = 10,
        CreateBy = "yao",
        CreateAt = DateTimeOffset.Now,
        Name = "yao",
        Remark = "等待更新"
    };
    toDb.Add(update);

    using (var db = new TestDBContext())
    {
        var config = new BulkConfig { SetOutputIdentity = false, BatchSize = 4000, UseTempDB = true };

        using (var tran = db.Database.BeginTransaction())
        {
            try
            {
                var watch = new Stopwatch();
                watch.Restart();
                db.BulkInsert(toDb, config);
                watch.Stop();

                watch.Reset();
                watch.Restart();
                db.Employee
                  .Where(p => p.Id == update.Id)
                  .BatchUpdate(new Employee { Remark = "Updated" });
                watch.Stop();

                var count = db.Employee.Count();
                Console.WriteLine($"資料庫存在筆數={count},共花費={watch.Elapsed}");

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }
    }

}
