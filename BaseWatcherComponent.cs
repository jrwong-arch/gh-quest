using System;
using System.Collections.Generic;
using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace gh_quest
{
    public class BaseWatcherComponent : GH_Component
    {
        //************************** GLOBAL VARIABLES **************************//




        //************************** CONSTRUCTOR **************************//
        public BaseWatcherComponent()
        : base("Base Watcher", "BW",
            "Description",
            "GH Quest", "Primary")

        {
        }

        public override bool Write(GH_IWriter writer)
        {
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            return base.Read(reader);
        }

        public override void ComputeData()
        {
            base.ComputeData();
        }


        //************************** INPUTS/OUTPUTS **************************//


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }


        //************************** SOLVE INSTANCE **************************//

        protected override void BeforeSolveInstance()
        {

        }

        protected override void AfterSolveInstance()
        {

        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {

        }


        


        //************************** CUSTOM METHODS **************************//





        //**************************UTILITIES**************************//

        //Set Component Exposure
        public override GH_Exposure Exposure => GH_Exposure.quinary;

        //Override the keywords for searching
        public override IEnumerable<string> Keywords => new List<string>(){};

        //Set Icons
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("b590d30b-c3cb-44fd-a5a5-d05b30171f71");

    }
}