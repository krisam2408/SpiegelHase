namespace SpiegelHase.Interfaces;

public interface IPagination
{
    public int CurrentPage { get; set; }
    public int MaxPages { get; set; }
    public int TotalItems { get; set; }
}

public interface IPagination<T> : IPagination
{
    public T[] List { get; set; }
}
