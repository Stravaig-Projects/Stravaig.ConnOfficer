using Microsoft.Extensions.DependencyInjection;
using System;

namespace Stravaig.ConnOfficer.Glue;

public interface IViewModelFactory
{
    T CreateViewModel<T>()
        where T : notnull;

    T CreateViewModel<T>(params object[] args)
        where T : notnull;
}

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateViewModel<T>()
        where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public T CreateViewModel<T>(params object[] args)
        where T : notnull
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, args);
    }
}

public class NotResolvedViewModelFactory : IViewModelFactory
{
    public static readonly NotResolvedViewModelFactory Instance = new();

    public T CreateViewModel<T>()
        where T : notnull
    {
        throw new InvalidOperationException("Cannot create a view model {typeof(T).Name} as the factory has not been resolved.");
    }

    public T CreateViewModel<T>(params object[] args)
        where T : notnull
    {
        throw new InvalidOperationException("Cannot create a view model {typeof(T).Name} as the factory has not been resolved.");
    }
}
