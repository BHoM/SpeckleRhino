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
using System.Windows.Forms;
using CefSharp.WinForms;
using CefSharp;
using System.Reflection;
using System.IO;
using Rhino;
using System.Net;
using System.Diagnostics;

namespace SpeckleRhino
{
  /// <summary>
  /// This is the user control that is buried in the tabbed, docking panel.
  /// </summary>
  [System.Runtime.InteropServices.Guid( "5736E01B-1459-48FF-8021-AE8E71257795" )]
  public partial class SpeckleRhinoUserControl : UserControl
  {
    /// <summary>
    /// This gets called every time a new file is opened. 
    /// Therefore, we must init things here. It's the best way.
    /// </summary>
    public SpeckleRhinoUserControl( )
    {
      if( SpecklePlugIn.Store!=null)
        SpecklePlugIn.Store.Dispose();

      InitializeComponent();

      SpecklePlugIn.InitializeCef();
      SpecklePlugIn.InitializeChromium();

      SpecklePlugIn.Store = new Interop( SpecklePlugIn.Browser );

      SpecklePlugIn.Browser.RegisterAsyncJsObject( "Interop", SpecklePlugIn.Store );

      this.Controls.Add( SpecklePlugIn.Browser );
      
      // Set the user control property on our plug-in
      SpecklePlugIn.Instance.PanelUserControl = this;
    }

    /// <summary>
    /// Returns the ID of this panel.
    /// </summary>
    public static Guid PanelId
    {
      get
      {
        return typeof( SpeckleRhinoUserControl ).GUID;
      }
    }
  }
}


