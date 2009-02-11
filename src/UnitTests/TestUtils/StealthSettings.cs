namespace WatiN.Core.UnitTests
{
    public class StealthSettings : DefaultSettings
    {
        public StealthSettings()
        {
            SetDefaults();
        }

        public override void Reset()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            base.Reset();
            AutoMoveMousePointerToTopLeft = false;
            HighLightElement = false;
            MakeNewIeInstanceVisible = false;
        }
    }
}