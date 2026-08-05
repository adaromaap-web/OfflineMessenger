using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OfflineMessenger.UI.Windows;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        bool isMine = (bool)value;

        if (isMine)
        {
            // своё сообщение — светло-голубой
            return new SolidColorBrush(
                Color.FromRgb(173, 216, 255)
            );
        }

        // чужое сообщение — светло-серый
        return new SolidColorBrush(
            Color.FromRgb(224, 224, 224)
        );
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