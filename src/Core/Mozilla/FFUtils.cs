namespace WatiN.Core.Mozilla
{
    public static class FFUtils
    {
        public static string WrappCommandInTimer(string command)
        {
            return "window.setTimeout(function() {" + command + ";" + "}, 5);";
        }
    }
}