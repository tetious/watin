namespace WatiN.Core.Actions
{
    public interface ITypeTextAction
    {
        void TypeText(string value);
        void AppendText(string value);
        void Clear();
    }
}