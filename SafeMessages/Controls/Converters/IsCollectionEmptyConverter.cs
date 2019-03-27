﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace SafeMessages.Controls.Converters
{
    public class IsCollectionEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => int.Parse(value.ToString()) == 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }
}