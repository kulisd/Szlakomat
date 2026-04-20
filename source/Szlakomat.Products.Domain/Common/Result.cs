namespace Szlakomat.Products.Domain.Common;

public abstract record Result<TF, TS>
{
    public sealed record Success : Result<TF, TS>
    {
        public TS Value { get; }

        public Success(TS value)
        {
            Value = value;
        }
    }

    public sealed record Failure : Result<TF, TS>
    {
        public TF Value { get; }

        public Failure(TF value)
        {
            Value = value;
        }
    }

    public bool IsSuccess() => this is Success;
    public bool IsFailure() => this is Failure;

    public TS? GetSuccess() => this switch
    {
        Success s => s.Value,
        _ => default
    };

    public TF? GetFailure() => this switch
    {
        Failure f => f.Value,
        _ => default
    };

    public TS SuccessValue => this is Success s
        ? s.Value
        : throw new InvalidOperationException("Result is not Success");

    public TF FailureValue => this is Failure f
        ? f.Value
        : throw new InvalidOperationException("Result is not Failure");

    public Result<TF, TR> BiMap<TR>(Func<TS, TR> successMapper, Func<TF, TF> failureMapper)
    {
        Guard.IsNotNull(successMapper);
        Guard.IsNotNull(failureMapper);
        return this switch
        {
            Success s => new Result<TF, TR>.Success(successMapper(s.Value)),
            Failure f => new Result<TF, TR>.Failure(failureMapper(f.Value)),
            _ => throw new InvalidOperationException()
        };
    }

    public Result<TF, TR> Map<TR>(Func<TS, TR> mapper)
    {
        Guard.IsNotNull(mapper);
        return this switch
        {
            Success s => new Result<TF, TR>.Success(mapper(s.Value)),
            Failure f => new Result<TF, TR>.Failure(f.Value),
            _ => throw new InvalidOperationException()
        };
    }

    public Result<TL, TS> MapFailure<TL>(Func<TF, TL> mapper)
    {
        Guard.IsNotNull(mapper);
        return this switch
        {
            Success s => new Result<TL, TS>.Success(s.Value),
            Failure f => new Result<TL, TS>.Failure(mapper(f.Value)),
            _ => throw new InvalidOperationException()
        };
    }

    public Result<TF, TS> Peek(Action<TS> successConsumer, Action<TF> failureConsumer)
    {
        Guard.IsNotNull(successConsumer);
        Guard.IsNotNull(failureConsumer);
        switch (this)
        {
            case Success s:
                successConsumer(s.Value);
                break;
            case Failure f:
                failureConsumer(f.Value);
                break;
        }
        return this;
    }

    public Result<TF, TS> PeekSuccess(Action<TS> successConsumer)
    {
        Guard.IsNotNull(successConsumer);
        return Peek(successConsumer, _ => { });
    }

    public Result<TF, TS> PeekFailure(Action<TF> failureConsumer)
    {
        Guard.IsNotNull(failureConsumer);
        return Peek(_ => { }, failureConsumer);
    }

    public TR IfSuccessOrElse<TR>(Func<TS, TR> successMapping, Func<TF, TR> failureMapping)
    {
        Guard.IsNotNull(successMapping);
        Guard.IsNotNull(failureMapping);
        return this switch
        {
            Success s => successMapping(s.Value),
            Failure f => failureMapping(f.Value),
            _ => throw new InvalidOperationException()
        };
    }

    public Result<TF, TR> FlatMap<TR>(Func<TS, Result<TF, TR>> mapping)
    {
        Guard.IsNotNull(mapping);
        return this switch
        {
            Success s => mapping(s.Value),
            Failure f => new Result<TF, TR>.Failure(f.Value),
            _ => throw new InvalidOperationException()
        };
    }

    public TU Fold<TU>(Func<TF, TU> leftMapper, Func<TS, TU> rightMapper)
    {
        Guard.IsNotNull(leftMapper);
        Guard.IsNotNull(rightMapper);
        return this switch
        {
            Success s => rightMapper(s.Value),
            Failure f => leftMapper(f.Value),
            _ => throw new InvalidOperationException()
        };
    }

    public Result<TFailure, TSuccess> Combine<TFailure, TSuccess>(
        Result<TF, TS> secondResult,
        Func<TF?, TF?, TFailure> failureCombiner,
        Func<TS, TS, TSuccess> successCombiner)
    {
        Guard.IsNotNull(secondResult);
        Guard.IsNotNull(failureCombiner);
        Guard.IsNotNull(successCombiner);

        if (IsSuccess() && secondResult.IsSuccess())
        {
            return new Result<TFailure, TSuccess>.Success(
                successCombiner(GetSuccess()!, secondResult.GetSuccess()!));
        }

        var firstFailure = IsFailure() ? GetFailure() : default;
        var secondFailure = secondResult.IsFailure() ? secondResult.GetFailure() : default;
        return new Result<TFailure, TSuccess>.Failure(failureCombiner(firstFailure, secondFailure));
    }

    public static Result<TF, TS> SuccessOf(TS value) => new Success(value);
    public static Result<TF, TS> FailureOf(TF value) => new Failure(value);

    public static CompositeResult<TF, TS> Composite() => new(new List<TS>());
    public static CompositeSetResult<TF, TS> CompositeSet() => new(new HashSet<TS>());

}

public sealed class CompositeResult<TF, TS>
{
    private readonly Result<TF, List<TS>> _result;

    public CompositeResult(List<TS> initialList)
    {
        _result = new Result<TF, List<TS>>.Success(initialList);
    }

    private CompositeResult(TF failure)
    {
        _result = new Result<TF, List<TS>>.Failure(failure);
    }

    public CompositeResult<TF, TS> Accumulate(Result<TF, TS> newResult)
    {
        Guard.IsNotNull(newResult);
        if (_result.IsFailure())
        {
            return this;
        }
        if (newResult.IsFailure())
        {
            return new CompositeResult<TF, TS>(newResult.GetFailure()!);
        }

        var accumulated = new List<TS>(_result.GetSuccess()!);
        accumulated.Add(newResult.GetSuccess()!);
        return new CompositeResult<TF, TS>(accumulated);
    }

    public bool IsSuccess() => _result.IsSuccess();
    public bool IsFailure() => _result.IsFailure();
    public Result<TF, List<TS>> ToResult() => _result;
}

public sealed class CompositeSetResult<TF, TS>
{
    private readonly Result<TF, HashSet<TS>> _result;

    public CompositeSetResult(HashSet<TS> initialSet)
    {
        _result = new Result<TF, HashSet<TS>>.Success(initialSet);
    }

    private CompositeSetResult(TF failure)
    {
        _result = new Result<TF, HashSet<TS>>.Failure(failure);
    }

    public CompositeSetResult<TF, TS> Accumulate(Result<TF, TS> newResult)
    {
        Guard.IsNotNull(newResult);
        if (_result.IsFailure())
        {
            return this;
        }
        if (newResult.IsFailure())
        {
            return new CompositeSetResult<TF, TS>(newResult.GetFailure()!);
        }

        var accumulated = new HashSet<TS>(_result.GetSuccess()!);
        accumulated.Add(newResult.GetSuccess()!);
        return new CompositeSetResult<TF, TS>(accumulated);
    }

    public bool IsSuccess() => _result.IsSuccess();
    public bool IsFailure() => _result.IsFailure();
    public Result<TF, HashSet<TS>> ToResult() => _result;
}
