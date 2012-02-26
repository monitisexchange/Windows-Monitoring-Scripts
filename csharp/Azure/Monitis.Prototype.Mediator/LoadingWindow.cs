using System;
using System.Windows.Forms;

namespace Monitis.Prototype.UI
{
    /// <summary>
    /// Represents class for simple form which display some activity
    /// </summary>
    public partial class LoadingWindow : Form
    {
        /// <summary>
        /// Default activity text
        /// </summary>
        private const String DefaultDisplayText = "Loading.... please wait";

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoadingWindow()
        {
            InitializeComponent();
            lblDisplayText.Text = DefaultDisplayText;
        }

        /// <summary>
        /// Constructor with display text for current activity
        /// </summary>
        /// <param name="displayText"></param>
        public LoadingWindow(String displayText)
        {
            InitializeComponent();
            lblDisplayText.Text = displayText;
        }

        /// <summary>
        /// Update display text on form
        /// </summary>
        /// <param name="displayText"></param>
        public void UpdateDisplay(String displayText)
        {
            lblDisplayText.Text = displayText;
            Update();
        }
    }
}