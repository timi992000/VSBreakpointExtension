using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using VSBreakpointExtension;

namespace BreakpointManager.Common
{
  public class PackageContext
  {
    private static readonly PackageContext _instance = new PackageContext();
    public static PackageContext Instance
    {
      get { return _instance; }
    }
    public IServiceProvider ServiceProvider => Package;
    public DTE2 DTE => ServiceProvider.GetService(typeof(DTE)) as DTE2;
    public BreakpointManagerPackage Package { get; set; }
    public IVsSolution Solution { get; set; }
    public Solution CurrentSolution => DTE.Solution;
    public Document CurrentDocument => DTE.ActiveDocument;
    public uint SolutionCookie { get; set; }
    public IVsHierarchy Hierarchy { get; set; }
  }
}
