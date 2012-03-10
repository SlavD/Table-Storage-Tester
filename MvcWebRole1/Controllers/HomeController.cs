using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using System.Data.Services.Client;
using System.Diagnostics;

namespace MvcWebRole1.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        static CloudStorageAccount GetAccountInfo()
        {
            return CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        const string TABLE_NAME = "TestTable";

        public ActionResult DropTable()
        {
            var account = GetAccountInfo();
            account.CreateCloudTableClient().DeleteTableIfExist(TABLE_NAME);

            return View("Index");
        }

        public ActionResult CreateTable()
        {
            var account = GetAccountInfo();
            var newPublicationTable = account.CreateCloudTableClient().CreateTableIfNotExist(TABLE_NAME);

            return View("Index");
        }

        public ActionResult FillWithDataOnePartitionKey(int numberOfRows)
        {
            var sw = new Stopwatch();
            sw.Start();

            var account = GetAccountInfo();
            var context = new TestContext(account.TableEndpoint.AbsoluteUri, account.Credentials);

            if (numberOfRows == 0) numberOfRows = 1000000;

            var entityList = new List<TestEntity>();
            const int MAX_OBJECTS_IN_BATCH = 100;
            int objectsInBatch = 0;
            for (int i = 0; i < numberOfRows; i++)
            {
                var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");
                var rowKey = (DateTime.MaxValue - DateTime.Now).Ticks.ToString("d19") + id;
                var entity = new TestEntity() { PartitionKey ="X", RowKey=rowKey, 
                    FirstName="John" + i.ToString(), LastName = "Doe" + i.ToString(), SomeId = id };

                context.AddObject(TABLE_NAME, entity);

                if (++objectsInBatch == MAX_OBJECTS_IN_BATCH)
                {
                    entityList.Add(entity);
                    objectsInBatch = 0;
                    context.SaveChangesWithRetries(SaveChangesOptions.Batch);
                }
            }

            context.SaveChangesWithRetries(SaveChangesOptions.Batch);

            ViewData["Data"] = entityList;
            ViewData["Elapsed"] = sw.ElapsedMilliseconds;
            return View("Index");
        }

        public ActionResult FillWithDataDifferentPartitionKeys(int numberOfRows)
        {
            var sw = new Stopwatch();
            sw.Start();
            var account = GetAccountInfo();
            var context = new TestContext(account.TableEndpoint.AbsoluteUri, account.Credentials);

            if (numberOfRows == 0) numberOfRows = 1000000;

            var entityList = new List<TestEntity>();

            const int MAX_OBJECTS_IN_BATCH = 100;
            int objectsInBatch = 0;
            string partitionKey = "C";
            for (int i = 0; i < numberOfRows; i++)
            {
                var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");
                var rowKey = (DateTime.MaxValue - DateTime.Now).Ticks.ToString("d19") + id;
                var entity = new TestEntity()
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey,
                    FirstName = "John" + i.ToString(),
                    LastName = "Doe" + i.ToString(),
                    SomeId = id
                };

                context.AddObject(TABLE_NAME, entity);

                if (++objectsInBatch == MAX_OBJECTS_IN_BATCH)
                {
                    entityList.Add(entity);
                    partitionKey += i.ToString();
                    objectsInBatch = 0;
                    context.SaveChangesWithRetries(SaveChangesOptions.Batch);
                }
            }

            context.SaveChangesWithRetries(SaveChangesOptions.Batch);
            ViewData["Data"] = entityList;
            ViewData["Elapsed"] = sw.ElapsedMilliseconds;
            return View("Index");
        }

        public ActionResult QueryByRowkeyAndPartitionKey(string partitionKey, string rowKey)
        {
            var sw = new Stopwatch();
            sw.Start();

            var account = GetAccountInfo();
            var context = new TestContext(account.TableEndpoint.AbsoluteUri, account.Credentials);
            var query = context.CreateQuery<TestEntity>(TABLE_NAME);

            var entity = query.Where(e => e.PartitionKey.Equals(partitionKey) && e.RowKey.Equals(rowKey)).ToList();

            ViewData["Data"] = entity;
            ViewData["Elapsed"] = sw.ElapsedMilliseconds;
            return View("Index");
        }

        public ActionResult QueryByRowKey(string rowKey)
        {
            var sw = new Stopwatch();
            sw.Start();

            var account = GetAccountInfo();
            var context = new TestContext(account.TableEndpoint.AbsoluteUri, account.Credentials);
            var query = context.CreateQuery<TestEntity>(TABLE_NAME);

            var entity = query.Where(e => e.RowKey== rowKey).ToList();

            //var entity = (from e in context.CreateQuery<TestEntity>(TABLE_NAME) where e.RowKey==rowKey select e);
           
            ViewData["Data"] = entity;
            ViewData["Elapsed"] = sw.ElapsedMilliseconds;
            return View("Index");
        }

        public ActionResult QueryByIdColumn(string id)
        {
            var sw = new Stopwatch();
            sw.Start();

            var account = GetAccountInfo();
            var context = new TestContext(account.TableEndpoint.AbsoluteUri, account.Credentials);
            var query = context.CreateQuery<TestEntity>(TABLE_NAME);

            var entity = query.Where(e => e.SomeId == id).ToList();

            ViewData["Data"] = entity;
            ViewData["Elapsed"] = sw.ElapsedMilliseconds;
            return View("Index");
        }

        public ActionResult QueryByLastNameColumn(string lastName)
        {
            var sw = new Stopwatch();
            sw.Start();

            var account = GetAccountInfo();
            var context = new TestContext(account.TableEndpoint.AbsoluteUri, account.Credentials);
            var query = context.CreateQuery<TestEntity>(TABLE_NAME);

            var entity = query.Where(e => e.LastName == lastName).ToList();

            ViewData["Data"] = entity;
            ViewData["Elapsed"] = sw.ElapsedMilliseconds;
            return View("Index");
        }
    }
    
    public class TestEntity : TableServiceEntity {
        public string SomeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class TestContext : TableServiceContext {

        public TestContext(string tableEndpoint, StorageCredentials credentials) : base(tableEndpoint, credentials) { }
    }
}
