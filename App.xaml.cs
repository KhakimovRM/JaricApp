namespace JaricApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState) =>
        new Window(new AppShell())
        {
            Width = 1600,
            Height = 1024,
            X = 100,
            Y = 10
        };
    }
}
