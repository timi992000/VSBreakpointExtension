using VSBreakpointExtension.ExtensionFiles;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace VSBreakpointExtension
{

  [ProvideBindingPath]
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
  [ProvideMenuResource("Menus.ctmenu", 1)]
  [ProvideToolWindow(typeof(BreakpointWindowPane))]
  [Guid(PackageGuidString)]
  public sealed class VSBreakpointExtensionPackage : AsyncPackage
  {
    /// <summary>
    /// VSBreakpointExtensionPackage GUID string.
    /// </summary>
    public const string PackageGuidString = "6f5361e4-63cd-42bc-ade7-72a83192d441";

    #region Package Members

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initialization code that rely on services provided by VisualStudio.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
    /// <param name="progress">A provider for progress updates.</param>
    /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
      // When initialized asynchronously, the current thread may be a background thread at this point.
      // Do any initialization that requires the UI thread after switching to the UI thread.
      await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

      await BreakpointAddin.InitializeAsync(this);
    }

    #endregion
  }
}
