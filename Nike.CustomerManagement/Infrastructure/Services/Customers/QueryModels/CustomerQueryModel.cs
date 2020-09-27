using System;
using Nike.CustomerManagement.Domain.Customers;

namespace Nike.CustomerManagement.Infrastructure.Services.Customers.QueryModels
{
    public class CustomerQueryModel
    {
        public CustomerQueryModel(Customer customer)
        {
            this.Id = customer.Id;
            this.FirstName = customer.FullName.FirstName;
            this.LastName = customer.FullName.LastName;
            this.NationalCode = customer.NationalCode.Code;
            this.IsActive = customer.IsActive;
            this.CreatedAt = customer.CreatedAt;
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NationalCode { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}