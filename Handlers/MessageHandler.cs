using Newtonsoft.Json;
using SpiegelHase.DataTransfer;

namespace SpiegelHase.Handlers;

public class MessageHandler
{
    public List<Message> Messages { get; set; } = new();

    public void Add(Message message)
    {
        Messages.Add(message);
    }

    public string Serialize()
    {
        string result = JsonConvert.SerializeObject(Messages);
        return result;
    }

    public static MessageHandler Deserialize(string serial)
    {
        MessageHandler result = new();
        List<Message>? messages = JsonConvert.DeserializeObject<List<Message>>(serial);

        if (messages == null)
            return result;

        result.Messages = messages;
        return result;
    }

    public void TransferMessages(MessageHandler? originalHandler)
    {
        if (originalHandler == null)
            return;

        foreach (Message ogMsg in originalHandler.Messages)
        {
            foreach (Message rMsg in Messages)
            {
                if (rMsg.Content == ogMsg.Content)
                    continue;
            }

            Messages.Add(ogMsg);
        }
    }
}
