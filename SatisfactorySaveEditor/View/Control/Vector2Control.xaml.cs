using System.Windows;
using System.Windows.Controls;
using Vector2 = SatisfactorySaveParser.Structures.Vector2;

namespace SatisfactorySaveEditor.View.Control;

public partial class Vector2Control : UserControl
{
    public Vector2Control() => InitializeComponent();

    public Vector2 Vector
    {
        get => (Vector2)GetValue(VectorProperty);
        set => SetValue(VectorProperty, value);
    }

    public static readonly DependencyProperty VectorProperty = 
        DependencyProperty.Register("Vector", typeof(Vector2), typeof(Vector2Control));
}
