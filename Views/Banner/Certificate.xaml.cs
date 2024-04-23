using JaricApp.ViewModel.CertificateBanner;

namespace JaricApp.Views.Banner;

public partial class Certificate : ContentPage
{
	public Certificate()
	{
		InitializeComponent();
        // привязка к ViewModel
        BindingContext = new MainCertificateViewModel();

        Unloaded += (object sender, EventArgs e) =>
        {
            (((ContentPage)sender).BindingContext as IDisposable)?.Dispose();
        };
    }
}