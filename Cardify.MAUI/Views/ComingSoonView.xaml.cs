namespace Cardify.MAUI.Views
{
    public partial class ComingSoonView : ContentView
    {
        public ComingSoonView()
        {
            InitializeComponent();
        }

        public void SetContent(string title, string subtitle, string message)
        {
            PageTitleLabel.Text = title;
            PageSubtitleLabel.Text = subtitle;
            MessageLabel.Text = message;
        }
    }
} 