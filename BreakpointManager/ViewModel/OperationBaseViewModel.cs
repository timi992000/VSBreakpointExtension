using BreakpointManager.BaseClasses;
using BreakpointManager.Common;
using BreakpointManager.Enums;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Windows;

namespace BreakpointManager.ViewModel
{
	public class OperationBaseViewModel : ViewModelBase
	{
		private SolutionEvents m_SolutionEvents;
		private DocumentEvents m_DocumentEvents;
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

		public string SetBreakpointsToMethodsHeader
		{
			get => Get<string>();
			set => Set(value);
		}

		public string SetBreakpointsToPropertiesHeader
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

		private void __Init()
		{
			PackageContext.Instance.SolutionChanged += (sender, e) => { __RefreshTexts(); };
			ThreadHelper.ThrowIfNotOnUIThread();
			var dteSvc = PackageContext.Instance.DTE;
			m_DocumentEvents = dteSvc.Events.DocumentEvents;
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
					return PackageContext.Instance.CurrentProject?.Name;
				case eOperationMode.Solution:
					return PackageContext.Instance.CurrentSolution?.FileName;
				default:
					return "Unknown mode";
			}
		}

		private void __RefreshTexts()
		{
			var modeName = __GetModeName();
			SetBreakpointsToMethodsHeader = $"Set breakpoints to methods";
			SetBreakpointsToPropertiesHeader = $"Set breakpoints to properties";
			Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input , new Action(() => CurrentItemText = $"{modeName}: {__GetCurrentItem()}"));
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
