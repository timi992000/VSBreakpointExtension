using BreakpointManager.Common;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BreakpointManager
{
  public class BreakpointManagerWindowViewModel : ViewModelBase
  {
    private SolutionEvents m_SolutionEvents;
    private DocumentEvents m_DocumentEvents;
    public BreakpointManagerWindowViewModel()
    {
      __Initialize();
    }

    public string DocumentName => PackageContext.Instance.CurrentDocument?.Name;
    public string SolutionName => PackageContext.Instance.CurrentSolution?.FileName;
    public SolutionEvents SolutionEvents => m_SolutionEvents;

    public void RefreshCurrentState()
    {
      OnPropertyChanged(nameof(DocumentName));
    }

    public void SetBreakpointsToAll()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      TextSelection ts = PackageContext.Instance.DTE.ActiveDocument.Selection as TextSelection;
      if (ts == null)
        return;

      CodeClass c = ts.ActivePoint.CodeElement[vsCMElement.vsCMElementClass] as CodeClass;
      if (c == null)
        return;

      foreach (CodeElement e in c.Members)
      {
        if (e.Kind == vsCMElement.vsCMElementFunction)
        {
          TextPoint p = (e as CodeFunction).GetStartPoint();
          PackageContext.Instance.DTE.Debugger.Breakpoints.Add("", p.Parent.Parent.FullName, p.Line);
        }
      }
    }

    private void __Initialize()
    {
      __AttachEvents();
      var solution = PackageContext.Instance.ServiceProvider.GetService<SVsSolution, IVsSolution>();
      PackageContext.Instance.Solution = solution;
    }

    private void __AttachEvents()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var dteSvc = PackageContext.Instance.DTE;
      m_SolutionEvents = dteSvc.Events.SolutionEvents;
      m_DocumentEvents = dteSvc.Events.DocumentEvents;
      m_SolutionEvents.Opened += __SolutionOpened;
    }
    private void __SolutionOpened()
    {
      PackageContext.Instance.Solution = BreakpointManagerWindowCommand.Instance.Package.GetService<SVsSolution, IVsSolution>();
      OnPropertyChanged(nameof(SolutionName));
    }
  }
}
