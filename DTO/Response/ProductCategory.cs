using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO.Response
{
    /// <summary>
    /// Полученные категории товаров с сервера
    /// </summary>
    public class ProductCategory
    {
        /// <summary>
        /// Идентификатор категория товара
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
        /// Очередность показа на сайте категория товара
        /// </summary>
        public int SortBy { get; set; }
        /// <summary>
        /// Ссылка перехода при нажатии на категорию товара
        /// </summary>
        public string? Url { get; set; }
    }
}
