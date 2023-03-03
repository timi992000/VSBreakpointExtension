using System.Windows;
using System.Windows.Controls;

namespace BreakpointManager.Views
{
  /// <summary>
  /// Interaction logic for BreakpointTab.xaml
  /// </summary>
  public partial class BreakpointTab : UserControl
  {
    public BreakpointTab()
    {
      InitializeComponent();
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

  }
}
