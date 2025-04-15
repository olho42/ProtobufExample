using System;
using System.Globalization;
using System.Windows.Data;
using Google.Protobuf.Collections;

namespace TaskApp.Client;

public class TagsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RepeatedField<string> tags)
            return string.Join(", ", tags);
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}