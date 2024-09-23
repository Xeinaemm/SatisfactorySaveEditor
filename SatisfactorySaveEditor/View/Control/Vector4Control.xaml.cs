using System.Windows;
using System.Windows.Controls;
using Vector4 = SatisfactorySaveParser.Structures.Vector4;

namespace SatisfactorySaveEditor.View.Control;
public partial class Vector4Control : UserControl
{
    public Vector4Control() => InitializeComponent();

    public Vector4 Vector
    {
        get => (Vector4)GetValue(VectorProperty);
        set => SetValue(VectorProperty, value);
    }

    public static readonly DependencyProperty VectorProperty =
        DependencyProperty.Register("Vector", typeof(Vector4), typeof(Vector4Control));
}
