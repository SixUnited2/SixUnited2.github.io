using System.Windows;

namespace AutoUpdaterApp
{
    public partial class UpdateProgressWindow : Window
    {
        public UpdateProgressWindow()
        {
            InitializeComponent();
        }

        public void UpdateProgress(double percentage)
        {
            downloadProgress.Value = percentage;
            percentageText.Text = $"{percentage:0.0}%";
        }

        public void UpdateProgressText(string text)
        {
            progressText.Text = text;
        }
    }

}