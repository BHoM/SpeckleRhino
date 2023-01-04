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
using Rhino.Input.Custom;
using Rhino.UI;

namespace SpeckleRhino
{
    [System.Runtime.InteropServices.Guid("8C8930B3-637C-4DE0-8D42-5B109171B94D")]
    public class SpecklePanelCommand : Command
    {

        public SpecklePanelCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static SpecklePanelCommand Instance
        {
            get; private set;
        }

        public override string EnglishName
        {
            get { return "SpecklePanel"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var panel_id = SpeckleRhinoUserControl.PanelId;
            var visible = Panels.IsPanelVisible(panel_id);

            var prompt = visible
              ? "Speckle panel is visible."
              : "Speckle panel is hidden.";

            var go = new GetOption();
            go.SetCommandPrompt(prompt);
            var hide_index = go.AddOption("Hide");
            var show_index = go.AddOption("Show");
            var toggle_index = go.AddOption("Toggle");

            go.Get();
            if (go.CommandResult() != Result.Success)
                return go.CommandResult();

            var option = go.Option();
            if (null == option)
                return Result.Failure;

            var index = option.Index;

            if (index == hide_index)
            {
                if (visible)
                    Panels.ClosePanel(panel_id);
            }
            else if (index == show_index)
            {
                if (!visible)
                    Panels.OpenPanel(panel_id);
            }
            else if (index == toggle_index)
            {
                if (visible)
                    Panels.ClosePanel(panel_id);
                else
                    Panels.OpenPanel(panel_id);
            }

            return Result.Success;
        }
    }
}
