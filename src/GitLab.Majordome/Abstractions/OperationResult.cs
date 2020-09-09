namespace GitLab.Majordome.Abstractions
{
    public class OperationResult
    {
        public bool IsSuccess { get; }
        public ErrorType? ErrorType { get; }
        public string? Message { get; }

        private OperationResult(bool isSuccess, string? message = null, ErrorType? errorType = null)
        {
            IsSuccess = isSuccess;
            ErrorType = errorType;
            Message = message;
        }

        public static OperationResult Error(ErrorType errorType, string? message = null) => new OperationResult(false, message, errorType);
        public static OperationResult Ok(string? message = null) => new OperationResult(true, message);
    }
}