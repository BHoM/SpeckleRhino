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
using SpeckleGrasshopper.Properties;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using System.Diagnostics;

namespace SpeckleGrasshopper.Contrib
{
  public class MeshTexCoords : GH_Component
  {
    /// <summary>
    /// Initializes a new instance of the Cmpt_GetPlane class.
    /// </summary>
    public MeshTexCoords( )
      : base( "Get Mesh Texture Coordinates", "TexCoords",
          "Encode Mesh texture coordinates to a string.",
          "Speckle", "Contrib" )
    {
      SpeckleCore.SpeckleInitializer.Initialize();
      SpeckleCore.LocalContext.Init();
    }

    protected override void RegisterInputParams( GH_Component.GH_InputParamManager pManager )
    {
      pManager.AddMeshParameter( "Mesh", "M", "Source mesh.", GH_ParamAccess.item );
      pManager.AddBooleanParameter( "Base64", "B", "Encode as base64 string.", GH_ParamAccess.item, false );
    }

    protected override void RegisterOutputParams( GH_Component.GH_OutputParamManager pManager )
    {
      pManager.AddTextParameter( "TexCoords", "TX", "Texture coordinates as human-readable string or base64 string.", GH_ParamAccess.item );
    }

    protected override void SolveInstance( IGH_DataAccess DA )
    {
      Mesh iMesh = new Mesh();
      bool iB = false;

      if ( !DA.GetData( "Mesh", ref iMesh ) )
      {
        this.AddRuntimeMessage( GH_RuntimeMessageLevel.Error, "No input mesh supplied." );
        return;
      }

      if ( iMesh.TextureCoordinates.Count != iMesh.Vertices.Count )
      {
        this.AddRuntimeMessage( GH_RuntimeMessageLevel.Error, "Mesh contains no texture coordinates." );
        return;
      }

      DA.GetData( "Base64", ref iB );

      int N = iMesh.TextureCoordinates.Count;
      System.Text.StringBuilder uv_string = new System.Text.StringBuilder();

      for ( int i = 0; i < N; ++i )
        uv_string.Append( string.Format( "{0} {1} ", iMesh.TextureCoordinates[ i ].X, iMesh.TextureCoordinates[ i ].Y ) );

      string strOutput;

      if ( iB )
        strOutput = System.Convert.ToBase64String( System.Text.Encoding.UTF8.GetBytes( uv_string.ToString() ) );
      else
        strOutput = uv_string.ToString();

      DA.SetData( "TexCoords", strOutput );
    }

    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        return Resources.GenericIconXS;
      }
    }

    public override Guid ComponentGuid
    {
      get { return new Guid( "0a92342f-7a51-40a0-96dd-b64915ad2bbc" ); }
    }
  }
}

