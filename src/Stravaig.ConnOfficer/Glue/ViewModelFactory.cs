using Microsoft.Extensions.DependencyInjection;
using Stravaig.ConnOfficer.ViewModels;
using System;

namespace Stravaig.ConnOfficer.Glue;

public interface IViewModelFactory
{
    T CreateViewModel<T>()
        where T : notnull, ViewModelBase;

    ViewModelBase CreateViewModel(Type viewModelType);

    T CreateViewModel<T>(params object[] args)
        where T : notnull, ViewModelBase;

    ViewModelBase CreateViewModel(Type viewModelType, params object[] args);
}

public class ViewModelFactory : IViewModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T CreateViewModel<T>()
        where T : notnull, ViewModelBase
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public ViewModelBase CreateViewModel(Type viewModelType)
    {
        return (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);
    }

    public T CreateViewModel<T>(params object[] args)
        where T : notnull, ViewModelBase
    {
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider, args);
    }

    public ViewModelBase CreateViewModel(Type viewModelType, params object[] args)
    {
        return (ViewModelBase)ActivatorUtilities.CreateInstance(_serviceProvider, viewModelType, args);
    }
}

public class NotResolvedViewModelFactory : IViewModelFactory
{
    public static readonly NotResolvedViewModelFactory Instance = new();

    public T CreateViewModel<T>()
        where T : notnull, ViewModelBase
    {
        throw new InvalidOperationException($"Cannot create a view model {typeof(T).Name} as the factory has not been resolved.");
    }

    public ViewModelBase CreateViewModel(Type viewModelType)
    {
        throw new InvalidOperationException($"Cannot create a view model {viewModelType.Name} as the factory has not been resolved.");
    }

    public T CreateViewModel<T>(params object[] args)
        where T : notnull, ViewModelBase
    {
        throw new InvalidOperationException($"Cannot create a view model {typeof(T).Name} as the factory has not been resolved.");
    }

    public ViewModelBase CreateViewModel(Type viewModelType, params object[] args)
    {
        throw new InvalidOperationException($"Cannot create a view model {viewModelType.Name} as the factory has not been resolved.");
    }
}
