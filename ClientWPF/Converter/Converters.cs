using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClientWPF.Converter
{
    public class MultipleBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(k => k is bool && (bool)k) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringNullOrEmptyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as String;
            if (String.IsNullOrEmpty(s))
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible;
            if (!bool.TryParse("" + value, out isVisible))
                isVisible = false;
            return isVisible ? "Oui" : "Non";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public class TrueToStringConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        bool isVisible;
    //        if (!bool.TryParse("" + value, out isVisible))
    //            isVisible = false;
    //        return isVisible ? "Oui" : "";
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class BooleanToVisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = false;
            bool.TryParse("" + value, out isVisible);
            return isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            Visibility visibility = (Visibility) value;
            switch (visibility)
            {
                case Visibility.Visible:
                    return true;
                case Visibility.Hidden:
                    return false;
            }
            return false;
        }
    }

    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isHidden;
            if( bool.TryParse(""+value, out isHidden))
                return (new BooleanToVisibilityConverter()).Convert(!isHidden,targetType,parameter,culture);
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (new BooleanToVisibilityConverter()).ConvertBack(value,
                                                             targetType,
                                                             parameter,
                                                             culture);
            bool isHidden;
            if (bool.TryParse("" + result,
                              out isHidden))
                return !isHidden;
            return false;
        }
    }
    public class CollectionToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Any())
                return ((ObservableCollection<object>)values[0]).Aggregate(String.Empty, (r, i) => string.Format("{0}, {1}", r, i.ToString()));
            else
                return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RadioBoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int integer = (int)value;
            return integer == int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }

    public class CompareToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }

    public class MilestoneWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Length < 2) return 20;
            double totalWidth = 0;
            double.TryParse(value[0].ToString(), out totalWidth);

            System.Collections.IEnumerable items = value[1] as System.Collections.IEnumerable;

            return totalWidth > 0 && items != null ? (parameter != null && bool.TrueString == parameter.ToString() ? 5 * totalWidth / (items.OfType<object>().Count() + 4) : totalWidth / (items.OfType<object>().Count() + 4)) : 20;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class PriceMonthlyToYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is double)
                return System.Convert.ToDouble(value) * 10;
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
    
    public class DataGridRowNumberConverter : IMultiValueConverter  //Used to display Row Number in dataGrid View Ex: Invoice->PaymentDelay->ActifPaymentDelay 
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //get the grid and the item
            Object item = values[0];
            DataGrid grid = values[1] as DataGrid;

            int index = grid == null ? 0 : grid.Items.IndexOf(item);
            index++;    //Begin with 1 instead of 0.
            return index.ToString(CultureInfo.InvariantCulture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NegateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

    }
    
//    //http://www.codewrecks.com/blog/index.php/2010/07/23/bind-an-image-to-a-property-in-wpf/
//    public class PaymentDelayStatusImageConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            //	RME2028 : le champ « statut » répond aux règles suivantes :
//            //o	Rond rouge   pour le statut « impayé »
//            //o	Carré vert   pour le statut « rapproché »
//            //o	Triangle jaune   pour le statut « due »

//            string url = String.Empty;

//            switch ((string)value)
//            {
//                /*
//9        rapproché
//OCT_01   impayé
//OCT_02   due
//*/
//                case "impayé": url = "../Images/RedPoint.png";
//                    break;
//                case "rapproché": url = "../Images/GreenSquare.png";
//                    break;
//                case "due": url = "../Images/YellowTriangle.png";
//                    break;
//                default: url = "../Images/RedPoint.png";
//                    break;
//            }
//            return new BitmapImage(new Uri("/AssemblyName;component/" + url, UriKind.Relative));
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }

    //public class MonthlyPaymentStatusImageConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        //	RME2028 : le champ « statut » répond aux règles suivantes :
    //        //o	Rond rouge   pour le statut « impayé »
    //        //o	Carré vert   pour le statut « rapproché »
    //        //o	Triangle jaune   pour le statut « due »

    //        string url = String.Empty;

    //        switch ((InvoiceMonthlyPaymentClientStatus)value)
    //        {
    //            case InvoiceMonthlyPaymentClientStatus.Ok: url = "../Images/GreenSquare.png";
    //                break;
    //            case InvoiceMonthlyPaymentClientStatus.Attention: url = "../Images/YellowTriangle.png";
    //                break;
    //            case InvoiceMonthlyPaymentClientStatus.Nok: url = "../Images/RedPoint.png";
    //                break;
    //            default: url = "../Images/RedPoint.png";
    //                break;
    //        }

    //        return new BitmapImage(new Uri("/AssemblyName;component/" + url, UriKind.Relative));
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
