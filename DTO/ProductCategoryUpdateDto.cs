using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO
{
    public class ProductCategoryUpdateDto
    {
        /// <summary>
        /// Идентификатор категории товара
        /// </summary>
        public Guid ProductCategoryId { get; set; }
        /// <summary>
        /// Очередность показа на сайте категория товара
        /// </summary>
        public int SortBy { get; set; }
    }
}
