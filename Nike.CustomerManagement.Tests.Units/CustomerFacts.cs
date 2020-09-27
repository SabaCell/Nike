using FluentAssertions;
using Nike.CustomerManagement.Domain.Customers;
using Nike.CustomerManagement.Domain.Customers.Exception;
using Nike.CustomerManagement.Domain.Customers.ValueObjects;
using Nike.CustomerManagement.Tests.Units.Constants;
using Nike.CustomerManagement.Tests.Units.TestDoubles;
using System;
using Xunit;

namespace Nike.CustomerManagement.Tests.Units
{
    public class CustomerFacts
    {
        private readonly ClockStub Clock;

        public CustomerFacts()
        {
            Clock = new ClockStub();
        }

        [Fact]
        public void Should_be_able_to_register_new_customer()
        {
            Clock.Adjust(new DateTime(2020, 01, 01));
            var fullName = new FullName(JohnSmith.FirstName, JohnSmith.LastName);
            var nationalCode = new NationalCode(JohnSmith.NationalCode);

            var customer = new Customer(fullName, nationalCode, Clock);

            customer.Id.Should().NotBeEmpty();
            customer.FullName.Should().Be(fullName);
            customer.NationalCode.Should().Be(nationalCode);
            customer.CreatedAt.Should().Be(Clock.Now());
        }

        [Fact]
        public void Customers_by_default_is_active()
        {
            var customer = CreateSomeCustomer();

            customer.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_be_able_to_deactive_customer()
        {
            var customer = CreateSomeCustomer();

            customer.Deactive();

            customer.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_be_able_to_active_deactivated_customer()
        {
            var customer = CreateSomeDeactivatedCustomer();

            customer.Active();

            customer.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Active_already_activated_customer_is_invalid()
        {
            var customer = CreateSomeCustomer();

            Action deactive = () => customer.Active();

            deactive.Should().Throw<CustomerAlreadyActivatedException>();
        }

        [Fact]
        public void Deactive_already_deactivated_customer_is_invalid()
        {
            var customer = CreateSomeDeactivatedCustomer();

            Action deactive = () => customer.Deactive();

            deactive.Should().Throw<CustomerAlreadyDeactivatedException>();
        }

        private Customer CreateSomeCustomer()
        {
            var fullName = new FullName(JohnSmith.FirstName, JohnSmith.LastName);
            var nationalCode = new NationalCode(JohnSmith.NationalCode);

            return new Customer(fullName, nationalCode, Clock);
        }

        private Customer CreateSomeDeactivatedCustomer()
        {
            var customer = CreateSomeCustomer();
            customer.Deactive();
            return customer;
        }
    }
}
