using BreakpointManager.BaseClasses;
using BreakpointManager.Common;
using BreakpointManager.Enums;
using EnvDTE;
using EnvDTE80;
using EnvDTE90a;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
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

    public bool CanExecute_SetBreakpoints()
    {
      return _Mode == eOperationMode.Document;
    }

    public void Execute_SetBreakpoints(eSetBreakpointMode breakpointMode)
    {
      switch (breakpointMode)
      {
        case eSetBreakpointMode.PropertiesGetter:
          __SetBreakpointsToProperties(eSetBreakpointMode.PropertiesGetter);
          break;
        case eSetBreakpointMode.PropertiesSetter:
          __SetBreakpointsToProperties(eSetBreakpointMode.PropertiesSetter);
          break;
        case eSetBreakpointMode.Properties:
          __SetBreakpointsToProperties(eSetBreakpointMode.PropertiesGetter);
          __SetBreakpointsToProperties(eSetBreakpointMode.PropertiesSetter);
          break;
        case eSetBreakpointMode.MethodsPrivate:
          __SetBreakpointsToAllMethods(eSetBreakpointMode.MethodsPrivate);
          break;
        case eSetBreakpointMode.MethodsPublic:
          __SetBreakpointsToAllMethods(eSetBreakpointMode.MethodsPublic);
          break;
        case eSetBreakpointMode.Methods:
          __SetBreakpointsToAllMethods(eSetBreakpointMode.Methods);
          break;
        default:
          break;
      }
    }

    public void Execute_EnableBreakpoints(eSetBreakpointMode mode)
    {
      try
      {
        __ActionOnModeMatchedBreakpoints((breakPoint) =>
        {
          breakPoint.Enabled = true;
        });
      }
      catch (Exception ex)
      {
      }
    }
    public void Execute_DisableBreakpoints(eSetBreakpointMode mode)
    {
      try
      {
        __ActionOnModeMatchedBreakpoints((breakPoint) =>
        {
          switch (mode)
          {
            case eSetBreakpointMode.Properties:
              break;
            case eSetBreakpointMode.PropertiesGetter:
              break;
            case eSetBreakpointMode.PropertiesSetter:
              break;
            case eSetBreakpointMode.Methods:
              break;
            case eSetBreakpointMode.MethodsPrivate:
              break;
            case eSetBreakpointMode.MethodsPublic:
              break;
            default:
              break;
          }
          breakPoint.Enabled = false;
        });
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

    private void __SetBreakpointsToProperties(eSetBreakpointMode getterOrSetter)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      TextSelection ts = PackageContext.Instance.DTE.ActiveDocument?.Selection as TextSelection;
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
          if (e is CodeProperty property)
          {

            if (getterOrSetter == eSetBreakpointMode.PropertiesGetter)
            {
              TextPoint getter = property.Getter?.StartPoint;
              __SetBreakpoint(getter);
            }
            else if (getterOrSetter == eSetBreakpointMode.PropertiesSetter)
            {
              TextPoint setter = property.Setter?.StartPoint;
              __SetBreakpoint(setter);
            }
          }
        }
      }
    }

    private void __SetBreakpointsToAllMethods(eSetBreakpointMode privateOrOther)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      TextSelection ts = PackageContext.Instance.DTE.ActiveDocument?.Selection as TextSelection;
      if (ts == null)
        return;

      PackageContext.Instance.DTE.ActiveDocument.Activate();

      var codeClasses = new List<CodeClass>();

      CodeClass cc = ts.ActivePoint.CodeElement[vsCMElement.vsCMElementClass] as CodeClass;
      if (cc != null)
        codeClasses.Add(cc);

      var doc = PackageContext.Instance.CurrentDocument;

      if (doc == null)
        return;

      if (_Mode == eOperationMode.Project)
        foreach (CodeElement element in doc.ProjectItem.ContainingProject.CodeModel.CodeElements)
        {
          //Klappt irgendwie nicht
          if (element.Kind == vsCMElement.vsCMElementClass)
          {
            if (element is CodeClass castedClass)
              codeClasses.Add(castedClass);
            //var myClass = (EnvDTE.CodeClass)element;
          }
        }


      //Todo: For whole project
      //Todo: For whole solution

      foreach (var c in codeClasses)
      {
        foreach (CodeElement e in c.Members)
        {
          if (e.Kind == vsCMElement.vsCMElementFunction && e is CodeFunction function)
          {
            if (privateOrOther == eSetBreakpointMode.MethodsPublic && function.Access != vsCMAccess.vsCMAccessPublic)
              continue;
            else if (privateOrOther == eSetBreakpointMode.MethodsPrivate && function.Access != vsCMAccess.vsCMAccessPrivate)
              continue;
            TextPoint p = function.StartPoint;
            __SetBreakpoint(p);
          }
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
