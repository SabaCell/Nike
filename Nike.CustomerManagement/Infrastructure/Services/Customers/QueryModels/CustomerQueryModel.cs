using System;
using Nike.CustomerManagement.Domain.Customers;

namespace Nike.CustomerManagement.Infrastructure.Services.Customers.QueryModels
{
    public class CustomerQueryModel
    {
        public CustomerQueryModel(Customer customer)
        {
            Id = customer.Id;
            FirstName = customer.FullName.FirstName;
            LastName = customer.FullName.LastName;
            NationalCode = customer.NationalCode.Code;
            IsActive = customer.IsActive;
            CreatedAt = customer.CreatedAt;
        }

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NationalCode { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}