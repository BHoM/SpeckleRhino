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

using Newtonsoft.Json;
using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using Rhino;
using System.Reflection;
using System.IO;
using System.Net;

namespace SpeckleRhino
{
    public partial class WinForm : Form
    {
        public ChromiumWebBrowser chromeBrowser;
        public Interop Store;

        public WinForm()
        {
            InitializeComponent();
            // Start the browser after initialize global component
            InitializeChromium();
        }

        public void InitializeChromium()
        {
            Cef.EnableHighDPISupport();

            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyPath = Path.GetDirectoryName(assemblyLocation);
            string pathSubprocess = Path.Combine(assemblyPath, "CefSharp.BrowserSubprocess.exe");

            CefSettings settings = new CefSettings();
            settings.LogSeverity = LogSeverity.Verbose;
            settings.LogFile = "ceflog.txt";
            settings.BrowserSubprocessPath = pathSubprocess;
            settings.CefCommandLineArgs.Add("disable-gpu", "1");


            // Initialize cef with the provided settings
            Cef.Initialize(settings);

            // Create a browser component. 
            // Change the below to wherever your webpack ui server is running.
            //chromeBrowser = new ChromiumWebBrowser(@"http://10.211.55.2:9090/");
            //chromeBrowser = new ChromiumWebBrowser(@"http://localhost:9090/");
            // Add it to the form and fill it to the form window.
            
            // IF DEBUG
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://localhost:9090/");
            request.Timeout = 100;
            request.Method = "HEAD";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // IF NORMAL PEOPLE
                    chromeBrowser = new ChromiumWebBrowser(@"http://localhost:9090/");
                }
            }
            catch (WebException)
            {
                // IF DIMITRIE
                chromeBrowser = new ChromiumWebBrowser(@"http://10.211.55.2:9090/");
            }

            //IF RELEASE
            // TODO

            this.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;

            // Allow the use of local resources in the browser
            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;

            Store = new Interop(chromeBrowser, this);

            chromeBrowser.RegisterAsyncJsObject("Interop", Store);
        }

        private void WinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Action ShutDownCef = () =>
            {
                Cef.Shutdown();
            };

            Rhino.RhinoApp.MainApplicationWindow.Invoke(ShutDownCef);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            base.OnFormClosing(e);
        }
    }
}

