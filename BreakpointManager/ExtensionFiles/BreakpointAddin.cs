using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Task = System.Threading.Tasks.Task;

namespace VSBreakpointExtension.ExtensionFiles
{
  /// <summary>
  /// This is the class that implements the package exposed by this assembly.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The minimum requirement for a class to be considered a valid package for Visual Studio
  /// is to implement the IVsPackage interface and register itself with the shell.
  /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
  /// to do it: it derives from the Package class that provides the implementation of the
  /// IVsPackage interface and uses the registration attributes defined in the framework to
  /// register itself and its components with the shell. These attributes tell the pkgdef creation
  /// utility what data to put into .pkgdef file.
  /// </para>
  /// <para>
  /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
  /// </para>
  /// </remarks>
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [Guid("8eee401c-e495-4390-8177-96643c0a23ec")]
  [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
  [ProvideToolWindow(typeof(BreakpointWindow))]
  [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
  public sealed class BreakpointAddin : AsyncPackage
  {
    /// <summary>
    /// BreakpointAddin GUID string.
    /// </summary>
    public const int CommandId = 257;
    public static readonly Guid CommandSet = new Guid(PackageGuidString);
    public static string PackageGuidString => "8eee401c-e495-4390-8177-96643c0a23ec";
    private readonly Package package;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BreakpointAddin"/> class.
    /// </summary>
    private BreakpointAddin(Package package, IMenuCommandService commandService)
    {
      if (package == null)
      {
        throw new ArgumentNullException("package");
      }
      this.package = package;
      if (commandService != null)
      {
        var menuCommandID = new CommandID(CommandSet, CommandId);
        var menuItem = new MenuCommand(ShowToolWindow, menuCommandID);
        commandService.AddCommand(menuItem);
      }
      //__TrySetBreakPoints();
    }

    #region Package Members

    public static BreakpointAddin Instance
    {
      get;
      private set;
    }

    private IServiceProvider ServiceProvider
    {
      get
      {
        return this.package;
      }
    }

    public static async Task InitializeAsync(AsyncPackage package)
    {
      var commandService = (IMenuCommandService)await package.GetServiceAsync(typeof(IMenuCommandService));
      Instance = new BreakpointAddin(package, commandService);
    }

    #endregion

    private void ShowToolWindow(object sender, EventArgs e)
    {

      ToolWindowPane window = this.package.FindToolWindow(typeof(BreakpointWindow), 0, true);
      if ((null == window) || (null == window.Frame))
      {
        throw new NotSupportedException("Cannot create breakpoint pane");
      }
      ThreadHelper.ThrowIfNotOnUIThread();
      IVsWindowFrame windowFrame = window.Frame as IVsWindowFrame;
      Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
    }

    private void __TrySetBreakPoints()
    {
      var tmpDte = (DTE2)ServiceProvider.GetService(typeof(DTE));
      EnvDTE.TextSelection ts = tmpDte.ActiveWindow.Selection as EnvDTE.TextSelection;
      if (ts == null)
        return;

      EnvDTE.CodeClass c = ts.ActivePoint.CodeElement[vsCMElement.vsCMElementClass]
                  as EnvDTE.CodeClass;
      if (c == null)
        return;

      foreach (EnvDTE.CodeElement e in c.Members)
      {
        if (e.Kind == vsCMElement.vsCMElementFunction)
        {
          EnvDTE.TextPoint p = (e as EnvDTE.CodeFunction).GetStartPoint();
          tmpDte.Debugger.Breakpoints.Add("", p.Parent.Parent.FullName, p.Line);
        }
      }

      
      
    }

  }
}
