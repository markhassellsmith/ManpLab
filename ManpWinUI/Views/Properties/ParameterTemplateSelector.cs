#nullable enable

using ManpWinUI.ViewModels.Properties;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ManpWinUI.Views.Properties;

/// <summary>
/// Selects the appropriate DataTemplate for a parameter based on its type and read-only status.
/// </summary>
public sealed class ParameterTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Template for read-only string parameters (displays as TextBlock).
    /// </summary>
    public DataTemplate? ReadOnlyStringTemplate { get; set; }

    /// <summary>
    /// Template for editable double parameters (NumberBox with min/max).
    /// </summary>
    public DataTemplate? EditableDoubleTemplate { get; set; }

    /// <summary>
    /// Template for editable integer parameters (NumberBox in integer mode).
    /// </summary>
    public DataTemplate? EditableIntegerTemplate { get; set; }

    /// <summary>
    /// Template for editable complex parameters (two NumberBoxes for real/imaginary).
    /// </summary>
    public DataTemplate? EditableComplexTemplate { get; set; }

    /// <summary>
    /// Template for boolean parameters (CheckBox).
    /// </summary>
    public DataTemplate? EditableBooleanTemplate { get; set; }

    /// <summary>
    /// Fallback template for unsupported types.
    /// </summary>
    public DataTemplate? FallbackTemplate { get; set; }

    protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container)
    {
        if (item is not ParameterItem parameter)
        {
            return FallbackTemplate;
        }

        // Read-only parameters always use text display
        if (parameter.IsReadOnly)
        {
            return ReadOnlyStringTemplate;
        }

        // Editable parameters use type-specific controls
        return parameter.Type switch
        {
            ParameterType.Double => EditableDoubleTemplate,
            ParameterType.Integer => EditableIntegerTemplate,
            ParameterType.Complex => EditableComplexTemplate,
            ParameterType.Boolean => EditableBooleanTemplate,
            ParameterType.String => ReadOnlyStringTemplate, // Strings are display-only
            _ => FallbackTemplate
        };
    }
}
