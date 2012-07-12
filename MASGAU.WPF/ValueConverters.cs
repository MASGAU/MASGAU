using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace MASGAU {
    public class GameColor : IValueConverter {
        #region IValueConverter Member

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            System.Drawing.Color color = (System.Drawing.Color)value;

            Color nat_color = new Color();

            nat_color.A = color.A;
            nat_color.B = color.B;
            nat_color.G = color.G;
            nat_color.R = color.R;

            SolidColorBrush ret = new SolidColorBrush(nat_color);

            return ret;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DisabledToColor : IValueConverter {
        #region IValueConverter Member

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            bool disabled = (bool)value;
            Brush retval;

            if (disabled)
                retval = Brushes.Red;
            else
                retval = Brushes.Black;

            return retval;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class VisibilityConverter : IValueConverter {
        #region IValueConverter Member

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            bool visible = (bool)value;
            Visibility retval;

            if (visible)
                retval = Visibility.Visible;
            else
                retval = Visibility.Collapsed;

            return retval;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class PathValueConverter : IValueConverter {
        #region IValueConverter Member

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            string path = (string)value;
            string retval;

            if (path == null)
                retval = "Not Set";
            else
                retval = path;

            return retval;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class IntStringConverter : IValueConverter {
        #region IValueConverter Member

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value.ToString();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            string original = (string)value;
            long return_me;

            if (original == "") {
                return_me = 0;
            } else {
                try {
                    return_me = Int64.Parse(original);
                } catch {
                    return_me = 0;
                }
            }
            return return_me;
        }

        #endregion
    }
}
