/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SpeckleGrasshopper.Properties;

namespace SpeckleGrasshopper
{
  public class EncapsulateList : GH_Component
  {

    public EncapsulateList( )
      : base( "Encapsulate a List", "EL",
          "Encapsulates a list to be able to set it in the properties of a speckle object. Mostly because Dimitrie is confused about list and tree management in grasshopper.",
          "Speckle", "Special" )
    {
    }


    public override Guid ComponentGuid
    {
      get { return new Guid( "{9D7837B0-F497-466B-92C0-ECF4CA39E97F}" ); }
    }

    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        return Resources.GenericIconXS;
      }
    }

    protected override void RegisterInputParams( GH_InputParamManager pManager )
    {
      pManager.AddGenericParameter( "List", "L", "List to encapsulate.", GH_ParamAccess.list );
    }

    protected override void RegisterOutputParams( GH_OutputParamManager pManager )
    {
      pManager.AddGenericParameter( "System List", "SL", "Encapsulated list", GH_ParamAccess.item );
    }

    protected override void SolveInstance( IGH_DataAccess DA )
    {
      var myList = new List<object>();
      DA.GetDataList( 0, myList);
      // DO NOT JUDGE
      try
      {
        DA.SetData( 0, new GH_ObjectWrapper( myList.Select( o => o.GetType().GetProperty( "Value" ).GetValue( o, null ) ).Cast<int>().ToList() ) );
        return;
      }
      catch { }
      try
      {
        DA.SetData( 0, new GH_ObjectWrapper( myList.Select( o => o.GetType().GetProperty( "Value" ).GetValue( o, null ) ).Cast<double>().ToList() ) );
        return;
      }
      catch { }
      try
      {
        DA.SetData( 0, new GH_ObjectWrapper( myList.Select( o => o.GetType().GetProperty( "Value" ).GetValue( o, null ) ).Cast<string>().ToList() ) );
        return;
      }
      catch { }
      try
      {
        DA.SetData( 0, new GH_ObjectWrapper( myList.Select( o => o.GetType().GetProperty( "Value" ).GetValue( o, null ) ).Cast<bool>().ToList() ) );
        return;
      }
      catch { }
      DA.SetData( 0, new GH_ObjectWrapper( myList.Select( o => o.GetType().GetProperty( "Value" ).GetValue( o, null ) ).ToList() ) );
    }
  }
}

