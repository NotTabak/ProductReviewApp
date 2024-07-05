using System.Windows;

namespace ProductReviewApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ProductReviewViewModel();
        }
    }
}
