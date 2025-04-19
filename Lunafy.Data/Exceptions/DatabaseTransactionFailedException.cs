using System;

namespace Lunafy.Data.Exceptions;

public class DatabaseTransactionFailedException : Exception
{
    public DatabaseTransactionFailedException(Exception inner)
        : base("Database transaction failed and rolled back to previous.", inner)
    { }
}