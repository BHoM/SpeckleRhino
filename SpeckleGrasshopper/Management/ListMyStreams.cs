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

using Grasshopper.Kernel;
using SpeckleCore;
using SpeckleGrasshopper.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleGrasshopper.Management
{
  public class ListStreams : GH_Component
  {
    List<SpeckleStream> UserStreams = new List<SpeckleStream>();
    List<SpeckleStream> SharedStreams = new List<SpeckleStream>();
    SpeckleApiClient Client = new SpeckleApiClient();

    Action ExpireComponent;

    public ListStreams( ) : base( "Streams", "Streams", "Lists your existing Speckle streams for a specified account.", "Speckle", "Management" )
    {
      SpeckleCore.SpeckleInitializer.Initialize();
      SpeckleCore.LocalContext.Init();

    }

    protected override void RegisterInputParams( GH_InputParamManager pManager )
    {
      pManager.AddGenericParameter( "Account", "A", "Speckle account you want to retrieve your streams from.", GH_ParamAccess.item );

    }

    protected override void RegisterOutputParams( GH_OutputParamManager pManager )
    {
      pManager.Register_GenericParam( "streams", "S", "Streams that you own or are shared with you." );
    }

    public override void AddedToDocument( GH_Document document )
    {
      base.AddedToDocument( document );

      ExpireComponent = ( ) => this.ExpireSolution( true );
    }

    protected override void SolveInstance( IGH_DataAccess DA )
    {
      Account Account = null;
      DA.GetData( 0, ref Account );

      if ( Account == null)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Couldn't set the account");
        return;
      }

      DA.SetDataList( 0, UserStreams );
           
      Client.BaseUrl = Account.RestApi; Client.AuthToken = Account.Token;
      Client.StreamsGetAllAsync( "fields=streamId,name,description&isComputedResult=false&deleted=false" ).ContinueWith( tsk =>
         {
           if (tsk.Result.Success == false)
           {
             AddRuntimeMessage(GH_RuntimeMessageLevel.Error, tsk.Result.Message);
             return;
           }
           var newStreams = tsk.Result.Resources.ToList();
           var notUpdated = UserStreams.Select(x => x._id).SequenceEqual(newStreams.Select(x => x._id));

           if (!notUpdated)
           {
             UserStreams = tsk.Result.Resources.ToList();
             Rhino.RhinoApp.MainApplicationWindow.Invoke(ExpireComponent);
           }
         } );
    }

    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        return Resources.Streams;
      }
    }

    public override Guid ComponentGuid
    {
      get { return new Guid( "{12d9bb3a-6cc3-4fe2-95cb-c58b1415977b}" ); }
    }
  }
}
