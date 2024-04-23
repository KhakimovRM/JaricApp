using JaricApp.DTO.Response;
using JaricApp.ViewModel.BannerWithLegend;

namespace JaricApp.Views.Banner;

public partial class MainBanner : ContentPage
{
    public MainBanner()
    {

        InitializeComponent();
        // �������� � ViewModel
        BindingContext = new WithLegendViewModel();
        bunchSave.Items.Add("�������"); bunchSave.Items.Add("������"); bunchSave.SelectedIndex = 0;
        bunchWeb.Items.Add("�������"); bunchWeb.Items.Add("������"); bunchWeb.SelectedIndex = 0; //VisibleUpBanner.HeightRequest = 520;
        VisibleUpBanner.Source = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", 0);
        Unloaded += (object sender, EventArgs e) =>
        {
            (((ContentPage)sender).BindingContext as IDisposable)?.Dispose();
        };
        // ToolTipProperties.SetText(OpenImageButton, "���� �������� ��������, �� ��������� �������� ����� ��������� ��� �����");
    }

    /// <summary>
    /// ������� ���������� ��� ������� ����� � ������� ��������
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
    /// ��������� �������� ���������� �������� Picker � Web
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