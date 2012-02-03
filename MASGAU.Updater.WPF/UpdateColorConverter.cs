using System;
using System.Windows.Data;
using System.Windows.Media;

namespace MASGAU.Updater
{
    class UpdateColorConverter : IValueConverter
    {
        #region IValueConverter Member

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? updating = (bool?)value;
            Brush retval;

            if(updating==null)
                retval = Brushes.LightGreen;
            else if(updating==true)
                retval = Brushes.LightYellow;
            else
                retval = Brushes.LightSalmon;

            return retval;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
