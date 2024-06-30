namespace Stravaig.ConnOfficer.Domain.Glue;

public class ResettableLazy<T>
{
    #nullable disable
    private readonly Func<T> _valueFactory;
    #nullable enable
    private readonly LazyThreadSafetyMode _threadSafetyMode;
    private Lazy<T> _lazy;

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

    public bool IsValueCreated => _lazy.IsValueCreated;

    public T Value => _lazy.Value;

    public void Reset()
    {
        _lazy = _valueFactory == null ? _lazy : BuildLazy();
    }

    private Lazy<T> BuildLazy()
    {
        return new Lazy<T>(_valueFactory, _threadSafetyMode);
    }
}
