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

namespace BreakpointManager.Views
{
  /// <summary>
  /// Interaction logic for BreakpointCommonView.xaml
  /// </summary>
  public partial class BreakpointCommonView : UserControl
  {
    public BreakpointCommonView()
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
