using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using WatiN.Core.DialogHandlers;
using WatiN.Core.Interfaces;
using WatiN.Core.Native.Windows;
using WatiN.Core.UtilityClasses;

namespace WatiN.Core.UnitTests.DialogHandlerTests
{
    /// <summary>
    /// This dialoghandler can be used when it isn't clear which DialogHandler should be used to handle a dialog. 
    /// The property <see cref="DialogHandlerHelper.CandidateDialogHandlers"/> will contain a list of dialoghandlers which can handle this dialog,
    /// bases on calling their <see cref="IDialogHandler.CanHandleDialog"/> method.
    /// </summary>
    /// <example>
    /// Following an example on how to use this Dialoghandler. After the using helper.CandidateDialogHandlers will contain "AlertDialogHandler".
    ///<code>
    /// var helper = new DialogHandlerHelper()
    /// using (new UseDialogOnce(ie.DialogWatcher, helper)
    /// {
    ///     ie.Button("showAlert").Click();
    /// }
    ///</code>
    /// </example>
    public class DialogHandlerHelper : BaseDialogHandler
    {
        private readonly IEnumerable<IDialogHandler> _dialogHandlers;

        /// <summary>
        /// Returns a list of type names of dialoghandlers which can handle this dialog, 
        /// bases on calling their <see cref="IDialogHandler.CanHandleDialog"/> method.
        /// </summary>
        /// <value>The candidate dialog handlers.</value>
        public List<string> CandidateDialogHandlers { get; private set; }

        public DialogHandlerHelper()
        {
            _dialogHandlers = GetDialogHandlers();
            CandidateDialogHandlers = new List<string>();
        }

        public override bool CanHandleDialog(Window window, IntPtr mainWindowHwnd)
        {
            foreach (var dialogHandler in _dialogHandlers)
            {
                HandlePossibleCandidate(dialogHandler, window, mainWindowHwnd);
            }

            return false;
        }

        internal void HandlePossibleCandidate(IDialogHandler dialogHandler, Window window, IntPtr mainWindowHwnd)
        {
            UtilityClass.TryActionIgnoreException(() =>
                                                      {
                                                          if (dialogHandler.CanHandleDialog(window, mainWindowHwnd))
                                                          {
                                                              CandidateDialogHandlers.Add(
                                                                  dialogHandler.GetType().FullName);
                                                          }
                                                      });
        }

        public override bool HandleDialog(Window window)
        {
            throw new NotImplementedException();
        }

        public override bool CanHandleDialog(Window window)
        {
            return false;
        }

        internal static IEnumerable<IDialogHandler> GetDialogHandlers()
        {
            var assembly = Assembly.GetAssembly(typeof(BaseDialogHandler));

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(BaseDialogHandler))) continue;
                if (type.Equals(typeof(DialogHandlerHelper))) continue;
                if (type.IsAbstract) continue;

                yield return (IDialogHandler)FormatterServices.GetUninitializedObject(type);
            }
        }

    }
}