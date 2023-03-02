using BreakpointManager.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VSBreakpointExtension;

namespace BreakpointManager
{
  internal sealed class BreakpointManagerWindowCommand
  {
    public const int CommandId = 257;
    public static readonly Guid CommandSet = new Guid("e73b1379-6d9d-4057-adf1-aca51e35f7c4");
    public readonly Package Package;
    private BreakpointManagerWindowCommand(Package package, IMenuCommandService commandService)
    {
      if (package == null)
      {
        throw new ArgumentNullException("Package");
      }

      Package = package;

      if (commandService != null)
      {
        var menuCommandID = new CommandID(CommandSet, CommandId);
        var menuItem = new MenuCommand(ShowToolWindow, menuCommandID);
        commandService.AddCommand(menuItem);
      }
    }
    public static BreakpointManagerWindowCommand Instance
    {
      get;
      private set;
    }
    public static async Task InitializeAsync(AsyncPackage package)
    {
      var svcProvider = await package.GetServiceAsync(typeof(IMenuCommandService));
      var commandService = svcProvider as OleMenuCommandService;
      PackageContext.Instance.Package = package as BreakpointManagerPackage;
      Instance = new BreakpointManagerWindowCommand(package, commandService);
    }
    private void ShowToolWindow(object sender, EventArgs e)
    {
      ToolWindowPane window = this.Package.FindToolWindow(typeof(BreakpointManagerWindow), 0, true);
      if ((null == window) || (null == window.Frame))
      {
        throw new NotSupportedException("Cannot create tool window");
      }
      ThreadHelper.ThrowIfNotOnUIThread();
      IVsWindowFrame windowFrame = window.Frame as IVsWindowFrame;
      Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
    }
  }
}
