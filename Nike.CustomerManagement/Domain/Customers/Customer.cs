using Nike.CustomerManagement.Domain.Customers.Exception;
using Nike.CustomerManagement.Domain.Customers.ValueObjects;
using Nike.Framework.Domain;
using System;

namespace Nike.CustomerManagement.Domain.Customers
{
    public class Customer : AggregateRoot<Guid>
    {
        /// <inheritdoc />
        public Customer(FullName fullName, NationalCode nationalCode, IClock clock)
        {
            this.Id = Guid.NewGuid();
            this.FullName = fullName;
            this.NationalCode = nationalCode;
            this.IsActive = true;
            this.CreatedAt = clock.Now();
        }

        public FullName FullName { get; private set; }

        public NationalCode NationalCode { get; private set; }

        public bool IsActive { get; private set; }

        public void Deactive()
        {
            if (!this.IsActive)
                throw new CustomerAlreadyDeactivatedException();

            this.IsActive = false;
        }

        public void Active()
        {
            if (this.IsActive)
                throw new CustomerAlreadyActivatedException();

            this.IsActive = true;
        }

        // FOR ORM!
        private Customer() { }
    }
}