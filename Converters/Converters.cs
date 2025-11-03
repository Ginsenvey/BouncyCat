using FluentIcons.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace BouncyCat;

public partial class BooltoVariantConverter : IValueConverter
{
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool flag)
        {
            return flag ? IconVariant.Filled : IconVariant.Regular;
        }
        else
        {
            return IconVariant.Regular;
        }
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
public sealed partial class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Whether to invert the value.
    /// </summary>

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var vis = Visibility.Visible;
        if (value is bool v)
        {
            vis = v ? Visibility.Visible : Visibility.Collapsed;
        }

        return vis;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
public sealed partial class BoolToVisibilityReverseConverter : IValueConverter
{
    /// <summary>
    /// Whether to invert the value.
    /// </summary>

    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var vis = Visibility.Collapsed;
        if (value is bool v)
        {
            vis = v ? Visibility.Collapsed : Visibility.Visible;
        }

        return vis;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}

public class BoolToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool isSelected)
        {
            if (isSelected)
            {
                // 选中状态：使用 AccentAAFillColorDefaultBrush
                return Application.Current.Resources["AccentAAFillColorDefaultBrush"];
            }
            else
            {
                // 未选中状态：使用 TextFillColorSecondaryBrush
                return Application.Current.Resources["TextFillColorSecondaryBrush"];
            }
        }

        // 默认情况
        return Application.Current.Resources["TextFillColorSecondaryBrush"];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}