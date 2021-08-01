using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MemoryGame.Converters
{
    class TrumpImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TrumpModel trump)
            {
                if (trump.IsFront)
                {
                    //return new System.Windows.Media.Imaging.BitmapImage(new Uri(trump.FrontImage, UriKind.Relative));
                    return trump.FrontImage;
                }
                else
                {
                    return trump.BackImage;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
