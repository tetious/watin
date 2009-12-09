namespace WatiN.Core.UtilityClasses
{
    public class VariableNameHelper
    {
        /// <summary>
        /// Used by CreateElementVariableName
        /// </summary>
        private long _elementCounter;

        /// <summary>
        /// Creates a unique variable name, i.e. doc.watin23
        /// </summary>
        /// <returns>A unique variable.</returns>
        public string CreateVariableName()
        {
            if (_elementCounter == long.MaxValue)
            {
                _elementCounter = 0;
            }

            _elementCounter++;
            return string.Format("watin{0}", _elementCounter);
        }

    }
}
