using JaricApp.ViewModel.ReferenceBook;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using static Microsoft.Maui.Controls.VisualStateManager;

namespace JaricApp.Views.Сatalogue;

public partial class CategoryView : ContentPage
{
    public ResourceDictionary rd;
	public CategoryView()
	{
		InitializeComponent();
        // привязка к ViewModel
        BindingContext = new ProductCategotyViewModel();
        btnSaveCategory.IsEnabled = false;
        btnEditCategory.IsEnabled = false;
        btnSaveCategory.AutomationId = "";
                
        //определить ширину экрана телефона
        //LabelWidth = Math.Floor(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density)
    }
    void OnEntryNameTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if ((e.OldTextValue == null ? "" : e.OldTextValue) != e.NewTextValue.Trim())
        {
            if (entry.AutomationId=="categoryName")
            {
                btnSaveCategory.IsEnabled = true;
                //btnEditCategory.IsEnabled = true;
            };
            if (entry.AutomationId == "subcategoryName")
            {
                btnSaveSubCategory.IsEnabled = true;
                //btnEditSubCategory.IsEnabled = true;
            };

        };
        //Не активны обе кнопки... если Имя категории пустое и поле ввода Url пустое
        if (e.NewTextValue == "")
        {
            //КАТЕГОРИЯ
            if (entNameCatecory.Text == "" /*&& entUrlCatecory.Text == ""*/)
            {
                btnSaveCategory.IsEnabled = false;
                btnEditCategory.IsEnabled = false;
            };
            //ПОДКАТЕГОРИЯ
            if (entNameSubCatecory.Text == "" /*&& entUrlSubCatecory.Text == ""*/)
            {
                btnSaveSubCategory.IsEnabled = false;
                btnEditSubCategory.IsEnabled = false;
            };
        };

    }
    

}