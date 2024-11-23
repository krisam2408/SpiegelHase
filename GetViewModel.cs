using SpiegelHase.DataAnnotations;
using SpiegelHase.DataTransfer;
using SpiegelHase.Handlers;
using System.Text;

namespace SpiegelHase;

public class GetViewModel : HaseViewModel
{
    [Ignorable] public string MessageString { get; set; }

    protected override void AddMessage(Message message)
    {
        base.AddMessage(message);
        string messages = GetSerializedMessages();
        byte[] hashedMessages = Encoding.UTF8.GetBytes(messages);
        MessageString = Convert.ToBase64String(hashedMessages);
    }

    public override void TransferMessages(HaseViewModel originalModel)
    {
        base.TransferMessages(originalModel);
        string messages = GetSerializedMessages();
        byte[] hashedMessages = Encoding.UTF8.GetBytes(messages);
        MessageString = Convert.ToBase64String(hashedMessages);
    }

    public void Initialize()
    {
        if (!string.IsNullOrWhiteSpace(MessageString))
        {
            byte[] hashedMessages = Convert.FromBase64String(MessageString);
            string messages = Encoding.UTF8.GetString(hashedMessages);
            MessageHandler messagesHandler = MessageHandler.Deserialize(messages);
            MessageHandler = messagesHandler;
        }
    }

    public void Clear()
    {
        MessageHandler = null;
    }
}
