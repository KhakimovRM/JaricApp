using JaricApp.DTO;
using JaricApp.DTO.Response;
using Microsoft.Maui.Controls.Shapes;
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

namespace JaricApp.ViewModel.ReferenceBook
{
    public class ProductCategotyViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Сохранем новую категорию товара
        /// </summary>
        public ICommand SaveCommand { get; set; }
        /// <summary>
        /// Редактируем новую категорию товара
        /// </summary>
        public ICommand EditCommand { get; set; }
        /// <summary>
        /// Удаляем выбранную категорию товара
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        /// <summary>
        /// Получаем список категорий товаров
        /// </summary>
        public ICommand GetCommand { get; set; }
        /// <summary>
        /// Перемещаем очередность показа категории товара на экране - на одну позицию вверх
        /// </summary>
        public ICommand UpCommand { get; set; }
        /// <summary>
        /// Сохранем перемещенные категории товаров на сервере
        /// </summary>
        public ICommand UpdateCommand { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        public ProductCategotyViewModel()
        {
            //*******************************************************************************************************************************************
            //**************ПОЛУЧАЕМ СПИСОК КАТЕГОРИЙ ТОВАРА*********************************************************************************************
            //*******************************************************************************************************************************************
            GetCommand = new Command(async (object shape) =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/reference/catprod?shape={0}", shape.ToString() == "category" ? 1 : 2));
                    
                    HttpResponseMessage? response = null;
                    response = await _client.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert(string.Format("Ошибка при получении списка {0} товара", shape.ToString() == "category" ? "КАТЕГОРИИ" : "ПОДКАТЕГОРИИ"), await response.Content.ReadAsStringAsync(), "ОK");
                        return;
                    };
                    string content_out = await response.Content.ReadAsStringAsync();
                    if (shape.ToString() == "category")
                        CategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);

                    if (shape.ToString() == "subcategory")
                        SubCategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(string.Format("Ошибка при получении списка {0} товара", shape.ToString() == "category" ? "КАТЕГОРИИ" : "ПОДКАТЕГОРИИ"), ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************СОХРАНЯЕМ КАТЕГОРИЮ ТОВАРА В БАЗЕ ДАННЫХ*************************************************************************************
            //*******************************************************************************************************************************************
            SaveCommand = new Command(async (object shape) =>
            {
                //CategoryNomenclature = null;
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                //Валидация
                if (shape.ToString() == "category")
                   if (TxtCategoryName.Trim() == "") { await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении", "Строка ввода КАТЕГОРИИ товара пустая.", "ОK"); return; }
                                
                if (shape.ToString() == "subcategory")
                    if (TxtSubCategoryName.Trim() == "") { await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении", "Строка ввода ПОДКАТЕГОРИИ товара пустая.", "ОK"); return; }
                
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", string.Format("Вы хотите СОХРАНИТЬ новую {0} товара?", shape.ToString() == "category" ? "КАТЕГОРИЮ" : "ПОДКАТЕГОРИЮ"), "Да", "Нет")) { return; }
                
                //готовим информацию которую сохранем
                ProductCategoryDto product = new ProductCategoryDto()
                {
                    Name = shape.ToString() == "category" ? TxtCategoryName.Trim() : TxtSubCategoryName.Trim(),
                    Url =  shape.ToString() == "category" ? TxtCategoryUrl.Trim() : TxtSubCategoryUrl.Trim(),
                    Category = shape.ToString() == "category" ? 1 : 2,
                };
                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/reference/savecatprod", string.Empty));

                    string json = JsonSerializer.Serialize<ProductCategoryDto>(product, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage? response = null;
                    response = await _client.PostAsync(uri, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert(string.Format("Ошибка при сохранении {0} товара", shape.ToString() == "category" ? "КАТЕГОРИИ" : "ПОДКАТЕГОРИИ"), await response.Content.ReadAsStringAsync(), "ОK");
                        return;
                    };
                    string content_out = await response.Content.ReadAsStringAsync();

                    if (shape.ToString() == "category")
                    {
                        CategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);
                        //выделяем новую(которую только что сохранили) категорию продукта
                        SelectedCategory = CategoryNomenclature.Where(x => x.Name == product.Name).SingleOrDefault()!;
                    }

                    if (shape.ToString() == "subcategory")
                    {
                        SubCategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);
                        //выделяем новую(которую только что сохранили) подкатегорию продукта
                        SelectedSubCategory = SubCategoryNomenclature.Where(x => x.Name == product.Name).SingleOrDefault()!;
                    }

                    
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(string.Format("Ошибка при сохранении новой {0} товара", shape.ToString() == "category" ? "КАТЕГОРИИ" : "ПОДКАТЕГОРИИ"), ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************РЕДАКТИРУЕМ КАТЕГОРИЮ ТОВАРА В БАЗЕ ДАННЫХ***********************************************************************************
            //*******************************************************************************************************************************************
            EditCommand = new Command(async (object shape) =>
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                //Валидация
                if (shape.ToString() == "category")
                {
                    if (TxtCategoryName.Trim() == "") { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании", "Строка ввода КАТЕГОРИИ товара пустая.", "ОK"); return; }
                    if (SelectedCategory.ProductCategoryId == new Guid()) { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании", string.Format("Отсуствует идентификатор {0} товара.", shape.ToString() == "category" ? "КАТЕГОРИИ" : "ПОДКАТЕГОРИИ"), "ОK"); return; }
                    if (SelectedCategory.Name == TxtCategoryName.Trim()) { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании", string.Format("Эта {0} товара уже существует", shape.ToString() == "category" ? "КАТЕГОРИЯ" : "ПОДКАТЕГОРИЯ"), "ОK"); return; }
                }

                if (shape.ToString() == "subcategory")
                {
                    if (TxtSubCategoryName.Trim() == "") { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании", "Строка ввода ПОДКАТЕГОРИИ товара пустая.", "ОK"); return; }
                    if (SelectedSubCategory.ProductCategoryId == new Guid()) { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании", string.Format("Отсуствует идентификатор {0} товара.", shape.ToString() == "category" ? "КАТЕГОРИИ" : "ПОДКАТЕГОРИИ"), "ОK"); return; }
                    if (SelectedSubCategory.Name == TxtSubCategoryName.Trim()) { await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании", string.Format("Эта {0} товара уже существует", shape.ToString() == "category" ? "КАТЕГОРИЯ" : "ПОДКАТЕГОРИЯ"), "ОK"); return; }
                }
                
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", string.Format("Вы хотите РЕДАКТИРОВАТЬ {0} категорию} товара?", shape.ToString() == "category" ? "КАТЕГОРИЮ" : "ПОДКАТЕГОРИЮ"), "Да", "Нет")) { return; }

                

                //готовим информацию которую редактируем
                ProductCategoryDto product = new ProductCategoryDto()
                {
                    ProductCategoryId = shape.ToString()=="category" ? SelectedCategory.ProductCategoryId : SelectedSubCategory.ProductCategoryId,
                    Name = shape.ToString() == "category" ? TxtCategoryName.Trim() : TxtSubCategoryName.Trim(),
                    Url = shape.ToString() == "category" ? TxtCategoryUrl.Trim() : TxtSubCategoryUrl.Trim() ,
                    Category = shape.ToString() == "category" ? 1 : 2
                };
                
                //отправляем на сервер
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/reference/editcatprod", string.Empty));

                    string json = JsonSerializer.Serialize<ProductCategoryDto>(product, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage? response = null;
                    response = await _client.PostAsync(uri, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка при редактировании категории товара", await response.Content.ReadAsStringAsync(), "ОK");
                        return;
                    };
                    string content_out = await response.Content.ReadAsStringAsync();

                    if (shape.ToString() == "category")
                    {
                        CategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);
                        SelectedCategory = CategoryNomenclature.Where(x => x.Name == product.Name).SingleOrDefault()!;
                    }
                    if (shape.ToString() == "subcategory")
                    {
                        SubCategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);
                        SelectedSubCategory = SubCategoryNomenclature.Where(x => x.Name == product.Name).SingleOrDefault()!;
                    }

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert(string.Format("Ошибка при редактировании {0} товара", shape.ToString() == "category" ? "КАТЕГОРИИ" : "ПОДКАТЕГОРИИ"), ex.Message, "ОK");
                }

            });
            //*******************************************************************************************************************************************
            //**************УДАЛЯЕМ ВЫБРАННУЮ КАТЕГОРИЮ ТОВАРА С СЕРВЕРА*********************************************************************************
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
                //Валидация
                if (SelectedCategory==null || SelectedCategory.ProductCategoryId == new Guid())
                   { await Application.Current.MainPage.DisplayAlert("Ошибка при УДАЛЕНИИ категории товара", "Не выбрана категория товара.", "ОK"); return; }
                
                if(!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите УДАЛИТЬ категорию товара?", "Да", "Нет")) { return; }

                if(SelectedCategory.ProductCategoryId == new Guid()) { await Application.Current.MainPage.DisplayAlert("Ошибка при УДАЛЕНИИ категории товара", "Отсуствует идентификатор категории товара.", "ОK"); return; }

                //готовим информацию которую удаляем
                ProductCategoryDto product = new ProductCategoryDto()
                {
                    ProductCategoryId = SelectedCategory.ProductCategoryId,
                    Name = SelectedCategory.Name,
                    Url = SelectedCategory.Url,
                };
                //отправляем на сервер
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/reference/delcatprod", string.Empty));

                    string json = JsonSerializer.Serialize<ProductCategoryDto>(product, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage? response = null;
                    response = await _client.PostAsync(uri, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка при УДАЛЕНИИ категории товара", await response.Content.ReadAsStringAsync(), "ОK");
                        return;
                    };
                    string content_out = await response.Content.ReadAsStringAsync();
                    CategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);
                    SelectedCategory = null;

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при УДАЛЕНИИ категории товара", ex.Message, "ОK");
                }
            });
            //*******************************************************************************************************************************************
            //**************ПЕРЕМЕЩАЕМ ВЫБРАННУЮ КАТЕГОРИЮ ТОВАРА НА ОДНУ ПОЗИЦЮ ВВЕРХ*******************************************************************
            //*******************************************************************************************************************************************
            UpCommand = new Command(async () => 
            {
                string name= SelectedCategory.Name;
                int currentSortBy = SelectedCategory.SortBy;
                int minSortBy = categoryNomenclature.Min(x => x.SortBy);
                int maxSortBy = categoryNomenclature.Max(x => x.SortBy);
                
                if (currentSortBy == minSortBy)
                {
                    categoryNomenclature[categoryNomenclature.IndexOf(categoryNomenclature.First(x=>x.SortBy== maxSortBy))].SortBy = currentSortBy;
                    SelectedCategory.SortBy = maxSortBy; 
                }
                else
                {
                    SelectedCategory.SortBy = categoryNomenclature[categoryNomenclature.IndexOf(categoryNomenclature.First(x => x.ProductCategoryId == SelectedCategory.ProductCategoryId)) - 1].SortBy;
                    categoryNomenclature[categoryNomenclature.IndexOf(categoryNomenclature.First(x => x.ProductCategoryId == SelectedCategory.ProductCategoryId)) - 1].SortBy = currentSortBy;
                };
                
                CategoryNomenclature = new ObservableCollection<ProductCategory>(categoryNomenclature.OrderBy(x => x.SortBy).ToList());
                SelectedCategory = CategoryNomenclature.Where(x => x.Name == name).SingleOrDefault()!;
                BtnSaveChangeCategoryEnabled = true;
            });
            //*******************************************************************************************************************************************
            //**************СОХРАНЯЕМ ПЕРЕМЕЩЕННЫЕ КАТЕГОРИИ ТОВАРОВ НА СЕРВЕРЕ**************************************************************************
            //*******************************************************************************************************************************************
            UpdateCommand = new Command(async () => 
            {
                HttpClient _client;
                JsonSerializerOptions _serializerOptions;
                _client = new HttpClient();
                _serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                //Валидация
                if (!await Application.Current.MainPage.DisplayAlert("Подтвердить действие", "Вы хотите сохранить ПЕРЕМЕЩЕННУЮ категорию товара?", "Да", "Нет")) { return; }
                //готовим информацию для отправки
                string name = SelectedCategory.Name;
                List<ProductCategoryUpdateDto> product=new List<ProductCategoryUpdateDto>();
                foreach (var item in CategoryNomenclature)
                {
                    product.Add(new ProductCategoryUpdateDto { ProductCategoryId=item.ProductCategoryId, SortBy=item.SortBy});
                };

                //отправляем данные на сервер для сохранения в формате JSON
                try
                {
                    Uri uri = new Uri(string.Format("https://localhost:7091/reference/updcatprod", string.Empty));

                    string json = JsonSerializer.Serialize<List<ProductCategoryUpdateDto>>(product, _serializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage? response = null;
                    response = await _client.PostAsync(uri, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении ПЕРЕМЕЩЕНИИ категории товара", await response.Content.ReadAsStringAsync(), "ОK");
                        return;
                    };
                    string content_out = await response.Content.ReadAsStringAsync();
                    CategoryNomenclature = JsonSerializer.Deserialize<ObservableCollection<ProductCategory>>(content_out, _serializerOptions);
                    //выделяем новую(которую только что сохранили) категорию продукта

                    SelectedCategory = CategoryNomenclature.Where(x => x.Name == name).SingleOrDefault()!;
                    BtnSaveChangeCategoryEnabled = false;
                    await Application.Current.MainPage.DisplayAlert("Сохранение изменений категории товара", "Изменения сохранены по категории товара", "ОK");

                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка при сохранении ПЕРЕМЕЩЕНИИ категории товара", ex.Message, "ОK");
                }

            });

            //Запускаем процедуру получения списка категорий товара с сервера
            ((Command)GetCommand).Execute("category");
            //Запускаем процедуру получения списка категорий товара с сервера
            ((Command)GetCommand).Execute("subcategory");

        }
        
        /// <summary>
        /// Список категорий товаров
        /// </summary>
        public ObservableCollection<ProductCategory>? categoryNomenclature;
        public ObservableCollection<ProductCategory>? CategoryNomenclature
        {
            get => categoryNomenclature;
            set
            {
                if (categoryNomenclature != value)
                {
                    categoryNomenclature = value;
                    OnPropertyChanged();
                }
            }
        }
        /// <summary>
        /// Список ПОДкатегорий товаров
        /// </summary>
        public ObservableCollection<ProductCategory>? subcategoryNomenclature;
        public ObservableCollection<ProductCategory>? SubCategoryNomenclature
        {
            get => subcategoryNomenclature;
            set
            {
                if (subcategoryNomenclature != value)
                {
                    subcategoryNomenclature = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Выбранная категория товара
        /// </summary>
        ProductCategory selectedCategory;
        public ProductCategory SelectedCategory
        {
            get
            {
                return selectedCategory;
            }
            set
            {
                if (selectedCategory != value)
                {
                    selectedCategory = value;
                    TxtCategoryName = value==null ? "" : value.Name; //Выбранное НАЗВАНИЕ категории товара выводим на экран в Entry 
                    TxtCategoryUrl = value == null ? "" : value.Url; //Выбранную ССЫЛКУ категории товара выводим на экран в Entry
                    BtnSaveCategoryEnabled = true;
                    if (SelectedCategory != null && SelectedCategory.ProductCategoryId != new Guid()) BtnEditCategoryEnabled = true; else BtnEditCategoryEnabled = false;
                    if (value == null) BtnDeleteCategoryEnabled = false; else BtnDeleteCategoryEnabled = true;
                    if (value == null) BtnUpdateCategoryEnabled = false; else BtnUpdateCategoryEnabled = true;
                    OnPropertyChanged();
                };
            }
        }

        /// <summary>
        /// Выбранная ПОДкатегория товара
        /// </summary>
        ProductCategory selectedSubCategory;
        public ProductCategory SelectedSubCategory
        {
            get
            {
                return selectedSubCategory;
            }
            set
            {
                if (selectedSubCategory != value)
                {
                    selectedSubCategory = value;
                    TxtSubCategoryName = value == null ? "" : value.Name; //Выбранное НАЗВАНИЕ категории товара выводим на экран в Entry 
                    TxtSubCategoryUrl = value == null ? "" : value.Url; //Выбранную ССЫЛКУ категории товара выводим на экран в Entry
                    BtnSaveSubCategoryEnabled = true;
                    BtnEditSubCategoryEnabled = false;
                    if (SelectedSubCategory != null && SelectedSubCategory.ProductCategoryId != new Guid()) BtnEditSubCategoryEnabled = true; else BtnEditSubCategoryEnabled = false;
                    if (value == null) BtnDeleteSubCategoryEnabled = false; else BtnDeleteSubCategoryEnabled = true;
                    if (value == null) BtnUpdateSubCategoryEnabled = false; else BtnUpdateSubCategoryEnabled = true;
                    OnPropertyChanged();
                };
            }
        }

        /// <summary>
        /// Название категории товара
        /// </summary>
        public string txtCategoryName="";
        public string TxtCategoryName
        {
            get => txtCategoryName;
            set
            {
                if (txtCategoryName != value)
                {
                    txtCategoryName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Название подкатегории товара
        /// </summary>
        public string txtSubCategoryName = "";
        public string TxtSubCategoryName
        {
            get => txtSubCategoryName;
            set
            {
                if (txtSubCategoryName != value)
                {
                    txtSubCategoryName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Ссылка при нажатии на категорию товара
        /// </summary>
        public string txtCategoryUrl="";
        public string TxtCategoryUrl
        {
            get => txtCategoryUrl;
            set
            {
                if (txtCategoryUrl != value)
                {
                    txtCategoryUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Ссылка при нажатии на ПОДкатегорию товара
        /// </summary>
        public string txtSubCategoryUrl = "";
        public string TxtSubCategoryUrl
        {
            get => txtSubCategoryUrl;
            set
            {
                if (txtSubCategoryUrl != value)
                {
                    txtSubCategoryUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки сохранить категорию товара
        /// </summary>
        public bool btnSaveCategoryEnabled;
        public bool BtnSaveCategoryEnabled
        {
            get => btnSaveCategoryEnabled;
            set
            {
                if (btnSaveCategoryEnabled != value)
                {
                    btnSaveCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки сохранить ПОДкатегорию товара
        /// </summary>
        public bool btnSaveSubCategoryEnabled=false;
        public bool BtnSaveSubCategoryEnabled
        {
            get => btnSaveSubCategoryEnabled;
            set
            {
                if (btnSaveSubCategoryEnabled != value)
                {
                    btnSaveSubCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки редактировать ПОДкатегорию товара
        /// </summary>
        public bool btnEditSubCategoryEnabled=false;
        public bool BtnEditSubCategoryEnabled
        {
            get => btnEditSubCategoryEnabled;
            set
            {
                if (btnEditSubCategoryEnabled != value)
                {
                    btnEditSubCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Активность кнопки редактировать категорию товара
        /// </summary>
        public bool btnEditCategoryEnabled;
        public bool BtnEditCategoryEnabled
        {
            get => btnEditCategoryEnabled;
            set
            {
                if (btnEditCategoryEnabled != value)
                {
                    btnEditCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки удалить категорию товара
        /// </summary>
        public bool btnDeleteCategoryEnabled = false;
        public bool BtnDeleteCategoryEnabled
        {
            get => btnDeleteCategoryEnabled;
            set
            {
                if (btnDeleteCategoryEnabled != value)
                {
                    btnDeleteCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки удалить ПОДкатегорию товара
        /// </summary>
        public bool btnDeleteSubCategoryEnabled = false;
        public bool BtnDeleteSubCategoryEnabled
        {
            get => btnDeleteSubCategoryEnabled;
            set
            {
                if (btnDeleteSubCategoryEnabled != value)
                {
                    btnDeleteSubCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Активность кнопки переместить вверх категорию товара
        /// </summary>
        public bool btnUpdateCategoryEnabled = false;
        public bool BtnUpdateCategoryEnabled
        {
            get => btnUpdateCategoryEnabled;
            set
            {
                if (btnUpdateCategoryEnabled != value)
                {
                    btnUpdateCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки переместить вверх ПОДкатегорию товара
        /// </summary>
        public bool btnUpdateSubCategoryEnabled = false;
        public bool BtnUpdateSubCategoryEnabled
        {
            get => btnUpdateSubCategoryEnabled;
            set
            {
                if (btnUpdateSubCategoryEnabled != value)
                {
                    btnUpdateCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки сохранить перемещения на вверх категорию товара
        /// </summary>
        public bool btnSaveChangeCategoryEnabled = false;
        public bool BtnSaveChangeCategoryEnabled
        {
            get => btnSaveChangeCategoryEnabled;
            set
            {
                if (btnSaveChangeCategoryEnabled != value)
                {
                    btnSaveChangeCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Активность кнопки сохранить перемещения на вверх ПОДкатегорию товара
        /// </summary>
        public bool btnSaveChangeSubCategoryEnabled = false;
        public bool BtnSaveChangeSubCategoryEnabled
        {
            get => btnSaveChangeSubCategoryEnabled;
            set
            {
                if (btnSaveChangeSubCategoryEnabled != value)
                {
                    btnSaveChangeSubCategoryEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Для того чтоб работала привязка
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Чтоб работала привязка MVVVM
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }




    }
}
