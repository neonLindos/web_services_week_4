namespace hw.DTO;

public record ReturnResult<T>(bool Success, T? Data, string? Message = null,
    IDictionary<string, string[]>? Errors = null);
