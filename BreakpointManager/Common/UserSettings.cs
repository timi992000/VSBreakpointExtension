using BreakpointManager.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakpointManager.Common
{
  public static class UserSettings
  {
    public static IServiceProvider ServiceProvider { get; set; }

    public static string TracepointExpression
    {
      get { return Options.LogMessaage; }
      set { Options.LogMessaage = value; }
    }

    public static bool ContinueExecution
    {
      get { return Options.ContinueExecution; }
      set { Options.ContinueExecution = value; }
    }

    public static BreakpointGeneratorOptions Options { get; set; }

    private static void OnOptionsChanged(object sender, OptionsChangedEventArgs e)
    {
    }
  }
}
