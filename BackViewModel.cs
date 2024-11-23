using SpiegelHase.DataAnnotations;
using SpiegelHase.Interfaces;

namespace SpiegelHase;

public class BackViewModel : HaseViewModel, IBack
{
    [Ignorable] public string BackController { get; set; }
    [Ignorable] public string BackAction { get; set; }
    [Ignorable] public string? BackId { get; set; }
}
