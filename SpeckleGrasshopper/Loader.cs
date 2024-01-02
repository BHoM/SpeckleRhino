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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Interop;
using Grasshopper.Kernel;

namespace SpeckleGrasshopper
{
  public class Loader : GH_AssemblyPriority
  {
    System.Timers.Timer loadTimer;
    static bool MenuHasBeenAdded = false;

    public Loader( ) { }

    public override GH_LoadingInstruction PriorityLoad( )
    {
      loadTimer = new System.Timers.Timer( 500 );
      loadTimer.Start();
      loadTimer.Elapsed += AddSpeckleMenu;
      return GH_LoadingInstruction.Proceed;
    }

    private void AddSpeckleMenu( object sender, ElapsedEventArgs e )
    {
      if ( Grasshopper.Instances.DocumentEditor == null ) return;

      if ( MenuHasBeenAdded )
      {
        loadTimer.Stop();
        return;
      }

      var speckleMenu = new ToolStripMenuItem( "Speckle" );
      speckleMenu.DropDown.Items.Add( "Speckle Account Manager", null, ( s, a ) =>
      {
        var signInWindow = new SpecklePopup.SignInWindow( false );
        var helper = new System.Windows.Interop.WindowInteropHelper( signInWindow );
        helper.Owner = Rhino.RhinoApp.MainWindowHandle();
        signInWindow.Show();
      } );

      speckleMenu.DropDown.Items.Add( new ToolStripSeparator() );

      speckleMenu.DropDown.Items.Add( "Speckle Home", null, ( s, a ) =>
      {
        Process.Start( @"https://speckle.works" );
      } );

      speckleMenu.DropDown.Items.Add( "Speckle Documentation", null, ( s, a ) =>
      {
        Process.Start( @"https://speckle.works/docs/essentials/start" );
      } );

      speckleMenu.DropDown.Items.Add( "Speckle Forum", null, ( s, a ) =>
      {
        Process.Start( @"https://discourse.speckle.works" );
      } );

      try
      {
        var mainMenu = Grasshopper.Instances.DocumentEditor.MainMenuStrip;
        Grasshopper.Instances.DocumentEditor.Invoke( new Action( ( ) =>
        {
          mainMenu.Items.Insert( mainMenu.Items.Count - 2, speckleMenu );
        } ) );
        MenuHasBeenAdded = true;
        loadTimer.Stop();
      }
      catch ( Exception err )
      {
        Debug.WriteLine( err.Message );
      }
    }
  }
}

