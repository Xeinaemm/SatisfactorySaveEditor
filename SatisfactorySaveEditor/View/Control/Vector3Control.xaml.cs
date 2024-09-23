using System.Windows;
using System.Windows.Controls;
using Vector3 = SatisfactorySaveParser.Structures.Vector3;

namespace SatisfactorySaveEditor.View.Control;

public partial class Vector3Control : UserControl
{
    public Vector3Control() => InitializeComponent();

    public Vector3 Vector
    {
        get => (Vector3)GetValue(VectorProperty);
        set => SetValue(VectorProperty, value);
    }

    public static readonly DependencyProperty VectorProperty = 
        DependencyProperty.Register("Vector", typeof(Vector3), typeof(Vector3Control));
}
