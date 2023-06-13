using BreakpointManager.BaseClasses;
using BreakpointManager.Common;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Timers;
using System.Windows;

namespace BreakpointManager
{
	public class BreakpointManagerWindowViewModel : ViewModelBase
	{
		private SolutionEvents m_SolutionEvents;
		public BreakpointManagerWindowViewModel()
		{
			__Initialize();
			__SetTimer();
		}

		private void __SetTimer()
		{
			var documentCheckTimer = new Timer();
			documentCheckTimer.Interval = 1000;
			documentCheckTimer.Elapsed += (s, e) =>
			{
				PackageContext.Instance.ExecuteSolutionChangedEvent();
			};
			documentCheckTimer.Start();
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
			m_SolutionEvents.Opened += __SolutionOpened;
		}
		private void __SolutionOpened()
		{
			PackageContext.Instance.Solution = BreakpointManagerWindowCommand.Instance.Package.GetService<SVsSolution, IVsSolution>();
			PackageContext.Instance.ExecuteSolutionChangedEvent();

		}
	}
}
