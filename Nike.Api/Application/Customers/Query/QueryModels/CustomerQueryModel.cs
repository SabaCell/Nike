using System;

namespace Nike.Api.Application.Customers.Query.QueryModels
{
    public class CustomerQueryModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string NationalCode { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}