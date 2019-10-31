using Grpc.Core;
using GrpcServer.NW;
using GrpcServer.Protos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer.Services
{
    public class CustomersService : CustomerRemote.CustomerRemoteBase
    {
        private readonly ILogger<CustomersService> _logger;
        private readonly NorthwindContext _context;

        public CustomersService(ILogger<CustomersService> logger, NorthwindContext context)
        {
            _logger = logger;
            _context = context;
        }

        public override Task<CustomerModel> GetCustomerInfo(CustomerLookupModel request, ServerCallContext context)
        {
            CustomerModel output = new CustomerModel();

            Customers c = _context.Customers.Find(request.CustomerId);

            _logger.LogInformation("Sending Customer response");

            if (c != null)
            {
                output.CustomerId = c.CustomerId;
                output.CompanyName = c.CompanyName;
                output.ContactName = c.ContactName;
                output.City = c.City;
                output.Country = c.Country;
            }

            return Task.FromResult(output);
        }

        public override Task<Reply> InsertCustomer(CustomerModel request, ServerCallContext context)
        {
            var c = _context.Customers.Find(request.CustomerId);

            if (c != null)
            {
                return Task.FromResult(
                  new Reply()
                  {
                      Result = $"Customer {request.CompanyName} already exists.",
                      IsOk = false
                  }
                );
            }

            Customers customer = new Customers()
            {
                CustomerId = request.CustomerId,
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
                City = request.City,
                Country = request.Country
            };

            _logger.LogInformation("Sending Customer response");

            try
            {
                _context.Customers.Add(customer);
                var returnVal = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return Task.FromResult(
               new Reply()
               {
                   Result = $"Customer {request.CompanyName} was successfully inserted.",
                   IsOk = true
               }
            );
        }

        public override Task<Reply> UpdateCustomer(CustomerModel request, ServerCallContext context)
        {
            var c = _context.Customers.Find(request.CustomerId);

            if (c == null)
            {
                return Task.FromResult(
                  new Reply()
                  {
                      Result = $"Customer {request.CompanyName} cannot be found.",
                      IsOk = false
                  }
                );
            }

            c.CompanyName = request.CompanyName;
            c.ContactName = request.ContactName;
            c.City = request.City;
            c.Country = request.Country;

            _logger.LogInformation("Sending Customer response");

            try
            {
                var returnVal = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return Task.FromResult(
               new Reply()
               {
                   Result = $"Customer {request.CompanyName} was successfully updated.",
                   IsOk = true
               }
            );
        }

        public override Task<Reply> DeleteCustomer(CustomerLookupModel request, ServerCallContext context)
        {
            var c = _context.Customers.Find(request.CustomerId);

            if (c == null)
            {
                return Task.FromResult(
                  new Reply()
                  {
                      Result = $"Customer ID {request.CustomerId} cannot be found.",
                      IsOk = false
                  }
                );
            }

            _logger.LogInformation("Sending Customer response");

            try
            {
                _context.Customers.Remove(c);
                var returnVal = _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return Task.FromResult(
               new Reply()
               {
                   Result = $"Customer with {request.CustomerId} was successfully deleted.",
                   IsOk = true
               }
            );
        }

        public override Task<CustomerList> RetrieveAllCustomers(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Sending Customer response");

            CustomerList list = new CustomerList();

            try
            {
                List<CustomerModel> customerList = new List<CustomerModel>();

                var customers = _context.Customers.ToList();

                foreach (Customers c in customers)
                {
                    customerList.Add(new CustomerModel()
                    {
                        CustomerId = c.CustomerId,
                        CompanyName = c.CompanyName,
                        ContactName = c.ContactName,
                        City = c.City,
                        Country = c.Country
                    });
                }

                list.Items.AddRange(customerList);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return Task.FromResult(list);
        }
    }
}
