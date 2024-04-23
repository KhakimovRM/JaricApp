using ImageMagick;
using JaricApp.DTO;
using JaricApp.DTO.Response;
using JaricApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JaricApp.ViewModel.BannerWithLegend
{
    public class WithLegendViewModel : INotifyPropertyChanged
    {
        public MagickImage imageFromFile;
        /// <summary>
        /// Текстовое поле в теге <label Text="Размер файла 0 байт"/>. Параметры выбранного файла(картинки)
        /// </summary>
        public string lblImageSize = "Размер файла 0 байт";
        /// <summary>
        /// Текстовое поле в теге <label Text="Разрешение файла: 0х0"/>. Параметры выбранного файла(картинки)
        /// </summary>
        public string lblImageHeightAndWidth = "Разрешение файла: 0х0";
        /// <summary>
        /// Текстовое поле в теге <label Text="Название файла: *.*"/>. Параметры выбранного файла(картинки)
        /// </summary>
        public string lblImageName = "Название файла: *.*";
        /// <summary>
        /// Текстовое полев в теге <label Text="Размер файла 0 байт"/>. Параметры измененного файла(картинки)
        /// </summary>
        public string lblImageSizeModified = "Размер файла 0 байт";
        /// <summary>
        /// Текстовое поле в теге <label Text="Разрешение файла: 0х0"/>. Параметры измененного файла(картинки)
        /// </summary>
        public string lblImageHeightAndWidthModified = "Разрешение файла: 0х0";
        /// <summary>
        /// Текстовое поле в теге <label Text="Формат файла: *.*"/>. Параметры измененного файла(картинки)
        /// </summary>
        public string lblImageNameModified = "Формат файла: *.*";
        /// <summary>
        /// Текстовое поле в теге <label Text="Качество файла: 0%"/>. Параметры измененного файла(картинки) 
        /// </summary>
        public string lblImageQualityModified = "Качество файла: 0%";
        /// <summary>
        /// Ширина картинки баннера в которую преобразуем
        /// </summary>
        public int[] imageWithPattern = new int[2] { 1110, 1110 };
        /// <summary>
        /// Высота  картинки баннера в которую преобразуем
        /// </summary>
        public int[] imageHeightPattern = new int[2] { 520, 345 };

        public int imageWith;
        public int imageHeight;

        public ObservableCollection<BannerWithLegendDto> banners { get; set; }
        /// <summary>
        /// URL адрес для предварительного показа баннеров
        /// </summary>
        public UrlWebViewSource sourceWeb;
        /// <summary>
        /// Для того чтоб работала привязка
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Выбираем(открываем) картинку баннера из файла
        /// </summary>
        public ICommand OpenCommand { get; set; }
        /// <summary>
        /// Получаем с сервера список цветов которыми можно раскрасить надписи
        /// </summary>
        public ICommand GetColorsCommand { get; set; }
        /// <summary>
        /// Сохранем новую картинку баннера
        /// </summary>
        public ICommand SaveCommand { get; set; }
        /// <summary>
        /// Получаем список баннеров с сервера
        /// </summary>
        public ICommand GetCommand { get; set; }
        /// <summary>
        /// Удаляем выбранный баннер с сервера
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        /// <summary>
        /// Перемещаем выбранный баннер на вверх
        /// </summary>
        public ICommand UpCommand { get; set; }
        /// <summary>
        /// Сохраняем изменения(перемещения картинок, видимость картинок)
        /// </summary>
        public ICommand ChangeCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ToggledCommand { get; set; }

        public WithLegendViewModel()
        {
            SelectedImageRow = new BannerWithLegendDto();
            SelectedImageRow.PatternX = 1110;
            SelectedImageRow.PatternY = 520;

            //*******************************************************************************************************************************************
            //**************ПОЛУЧАЕМ ЦВЕТА ДЛЯ ОКРАШИВАНИЯ НАДПИСЕЙ БАННЕРА******************************************************************************
            //*******************************************************************************************************************************************
            GetColorsCommand = new Command(async () =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/reference/colban", string.Empty));

                    HttpResponseMessage response = null;
                    response = await _client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<OutResult<List<ListColor>>>(content, _serializerOptions);
                        if (result.Errors != null && result.Errors.Count() > 0)
                        {
                            await Application.Current.MainPage.DisplayAlert("Получение цветов", "Не удалось получить список цветов для окрашивания надписей баннера", "ОK");
                            return;
                        };
                        //Добавляем в список SELECT выбор цвета
                        foreach (var item in result.Data.ToList())
                        {
                            ListColors.Add(item);
                        }

                    }
                    else await Application.Current.MainPage.DisplayAlert("Получение цветов", "Не удалось получить список цветов для окрашивания надписей баннера" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при получении списка цветов для окрашивания надписей баннера", ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //**************ОТКРЫВАЕМ КАРТИНКУ ИЗ ФАЙЛА**************************************************************************************************
            //*******************************************************************************************************************************************
            OpenCommand = new Command(async () => {

                try
                {
                    //открываем диалог выбора файла
                    var imagesParam = await FilePicker.Default.PickAsync(new PickOptions
                    {
                        PickerTitle = "Pick Barcode/QR Code Image",
                        FileTypes = FilePickerFileType.Images
                    });
                    if (imagesParam != null)
                    {
                        imageFromFile = new MagickImage(imagesParam.FullPath);
                        var _imageFromFile = new MagickImage(imagesParam.FullPath);
                        //Выводим на экран параметры выбанного файла
                        FileInfo info = new FileInfo(imagesParam.FullPath);
                        LblImageSize = String.Format("Размер файла: {0} байт", info.Length.ToString());
                        LblImageHeightAndWidth = String.Format("Разрешение файла: {0}x{1} ", imageFromFile.Width.ToString(), imageFromFile.Height.ToString());
                        LblImageName = String.Format("Название файла: {0} ", info.Name.ToString());

                        //ПРЕОБРАЗУЕМ ФАЙЛ
                        //в тип JPG
                        if (imageFromFile.Format.ToString() != "Jpg") imageFromFile.Format = MagickFormat.Jpg;

                        var size = new MagickGeometry(imageWithPattern[SelectedImageRow.Bunch], imageHeightPattern[SelectedImageRow.Bunch]);
                        size.IgnoreAspectRatio = true; //изменение размера изображения до фиксированного размера без сохранения соотношения сторон.
                        _imageFromFile.Quality = 75;
                        _imageFromFile.Resize(size);
                        SelectedPicture = _imageFromFile.ToByteArray();
                        //SelectedImageRow.Image = _imageFromFile.ToByteArray();


                        //var optimizer = new ImageOptimizer();
                        //optimizer.LosslessCompress(imageFromFile.FileName);

                        ////выводим на экран  выбранную картинку в измененном ввиде
                        //int b = SelectedImageRow.Bunch;
                        //string l1 = SelectedImageRow.Legend1;
                        //string l2 = SelectedImageRow.Legend2;
                        //string l3 = SelectedImageRow.Legend3;
                        //string l4 = SelectedImageRow.Legend4;
                        //string l5 = SelectedImageRow.Legend5;
                        //string l6 = SelectedImageRow.Legend6;
                        //string c1 = SelectedImageRow.Legend1Color;
                        //string c2 = SelectedImageRow.Legend2Color;
                        //string c3 = SelectedImageRow.Legend3Color;
                        //string c4 = SelectedImageRow.Legend4Color;
                        //string c5 = SelectedImageRow.Legend5Color;
                        //string c6 = SelectedImageRow.Legend6Color;
                        //string u = SelectedImageRow.Url;

                        //SelectedImageRow = new BannerWithLegendDto
                        //{
                        //    Image = _imageFromFile.ToByteArray(),
                        //    Bunch = b,
                        //    Legend1 = l1,
                        //    Legend2 = l2,
                        //    Legend3 = l3,
                        //    Legend4 = l4,
                        //    Legend5 = l5,
                        //    Legend6 = l6,
                        //    Legend1Color = c1,
                        //    Legend2Color = c2,
                        //    Legend3Color = c3,
                        //    Legend4Color = c4,
                        //    Legend5Color = c5,
                        //    Legend6Color = c6,
                        //    Url = u,
                        //}; 
                        //SelectedImageRow.Image = null;
                        //SelectedImageRow.Image = imageFromFile.ToByteArray();
                        //  SelectedImage.Source = ImageSource.FromStream(() => new MemoryStream(imageByte));
                        //Image = "<img style=\"max-width:100%; max-height:100%;\" src=data:image/jpg;base64," + Convert.ToBase64String(image) + ">";
                        //MyWebSource.Html = "<img style=\"max-width:100%; max-height:100%;\" src=data:image/jpg;base64," + Convert.ToBase64String(imageByte) + ">";
                        //Выводим параметры файла после изменения
                        LblImageSizeModified = String.Format("Размер файла: {0} байт", SelectedPicture.Length.ToString());
                        LblImageHeightAndWidthModified = String.Format("Разрешение файла: {0}x{1} ", _imageFromFile.Width.ToString(), _imageFromFile.Height.ToString());
                        LblImageNameModified = String.Format("Формат файла: {0} ", _imageFromFile.Format.ToString());
                        LblImageQualityModified = String.Format("Качество файла: {0}% ", _imageFromFile.Quality.ToString());
                        //Фиксируем реальную ширину и высоту картинки
                        imageWith = _imageFromFile.Width;
                        imageHeight = _imageFromFile.Height;
                        //Для того чтоб новая картинка сохранилась а не редактировалась.
                        ModeSave = true; ModeEdit = !ModeSave;
                        // OnPropertyChanged("SelectedImageRow");
                    };

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при окрытие файла", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************СОХРАНЯЕМ КАРТИНКУ НА СЕРВЕР*************************************************************************************************
            //*******************************************************************************************************************************************
            SaveCommand = new Command(async () =>
            {
                Guid IdImage = Guid.NewGuid();
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                //Валидация
                if (SelectedPicture == null) { await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении файла", "Выберите картинку баннера.", "ОK"); return; }

                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите сохранить картинку?", "Да", "Нет")) { return; }

                //Формириуем данные для сохранения в базе данных
                BannerWithLegendDto banner = new BannerWithLegendDto()
                {
                    Id = IdImage,
                    Bunch = SelectedImageRow.Bunch,
                    Legend1 = SelectedImageRow.Legend1 == null ? "" : SelectedImageRow.Legend1.Trim(),
                    Legend2 = SelectedImageRow.Legend2 == null ? "" : SelectedImageRow.Legend2.Trim(),
                    Legend3 = SelectedImageRow.Legend3 == null ? "" : SelectedImageRow.Legend3.Trim(),
                    Legend4 = SelectedImageRow.Legend4 == null ? "" : SelectedImageRow.Legend4.Trim(),
                    Legend5 = SelectedImageRow.Legend5 == null ? "" : SelectedImageRow.Legend5.Trim(),
                    Legend6 = SelectedImageRow.Legend6 == null ? "" : SelectedImageRow.Legend6.Trim(),
                    Legend1Color = SelectedImageRow.Legend1Color == "" ? "#000000" : SelectedImageRow.Legend1Color,
                    Legend2Color = SelectedImageRow.Legend2Color == "" ? "#000000" : SelectedImageRow.Legend2Color,
                    Legend3Color = SelectedImageRow.Legend3Color == "" ? "#000000" : SelectedImageRow.Legend3Color,
                    Legend4Color = SelectedImageRow.Legend4Color == "" ? "#000000" : SelectedImageRow.Legend4Color,
                    Legend5Color = SelectedImageRow.Legend5Color == "" ? "#000000" : SelectedImageRow.Legend5Color,
                    Legend6Color = SelectedImageRow.Legend6Color == "" ? "#000000" : SelectedImageRow.Legend6Color,
                    Image = SelectedPicture,//SelectedImageRow.Image,
                    Url = SelectedImageRow.Url == null ? "" : SelectedImageRow.Url.Trim(),
                    PatternX = imageWithPattern[SelectedImageRow.Bunch],
                    PatternY = imageHeightPattern[SelectedImageRow.Bunch],
                    RealImageX = imageWith,
                    RealImageY = imageHeight

                };

                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/savebanleg", string.Empty));

                    string json = JsonSerializer.Serialize<BannerWithLegendDto>(banner, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await _client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string content_out = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerWithLegendDto>>(content_out, _serializerOptions);
                        var groups = Banners.GroupBy(p => p.Bunch).Select(g => new Grouping<int, BannerWithLegendDto>(g.Key, g.OrderBy(x => x.SortBy)));
                        BannerGroups = new ObservableCollection<Grouping<int, BannerWithLegendDto>>(groups);
                        //Выделяем строку которую редактировали
                        SelectedImageRow = Banners.Where(g => g.Id == IdImage).SingleOrDefault();
                        //обновляем страницу предварительного просмотра (как выглядет на сайте)
                        SourceWeb = new UrlWebViewSource() { Url = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", SelectedImageRow.Bunch) };
                        await Application.Current.MainPage.DisplayAlert("Сохранение файла", "Картинка баннера сохранена!", "ОK");
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Сохранение файла", "Картинка баннера не сохранена!" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении файла", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************ПОЛУЧАЕМ СПИСОК БАННЕРОВ С СЕРВЕРА*******************************************************************************************
            //*******************************************************************************************************************************************
            GetCommand = new Command(async () =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                //Banners.Clear();
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/banleg", string.Empty));

                    HttpResponseMessage response = null;
                    response = await _client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerWithLegendDto>>(content, _serializerOptions);
                        var groups = Banners.GroupBy(p => p.Bunch).Select(g => new Grouping<int, BannerWithLegendDto>(g.Key, g.OrderBy(x => x.SortBy)));
                        BannerGroups = new ObservableCollection<Grouping<int, BannerWithLegendDto>>(groups);
                    }
                    else await Application.Current.MainPage.DisplayAlert("Получение баннеров", "Не удалось получить картинки баннеров с сервера" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при получении картинок баннеров", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************УДАЛЯЕМ ВЫБРАННЫЙ БАННЕР С СЕРВЕРА*******************************************************************************************
            //*******************************************************************************************************************************************
            DeleteCommand = new Command(async () =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                //Блокируем кнопки чтоб пользователь во время запроса не смог никуда нажать
                if (SelectedImageRow.Id == new Guid("00000000-0000-0000-0000-000000000000"))
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при удалении рисунка баннера", "Не выбран рисунок баннера", "ОK");
                    return;
                };

                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы действительно хотите удалить картинку?", "Да", "Нет"))
                { return; };
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/delbanleg?id={0}", SelectedImageRow.Id));
                    HttpResponseMessage response = null;
                    response = await _client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerWithLegendDto>>(content, _serializerOptions);
                        var groups = Banners.GroupBy(p => p.Bunch).Select(g => new Grouping<int, BannerWithLegendDto>(g.Key, g.OrderBy(x => x.SortBy)));
                        BannerGroups = new ObservableCollection<Grouping<int, BannerWithLegendDto>>(groups);
                        SelectedImageRow.Image = null;// new BannerWithLegendDto();
                        OnPropertyChanged("SelectedImageRow");
                        //обновляем страницу предварительного просмотра (как выглядет на сайте)
                        SourceWeb = new UrlWebViewSource() { Url = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", SelectedImageRow.Bunch) };
                        await Application.Current.MainPage.DisplayAlert("Удаление картинки", "Картинка баннера удалена!", "ОK");
                    }
                    else await Application.Current.MainPage.DisplayAlert("Удаление файла", "Не удалось удалить картинку баннера с сервера" + await response.Content.ReadAsStringAsync(), "ОK");

                    //SaveImageButton.Text = "Сохранить";
                    ////Удаленную картинку не показваем в редактировании
                    //imageByte = null;
                    // MyWebSource.Html = "<img style=\"max-width:100%; max-height:100%;\" src=''";

                    //ToolTipProperties.SetText(OpenImageButton, "Нажмите чтоб выбрать новую картинку.");
                    //ToolTipProperties.SetText(SaveImageButton, "Нажмите чтоб сохранить картинку.");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при удаление картинки баннера", ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //**************ПЕРЕМЕЩАЕМ ВЫБРАННЫЙ БАННЕР НА ВВЕРХ*****************************************************************************************
            //*******************************************************************************************************************************************
            UpCommand = new Command(async () =>
            {
                if (selectedImageRow.Id == new Guid())
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при перемещении картинки баннера", "Выберите картинку", "ОK");
                    return;
                }
                //находим к верхнему или нижнему баннеру относится выбранная картинка баннера
                int bunch = Banners.Where(x => x.Id == selectedImageRow.Id).Select(x => x.Bunch).First();
                int sortBy = 0; ;
                List<BannerWithLegendDto> b = Banners.Where(x => x.Bunch == bunch).OrderBy(x => x.SortBy).ToList(); //.Reverse();
                int count = b.Count();
                //int selectId = 0;
                //перебираем все картинки данного баннера
                for (int i = 0; i < count; i++)
                {
                    if (b[i].Id == selectedImageRow.Id)
                    {
                        if (i != 0)
                        {
                            sortBy = b[i - 1].SortBy;
                            b[i - 1].SortBy = b[i].SortBy;
                            b[i].SortBy = sortBy;

                        }
                        else
                        {
                            int k;
                            sortBy = b.Max(x => x.SortBy);
                            for (k = count - 1; k > 0; k = k - 1)
                                b[k].SortBy = b[k - 1].SortBy;
                            b[k].SortBy = sortBy;
                        };
                    }
                }
                var groups = Banners.GroupBy(p => p.Bunch).Select(g => new Grouping<int, BannerWithLegendDto>(g.Key, g.OrderBy(x => x.SortBy)));
                BannerGroups = new ObservableCollection<Grouping<int, BannerWithLegendDto>>(groups);
                //обновляем страницу предварительного просмотра (как выглядет на сайте)
                SourceWeb = new UrlWebViewSource() { Url = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", SelectedImageRow.Bunch) };
            });
            //*******************************************************************************************************************************************
            //**************СОХРАНЕМ ИЗМЕНЕНИЯ(перемещения картинок, видимость картинок)*****************************************************************
            //*******************************************************************************************************************************************
            ChangeCommand = new Command(async () =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите сохранить изменения в картинках баннера?", "Да", "Нет")) return;

                List<BannerWithLegendChangedDto> banner = new List<BannerWithLegendChangedDto>();
                //отправляем на сервер только часть
                foreach (var item in Banners)
                {
                    banner.Add(new BannerWithLegendChangedDto
                    {
                        Id = item.Id,
                        Visible = item.Visible,
                        SortBy = item.SortBy
                    });
                };

                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/chanbanleg", string.Empty));

                    string json = JsonSerializer.Serialize<List<BannerWithLegendChangedDto>>(banner, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await _client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string content_out = await response.Content.ReadAsStringAsync();
                        //обновляем страницу предварительного просмотра (как выглядет на сайте)
                        SourceWeb = new UrlWebViewSource() { Url = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", SelectedImageRow.Bunch) };

                        await Application.Current.MainPage.DisplayAlert("Изменение баннера", "Изменения баннера сохранены!", "ОK");
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Изменение баннера", "Изменения баннера не сохранены!" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении изменений", ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //**************РЕДАКТИРУЕМ КАРТИНКУ В БАЗЕ ДАННЫХ*******************************************************************************************
            //*******************************************************************************************************************************************
            EditCommand = new Command(async () =>
            {
                Guid IdImage = (Guid)SelectedImageRow.Id;
                var row = BannerGroups[0].Where(x => x.Id == IdImage).SingleOrDefault();
                if (row == null)
                {
                    row = BannerGroups[1].Where(x => x.Id == IdImage).SingleOrDefault();
                    if (row == null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании картинки баннера", "Не выбрана картинка", "ОK");
                        return;
                    }
                };

                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы действительно хотите редактировать картинку?", "Да", "Нет"))
                    return;

                try
                {
                    //Формириуем данные для сохранения в базе данных
                    BannerWithLegendDto banner = new BannerWithLegendDto()
                    {
                        Id = SelectedImageRow.Id,
                        Bunch = SelectedImageRow.Bunch,
                        Legend1 = SelectedImageRow.Legend1 == null ? "" : SelectedImageRow.Legend1.Trim(),
                        Legend2 = SelectedImageRow.Legend2 == null ? "" : SelectedImageRow.Legend2.Trim(),
                        Legend3 = SelectedImageRow.Legend3 == null ? "" : SelectedImageRow.Legend3.Trim(),
                        Legend4 = SelectedImageRow.Legend4 == null ? "" : SelectedImageRow.Legend4.Trim(),
                        Legend5 = SelectedImageRow.Legend5 == null ? "" : SelectedImageRow.Legend5.Trim(),
                        Legend6 = SelectedImageRow.Legend6 == null ? "" : SelectedImageRow.Legend6.Trim(),
                        Legend1Color = SelectedImageRow.Legend1Color == "" ? "#000000" : SelectedImageRow.Legend1Color,
                        Legend2Color = SelectedImageRow.Legend2Color == "" ? "#000000" : SelectedImageRow.Legend2Color,
                        Legend3Color = SelectedImageRow.Legend3Color == "" ? "#000000" : SelectedImageRow.Legend3Color,
                        Legend4Color = SelectedImageRow.Legend4Color == "" ? "#000000" : SelectedImageRow.Legend4Color,
                        Legend5Color = SelectedImageRow.Legend5Color == "" ? "#000000" : SelectedImageRow.Legend5Color,
                        Legend6Color = SelectedImageRow.Legend6Color == "" ? "#000000" : SelectedImageRow.Legend6Color,
                        Image = SelectedImageRow.Image,
                        Url = SelectedImageRow.Url == null ? "" : SelectedImageRow.Url.Trim(),
                    };

                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/editbanleg", string.Empty));

                    string json = JsonSerializer.Serialize<BannerWithLegendDto>(banner, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await _client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string content_out = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerWithLegendDto>>(content_out, _serializerOptions);
                        var groups = Banners.GroupBy(p => p.Bunch).Select(g => new Grouping<int, BannerWithLegendDto>(g.Key, g.OrderBy(x => x.SortBy)));
                        BannerGroups = new ObservableCollection<Grouping<int, BannerWithLegendDto>>(groups);
                        //Выделяем строку которую редактировали
                        SelectedImageRow = Banners.Where(g => g.Id == IdImage).SingleOrDefault();
                        //обновляем страницу предварительного просмотра (как выглядет на сайте)
                        SourceWeb = new UrlWebViewSource() { Url = string.Format("https://localhost:7091/desktop/previewbanner?bunch={0}", SelectedImageRow.Bunch) };
                        await Application.Current.MainPage.DisplayAlert("Редактирование картинки", "Картинка баннера отредактирована!", "ОK");
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Редактирование картинки", "Картинка баннера не отредактирована!" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании картинки баннера", ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //**************МЕНЯЕМ ВИДИМОСТЬ КАРТИНКИ БАННЕРА********************************************************************************************
            //*******************************************************************************************************************************************
            ToggledCommand = new Command(async (object id) =>
            {
                Guid i = (Guid)id;
                var row = banners.Where(x => x.Id == i).SingleOrDefault();
                var bunch = banners.Where(x => x.Id == i).SingleOrDefault().Bunch;
                //Если у картинки верхнего баннера поменяли видимость
                if (bunch == 0)
                    if (BannerGroups[0].Count() != 0)
                        BannerGroups[0].Where(x => x.Id == i).SingleOrDefault().Visible = !BannerGroups[0].Where(x => x.Id == i).SingleOrDefault().Visible;

                //Если нижний баннер, то все картинки делаем невидимыми, а выбранную видимой
                if (bunch == 1)
                {
                    foreach (var item in BannerGroups[1])
                        item.Visible = false;
                    row.Visible = true;
                };

            });

            //Получаем цвета для окраски надписией
            ((Command)GetColorsCommand).Execute("");
            //Запускаем процедуру получения списка баннеров с сервера
            ((Command)GetCommand).Execute("");
            //Видна кнопка редактирования
            ModeSave = true; ModeEdit = !ModeSave;
        }

        /// <summary>
        /// НАДПИСЬ "Размер файла 0 байт"
        /// </summary>
        public string LblImageSize
        {
            get => lblImageSize;
            set
            {
                if (lblImageSize != value)
                {
                    lblImageSize = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// НАДПИСЬ "Разрешение файла: 0х0"
        /// </summary>
        public string LblImageHeightAndWidth
        {
            get => lblImageHeightAndWidth;
            set
            {
                if (lblImageHeightAndWidth != value)
                {
                    lblImageHeightAndWidth = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Надпись "Название файла: *.*"
        /// </summary>
        public string LblImageName
        {
            get => lblImageName;
            set
            {
                if (lblImageName != value)
                {
                    lblImageName = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// НАДПИСЬ У МОДИФИЦИРОВАННОЙ картинки "Размер файла 0 байт"
        /// </summary>
        public string LblImageSizeModified
        {
            get => lblImageSizeModified;
            set
            {
                if (lblImageSizeModified != value)
                {
                    lblImageSizeModified = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// НАДПИСЬ У МОДИФИЦИРОВАННОЙ картинки "Разрешение файла 0х0"
        /// </summary>
        public string LblImageHeightAndWidthModified
        {
            get => lblImageHeightAndWidthModified;
            set
            {
                if (lblImageHeightAndWidthModified != value)
                {
                    lblImageHeightAndWidthModified = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// НАДПИСЬ У МОДИФИЦИРОВАННОЙ картинки "Тип файла"
        /// </summary>
        public string LblImageNameModified
        {
            get => lblImageNameModified;
            set
            {
                if (lblImageNameModified != value)
                {
                    lblImageNameModified = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// НАДПИСЬ У МОДИФИЦИРОВАННОЙ картинки "Качество картинки"
        /// </summary>
        public string LblImageQualityModified
        {
            get => lblImageQualityModified;
            set
            {
                if (lblImageQualityModified != value)
                {
                    lblImageQualityModified = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Надпись первая
        /// </summary>
        public string entLegend1;
        public string EntLegend1
        {
            get => entLegend1;
            set
            {
                if (entLegend1 != value)
                {
                    entLegend1 = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Надпись вторая
        /// </summary>
        public string entLegend2;
        public string EntLegend2
        {
            get => entLegend2;
            set
            {
                if (entLegend2 != value)
                {
                    entLegend2 = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Надпись третья
        /// </summary>
        public string entLegend3;
        public string EntLegend3
        {
            get => entLegend3;
            set
            {
                if (entLegend3 != value)
                {
                    entLegend3 = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Надпись четвертая
        /// </summary>
        public string entLegend4;
        public string EntLegend4
        {
            get => entLegend4;
            set
            {
                if (entLegend4 != value)
                {
                    entLegend4 = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Надпись пятая
        /// </summary>
        public string entLegend5;
        public string EntLegend5
        {
            get => entLegend5;
            set
            {
                if (entLegend5 != value)
                {
                    entLegend5 = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Надпись шестая
        /// </summary>
        public string entLegend6;
        public string EntLegend6
        {
            get => entLegend6;
            set
            {
                if (entLegend6 != value)
                {
                    entLegend6 = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Источник для показа баннеров как выглядят на сайте. URL
        /// </summary>
        public UrlWebViewSource SourceWeb
        {
            get => sourceWeb;
            set
            {
                sourceWeb = value;
                OnPropertyChanged(nameof(SourceWeb));
            }
        }
        /// <summary>
        /// Список цветов которыми можно вывести надписи к картинкам
        /// Получаем с сервера
        /// </summary>
        public ObservableCollection<ListColor> listColors { get; set; } = new ObservableCollection<ListColor>();
        public ObservableCollection<ListColor> ListColors
        {
            get => listColors;
            set
            {
                if (listColors != value)
                {
                    listColors = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Список хранящихся картинок баннеров на сервере. Получаем с сервера
        /// </summary>
        public ObservableCollection<BannerWithLegendDto> Banners
        {
            get => banners;
            set
            {
                if (banners != value)
                {
                    banners = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Группировка картинок баннера по верхнему и нижнему баннеру
        /// </summary>
        public ObservableCollection<Grouping<int, BannerWithLegendDto>> bannerGroups { get; set; }
        public ObservableCollection<Grouping<int, BannerWithLegendDto>> BannerGroups
        {
            get => bannerGroups;
            set
            {
                if (bannerGroups != value)
                {
                    bannerGroups = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// В текущий момент выбранная картинка баннера
        /// </summary>
        public BannerWithLegendDto selectedImageRow { get; set; } = new BannerWithLegendDto();
        public BannerWithLegendDto SelectedImageRow
        {
            get
            {
                return selectedImageRow;
            }
            set
            {
                if (selectedImageRow != value)
                {
                    SelectedPicture = value.Image;
                    //OnPropertyChanged("SelectedPicture");
                    ModeSave = false; ModeEdit = !ModeSave;

                    if (value.Image != null)
                        imageFromFile = new MagickImage(value.Image);
                    selectedImageRow = value;
                    SelectedIndexP1 = ListColors.FirstOrDefault(x => x.Value == value.Legend1Color) != null ? ListColors.IndexOf(ListColors.First(x => x.Value == value.Legend1Color)!) : 0;//Для того чтоб высветить название цвета при выборе картинки баннера
                    SelectedIndexP2 = ListColors.FirstOrDefault(x => x.Value == value.Legend2Color) != null ? ListColors.IndexOf(ListColors.First(x => x.Value == value.Legend2Color)!) : 0;//Для того чтоб высветить название цвета при выборе картинки баннера
                    SelectedIndexP3 = ListColors.FirstOrDefault(x => x.Value == value.Legend3Color) != null ? ListColors.IndexOf(ListColors.First(x => x.Value == value.Legend3Color)!) : 0;//Для того чтоб высветить название цвета при выборе картинки баннера
                    SelectedIndexP4 = ListColors.FirstOrDefault(x => x.Value == value.Legend4Color) != null ? ListColors.IndexOf(ListColors.First(x => x.Value == value.Legend4Color)!) : 0;//Для того чтоб высветить название цвета при выборе картинки баннера
                    SelectedIndexP5 = ListColors.FirstOrDefault(x => x.Value == value.Legend5Color) != null ? ListColors.IndexOf(ListColors.First(x => x.Value == value.Legend5Color)!) : 0;//Для того чтоб высветить название цвета при выборе картинки баннера
                    SelectedIndexP2 = ListColors.FirstOrDefault(x => x.Value == value.Legend6Color) != null ? ListColors.IndexOf(ListColors.First(x => x.Value == value.Legend6Color)!) : 0;//Для того чтоб высветить название цвета при выборе картинки баннера
                    OnPropertyChanged("SelectedImageRow");
                }
            }
        }

        //Картинка которую выбрали
        public byte[] selectedPicture;
        public byte[] SelectedPicture
        {
            get => selectedPicture;
            set
            {
                if (selectedPicture != value)
                {
                    selectedPicture = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Для вывода названия цвета надписи
        /// в Picker при выборе картинки баннера
        /// </summary>
        public int selectedIndexP1;
        public int SelectedIndexP1
        {
            get => selectedIndexP1;
            set
            {
                //if (selectedIndexP1 != value)
                //  {
                selectedIndexP1 = value;
                OnPropertyChanged();
                // }
                // else selectedIndexP1 = -1;
            }
        }
        public int selectedIndexP2;
        public int SelectedIndexP2
        {
            get => selectedIndexP2;
            set
            {
                selectedIndexP2 = value;
                OnPropertyChanged();
            }
        }
        public int selectedIndexP3;
        public int SelectedIndexP3
        {
            get => selectedIndexP3;
            set
            {
                selectedIndexP3 = value;
                OnPropertyChanged();
            }
        }
        public int selectedIndexP4;
        public int SelectedIndexP4
        {
            get => selectedIndexP4;
            set
            {
                selectedIndexP4 = value;
                OnPropertyChanged();
            }
        }
        public int selectedIndexP5;
        public int SelectedIndexP5
        {
            get => selectedIndexP5;
            set
            {
                selectedIndexP5 = value;
                OnPropertyChanged();
            }
        }
        public int selectedIndexP6;
        public int SelectedIndexP6
        {
            get => selectedIndexP6;
            set
            {
                selectedIndexP6 = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Режим сохранения или редактирования(false-режим сохранения новой картинки баннера)
        /// </summary>
        public bool modeSave = true;
        public bool modeEdit = false;
        public bool ModeSave
        {
            get => modeSave;
            set
            {
                if (modeSave != value)
                {
                    modeSave = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool ModeEdit
        {
            get => modeEdit;
            set
            {
                if (modeEdit != value)
                {
                    modeEdit = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Надпись рекомендованный размер 111
        /// </summary>
        public string recommendSize;
        public string RecommendSize
        {
            get => recommendSize;
            set
            {
                if (recommendSize != value)
                {
                    recommendSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public string bunchSelect;
        public string BunchSelect
        {
            get => bunchSelect;
            set
            {
                if (bunchSelect != value)
                {
                    bunchSelect = value;
                    //Устанавливаем разрешение картинки баннера в зависимости от выбранного баннера(верхнего или нижнего)
                    RecommendSize = "* Рекомендованный размер: 1110х" + imageHeightPattern[SelectedImageRow.Bunch];
                    SelectedImageRow.PatternX = imageWithPattern[SelectedImageRow.Bunch];
                    SelectedImageRow.PatternY = imageHeightPattern[SelectedImageRow.Bunch];
                    if (/*SelectedImageRow.Image != null &&*/ imageFromFile != null)
                    {
                        var _imageFromFile = new MagickImage(imageFromFile);
                        var size = new MagickGeometry(imageWithPattern[SelectedImageRow.Bunch], imageHeightPattern[SelectedImageRow.Bunch]);
                        size.IgnoreAspectRatio = true; //изменение размера изображения до фиксированного размера без сохранения соотношения сторон.

                        _imageFromFile.Quality = 75;
                        _imageFromFile.Resize(size);
                        SelectedPicture = _imageFromFile.ToByteArray();
                        //Выводим параметры файла после изменения
                        LblImageSizeModified = String.Format("Размер файла: {0} байт", SelectedPicture.Length.ToString());
                        LblImageHeightAndWidthModified = String.Format("Разрешение файла: {0}x{1} ", _imageFromFile.Width.ToString(), _imageFromFile.Height.ToString());
                        LblImageNameModified = String.Format("Формат файла: {0} ", _imageFromFile.Format.ToString());
                        LblImageQualityModified = String.Format("Качество файла: {0}% ", _imageFromFile.Quality.ToString());
                        //Фиксируем реальную ширину и высоту картинки
                        imageWith = _imageFromFile.Width;
                        imageHeight = _imageFromFile.Height;
                        //OnPropertyChanged("SelectedImageRow");
                    }

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Чтоб работала привязка MVVVM
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void Dispose()
        {
            //_logger.LogInformation(nameof(DopBannerViewModel) + "." + nameof(Dispose));
        }

    }
}
