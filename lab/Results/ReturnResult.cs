namespace ProductLab.Results;

public class ReturnResult<T>
{
    public bool IsSuccess { get; init; }

    public T? Result { get; init; }

    public List<string> ErrorMessage { get; init; } = [];

    public static ReturnResult<T> Success(T? result)
    {
        return new ReturnResult<T>
        {
            IsSuccess = true,
            Result = result,
            ErrorMessage = []
        };
    }

    public static ReturnResult<T> Failure(params string[] errors)
    {
        return new ReturnResult<T>
        {
            IsSuccess = false,
            Result = default,
            ErrorMessage = [.. errors]
        };
    }
}
