using JaricApp.Views.Banner;
using JaricApp.Views.Сatalogue;

namespace JaricApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("mainbanner", typeof(MainBanner));
            Routing.RegisterRoute("certificate", typeof(Certificate));
            Routing.RegisterRoute("dopbanner", typeof(DopBanner));
            Routing.RegisterRoute("category", typeof(CategoryView));
        }
    }
}
