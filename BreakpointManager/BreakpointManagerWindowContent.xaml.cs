using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
      if(DataContext is BreakpointManagerWindowViewModel vm)
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
  }
}
