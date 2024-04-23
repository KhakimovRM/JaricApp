using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO
{
    /// <summary>
    /// Для сохранения картинки по доп.баннеру
    /// </summary>
    public class BannerDopDto
    {
        /// <summary>
        /// Номер картинки
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Картинка в ввиде массива
        /// </summary>
        public byte[] Image { get; set; }
        /// <summary>
        /// Ссылка при выборе картинки на сайте
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Размер по ширине который должен быть по шаблону
        /// </summary>
        public int PatternX { get; set; }
        /// <summary>
        /// Размер по высоте который должен быть по шаблону
        /// </summary>
        public int PatternY { get; set; }
    }
}
