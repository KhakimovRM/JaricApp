using JaricApp.DTO.Response;
using JaricApp.ViewModel.BannerWithLegend;

namespace JaricApp.Views.Banner;

public partial class MainBanner : ContentPage
{
    public MainBanner()
    {

        InitializeComponent();
        // привязка к ViewModel
        BindingContext = new WithLegendViewModel();
        bunchSave.Items.Add("Верхнее"); bunchSave.Items.Add("Нижнее"); bunchSave.SelectedIndex = 0;
        bunchWeb.Items.Add("Верхнее"); bunchWeb.Items.Add("Нижнее"); bunchWeb.SelectedIndex = 0; //VisibleUpBanner.HeightRequest = 520;
        VisibleUpBanner.Source = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", 0);
        Unloaded += (object sender, EventArgs e) =>
        {
            (((ContentPage)sender).BindingContext as IDisposable)?.Dispose();
        };
        // ToolTipProperties.SetText(OpenImageButton, "Если выберите картинку, то выбранная картинка будет сохранена как новая");
    }

    /// <summary>
    /// Событие вызывается при измении цвета у надписи картинки
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            switch (picker.StyleId)
            {
                case "Legend1ColorPicker": Legend1ColorE.TextColor = Color.FromHex((picker.ItemsSource[selectedIndex] as ListColor).Value); break;
                case "Legend2ColorPicker": Legend2ColorE.TextColor = Color.FromHex((picker.ItemsSource[selectedIndex] as ListColor).Value); break;
                case "Legend3ColorPicker": Legend3ColorE.TextColor = Color.FromHex((picker.ItemsSource[selectedIndex] as ListColor).Value); break;
                case "Legend4ColorPicker": Legend4ColorE.TextColor = Color.FromHex((picker.ItemsSource[selectedIndex] as ListColor).Value); break;
                case "Legend5ColorPicker": Legend5ColorE.TextColor = Color.FromHex((picker.ItemsSource[selectedIndex] as ListColor).Value); break;
                case "Legend6ColorPicker": Legend6ColorE.TextColor = Color.FromHex((picker.ItemsSource[selectedIndex] as ListColor).Value); break;
            }

        }
    }
    /// <summary>
    /// Изменился значение выбранного элемента Picker в Web
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnPickerWebSelectChanged(object sender, EventArgs e)
    {
       VisibleUpBanner.Source = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", bunchWeb.SelectedIndex);
       //VisibleUpBanner.HeightRequest = bunchWeb.SelectedIndex==0 ? 520 : 345;
    }

    public void Dispose()
    {

    }
}