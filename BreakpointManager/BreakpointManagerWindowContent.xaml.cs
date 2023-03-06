using System.Windows;
using System.Windows.Controls;

namespace BreakpointManager
{
  /// <summary>
  /// Interaction logic for BreakpointManagerWindowContent.xaml
  /// </summary>
  public partial class BreakpointManagerWindowContent : UserControl
  {
    public BreakpointManagerWindowContent()
    {
      Loaded += __Loaded;
      InitializeComponent();

    }

    private void __Loaded(object sender, RoutedEventArgs e)
    {
      DataContext = new BreakpointManagerWindowViewModel();
      Loaded -= __Loaded;
    }
  }
}
