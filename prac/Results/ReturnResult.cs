namespace TeamPractice.Results;

public class ReturnResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public T? Data { get; init; }

    public static ReturnResult<T> Ok(T? data, string message = "OK")
    {
        return new ReturnResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ReturnResult<T> Error(string message)
    {
        return new ReturnResult<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
