using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.DTO.Response
{
    /// <summary>
    /// Перечень цветов для теста баннера с надписями
    /// </summary>
    public class ListColor
    {
        /// <summary>
        /// Название цвета
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Значение цвета в HEX
        /// </summary>
        public string Value { get; set; }
    }
}
