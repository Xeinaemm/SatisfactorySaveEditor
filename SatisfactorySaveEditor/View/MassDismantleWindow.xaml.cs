using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Vector3 = SatisfactorySaveParser.Structures.Vector3;

namespace SatisfactorySaveEditor.Cheats;

/// <summary>
/// Interaction logic for MassDeleteWindow.xaml
/// </summary>
public partial class MassDismantleWindow : Window
{
    public MassDismantleWindow(bool isZWindow = false)
    {
        InitializeComponent();
        this.isZWindow = isZWindow;
        if(isZWindow)
        {
            xLabel.Content = "minimum Z";
            yLabel.Content = "maximum Z";
            xCoordinate.Placeholder = "-inf";
            yCoordinate.Placeholder = "+inf";
            ((WrapPanel)grid.Children[4]).Children.Remove(nextButton);
        }
    }

    private bool isZWindow = false;

    private void Window_ContentRendered(object sender, EventArgs e) => xCoordinate.Focus();

    public bool ResultSet { get; private set; }
    public Vector3 Result { get; private set; }
    public bool Done { get; private set; }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(xCoordinate.Text) && !string.IsNullOrWhiteSpace(yCoordinate.Text) && !xCoordinate.IsPlaceholder && !yCoordinate.IsPlaceholder)
            {
                Result = new Vector3()
                {
                    X = float.Parse(xCoordinate.Text),
                    Y = float.Parse(yCoordinate.Text)
                };
                ResultSet = true;
                DialogResult = true;
            }
            else
                MessageBox.Show("Enter coordinates");
        }
        catch (FormatException)
        {
            MessageBox.Show("Coordinate format: 123456");
        }
    }

    private void Done_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!xCoordinate.IsPlaceholder && !yCoordinate.IsPlaceholder)
            {
                Result = new Vector3()
                {
                    X = float.Parse(xCoordinate.Text),
                    Y = float.Parse(yCoordinate.Text)
                };
                ResultSet = true;
            }
            if(isZWindow)
            {
                Result = new Vector3();
                if (xCoordinate.IsPlaceholder)
                    Result.X = float.NegativeInfinity;
                else
                    Result.X = float.Parse(xCoordinate.Text);
                if (yCoordinate.IsPlaceholder)
                    Result.Y = float.PositiveInfinity;
                else
                    Result.Y = float.Parse(yCoordinate.Text);
                ResultSet = true;
            }
            DialogResult = true;
            Done = true;
        }
        catch (FormatException)
        {
            MessageBox.Show("Coordinate format: 123456");
        }
    }

    private void HelpButton_Click(object sender, RoutedEventArgs e) => Process.Start("https://ficsit.app/guide/1Nk4oKqhpMhgN");
}

public partial class PlaceholderTextBox : TextBox
{
    private string _placeholder;

    public string Placeholder
    {
        get => _placeholder;
        set
        {
            _placeholder = value;
            if (IsPlaceholder)
            {
                IsPlaceholder = false;
                if (string.IsNullOrEmpty(Text))
                    HandlePlaceholder(this, null);
                else
                    Text = "";
            }
        }
    }
    public bool IsPlaceholder { get; set; }
    public Brush PlaceholderColor { get; set; } = Brushes.Gray;

    public PlaceholderTextBox() : base()
    {
        TextChanged += HandlePlaceholder;
        SelectionChanged += HandleCaretPosition;
        HandlePlaceholder(this, null);
    }

    private void HandleCaretPosition(object sender, RoutedEventArgs e)
    {
        SelectionChanged -= HandleCaretPosition;
        if (IsPlaceholder)
            CaretIndex = 0;
        SelectionChanged += HandleCaretPosition;
    }

    private void HandlePlaceholder(object sender, TextChangedEventArgs e)
    {
        var newIsPlaceHolder = string.IsNullOrEmpty(Text);
        if (!IsPlaceholder && newIsPlaceHolder)
        {
            Text = Placeholder;
            Foreground = PlaceholderColor;
            CaretIndex = 0;
        }
        if (IsPlaceholder)
        {
            var textChange = e.Changes.First();
            if (textChange.AddedLength == 0)
            {
                IsPlaceholder = false;
                Text = "";
                newIsPlaceHolder = true;
            }
            else
            {
                Text = Text.Substring(textChange.Offset, textChange.AddedLength);
                Foreground = Brushes.Black;
                CaretIndex = textChange.AddedLength;
            }
        }
        IsPlaceholder = newIsPlaceHolder;
    }
}
