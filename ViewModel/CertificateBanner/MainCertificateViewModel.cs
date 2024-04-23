using ImageMagick;
using JaricApp.DTO;
using JaricApp.DTO.Response;
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

namespace JaricApp.ViewModel.CertificateBanner
{
    public class MainCertificateViewModel : INotifyPropertyChanged
    {
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
        /// Временное хранение картинки. Для преобразования картинки баннера
        /// </summary>
        public byte[] imageByte;
        /// <summary>
        /// Картинка баннера ввиде html тега <img ...../>
        /// </summary>
        public byte[] imageWebHtml;
        /// <summary>
        /// Ширина картинки в которую преобразуем
        /// </summary>
        public int imageWithPattern = 310;
        /// <summary>
        /// Высота  картинки в которую преобразуем
        /// </summary>
        public int imageHeightPattern = 520;
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
        /// Полученные с сервера картинки
        /// </summary>
        public ObservableCollection<BannerCertificate> banners = new();
        /// <summary>
        /// Видимость кнопки сохранить картинку баннера
        /// </summary>
        public bool btnSaveIsVisible = true;
        /// <summary>
        /// Видимость кнопки редактировать картинку баннера
        /// </summary>
        public bool btnEditIsVisible = false;


        public string countImage = "";

        public Guid id;
        public byte[] image;
        public string? url;
        public int realImageX;
        public int realImageY;

