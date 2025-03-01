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

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using System.Diagnostics;
using Rhino.Collections;

namespace SpeckleGrasshopper
{
  public class CreateLotsOfUserData : GH_Component
  {

    public CreateLotsOfUserData( )
      : base( "Create Lots of Custom User Data", "CLUD",
          "Creates a custom user dictionary using lists of keys and values.",
          "Speckle", "User Data Utils" )
    {
    }

    protected override void RegisterInputParams( GH_Component.GH_InputParamManager pManager )
    {
      pManager.AddTextParameter( "Keys", "K", "Keys to place in dictionary.", GH_ParamAccess.list );
      pManager.AddGenericParameter( "Values", "V", "Values to attach to keys.", GH_ParamAccess.list );
    }

    protected override void RegisterOutputParams( GH_Component.GH_OutputParamManager pManager )
    {
      pManager.AddGenericParameter( "User Data", "UD", "The user data as an Archivable Dictionary.", GH_ParamAccess.item );
    }

    protected override void SolveInstance( IGH_DataAccess DA )
    {
      var props = new ArchivableDictionary();

      List<string> m_key_list = new List<string>();
      List<object> m_value_list = new List<object>();

      if ( !DA.GetDataList( "Keys", m_key_list ) )
      {
        AddRuntimeMessage( GH_RuntimeMessageLevel.Warning, "No keys supplied." );
        return;
      }
      if ( !DA.GetDataList( "Values", m_value_list ) )
      {
        AddRuntimeMessage( GH_RuntimeMessageLevel.Warning, "No values supplied." );
        return;
      }

      try
      {
        ValidateKeys( m_key_list );
      } catch(Exception err)
      {
        AddRuntimeMessage( GH_RuntimeMessageLevel.Error, err.Message );
        return;
      }

      int N = Math.Min( m_key_list.Count, m_value_list.Count );

      for ( int i = 0; i < N; ++i )
      {
        var key = m_key_list[ i ];

        object ghInputProperty = m_value_list[ i ];

        if ( ghInputProperty == null )
        {
          props.Set( key, "undefined" );
          continue;
        }

        object valueExtract = ghInputProperty.GetType().GetProperty( "Value" ).GetValue( ghInputProperty, null );

        Debug.WriteLine( key + ": " + valueExtract.GetType().ToString() );

        GeometryBase geometry = getGeometryBase( valueExtract );

        if ( geometry != null )
        {
          props.Set( key, geometry );
          continue;
        }

        if ( valueExtract is double )
          props.Set( key, ( double ) valueExtract );

        if ( valueExtract is Int32 || valueExtract is Int64 || valueExtract is Int16 || valueExtract is int )
          props.Set( key, ( int ) valueExtract );

        if ( valueExtract is string )
          props.Set( key, ( string ) valueExtract );

        if ( valueExtract is bool )
          props.Set( key, ( bool ) valueExtract );

        if ( valueExtract is Vector3d )
          props.Set( key, ( Vector3d ) valueExtract );

        if ( valueExtract is Point3d )
          props.Set( key, ( Point3d ) valueExtract );

        if ( valueExtract is Line )
          props.Set( key, ( Line ) valueExtract );

        if ( ( valueExtract is Circle ) )
          props.Set( key, new ArcCurve( ( Circle ) valueExtract ) );

        if ( valueExtract is Interval )
          props.Set( key, ( Interval ) valueExtract );

        if ( valueExtract is UVInterval )
          props.Set( key, "UV Interval not supported." );

        if ( valueExtract is Plane )
          props.Set( key, ( Plane ) valueExtract );

        if ( valueExtract is ArchivableDictionary )
          props.Set( key, ( ArchivableDictionary ) valueExtract );
      }

      DA.SetData( 0, props );
    }

    private void ValidateKeys( List<string> m_key_list )
    {
      var set = new HashSet<string>( m_key_list );
      if ( set.Count < m_key_list.Count )
      {
        throw new Exception( "Duplicate key names found." );
      }

      foreach ( var key in m_key_list )
      {
        if ( key == "Type" || key == "type" )
          throw new Exception( "Using 'Type' or 'type' as a key name is not possible. Please use different name, for example 'familiyType'. Thanks!" );

        if ( key.Contains( "." ) )
          throw new Exception( "Dots in key names are not supported. Sorry!" );
      }
    }

    public GeometryBase getGeometryBase( object myObject )
    {
      if ( myObject is Rectangle3d ) return ( ( Rectangle3d ) myObject ).ToNurbsCurve();
      if ( myObject is Polyline ) return ( ( Polyline ) myObject ).ToNurbsCurve();
      if ( myObject is Box ) return ( ( Box ) myObject ).ToBrep();

      return myObject as GeometryBase;
    }


    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        return Properties.Resources.CreateUserData;
      }
    }

    public override Guid ComponentGuid
    {
      get { return new Guid( "{d15e0aaa-65be-4882-92a0-021405e32f7b}" ); }
    }
  }


}

