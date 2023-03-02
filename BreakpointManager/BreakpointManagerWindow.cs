using Microsoft.VisualStudio.Shell;
using System;
using System.Reflection;

namespace BreakpointManager
{
  public class BreakpointManagerWindow : ToolWindowPane
  {
    public BreakpointManagerWindow() : base(null)
    {
      this.Caption = String.Format("Breakpoint Manager {0}", Assembly.GetExecutingAssembly().GetName().Version);
      Content = new BreakpointManagerWindowContent();
    }
  }
}
