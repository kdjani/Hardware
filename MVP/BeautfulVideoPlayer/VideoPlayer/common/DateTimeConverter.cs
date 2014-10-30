using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.BulkAccess;
using Windows.UI.Xaml.Data;

namespace App4.Common
{
  class DateTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      string dateString = "no date";

      if (value is DateTimeOffset)
      {
        DateTimeOffset offset = (DateTimeOffset)value;
        dateString = offset.ToString();
      }

      return(dateString);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}
