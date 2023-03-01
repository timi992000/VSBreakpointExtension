using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using System.Reflection;

namespace VSBreakpointExtension.ExtensionFiles
{
  [Guid("8eee401c-e495-4390-8177-96643c0a23ec")]
  public class BreakpointWindow : ToolWindowPane
  {
    private BreakpointWindowPane m_Content;
    public BreakpointWindow() : base(null)
    {
      //this.Caption = String.Format("Breakpoint Manager {0}", Assembly.GetExecutingAssembly().GetName().Version);
      m_Content = new BreakpointWindowPane(this);
      Content = m_Content;
    }
    protected override void Initialize()
    {
      base.Initialize();
      if (m_Content != null)
        m_Content.OnVisualStudioToolWindowCreated();
    }
  }
}
