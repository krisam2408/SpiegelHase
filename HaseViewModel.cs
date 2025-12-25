using SpiegelHase.DataAnnotations;
using SpiegelHase.DataTransfer;
using SpiegelHase.Handlers;
using System.Text;

namespace SpiegelHase;

public class HaseViewModel
{
    [Ignorable] 
    public string MessageString { get; set; }
    
    public MessageHandler? MessageHandler { get; set; } = new();

    public void Initialize()
    {
        if (string.IsNullOrWhiteSpace(MessageString))
            return;

        byte[] hashedMessages = Convert.FromBase64String(MessageString);
        string messages = Encoding.UTF8.GetString(hashedMessages);
        MessageHandler messagesHandler = MessageHandler.Deserialize(messages);
        MessageHandler = messagesHandler;
        
    }

    public void Serialize()
    {
        if (MessageHandler is null)
            return;

        string messages = MessageHandler.Serialize();
        byte[] buffer = Encoding.UTF8.GetBytes(messages);
        MessageString = Convert.ToBase64String(buffer);
        MessageHandler = null;
    }

    public void AddSuccessMessage(string message) => AddMessage(new(message, "Success"));
    
    public void AddInfoMessage(string message) => AddMessage(new(message, "Info"));
    
    public void AddWarningMessage(string message) => AddMessage(new(message, "Warning"));

    public void AddErrorMessage(string message) => AddMessage(new(message, "Error"));
    
    protected virtual void AddMessage(Message message) => MessageHandler?.Add(message);
    
    public string GetSerializedMessages()
    {
        if (MessageHandler is null)
            MessageHandler = new();

        return MessageHandler.Serialize();
    }

    public bool HasInterface(Type interfaceType)
    {
        Type type = GetType();
        Type[] interfaces = type.GetInterfaces();
        return interfaces.Contains(interfaceType);
    }

    public void SetModelMessages(ModelDictionary modelState)
    {
        foreach (ModelEntry entry in modelState)
        {
            if (entry.IsValid)
                continue;

            foreach (ModelError error in entry.Errors)
                AddErrorMessage(error.ErrorMessage);
        }
    }

    public virtual void TransferMessages(HaseViewModel originalModel)
    {
        if (MessageHandler == null)
            MessageHandler = new();

        MessageHandler.TransferMessages(originalModel.MessageHandler);
    }
}