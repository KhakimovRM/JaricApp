using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO
{
    ///// <summary>
    ///// Класс для передачи данных для сохранения в базе данных
    ///// Данные для баннера с надписями
    ///// </summary>
    public class BannerWithLegendDto : INotifyPropertyChanged
    {
        /// <summary>
        /// Идентифкатор строки(картинки)
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Номер связки картинок в единый банер(номер баннера). 1 - верхний баннер. 2-средний баннер 
        /// </summary>
        public int bunch;
        public int Bunch
        {
            get
            {
                //if (bunch > 0) bunch = bunch - 1;
                return bunch;
            }
            set
            {
                //Если поменяли банер в Picker(стал верхний или нижний), то меняем разрешение картинки
                if (bunch != value) { if (value == 0) PatternY = 520; else PatternY = 345; }
                SetProperty(ref bunch, value);
            }
        }
        /// <summary>
        /// Выплывающая надпись(первая)
        /// </summary>
        public string Legend1 { get; set; } = string.Empty;
        /// <summary>
        /// Выплывающая надпись(вторая)
        /// </summary>
        public string Legend2 { get; set; } = string.Empty;
        /// <summary>
        /// Выплывающая надпись(третья)
        /// </summary>
        public string Legend3 { get; set; } = string.Empty;
        /// <summary>
        /// Выплывающая надпись(четвертая)
        /// </summary>
        public string Legend4 { get; set; } = string.Empty;
        /// <summary>
        /// Выплывающая надпись(пятая)
        /// </summary>
        public string Legend5 { get; set; } = string.Empty;
        /// <summary>
        /// Выплывающая надпись(шестая)
        /// </summary>
        public string Legend6 { get; set; } = string.Empty;
        /// <summary>
        /// Цвет всплывающей надписи(первая)
        /// </summary>
        public string Legend1Color { get; set; } = string.Empty;
        /// <summary>
        /// Цвет всплывающей надписи(вторая)
        /// </summary>
        public string Legend2Color { get; set; } = string.Empty;
        ///// <summary>
        ///// Цвет всплывающей надписи(третья)
        ///// </summary>
        public string Legend3Color { get; set; } = string.Empty;
        ///// <summary>
        ///// Цвет всплывающей надписи(четвертая)
        ///// </summary>
        public string Legend4Color { get; set; } = string.Empty;
        ///// <summary>
        ///// Цвет всплывающей надписи(пятая)
        ///// </summary>
        public string Legend5Color { get; set; } = string.Empty;
        ///// <summary>
        ///// Цвет всплывающей надписи(шестая)
        ///// </summary>
        public string Legend6Color { get; set; } = string.Empty;

        /// <summary>
        /// Рисунок(картинка) 
        /// </summary>
        public byte[] image; // { get; set; }
        public byte[] Image
        {
            get => image;
            set
            {
                SetProperty(ref image, value);
            }
        }
        /// <summary>
        /// Ссылка при нажатии на кнопку "Купить" 
        /// </summary>
        public string url;
        public string Url
        {
            get => url;
            set
            {
                SetProperty(ref url, value);
            }
        }

        /// <summary>
        /// Какое должно быть разрешение картинки по координате Х 
        /// </summary>
        public int PatternX { get; set; }
        /// <summary>
        /// Какое должно быть разрешение картинки по координате Х 
        /// </summary>
        public int PatternY { get; set; }
        /// <summary>
        /// Реальное разрешение картинки по координате Y 
        /// </summary>
        public int RealImageX { get; set; }
        /// <summary>
        /// Реальное разрешение картинки по координате Y 
        /// </summary>
        public int RealImageY { get; set; }
        /// <summary>
        /// Видна ли картинка на сайте в текущий момент
        /// </summary>
        public bool visible;
        public bool Visible
        {
            get => visible;
            set
            {
                SetProperty(ref visible, value);
            }
        }
        /// <summary>
        /// Сортировка по данному полю. Порядо вывода на экран.
        /// </summary>
        public int SortBy { get; set; }
        /// <summary>
        /// Чтоб работала привязка MVVVM
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// Чтоб работала привязка MVVVM
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Чтоб работала привязка MVVVM
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
