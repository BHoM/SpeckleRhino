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

using Rhino;
using Rhino.Commands;
using System.IO;
using System.Reflection;

namespace SpeckleRhino
{
    public class SpeckleCommand : Command
    {
        private WinForm TheForm;

        public string PathResources { get; set; }
        public string IndexPath { get; set; }

        public bool Init { get; set; } = false;

        public SpeckleCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static SpeckleCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "Speckle"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("The {0} command is under construction.", EnglishName);

            if (!Init)
            {
                TheForm = new WinForm();
                TheForm.TopMost = true;
                TheForm.AllowDrop = true;
                TheForm.ShowInTaskbar = true;
                TheForm.BringToFront();
                TheForm.Show();
                Init = true;
                return Result.Success;
            }
            else
            {
                TheForm.Show();
                return Result.Success;
            }
        }
    }
}
