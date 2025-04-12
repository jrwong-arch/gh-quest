
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.GUI;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Eto.Forms;
using System.ComponentModel;
using GH_IO.Types;
using Rhino.DocObjects;
using System.IO;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Attributes; // For GH_ComponentAttributes

// Eto.Forms and Eto.Drawing for the dialogs:
using Eto.Forms;
using Eto.Drawing;
using CarverComponents.Templates;

namespace CarverComponents
{
    public class NewComponent : GH_Component
    {
        //************************** GLOBAL VARIABLES **************************//




        //************************** CONSTRUCTOR **************************//
        public NewComponent()
          : base("Name", "Nickname",
            "Description",
            "Category", "Subcategory")

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

