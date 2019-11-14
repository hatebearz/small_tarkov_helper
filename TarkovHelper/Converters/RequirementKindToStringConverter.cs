using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using TarkovHelper.Models;

namespace TarkovHelper.Converters
{
    public class RequirementKindToStringConverter : MarkupExtension, IValueConverter
    {
        private readonly Dictionary<RequirementKind, string> _map = new Dictionary<RequirementKind, string>
            {{RequirementKind.Hideout, "Убежище"}, {RequirementKind.Quest, "Квест"}};

        public static RequirementKindToStringConverter Instance { get; } = new RequirementKindToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<RequirementKind> kinds) 
                return kinds.Select(x => _map[x]);
            if (!(value is RequirementKind kind))
                return string.Empty;
            return _map[kind];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string text))
                return null;
            return _map.Single(x => x.Value.Equals(text)).Key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}