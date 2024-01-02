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

using Eto.Drawing;
using Eto.Forms;
using System;
using Newtonsoft.Json;

namespace SpeckleRhino
{
    public class EtoForm : Form
    {
        public WebView Wv { get; private set; }
        public bool IndexLoaded = false;

        string index;
        public EtoForm()
        {
            this.ClientSize = new Size(600, 600);
            Wv = new WebView();

            Wv.DocumentLoading += E_DocumentLoading;
            Wv.DocumentLoaded += E_DocumentLoaded;

            var layout = new DynamicLayout();
            layout.Padding = new Padding(0);
            layout.BeginHorizontal();
            layout.Add(Wv, true, true);
            layout.EndHorizontal();
            Content = layout;

        }

        private void E_DocumentLoaded(object sender, WebViewLoadedEventArgs e)
        {
            if (e.Uri.AbsolutePath == index) IndexLoaded = true;
        }

        public void SetWVUrl(string url)
        {
            index = url.Replace("\\", "/");
            Wv.Url = new Uri(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void E_DocumentLoading(object sender, WebViewLoadingEventArgs e)
        {
            Rhino.RhinoApp.WriteLine(e.Uri.ToString());
            Rhino.RhinoApp.WriteLine(index);

            if (e.Uri.AbsolutePath != index && IndexLoaded)
            {
                e.Cancel = true;

                var result = "";
                var deserializedObject = new TestObject();

                if (e.Uri.ToString().Contains("sayhi"))
                {
                    result = Wv.ExecuteScript("SayHi(\"Luis\"); return payload;");
                    deserializedObject = JsonConvert.DeserializeObject<TestObject>(result);
                }

                if (e.Uri.ToString().Contains("returndata"))
                {
                    result = Wv.ExecuteScript("ReturnData(1000); return payload;");
                    deserializedObject = JsonConvert.DeserializeObject<TestObject>(result);
                }

                Rhino.RhinoApp.WriteLine(deserializedObject.ReturnValue);

                foreach(var num in deserializedObject.Numbers)
                    Rhino.RhinoApp.Write("{0}{1}", num, ",");

                Rhino.RhinoApp.WriteLine();

            }

        }
    }
}

