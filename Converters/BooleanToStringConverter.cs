using CommunityToolkit.Maui.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaricApp.Converters
{
    public class BooleanToStringConverter : BaseConverter<bool, string>
    {
        /// <inheritdoc/>
        public override string DefaultConvertReturnValue { get; set; }

        /// <inheritdoc />
        public override bool DefaultConvertBackReturnValue { get; set; }

        /// <summary>
        /// Converts the incoming <see cref="int"/> to a <see cref="bool"/> indicating whether or not the value is not equal to 0.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="culture">(Not Used)</param>
        /// <returns><c>false</c> if the supplied <paramref name="value"/> is equal to <c>0</c> and <c>true</c> otherwise.</returns>
        public override string ConvertFrom(bool value, CultureInfo culture = null) => value ? "Да" : "Нет";

        /// <summary>
        /// Converts the incoming <see cref="bool"/> to an <see cref="int"/> indicating whether or not the value is true.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="culture">(Not Used)</param>
        /// <returns><c>1</c> if the supplied <paramref name="value"/> is <c>true</c> and <c>0</c> otherwise.</returns>
        public override bool ConvertBackTo(string value, CultureInfo? culture = null) => value == "Да" ? true : false;
    }
}
