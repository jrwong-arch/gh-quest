using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace gh_quest
{
  public class gh_questInfo : GH_AssemblyInfo
  {
    public override string Name => "gh-quest Info";

    //Return a 24x24 pixel bitmap to represent this GHA library.
    public override Bitmap Icon => null;

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "";

    public override Guid Id => new Guid("2c37ecbc-750a-43d8-8f3b-7487cc565f09");

    //Return a string identifying you or your company.
    public override string AuthorName => "";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "";

    //Return a string representing the version.  This returns the same version as the assembly.
    public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
  }
}