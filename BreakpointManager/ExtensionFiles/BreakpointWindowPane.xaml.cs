using EnvDTE;
using EnvDTE80;
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

namespace VSBreakpointExtension.ExtensionFiles
{
  /// <summary>
  /// Interaction logic for BreakpointWindowPane.xaml
  /// </summary>
  public partial class BreakpointWindowPane : UserControl
  {
    private Solution m_CurrentSolution;
 
    private string m_LastSolutionName; 
    private IServiceProvider m_ServiceProvider;
    private SolutionEvents m_SolutionEvents;
    private BuildEvents m_BuildEvents;

    public BreakpointWindowPane(IServiceProvider serviceProvider)
    {
      m_ServiceProvider = serviceProvider;
      InitializeComponent();
    }

    public Solution CurrentSolution
    {
      get
      {
        return m_CurrentSolution;
      }
      set
      {
        m_CurrentSolution = value;
      }
    }

    public void OnVisualStudioToolWindowCreated()
    {
      try
      {
        Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
        DTE2 dte = (DTE2)m_ServiceProvider.GetService(typeof(DTE));
        if (dte == null)
          return; 
        CurrentSolution = dte.Solution;
        if (m_SolutionEvents != null)
        {
          //m_SolutionEvents.Opened -= SolutionEvents_SolutionOpened;
          //m_SolutionEvents.AfterClosing -= SolutionEvents_AfterClosing;
        }
        //m_SolutionEvents.Opened += SolutionEvents_SolutionOpened;
        //m_SolutionEvents.AfterClosing += SolutionEvents_AfterClosing; 
        //if (!string.IsNullOrEmpty(CurrentSolution.FullName))
        //  UpdateLayoutAfterRebuild(vsBuildAction.vsBuildActionBuild);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString(), "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

  }
}
