using System;
using Nike.CustomerManagement.Domain.Customers.Exception;
using Nike.CustomerManagement.Domain.Customers.ValueObjects;
using Nike.Framework.Domain;

namespace Nike.CustomerManagement.Domain.Customers
{
    public class Customer : AggregateRoot<Guid>
    {
        /// <inheritdoc />
        public Customer(FullName fullName, NationalCode nationalCode, IClock clock)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            NationalCode = nationalCode;
            IsActive = true;
            CreatedAt = clock.Now();
        }

        // FOR ORM!
        private Customer()
        {
        }

        public FullName FullName { get; }

        public NationalCode NationalCode { get; }

        public bool IsActive { get; private set; }

        public void Deactive()
        {
            if (!IsActive)
                throw new CustomerAlreadyDeactivatedException();

            IsActive = false;
        }

        public void Active()
        {
            if (IsActive)
                throw new CustomerAlreadyActivatedException();

            IsActive = true;
        }
    }
}