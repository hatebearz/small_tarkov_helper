using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using TarkovHelper.Models;

namespace TarkovHelper.Converters
{
    public class RequirementsToStringConverter : MarkupExtension, IValueConverter
    {
        public static RequirementsToStringConverter Instance { get; } = new RequirementsToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IEnumerable<Requirement> requirements))
                return string.Empty;
            var stringBuilder = new StringBuilder();
            return requirements.Aggregate(new StringBuilder(), (builder, x) =>
                {
                    if (builder.Length == 0)
                        return builder.Append($"{x.For} = {x.Amount}");
                    return stringBuilder.AppendLine($"{x.For} = {x.Amount}");
                })
                .ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string text))
                return new ObservableCollection<Requirement>();
            if (string.IsNullOrWhiteSpace(text))
                return new ObservableCollection<Requirement>();
            return new ObservableCollection<Requirement>(text.Split('\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => x.Contains('='))
                .Select(x =>
                {
                    var parts = x.Split('=');
                    if (parts.Length != 2) return null;
                    if (!int.TryParse(parts[1].Trim(), out var amount))
                        return null;
                    return new Requirement {For = parts[0].Trim(), Amount = amount};
                })
                .Where(x => x != null));
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}