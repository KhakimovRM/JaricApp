using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO.Response
{
    /// <summary>
    /// Kартинки дополнительного баннера
    /// </summary>
    public class BannerDop
    {
        /// <summary>
        /// Идентификатор картинки дополнительного баннера
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Номер картинки
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// Рисунок(картинка) 
        /// </summary>
        public byte[] Image { get; set; }
        /// <summary>
        /// Ссылка при нажатии на картинку
        /// </summary>
        public string? Url { get; set; }
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
        /// Дата обновления текущей записи(картинки)
        /// </summary>
        public DateTime RowCreated { get; set; }
        /// <summary>
        /// Дата удаления текущей записи(картинки)
        /// </summary>
        public DateTime? RowDeleted { get; set; }

    }
}
