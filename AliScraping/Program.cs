using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.ThreadException += Application_ThreadException;
      Application.Run(new Form1());
    }

    static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      // Handle your exception here...
      var st = new StackTrace(e.Exception, true);
      MessageBox.Show(String.Format("There is an error line : "+ st.GetFrame(st.FrameCount-1).GetFileLineNumber() + "\n{0}", e.Exception.ToString()));
    }

  }
}
