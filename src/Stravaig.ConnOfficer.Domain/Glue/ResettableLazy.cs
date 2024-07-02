namespace Stravaig.ConnOfficer.Domain.Glue;

public class ResettableLazy<T> : IDisposable
{
    #nullable disable
    private readonly Func<T> _valueFactory;
    #nullable enable
    private readonly LazyThreadSafetyMode _threadSafetyMode;
    private Lazy<T> _lazy;
    private bool _isDisposed = false;

    public ResettableLazy(T obj)
    {
        _lazy = new Lazy<T>(obj);
    }

    public ResettableLazy(Func<T> valueFactory)
    {
        _valueFactory = valueFactory;
        _threadSafetyMode = LazyThreadSafetyMode.ExecutionAndPublication;
        _lazy = BuildLazy();
    }

    public ResettableLazy(Func<T> valueFactory, bool isThreadSafe)
    {
        _valueFactory = valueFactory;
        _threadSafetyMode = isThreadSafe ? LazyThreadSafetyMode.ExecutionAndPublication : LazyThreadSafetyMode.None;
        _lazy = BuildLazy();
    }

    public ResettableLazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
    {
        _valueFactory = valueFactory;
        _threadSafetyMode = mode;
        _lazy = BuildLazy();
    }

    public event LazyValueMaybeChangedEventHandler? LazyValueMaybeChanged;

    public bool IsValueCreated
    {
        get
        {
            ThrowIfDisposed();
            return _lazy.IsValueCreated;
        }
    }

    public T Value
    {
        get
        {
            ThrowIfDisposed();
            return _lazy.Value;
        }
    }

    public void Reset()
    {
        if (_valueFactory != null)
        {
            _lazy = BuildLazy();
            OnLazyValueMaybeChanged();
        }
    }

    public void Dispose()
    {
        DisposeOfLazyValue();
        LazyValueMaybeChanged = null;
        _isDisposed = true;
        _lazy = new Lazy<T>(default(T)!);
    }

    private Lazy<T> BuildLazy()
    {
        return new Lazy<T>(_valueFactory, _threadSafetyMode);
    }

    private void OnLazyValueMaybeChanged()
    {
        LazyValueMaybeChanged?.Invoke(this, LazyValueMaybeChangedEventArgs.Instance);
    }

    private void DisposeOfLazyValue()
    {
        try
        {
            if (_lazy.IsValueCreated && _lazy.Value is IDisposable disposableValue)
            {
                disposableValue.Dispose();
            }
        }
        catch
        {
            // Lazy<T>.Value can throw exceptions and if this is in a Dispose situation don't throw exceptions.
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
    }
}

public delegate void LazyValueMaybeChangedEventHandler(object? sender, LazyValueMaybeChangedEventArgs e);

public class LazyValueMaybeChangedEventArgs
{
    internal static readonly LazyValueMaybeChangedEventArgs Instance = new();
}
