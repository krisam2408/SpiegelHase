namespace SpiegelHase.DataTransfer;

public struct Message
{
    public string Content { get; set; }
    public string Level { get; set; }

    public Message(string content, string level)
    {
        Content = content;
        Level = level.ToLower();
    }
}
