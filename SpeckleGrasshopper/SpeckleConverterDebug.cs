/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

using GH_IO.Serialization;
using System.Diagnostics;
using Grasshopper.Kernel.Parameters;

using SpeckleGhRhConverter;


using Grasshopper;
using Grasshopper.Kernel.Data;

using Newtonsoft.Json;
using System.Dynamic;
using SpeckleCommon;

namespace SpeckleGrasshopper
{
    public class EncodeToSpeckle : GH_Component
    {
        public EncodeToSpeckle()
          : base("Speckle Converter", "Speckle Converter",
              "Speckle Converter",
              "Speckle", "Debug")
        {
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{c4442de1-c440-40ba-8da7-33c89eb1a529}"); }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "O", "Objects to convert.", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Conversion Result String", "S", "Conversion result string.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Conversion Result", "R", "Conversion result object.", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object myObj = new object();
            DA.GetData(0, ref myObj);

            var result = GhRhConveter.FromGhRhObject(myObj);
            DA.SetData(0, JsonConvert.SerializeObject(result, Formatting.Indented));
            DA.SetData(1, result);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }
    }
}
