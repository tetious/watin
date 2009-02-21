namespace WatiN.Core.Native.Mozilla
{
    internal static class FFUtils
    {
        public static string WrapCommandInTimer(string command)
        {
            return "window.setTimeout(function() {" + command + ";" + "}, 5);";
        }
    }
}