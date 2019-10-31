using Grpc.Net.Client;
using GrpcServer;
using GrpcServer.Protos;
using System;
using System.Threading.Tasks;

namespace GrpcClient
{
    class Program {

        static async Task Main(string[] args)
        {
            //var input = new HelloRequest { Name = "Jane Bond" };

            //var channel = GrpcChannel.ForAddress("https://localhost:5001");
            //var client = new Greeter.GreeterClient(channel);

            //var reply = await client.SayHelloAsync(input);

            //Console.WriteLine(reply.Message);

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var customerClient = new CustomerRemote.CustomerRemoteClient(channel);

            //var custInput = new CustomerLookupModel { CustomerId = "FISSA" };
            //var custReply = await customerClient.GetCustomerInfoAsync(custInput);
            //Console.WriteLine($"{custReply.CompanyName} {custReply.City}");

            CustomerModel newCust = new CustomerModel()
            {
                CustomerId = "AAAA",
                CompanyName = "DDDD",
                ContactName = "TTTT",
                City = "Toronto",
                Country = "Canada"
            };

            // await insertCustomer(channel, newCust);

            //await updateCustomer(channel, newCust);

            await deleteCustomer(channel, "AAAA");

            await displayAllCustomers(channel);

            Console.ReadLine();
        }

        static async Task findCustomerById(GrpcChannel channel, string id)
        {
            var customerClient = new CustomerRemote.CustomerRemoteClient(channel);

            var custInput = new CustomerLookupModel { CustomerId = id };
            var custReply = await customerClient.GetCustomerInfoAsync(custInput);
            Console.WriteLine($"{custReply.CompanyName} {custReply.City}");
        }

        static async Task insertCustomer(GrpcChannel channel, CustomerModel cust)
        {
            var customerClient = new CustomerRemote.CustomerRemoteClient(channel);

            var reply = await customerClient.InsertCustomerAsync(cust);
            Console.WriteLine(reply.Result);
        }

        static async Task updateCustomer(GrpcChannel channel, CustomerModel cust)
        {
            var customerClient = new CustomerRemote.CustomerRemoteClient(channel);

            var reply = await customerClient.UpdateCustomerAsync(cust);
            Console.WriteLine(reply.Result);
        }

        static async Task deleteCustomer(GrpcChannel channel, string id)
        {
            var customerClient = new CustomerRemote.CustomerRemoteClient(channel);

            var custInput = new CustomerLookupModel { CustomerId = id };
            var reply = await customerClient.DeleteCustomerAsync(custInput);
            Console.WriteLine(reply.Result);
        }

        static async Task displayAllCustomers(GrpcChannel channel)
        {
            var customerClient = new CustomerRemote.CustomerRemoteClient(channel);

            var empty = new Empty();
            var list = await customerClient.RetrieveAllCustomersAsync(empty);

            Console.WriteLine(">>>>>>>>>>>>>>>>>>++++++++++++<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            foreach (var item in list.Items)
            {
                Console.WriteLine($"{item.CustomerId}: {item.CompanyName}");
            }
        }


    }
}
