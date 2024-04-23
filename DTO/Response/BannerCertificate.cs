using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO.Response
{
    /// <summary>
    /// Полученный банер сертификат с сервера
    /// </summary>
    public class BannerCertificate
    {
        /// <summary>
        /// Идентификатор картинки сертификата
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Рисунок(картинка) 
        /// </summary>
        public byte[] Image { get; set; }
        /// <summary>
        /// Ссылка при нажатии на кнопку "Купить" 
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
        /// <summary>
        /// Видимость картинки баннера(true-видно на сайте)
        /// </summary>
        public bool Visible { get; set; } = false;
    }
}
