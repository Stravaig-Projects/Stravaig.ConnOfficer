using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ReactiveUI;
using Stravaig.ConnOfficer.Glue;
using System;
using System.Diagnostics;

namespace Stravaig.ConnOfficer.ViewModels;

public class ViewModelBase : ReactiveObject
{
    private readonly IViewModelFactory _factory;

    protected ViewModelBase()
    {
        _factory = NotResolvedViewModelFactory.Instance;
        Logger = NullLogger.Instance;
        Console.WriteLine($"Created {GetType().Name}, but should be instantiated by the DI container.");
        Trace.WriteLine($"Created {GetType().Name}, but should be instantiated by the DI container.");
    }

    protected ViewModelBase(IViewModelFactory factory, ILogger logger)
    {
        _factory = factory;
        Logger = logger;
    }

    protected ILogger Logger { get; }

    protected T CreateViewModel<T>()
        where T : notnull, ViewModelBase
        => _factory.CreateViewModel<T>();

    protected ViewModelBase CreateViewModel(Type type)
        => _factory.CreateViewModel(type);

    protected T CreateViewModel<T>(params object[] args)
        where T : notnull, ViewModelBase
        => _factory.CreateViewModel<T>(args);

    protected ViewModelBase CreateViewModel(Type type, params object[] args)
        => _factory.CreateViewModel(type, args);
}
