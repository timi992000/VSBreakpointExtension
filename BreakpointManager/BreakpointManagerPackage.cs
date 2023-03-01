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
    public const string PackageGuidString = "6f5361e4-63cd-42bc-ade7-72a83192d441";

    #region Package Members

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
      await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

      await BreakpointAddin.InitializeAsync(this);
    }

    #endregion
  }
}
