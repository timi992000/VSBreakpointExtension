using BreakpointManager;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace VSBreakpointExtension
{

  [ProvideBindingPath]
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this Package for Help/About
  [ProvideMenuResource("Menus.ctmenu", 1)]
  [ProvideToolWindow(typeof(BreakpointManagerWindow))]
  [Guid(PackageGuidString)]
  public sealed class BreakpointManagerPackage : AsyncPackage
  {
    public const string PackageGuidString = "33f9a5d7-1208-45c9-84e4-caae9e03b81b";

    #region Package Members

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
      await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
      await BreakpointManagerWindowCommand.InitializeAsync(this);
    }

    #endregion
  }
}