        public UrlWebViewSource webSourse = new UrlWebViewSource { Url = "https://localhost:7091/desktop/previewbancer" };
        public ObservableCollection<BannerCertificate> Certificates { get; } = new();
        public ICommand OpenCommand { get; set; }
        public ICommand DisplayCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand ChangeCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand RadioChangeCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public MainCertificateViewModel()
        {
            //*******************************************************************************************************************************************
            //**************ОТКРЫВАЕМ КАРТИНКУ ИЗ ФАЙЛА**************************************************************************************************
            //*******************************************************************************************************************************************
            OpenCommand = new Command(async () =>
            {
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
                        using var imageFromFile = new MagickImage(imagesParam.FullPath);
                        //Выводим на экран параметры выбанного файла
                        FileInfo info = new FileInfo(imagesParam.FullPath);
                        LblImageSize = String.Format("Размер файла: {0} байт", info.Length.ToString());
                        LblImageHeightAndWidth = String.Format("Разрешение файла: {0}x{1} ", imageFromFile.Width.ToString(), imageFromFile.Height.ToString());
                        LblImageName = String.Format("Название файла: {0} ", info.Name.ToString());

                        //ПРЕОБРАЗУЕМ ФАЙЛ
                        //в тип JPG
                        if (imageFromFile.Format.ToString() != "Jpg") imageFromFile.Format = MagickFormat.Jpg;
                        var size = new MagickGeometry(imageWithPattern, imageHeightPattern);
                        size.IgnoreAspectRatio = true; //изменение размера изображения до фиксированного размера без сохранения соотношения сторон.
                        imageFromFile.Quality = 75;
                        imageFromFile.Resize(size);
                        //выводим на экран  выбранную картинку в измененном ввиде
                        imageByte = imageFromFile.ToByteArray();
                        ImageWebHtml = imageByte;
                        //ImageWebHtml = "<img style=\"max-width:100%; max-height:100%; \" src=data:image/jpg;base64," + Convert.ToBase64String(imageByte) + ">";
                        //Выводим параметры файла после изменения
                        LblImageSizeModified = String.Format("Размер файла: {0} байт", imageByte.Length.ToString());
                        LblImageHeightAndWidthModified = String.Format("Разрешение файла: {0}x{1} ", imageFromFile.Width.ToString(), imageFromFile.Height.ToString());
                        LblImageNameModified = String.Format("Формат файла: {0} ", imageFromFile.Format.ToString());
                        LblImageQualityModified = String.Format("Качество файла: {0}% ", imageFromFile.Quality.ToString());
                        //Фиксируем реальную ширину и высоту картинки
                        realImageX = imageFromFile.Width;
                        realImageY = imageFromFile.Height;
                        //Для того чтоб новая картинка сохранилась а не редактировалась.
                        BtnSaveIsVisible = true;
                        BtnEditIsVisible = false;
                    };
                    return;
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при окрытие файла", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************ВЫВОДИМ НА ЭКРАН КАРТИНКИ БАННЕРА********************************************************************************************
            //*******************************************************************************************************************************************
            DisplayCommand = new Command(async () =>
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
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/bancer", string.Empty));

                    HttpResponseMessage response = null;
                    response = await _client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerCertificate>>(content, _serializerOptions);
                        CountImage = String.Format("Всего картинок {0}", Banners.Count);
                    }
                    else await Application.Current.MainPage.DisplayAlert("Получение картинки", "Не удалось получить картинки баннера с сервера" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при получение картинок баннера файла", ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //*************СОХРАНЯЕМ КАРТИНКУ БАННЕРА****************************************************************************************************
            //*******************************************************************************************************************************************
            AddCommand = new Command(async () =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                if (imageByte == null || imageByte.Length == 0)
                { await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении картинки", "Выберите картинку баннера.", "ОK"); return; }
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите сохранить картинку?", "Да", "Нет")) return;

                //Формириуем данные для сохранения в базе данных
                BannerCertificateDto banner = new BannerCertificateDto()
                {
                    Image = imageByte,
                    Url = Url.Trim() ?? "",
                    PatternX = imageWithPattern,
                    PatternY = imageHeightPattern,
                    RealImageX = RealImageX,
                    RealImageY = RealImageY
                };

                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/savebancer", string.Empty));

                    string json = JsonSerializer.Serialize<BannerCertificateDto>(banner, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await _client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string content_out = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerCertificate>>(content_out, _serializerOptions);

                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Картинка баннера сохранена!", "ОK");
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Картинка баннера не сохранена!" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении картинки", ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //**************РЕДАКТИРУЕМ КАРТИНКУ БАННЕРА*************************************************************************************************
            //*******************************************************************************************************************************************
            EditCommand = new Command(async (object? args) =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                if (args == null)
                { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании картинки", "Выберите картинку баннера.", "ОK"); return; }
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите отредактировать картинку?", "Да", "Нет")) return;
                if (Url.Trim() == (args as BannerCertificate).Url)
                { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании картинки", "URL ссылка не изменена!.", "ОK"); return; }
                //Формириуем данные для сохранения в базе данных
                BannerCertificateDto banner = new BannerCertificateDto()
                {
                    Id = (args as BannerCertificate).Id,
                    Url = Url.Trim() ?? "",
                };
                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/editbancer", string.Empty));

                    string json = JsonSerializer.Serialize<BannerCertificateDto>(banner, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await _client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string content_out = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerCertificate>>(content_out, _serializerOptions);

                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Картинка баннера отредактирована!", "ОK");
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Картинка баннера не отредактирована!" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании", ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //**************СОХРАНЯЕМ ИЗМЕННИЯ У КАРТИНКИ БАННЕРА(какой из картинок становится видимым на сайте)*****************************************
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
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите изменить видимую картинку?", "Да", "Нет")) return;
                //id картинки баннера у которй надо изменить видимость
                Guid banner = banners.Where(x => x.Visible).Select(q => q.Id).SingleOrDefault();

                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/updbancer?id={0}", banner));

                    string json = JsonSerializer.Serialize<Guid>(banner, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await _client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        string content_out = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerCertificate>>(content_out, _serializerOptions);
                        //Показываем видимую картинку баннера
                        WebSourse = new UrlWebViewSource { Url = "https://localhost:7091/desktop/previewbancer" };

                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Видимость картинки баннера изменена!", "ОK");
                    }
                    else
                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Видимость картинки баннера не изменена!" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при изменении", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************УДАЛЯЕМ КАРТИНКУ БАННЕРА*****************************************************************************************************
            //*******************************************************************************************************************************************
            DeleteCommand = new Command(async (object? args) =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                if (args == null)
                { await Application.Current.MainPage.DisplayAlert("Ошибка при удалении файла", "Выберите картинку баннера.", "ОK"); return; }
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите удалить картинку?", "Да", "Нет")) return;
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/delbancer?id={0}", (args as BannerCertificate).Id));

                    HttpResponseMessage response = null;
                    response = await _client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Banners = JsonSerializer.Deserialize<ObservableCollection<BannerCertificate>>(content, _serializerOptions);

                        await Application.Current.MainPage.DisplayAlert("Удаление картинки", "Картинка баннера удалена!", "ОK");
                    }
                    else await Application.Current.MainPage.DisplayAlert("Удаление файла", "Не удалось удалить картинку баннера с сервера" + await response.Content.ReadAsStringAsync(), "ОK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при удаление картинки баннера", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************ВЫБРАЛИ КАРТИНКУ БАННЕРА*****************************************************************************************************
            //*******************************************************************************************************************************************
            SelectCommand = new Command(async (object? args) =>
            {
                //На экране показываем ссылку у выбранного элемента
                Url = (args as BannerCertificate).Url;
                //На экране показываем картинку у выбранного элемента
                ImageWebHtml = (args as BannerCertificate).Image;
                //Сохрянем Id у выбранного элемента
                Id = (args as BannerCertificate).Id;
                imageByte = null;
                //Параметры выбранного файла
                LblImageSize = String.Format("Размер файла: {0} байт", (args as BannerCertificate).Image.Length.ToString());
                LblImageHeightAndWidth = String.Format("Разрешение файла: {0}x{1} ", (args as BannerCertificate).RealImageX.ToString(), (args as BannerCertificate).RealImageY.ToString());
                LblImageName = String.Format("Название файла: {0} ", "*.jpg");
                //Параметры измененого файла прячем
                LblImageSizeModified = "";
                LblImageHeightAndWidthModified = "";
                LblImageNameModified = "";
                LblImageQualityModified = "";
                //Для того чтоб новая картинка редактировалась, а не сохранялась
                BtnSaveIsVisible = false;
                BtnEditIsVisible = true;

            });

            //Запускаем процедуру получения картинок баннера сертификата с сервера
            ((Command)DisplayCommand).Execute("");
        }

        /// <summary>
        /// Видимость кнопки сохранить картинку баннера
        /// </summary>
        public bool BtnSaveIsVisible
        {
            get => btnSaveIsVisible;
            set
            {
                if (btnSaveIsVisible != value)
                {
                    btnSaveIsVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Видимость кнопки редактировать картинку баннера
        /// </summary>
        public bool BtnEditIsVisible
        {
            get => btnEditIsVisible;
            set
            {
                if (btnEditIsVisible != value)
                {
                    btnEditIsVisible = value;
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
        /// Картинка сертификат баннер
        /// </summary>
        public byte[] ImageWebHtml
        {
            get => imageWebHtml;
            set
            {
                if (imageWebHtml != value)
                {
                    imageWebHtml = value;
                    OnPropertyChanged();
                }
            }
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
        /// Полученные с сервера картинки баннера
        /// </summary>
        public ObservableCollection<BannerCertificate> Banners
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
        public string CountImage
        {
            get => countImage;
            set
            {
                if (countImage != value)
                {
                    countImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public Guid Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Url
        {
            get => url;
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RealImageX
        {
            get => realImageX;
            set
            {
                if (realImageX != value)
                {
                    realImageX = value;
                    OnPropertyChanged();
                }
            }
        }
        public int RealImageY
        {
            get => realImageY;
            set
            {
                if (realImageY != value)
                {
                    realImageY = value;
                    OnPropertyChanged();
                }
            }
        }
        public UrlWebViewSource WebSourse
        {
            get => webSourse;
            set
            {
                if (webSourse != value)
                {
                    webSourse = value;
                    OnPropertyChanged();
                }
            }
        }

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
