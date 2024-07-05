using ProductReviewApp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class ProductReviewViewModel : INotifyPropertyChanged
{
    private Product _selectedProduct;
    private Review _selectedReview;

    public ObservableCollection<Product> Products { get; set; }
    public ObservableCollection<Review> Reviews { get; set; }
    public ICommand AddReviewCommand { get; set; }
    public ICommand DeleteReviewCommand { get; set; }

    public Product SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            OnPropertyChanged();
            LoadReviews();
        }
    }

    public Review SelectedReview
    {
        get => _selectedReview;
        set
        {
            _selectedReview = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ProductReviewViewModel()
    {
        Products = new ObservableCollection<Product>(DatabaseHelper.GetProducts());
        Reviews = new ObservableCollection<Review>();
        AddReviewCommand = new RelayCommand(AddReview);
        DeleteReviewCommand = new RelayCommand(DeleteReview);
    }

    private void LoadReviews()
    {
        if (SelectedProduct != null)
        {
            Reviews.Clear();
            foreach (var review in DatabaseHelper.GetReviews(SelectedProduct.ProductId))
            {
                Reviews.Add(review);
            }
        }
    }

    private void AddReview(object parameter)
    {
        if (SelectedProduct != null)
        {
            var newReview = new Review
            {
                ProductId = SelectedProduct.ProductId,
                Content = "New Review",  // This would be from user input in a real app
                Rating = 5              // This would also be from user input
            };
            DatabaseHelper.AddReview(newReview);
            LoadReviews();
        }
    }

    private void DeleteReview(object parameter)
    {
        if (SelectedReview != null)
        {
            DatabaseHelper.DeleteReview(SelectedReview.ReviewId);
            LoadReviews();
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
