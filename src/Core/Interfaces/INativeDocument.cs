



namespace WatiN.Core.Interfaces
{
    public interface INativeDocument
    {
        object Object { get; }
        object Objects { get; }
        INativeElement Body { get; }
        string Url { get; }
        string Title { get; }
        INativeElement ActiveElement { get; }
        void RunScript(string scriptCode, string language);
    }
}
