using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino;


namespace gh_quest
{
  public class gh_questInfo : GH_AssemblyInfo
  {
    public override string Name => "GH Quest";

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "";

    public override Guid Id => new Guid("211e239c-a548-47c0-b48c-cd5a0e130d25");

    //Return a string identifying you or your company.
    public override string AuthorName => "";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "https://github.com/jrwong-arch/gh-quest";

    //Return a string representing the version.  This returns the same version as the assembly.
    public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();

  }
}