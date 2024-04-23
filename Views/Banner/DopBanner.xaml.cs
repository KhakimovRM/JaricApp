using JaricApp.ViewModel.DopBanner;

namespace JaricApp.Views.Banner;

public partial class DopBanner : ContentPage
{
	public DopBanner()
	{
        InitializeComponent();
        // �������� � ViewModel
        BindingContext = new DopBannerViewModel();

        Unloaded += (object sender, EventArgs e) =>
        {
            (((ContentPage)sender).BindingContext as IDisposable)?.Dispose();
        };
    }
}