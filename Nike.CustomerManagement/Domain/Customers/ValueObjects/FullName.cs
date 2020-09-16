using Nike.Framework.Domain;

namespace Nike.CustomerManagement.Domain.Customers.ValueObjects
{
    public class FullName : ValueObject<FullName>
    {
        /// <inheritdoc />
        public FullName(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        /// <inheritdoc />
        protected override void Validate() { }

        // FOR ORM !
        private FullName() { }
    }
}