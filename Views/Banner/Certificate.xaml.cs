using JaricApp.ViewModel.CertificateBanner;

namespace JaricApp.Views.Banner;

public partial class Certificate : ContentPage
{
	public Certificate()
	{
		InitializeComponent();
        // �������� � ViewModel
        BindingContext = new MainCertificateViewModel();

        Unloaded += (object sender, EventArgs e) =>
        {
            (((ContentPage)sender).BindingContext as IDisposable)?.Dispose();
        };
    }
}