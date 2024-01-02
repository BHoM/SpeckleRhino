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

using System.IO;
using CefSharp;
using Rhino.PlugIns;
using Rhino.UI;
using System.Reflection;
using CefSharp.WinForms;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpeckleRhino
{
  public class SpecklePlugIn : Rhino.PlugIns.PlugIn
  {

    public static Interop Store;
    public static ChromiumWebBrowser Browser;

    public SpecklePlugIn( )
    {
      Instance = this;
      SpeckleCore.SpeckleInitializer.Initialize();
      SpeckleCore.LocalContext.Init();
    }

    ///<summary>Gets the only instance of the TestEtoWebkitPlugIn plug-in.</summary>
    public static SpecklePlugIn Instance
    {
      get; private set;
    }

    /// <summary>
    /// The tabbed dockbar user control
    /// </summary>
    public SpeckleRhinoUserControl PanelUserControl { get; set; }

    protected override LoadReturnCode OnLoad( ref string errorMessage )
    {
      var panel_type = typeof( SpeckleRhinoUserControl );
      Panels.RegisterPanel( this, panel_type, "Speckle", SpeckleRhino.Properties.Resources.Speckle );

      return base.OnLoad( ref errorMessage );
    }

    protected override void OnShutdown( )
    {
      if ( Browser != null )
        Browser.Dispose();
      Cef.Shutdown();

      Store?.Dispose();
      SpecklePlugIn.Instance.PanelUserControl?.Dispose();
      base.OnShutdown();
    }

    public static void InitializeCef( )
    {
      if ( Cef.IsInitialized ) return;

      Cef.EnableHighDPISupport();

      var assemblyLocation = Assembly.GetExecutingAssembly().Location;
      var assemblyPath = Path.GetDirectoryName( assemblyLocation );
      var pathSubprocess = Path.Combine( assemblyPath, "CefSharp.BrowserSubprocess.exe" );
      CefSharpSettings.LegacyJavascriptBindingEnabled = true;
      var settings = new CefSettings
      {
        BrowserSubprocessPath = pathSubprocess
      };

#if WINR5
      //Not needed in Rhino 6
      settings.CefCommandLineArgs.Add( "disable-gpu", "1" );
#endif

      settings.CefCommandLineArgs.Add( "allow-file-access-from-files", "1" );
      settings.CefCommandLineArgs.Add( "disable-web-security", "1" );
      Cef.Initialize( settings );
    }

    public static void InitializeChromium( )
    {
      if ( Browser != null && !Browser.IsDisposed ) return;

#if DEBUG
      HttpWebRequest request = ( HttpWebRequest ) WebRequest.Create( @"http://localhost:9090/" );
      request.Timeout = 100;
      request.Method = "HEAD";
      HttpWebResponse response;
      try
      {
        response = ( HttpWebResponse ) request.GetResponse();
        var copy = response;
        Browser = new ChromiumWebBrowser( @"http://localhost:9090/" );
      }
      catch ( WebException )
      {
        //Browser = new ChromiumWebBrowser(@"http://localhost:9090/");
        // IF DIMITRIE ON PARALLELS
        Browser = new ChromiumWebBrowser( @"http://10.211.55.2:9090/" );
      }
#else
      var path = Directory.GetParent( Assembly.GetExecutingAssembly().Location );
      Debug.WriteLine( path, "SPK" );

      var indexPath = string.Format( @"{0}\app\index.html", path );

      if ( !File.Exists( indexPath ) )
        Debug.WriteLine( "Speckle for Rhino: Error. The html file doesn't exists : {0}", "SPK" );

      indexPath = indexPath.Replace( "\\", "/" );

      Browser = new ChromiumWebBrowser( indexPath );
#endif

      // Allow the use of local resources in the browser
      Browser.BrowserSettings = new BrowserSettings
      {
        FileAccessFromFileUrls = CefState.Enabled,
        UniversalAccessFromFileUrls = CefState.Enabled
      };

      Browser.Dock = DockStyle.Fill;
    }

  }
}

