using WatiN.Core.Interfaces;

namespace WatiN.Core.Mozilla
{
    public class FFDocument : INativeDocument
    {
        public FireFoxClientPort ClientPort { get; set; }

        public FFDocument(FireFoxClientPort clientPort)
        {
            ClientPort = clientPort;
        }

        public object Object
        {
            get { return GetBodyReference(); }
        }

        public object Objects
        {
            get { return GetBodyReference(); }
        }

        public INativeElement Body
        {
            get
            {
                return new FFElement(GetBodyReference(), ClientPort);
            }
        }

        private static string GetBodyReference()
        {
            return string.Format("{0}.body", FireFoxClientPort.DocumentVariableName);
        }

        public string Url
        {
            get { return ClientPort.WriteAndRead("{0}.location.href", FireFoxClientPort.WindowVariableName); }
        }

        public string Title
        {
            get { return ClientPort.WriteAndRead("{0}.title", FireFoxClientPort.DocumentVariableName); }
        }

        public INativeElement ActiveElement
        {
            get
            {
                // TODO: Make "activeElement" a FireFox constant
                var propertyName = "activeElement";

                var elementvar = FireFoxClientPort.CreateVariableName();
                var command = string.Format("{0}={1}.{2};{0}==null", elementvar, FireFoxClientPort.DocumentVariableName, propertyName);
                ClientPort.Write(command);

                return !ClientPort.LastResponseAsBool ? new FFElement(elementvar, ClientPort) : null;
            }
        }

        public void RunScript(string scriptCode, string language)
        {
            ClientPort.Write(scriptCode);
        }
    }
}
