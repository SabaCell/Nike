using System;

namespace Nike.Exceptions;

public class InvalidOperationException : GeneralException
{
    public const int ExceptionCode = 5;

    public InvalidOperationException(string message, string technicalMessage = "", string operation = "") :
        base(message, technicalMessage, ExceptionCode)
    {
        Operation = operation;
    }

    public InvalidOperationException(string message, string technicalMessage, string operation,
        Exception innerException) : base(message, technicalMessage, innerException, ExceptionCode)
    {
        Operation = operation;
    }

    public string Operation { get; }
}