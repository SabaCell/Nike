using Nike.Framework.Domain;

namespace Nike.CustomerManagement.Domain.Customers.ValueObjects
{
    public class NationalCode : ValueObject<NationalCode>
    {
        /// <inheritdoc />
        public NationalCode(string code)
        {
            Code = code;
        }

        // FOR ORM !
        private NationalCode()
        {
        }

        public string Code { get; }

        /// <inheritdoc />
        protected override void Validate()
        {
        }
    }
}