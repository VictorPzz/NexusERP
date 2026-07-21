namespace NexusERP.Domain.Common;

public enum ResultType
{
    Ok,
    NotFound,
    ValidationFailed,
    Failure,
    Conflict
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ResultType Type { get; }

    private Result(bool isSuccess, T? value, string? error, ResultType type)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Type = type;
    }

    public static Result<T> Success(T value) => new(true, value, null, ResultType.Ok);
    public static Result<T> NotFound(string error) => new(false, default, error, ResultType.NotFound);
    public static Result<T> ValidationFailed(string error) => new(false, default, error, ResultType.ValidationFailed);
    public static Result<T> Failure(string error) => new(false, default, error, ResultType.Failure);
    public static Result<T> Conflict(string error) => new(false, default, error, ResultType.Conflict);
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public ResultType Type { get; }

    private Result(bool isSuccess, string? error, ResultType type)
    {
        IsSuccess = isSuccess;
        Error = error;
        Type = type;
    }

    public static Result Success() => new(true, null, ResultType.Ok);
    public static Result NotFound(string error) => new(false, error, ResultType.NotFound);
    public static Result ValidationFailed(string error) => new(false, error, ResultType.ValidationFailed);
    public static Result Failure(string error) => new(false, error, ResultType.Failure);
    public static Result Conflict(string error) => new(false, error, ResultType.Conflict);
}
