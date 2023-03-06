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

    private void __SetBPToMethods(object sender, RoutedEventArgs e)
    {
      if (DataContext is BreakpointManagerWindowViewModel vm)
      {
        vm.SetBreakpointsToAllMethods();
      }
    }

    private void __SetBPToProperties(object sender, RoutedEventArgs e)
    {
      if (DataContext is BreakpointManagerWindowViewModel vm)
      {
        vm.SetBreakpointsToProperties();
      }
    }

    private void __GotFocus(object sender, RoutedEventArgs e)
    {
      if (DataContext is BreakpointManagerWindowViewModel vm)
      {
        vm.RefreshCurrentState();
      }
    }

    private void __DeleteDocumentBP(object sender, RoutedEventArgs e)
    {
      if (DataContext is BreakpointManagerWindowViewModel vm)
      {
        vm.DeleteBreakpointsForCurrentFile();
      }
    }
  }
}
