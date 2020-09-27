using Nike.Framework.Domain;

namespace Nike.CustomerManagement.Domain.Customers.ValueObjects
{
    public class NationalCode : ValueObject<NationalCode>
    {
        /// <inheritdoc />
        public NationalCode(string code)
        {
            this.Code = code;
        }

        public string Code { get; private set; }

        /// <inheritdoc />
        protected override void Validate() { }

        // FOR ORM !
        private NationalCode() { }
    }
}