using SpiegelHase.Interfaces;

namespace SpiegelHase;

public class ForwardViewModel : BackViewModel, IBack, IForward
{
    public string ForwardId { get; set; }
}
