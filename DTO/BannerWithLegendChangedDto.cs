using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO
{
    /// <summary>
    /// ДЛя передачи на сервер данных, изменены у картинок(видимость или положение относительно друг друга)
    /// </summary>
    public class BannerWithLegendChangedDto
    {
        /// <summary>
        /// Id картинки баннера
        /// </summary>
        public Guid Id { set; get; }
        /// <summary>
        /// Видимость картинки баннера
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// Значение для сортировки баннера(очередность показа)
        /// </summary>
        public int SortBy { get; set; }
    }
}
