using ImageMagick;
using JaricApp.DTO;
using JaricApp.DTO.Response;
using Microsoft.Extensions.Logging;
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

namespace JaricApp.ViewModel.DopBanner
{
    public class DopBannerViewModel : INotifyPropertyChanged, IDopBannerModel, IDisposable
    {
        /// <summary>
        /// Картинка баннера ввиде html тега <img ...../>
        /// </summary>
        public string url1;
        public string url2;
        public string url3;
        public string url4;
        public string url5;
        public string url6;
        public string url7;
        public string url8;

        public byte[] imageWebHtmlF1;
        public byte[] imageWebHtmlF2;
        public byte[] imageWebHtmlF3;
        public byte[] imageWebHtmlS1;
        public byte[] imageWebHtmlS2;
        public byte[] imageWebHtmlT1;
        public byte[] imageWebHtmlT2;
        public byte[] imageWebHtmlT3;

        public int imageWithPatternFirst = 722;
        public int imageHeightPatternFirst = 251;
        public int imageWithPatternSecond = 575;
        public int imageHeightPatternSecond = 200;
        public int imageWithPatternThird = 575;
        public int imageHeightPatternThird = 200;
        public ICommand OpenCommand { get; init; }
        public ICommand SaveCommand { get; init; }
        public ICommand GetCommand { get; init; }

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly ILogger<IDopBannerModel> _logger;
        public static IServiceProvider Services;
        public DopBannerViewModel()
        {
            //*******************************************************************************************************************************************
            //**************ПОЛУЧАЕМ КАРТИНКИ БАННЕРА С СЕРВЕРА******************************************************************************************
            //*******************************************************************************************************************************************
            GetCommand = new Command(async () =>
            {
                ObservableCollection<BannerDop> banners = new ObservableCollection<BannerDop>();
                try
                {
                    HttpClient _client;
                    JsonSerializerOptions _serializerOptions;
                    _client = new HttpClient();
                    _serializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/getbandop", string.Empty));

                    HttpResponseMessage response = null;
                    response = await _client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        banners = JsonSerializer.Deserialize<ObservableCollection<BannerDop>>(content, _serializerOptions);
                        foreach (var item in banners)
                        {
                            switch (item.Number)
                            {
                                case 1: { ImageWebHtmlF1 = item.Image; Url1 = item.Url; break; }
                                case 2: { ImageWebHtmlF2 = item.Image; Url2 = item.Url; break; }
                                case 3: { ImageWebHtmlF3 = item.Image; Url3 = item.Url; break; }
                                case 4: { ImageWebHtmlS1 = item.Image; Url4 = item.Url; break; }
                                case 5: { ImageWebHtmlS2 = item.Image; Url5 = item.Url; break; }
                                case 6: { ImageWebHtmlT1 = item.Image; Url6 = item.Url; break; }
                                case 7: { ImageWebHtmlT2 = item.Image; Url7 = item.Url; break; }
                                case 8: { ImageWebHtmlT3 = item.Image; Url8 = item.Url; break; }
                            };
                        };

                    }
                    else await Application.Current.MainPage.DisplayAlert("Получении картинок баннера", "Не удалось получить картинки баннера с сервера" + await response.Content.ReadAsStringAsync(), "ОK");
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при получении картинок баннера с сервера", ex.Message, "ОK");
                };
            });

