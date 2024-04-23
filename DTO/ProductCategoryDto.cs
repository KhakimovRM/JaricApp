using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO
{
    /// <summary>
    /// Класс для передачи данных для сохранения в базе данных
    /// Данные для категории товара
    /// </summary>
    public class ProductCategoryDto
    {
        /// <summary>
        /// Идентификатор категории товара
        /// </summary>
        public Guid ProductCategoryId { get; set; }
        /// <summary>
        /// 1-категория продукта
        /// 2-подкатегория продукта
        /// </summary>
        public int Category { get; set; }
        /// <summary>
        /// Название категории товара
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ссылка перехода при нажатии на категорию товара
        /// </summary>
        public string? Url { get; set; }
    }
}
