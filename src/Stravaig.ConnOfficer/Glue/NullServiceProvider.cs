using System;

namespace Stravaig.ConnOfficer.Glue;

public class NullServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType)
    {
        return null;
    }
}