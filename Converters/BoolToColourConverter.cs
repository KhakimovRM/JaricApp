﻿ using System;
 using System.Collections.Generic;
 using System.Globalization;
 using System.Linq;
 using System.Text;
 using System.Threading.Tasks;

 namespace JaricApp.Converters
 {
     public class BoolToColourConverter : IValueConverter
     {
         public object Convert(
             object value,
             Type targetType,
             object parameter,
             CultureInfo culture)
         {
             var valueBool = (bool)value;
             var colourNames = parameter.ToString().Split(',');
             var colourName = colourNames[valueBool ? 1 : 0];
             var colourFields = typeof(Colors).GetFields();
             var colourField = colourFields.SingleOrDefault(x => x.Name == colourName);
             return (Color)colourField.GetValue(null);
         }

         public object ConvertBack(
             object value,
             Type targetType,
             object parameter,
             CultureInfo culture)
         {
             throw new NotImplementedException();
         }
     }
 }

