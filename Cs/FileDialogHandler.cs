using System.Windows.Forms;

namespace UsefullClassesDevelopment
{
    /// <summary>
    /// A way to easily reuse a <c>OpenFileDialog</c>.
    /// </summary>
    internal class FileDialogHandler
    {
        public string InitialDirectory = "C:\\";
        public string Filter = "All files (*.*)|*.*";
        public int FilterIndex = 1;
        public bool RestoreDirectory = true;

        /// <summary>
        /// Open a <c>OpenFileDialog</c>.
        /// </summary>
        /// <returns>The filepath that was selected or null if none was selected.</returns>
        public string OpenFileSelector()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                // Initialize our values:
                ofd.InitialDirectory = this.InitialDirectory;
                ofd.Filter = this.Filter;
                ofd.FilterIndex = this.FilterIndex;
                ofd.RestoreDirectory = this.RestoreDirectory;

                DialogResult dialog = ofd.ShowDialog();

                // Check if our dialog went successful.
                switch (dialog)
                {
                    case DialogResult.OK:
                        return ofd.FileName;

                    case DialogResult.Retry:
                        return this.OpenFileSelector();

                    default:
                        return null;
                }
            }
        }
    }
}