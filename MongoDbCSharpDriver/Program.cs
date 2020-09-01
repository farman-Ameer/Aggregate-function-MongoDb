using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MongoDbCSharpDriver
{
  public  class Program
    {
      public  static void Main(string[] args)
        {
            string InputName;
            MongoClient dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            MongoDbFun ObjmongoDbFun = new MongoDbFun(dbClient);

            List<Customer> customers = new List<Customer>();
            customers.Add(new Customer { name="Jake", fatherName="John",age=20 });
            //if (ObjmongoDbFun.CreateCustomer(customers))
            //    Console.WriteLine("Record Inserted");

            ObjmongoDbFun.GetCustomers();
            Console.WriteLine("Type first name to count number of appearances");
            InputName = Console.ReadLine();
            ObjmongoDbFun.AggregationMdb(InputName);
            Console.Read();
        }
    }

   public class MongoDbFun
    {
       public MongoClient dbClient;
      
       public MongoDbFun(MongoClient _dbClient)
        {
            this.dbClient = _dbClient;
        }

        public bool CreateCustomer(List<Customer> custList)
        {
            try
            {
                IMongoDatabase db = dbClient.GetDatabase("mycustomers");
                var collection = db.GetCollection<Customer>("customers");
                collection.InsertMany(custList);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return true;
        }

        public void GetCustomers()
        {
            try
            {
                IMongoDatabase db = dbClient.GetDatabase("mycustomers");
                var Customers = db.GetCollection<BsonDocument>("customers");
                var resultDoc = Customers.Find(new BsonDocument()).ToList();

                foreach (var item in resultDoc)
                {
                    Console.WriteLine(item.ToString());
                }

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public void AggregationMdb(string _name)
        {
            IMongoDatabase db = dbClient.GetDatabase("mycustomers");
            var Customers = db.GetCollection<BsonDocument>("customers");
            var match = new BsonDocument {
                {
                    "$match",new BsonDocument{
                        { "name", _name}
                        }
                    }
                };


            var pipeline = new[] { match };
            var result = Customers.Aggregate<BsonDocument>(pipeline);
            int count = result.ToList().Count;

            if (count > 0)
            {
                Console.WriteLine(_name + " appeared " + count.ToString() + " times in the list");
            }
            else
            {
                Console.WriteLine(_name + " doesn't exist in the list");
            }
        }
    }
    public class Customer
    {
        public string name { get; set; }
        public string fatherName { get; set; }
        public int age { get; set; }
    }
}
