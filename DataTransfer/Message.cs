namespace SpiegelHase.DataTransfer;

public struct Message
{
    public string Content { get; set; }
    public string Level { get; set; }

    public Message(string content, string level)
    {
        Content = CleanContent(content);
        Level = level.ToLower();
    }

    private static string CleanContent(string content)
    {
        content = content
            .Replace("'", "&apos;");

        return content;
    }
}
