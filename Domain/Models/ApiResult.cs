namespace Domain.Model;

public class ApiResult(object Data, int StatusCode, bool IsSuccess, IEnumerable<string> Messages)
{
    public object Data { get; } = Data;
    public int StatusCode { get; } = StatusCode;
    public bool IsSuccess { get; } = IsSuccess;
    public IEnumerable<string> Messages { get; } = Messages;
}
