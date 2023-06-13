using BreakpointManager.BaseClasses;
using BreakpointManager.Common;
using BreakpointManager.Enums;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Windows;

namespace BreakpointManager.ViewModel
{
  public class OperationBaseViewModel : ViewModelBase
  {
    private eOperationMode _Mode;
    public OperationBaseViewModel(eOperationMode mode)
    {
      _Mode = mode;
      __Init();
    }

    public string CurrentItemText
    {
      get => Get<string>();
      set => Set(value);
    }

    public void Execute_SetBreakpoints(eSetBreakpointMode breakpointMode)
    {
      switch (breakpointMode)
      {
        case eSetBreakpointMode.Properties:
          __SetBreakpointsToProperties();
          break;
        case eSetBreakpointMode.Methods:
          __SetBreakpointsToAllMethods();
          break;
        default:
          break;
      }
    }

    public void Execute_EnableBreakpoints()
    {
      try
      {
        __ActionOnModeMatchedBreakpoints((breakPoint) => breakPoint.Enabled = true);
      }
      catch (Exception ex)
      {
      }
    }
    public void Execute_DisableBreakpoints()
    {
      try
      {
        __ActionOnModeMatchedBreakpoints((breakPoint) => breakPoint.Enabled = false);
      }
      catch (Exception ex)
      {
      }
    }

    public void Execute_DeleteBreakpoints()
    {
      try
      {
        __ActionOnModeMatchedBreakpoints((breakPoint) => breakPoint.Delete());
      }
      catch (Exception)
      {
      }
    }

    public void Execute_DeleteDisabledBreakpoints()
    {
      try
      {
        __ActionOnModeMatchedBreakpoints((breakPoint) =>
        {
          if (!breakPoint.Enabled)
            breakPoint.Delete();
        });
      }
      catch (Exception)
      {
      }
    }

    private void __ActionOnModeMatchedBreakpoints(Action<Breakpoint> actionOnBreakpoint)
    {
      var allBreakpoints = PackageContext.Instance.DTE.Debugger.Breakpoints;
      foreach (Breakpoint breakpoint in allBreakpoints)
      {
        if (_Mode == eOperationMode.Document && breakpoint.File == PackageContext.Instance.CurrentDocument.FullName)
          actionOnBreakpoint(breakpoint);
        else if (_Mode == eOperationMode.Project)
        {
          var fullProjectName = PackageContext.Instance.CurrentProject?.FileName;
          var dir = Path.GetDirectoryName(fullProjectName);
          if (breakpoint.File.StartsWith(dir))
            actionOnBreakpoint(breakpoint);
        }
        else if (_Mode == eOperationMode.Solution)
          actionOnBreakpoint(breakpoint);
      }
    }
    private void __Init()
    {
      PackageContext.Instance.SolutionChanged += (sender, e) => { __RefreshTexts(); };
      ThreadHelper.ThrowIfNotOnUIThread();
      __RefreshTexts();
    }

    private void __SetBreakpointsToProperties()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      TextSelection ts = PackageContext.Instance.DTE.ActiveDocument.Selection as TextSelection;
      if (ts == null)
        return;

      CodeClass c = ts.ActivePoint.CodeElement[vsCMElement.vsCMElementClass] as CodeClass;
      if (c == null)
        return;

      //Todo: For whole project
      //Todo: For whole solution

      foreach (CodeElement e in c.Members)
      {
        if (e.Kind == vsCMElement.vsCMElementProperty)
        {
          if (e is CodeProperty getterProperty)
          {
            TextPoint p = getterProperty.Getter?.StartPoint;
            __SetBreakpoint(p);
          }
          if (e is CodeProperty setterProperty)
          {
            TextPoint p = setterProperty.Setter?.StartPoint;
            __SetBreakpoint(p);
          }
        }
      }
    }

    private void __SetBreakpointsToAllMethods()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      TextSelection ts = PackageContext.Instance.DTE.ActiveDocument.Selection as TextSelection;
      if (ts == null)
        return;

      CodeClass c = ts.ActivePoint.CodeElement[vsCMElement.vsCMElementClass] as CodeClass;
      if (c == null)
        return;

      //Todo: For whole project
      //Todo: For whole solution

      foreach (CodeElement e in c.Members)
      {
        if (e.Kind == vsCMElement.vsCMElementFunction && e is CodeFunction function)
        {
          TextPoint p = function.StartPoint;
          __SetBreakpoint(p);
        }
      }
    }

    private string __GetModeName()
    {
      switch (_Mode)
      {
        case eOperationMode.Document:
          return "Document";
        case eOperationMode.Project:
          return "Project";
        case eOperationMode.Solution:
          return "Solution";
        default:
          return "Unknown mode";
      }
    }

    private string __GetCurrentItem()
    {
      switch (_Mode)
      {
        case eOperationMode.Document:
          return PackageContext.Instance.CurrentDocument?.Name;
        case eOperationMode.Project:
          var cc = PackageContext.Instance.CurrentProject?.Name;
          return cc;
        case eOperationMode.Solution:
          return Path.GetFileName(PackageContext.Instance.CurrentSolution?.FileName);
        default:
          return "Unknown mode";
      }
    }

    private void __RefreshTexts()
    {
      try
      {
        var modeName = __GetModeName();
        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() => CurrentItemText = $"{modeName}: {__GetCurrentItem()}"));
      }
      catch (Exception)
      {
      }
    }

    private void __SetBreakpoint(TextPoint p)
    {
      try
      {
        if (p == null)
          return;
        PackageContext.Instance.DTE.Debugger.Breakpoints.Add("", p.Parent.Parent.FullName, p.Line);
      }
      catch (System.Exception ex)
      {
      }
    }

  }
}