            //*******************************************************************************************************************************************
            //**************ВЫБИРАЕМ КАРТИНКУ БАННЕРА****************************************************************************************************
            //*******************************************************************************************************************************************
            OpenCommand = new Command(async (object? args) =>
            {
                try
                {
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
                        //ПРЕОБРАЗУЕМ ФАЙЛ
                        //в тип JPG
                        if (imageFromFile.Format.ToString() != "Jpg") imageFromFile.Format = MagickFormat.Jpg;
                        var size = new MagickGeometry();
                        if (Convert.ToInt32(args) == 1 || Convert.ToInt32(args) == 2 || Convert.ToInt32(args) == 3) size = new MagickGeometry(imageWithPatternFirst, imageHeightPatternFirst);
                        else size = new MagickGeometry(imageWithPatternSecond, imageHeightPatternSecond);
                        size.IgnoreAspectRatio = true; //изменение размера изображения до фиксированного размера без сохранения соотношения сторон.
                        imageFromFile.Quality = 75;
                        imageFromFile.Resize(size);
                        //выводим на экран  выбранную картинку в измененном ввиде
                        byte[] imageByte = imageFromFile.ToByteArray();
                        switch (args)
                        {
                            case "1":
                                {
                                    ImageWebHtmlF1 = imageByte;
                                    break;
                                }
                            case "2":
                                {
                                    ImageWebHtmlF2 = imageByte;
                                    break;
                                }
                            case "3":
                                {
                                    ImageWebHtmlF3 = imageByte;
                                    break;
                                }
                            case "4":
                                {
                                    ImageWebHtmlS1 = imageByte;
                                    break;
                                }
                            case "5":
                                {
                                    ImageWebHtmlS2 = imageByte;
                                    break;
                                }
                            case "6":
                                {
                                    ImageWebHtmlT1 = imageByte;
                                    break;
                                }
                            case "7":
                                {
                                    ImageWebHtmlT2 = imageByte;
                                    break;
                                }
                            case "8":
                                {
                                    ImageWebHtmlT3 = imageByte;
                                    break;
                                }
                        };
                    };
                    return;
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при окрытие файла", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************СОХРАНЯЕМ КАРТИНКУ БАННЕРА***************************************************************************************************
            //*******************************************************************************************************************************************
            SaveCommand = new Command(async (object? args) =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                bool flag = false;
                byte[] image = null;
                string url = "";
                int pX = 0;
                int pY = 0;
                switch (args)
                {
                    case "1":
                        {
                            if (imageWebHtmlF1 == null || imageWebHtmlF1.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlF1;
                                url = Url1;
                                pX = imageWithPatternFirst;
                                pY = imageHeightPatternFirst;
                            }
                            break;
                        }
                    case "2":
                        {
                            if (imageWebHtmlF2 == null || imageWebHtmlF2.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlF2;
                                url = Url2;
                                pX = imageWithPatternFirst;
                                pY = imageHeightPatternFirst;
                            }
                            break;
                        }
                    case "3":
                        {
                            if (imageWebHtmlF3 == null || imageWebHtmlF3.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlF3;
                                url = Url3;
                                pX = imageWithPatternFirst;
                                pY = imageHeightPatternFirst;
                            }
                            break;
                        }
                    case "4":
                        {
                            if (imageWebHtmlS1 == null || imageWebHtmlS1.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlS1;
                                url = Url4;
                                pX = imageWithPatternSecond;
                                pY = imageHeightPatternSecond;
                            }
                            break;
                        }
                    case "5":
                        {
                            if (imageWebHtmlS2 == null || imageWebHtmlS2.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlS2;
                                url = Url5;
                                pX = imageWithPatternSecond;
                                pY = imageHeightPatternSecond;
                            }
                            break;
                        }
                    case "6":
                        {
                            if (imageWebHtmlT1 == null || imageWebHtmlT1.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlT1;
                                url = Url6;
                                pX = imageWithPatternThird;
                                pY = imageHeightPatternThird;
                            }
                            break;
                        }
                    case "7":
                        {
                            if (imageWebHtmlT2 == null || imageWebHtmlT2.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlT2;
                                url = Url7;
                                pX = imageWithPatternThird;
                                pY = imageHeightPatternThird;
                            }
                            break;
                        }
                    case "8":
                        {
                            if (imageWebHtmlT3 == null || imageWebHtmlT3.Length == 0) flag = true;
                            else
                            {
                                image = imageWebHtmlT3;
                                url = Url8;
                                pX = imageWithPatternThird;
                                pY = imageHeightPatternThird;
                            }
                            break;
                        }
                };
                if (flag) { await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении картинки", "Выберите картинку баннера.", "ОK"); return; }

                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите сохранить картинку?", "Да", "Нет")) return;

                //Формириуем данные для сохранения в базе данных
                BannerDopDto banner = new BannerDopDto()
                {
                    Number = Convert.ToInt32(args),
                    Image = image,
                    Url = url ?? "",
                    PatternX = pX,
                    PatternY = pY,
                };
                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/desktop/savebandop", string.Empty));

                    string json = JsonSerializer.Serialize<BannerDopDto>(banner, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;
                    response = await _client.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Картинка баннера сохранена!", "ОK");
                    else
                        await Application.Current.MainPage.DisplayAlert("Сохранение картинки", "Картинка баннера не сохранена!" + await response.Content.ReadAsStringAsync(), "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении картинки", ex.Message, "ОK");
                }

            });

            //Запускаем процедуру получения картинок баннера сертификата с сервера
            ((Command)GetCommand).Execute("");
        }
        /// <summary>
        /// Ссылка перехода при нажатии на картинку
        /// </summary>
        public string Url1
        {
            get => url1;
            set
            {
                if (url1 != value)
                {
                    url1 = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Url2
        {
            get => url2;
            set
            {
                if (url2 != value)
                {
                    url2 = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Url3
        {
            get => url3;
            set
            {
                if (url3 != value)
                {
                    url3 = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Url4
        {
            get => url4;
            set
            {
                if (url4 != value)
                {
                    url4 = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Url5
        {
            get => url5;
            set
            {
                if (url5 != value)
                {
                    url5 = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Url6
        {
            get => url6;
            set
            {
                if (url6 != value)
                {
                    url6 = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Url7
        {
            get => url7;
            set
            {
                if (url7 != value)
                {
                    url7 = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Url8
        {
            get => url8;
            set
            {
                if (url8 != value)
                {
                    url8 = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Картинка сертификат баннер
        /// </summary>
        public byte[] ImageWebHtmlF1
        {
            get => imageWebHtmlF1;
            set
            {
                if (imageWebHtmlF1 != value)
                {
                    imageWebHtmlF1 = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte[] ImageWebHtmlF2
        {
            get => imageWebHtmlF2;
            set
            {
                if (imageWebHtmlF2 != value)
                {
                    imageWebHtmlF2 = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte[] ImageWebHtmlF3
        {
            get => imageWebHtmlF3;
            set
            {
                if (imageWebHtmlF3 != value)
                {
                    imageWebHtmlF3 = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte[] ImageWebHtmlS1
        {
            get => imageWebHtmlS1;
            set
            {
                if (imageWebHtmlS1 != value)
                {
                    imageWebHtmlS1 = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte[] ImageWebHtmlS2
        {
            get => imageWebHtmlS2;
            set
            {
                if (imageWebHtmlS2 != value)
                {
                    imageWebHtmlS2 = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte[] ImageWebHtmlT1
        {
            get => imageWebHtmlT1;
            set
            {
                if (imageWebHtmlT1 != value)
                {
                    imageWebHtmlT1 = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte[] ImageWebHtmlT2
        {
            get => imageWebHtmlT2;
            set
            {
                if (imageWebHtmlT2 != value)
                {
                    imageWebHtmlT2 = value;
                    OnPropertyChanged();
                }
            }
        }
        public byte[] ImageWebHtmlT3
        {
            get => imageWebHtmlT3;
            set
            {
                if (imageWebHtmlT3 != value)
                {
                    imageWebHtmlT3 = value;
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
