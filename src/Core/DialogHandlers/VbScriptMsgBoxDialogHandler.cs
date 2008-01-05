namespace WatiN.Core.DialogHandlers
{
	public class VbScriptMsgBoxDialogHandler : BaseDialogHandler
	{
		private bool hasHandledDialog = false;

		public enum Button
		{
			OK = 1,
			Cancel = 2,
			Abort = 3,
			Retry = 4,
			Ignore = 5,
			Yes = 6,
			No = 7
		}

		private Button _button = Button.Yes;

		public VbScriptMsgBoxDialogHandler(Button button)
		{
			_button = button;
		}

		public override bool HandleDialog(Window window)
		{
			if (IsVBScriptAlert(window))
			{
				ButtonToPush(window).Click();

				hasHandledDialog = true;
				return true;
			}

			return false;
		}

		private bool IsVBScriptAlert(Window window)
		{
			return window.StyleInHex == "94C803C5";
		}

		private WinButton ButtonToPush(Window window)
		{
			Button button = IfOKButtonThenGetTheRightButtonId(window, _button);

			return GetButton(window, button);
		}

		/// <summary>
		/// When OK is the only button on the msgbox (buttons value = 1)
		/// then the button Id = 2. In all other situations the button Id
		/// for OK is 1.
		/// </summary>
		/// <param name="window"></param>
		/// <param name="button"></param>
		/// <returns></returns>
		private Button IfOKButtonThenGetTheRightButtonId(Window window, Button button) 
		{
			if (button == Button.OK)
			{
				if (!GetButton(window, Button.OK).Exists())
				{
					return Button.Cancel;
				}
			}

			return button;
		}

		private WinButton GetButton(Window window, Button button) 
		{
			return new WinButton((int)button, window.Hwnd);
		}

		public bool HasHandledDialog
		{
			get { return hasHandledDialog; }
		}
	}
}