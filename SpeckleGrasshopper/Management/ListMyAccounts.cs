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

//extern alias SpeckleNewtonsoft;
using SNJ = Newtonsoft.Json;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SpeckleCore;
using SpeckleGrasshopper.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GH_IO.Serialization;
//using Newtonsoft.Json;

namespace SpeckleGrasshopper.Management
{
  public class ListMyAccounts : GH_Component
  {
    List<Account> Accounts = new List<Account>();
    Account Selected;
    Action ExpireComponent;

    public ListMyAccounts( ) : base( "Accounts", "Accounts", "Lists your existing Speckle accounts.", "Speckle", "Management" )
    {
      SpeckleCore.SpeckleInitializer.Initialize();
      SpeckleCore.LocalContext.Init();
    }


    public override bool Read( GH_IReader reader )
    {
      string restApi = "", email = "";
      reader.TryGetString( "restapi", ref restApi );
      reader.TryGetString( "email", ref email );

      try
      {
        var acc = LocalContext.GetAccountByEmailAndRestApi( email, restApi );
        Selected = acc;
      }
      catch ( Exception e )
      {
        AddRuntimeMessage( GH_RuntimeMessageLevel.Error, "Account not found." );
      }
      return base.Read( reader );
    }

    public override bool Write( GH_IWriter writer )
    {
      //writer.SetString( "selectedAccount", SNJ.JsonConvert.SerializeObject( Selected ) );
      writer.SetString( "restapi", Selected.RestApi );
      writer.SetString( "email", Selected.Email );
      return base.Write( writer );
    }

    protected override void RegisterInputParams( GH_InputParamManager pManager )
    {
    }

    protected override void RegisterOutputParams( GH_OutputParamManager pManager )
    {
      pManager.Register_GenericParam( "account", "A", "Selected account." );
    }

    public override void AddedToDocument( GH_Document document )
    {
      base.AddedToDocument( document );

      ExpireComponent = ( ) => this.ExpireSolution( true );

      Accounts = LocalContext.GetAllAccounts();
    }
    public override void AppendAdditionalMenuItems( ToolStripDropDown menu )
    {
      base.AppendAdditionalMenuItems( menu );
      int count = 0;

      foreach ( var account in Accounts )
      {
        string displayName = account.ServerName;
        displayName = displayName.Substring( 0, 10 );
        displayName += "... - " + account.Email.Substring( 0, 10 ) + "...";
        Menu_AppendItem( menu, count++ + ". " + displayName, ( sender, e ) =>
         {
           Selected = account;
           NickName = displayName;
           Rhino.RhinoApp.MainApplicationWindow.Invoke( ExpireComponent );
         }, true );
      }
    }

    protected override void SolveInstance( IGH_DataAccess DA )
    {
      if ( Selected == null )
      {
        this.AddRuntimeMessage( GH_RuntimeMessageLevel.Warning, "Right click the component and select an account." );
        return;
      }

      AddRuntimeMessage( GH_RuntimeMessageLevel.Remark, Selected.ServerName );

      DA.SetData( 0, Selected );
    }

    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        return Resources.Accounts;
      }
    }

    public override Guid ComponentGuid
    {
      get { return new Guid( "{958de333-1ad0-4989-acbe-f59329d5b569}" ); }
    }
  }
}

