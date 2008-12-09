



namespace WatiN.Core.Interfaces
{
    public interface INativeDocument
    {
        object Object { get; }
        INativeElement Body { get; }
        string Url { get; }
        string Title { get; }
        INativeElement ActiveElement { get; }
    }
}
