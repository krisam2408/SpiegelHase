namespace SpiegelHase.DataTransfer;

public sealed record ModelError
{
    public string ErrorMessage { get; set; }
    public Exception? Exception { get; set; }

    public ModelError(string message, Exception? exception)
    {
        ErrorMessage = message; 
        Exception = exception;
    }
}
