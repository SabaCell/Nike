﻿using Nike.Framework.Domain.Exceptions;

namespace Nike.CustomerManagement.Domain.Customers.Exception;

public class CustomerAlreadyDeactivatedException : DomainException
{
    /// <inheritdoc />
    public override string Message => "Customer already deactivated";
}