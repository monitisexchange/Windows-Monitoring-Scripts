using System;
using System.Windows.Forms;

namespace Monitis.Prototype.UI
{
    /// <summary>
    /// Utils methods
    /// </summary>
    public static class Util
    {
        private static LoadingWindow _loadingWindow;

        /// <summary>
        /// Show loading window to user
        /// </summary>
        public static void ShowWaitWindow()
        {
            if(_loadingWindow != null)
            {
               CloseWaitWindow();
            }
            _loadingWindow = new LoadingWindow();
            _loadingWindow.Show();
            _loadingWindow.Update();
        }

        /// <summary>
        ///  Show loading window to user
        /// </summary>
        /// <param name="displayText">Specify display text</param>
        public static void ShowWaitWindow(String displayText)
        {
            if (_loadingWindow != null)
            {
                CloseWaitWindow();
            }
            _loadingWindow = new LoadingWindow(displayText);
            _loadingWindow.Show();
            _loadingWindow.Update();
        }

        /// <summary>
        /// Update text on loading window if it showing
        /// </summary>
        /// <param name="displayText"></param>
        public static void UpdateWaitWindowDisplay(String displayText)
        {
            if(_loadingWindow != null)
            {
                _loadingWindow.UpdateDisplay(displayText);
            }
        }

        /// <summary>
        /// Close wait window if already opened
        /// </summary>
        public static void CloseWaitWindow()
        {
            if(_loadingWindow != null)
            {
                _loadingWindow.Hide();
                _loadingWindow.Close();
            }
            _loadingWindow = null;
        }

        /// <summary>
        /// Invokes action on UI thread if it needed
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void UIThread(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}