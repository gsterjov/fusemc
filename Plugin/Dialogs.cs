/*

	Copyright (c)  Goran Sterjov

    This file is part of the Fuse Project.

    Fuse is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    Fuse is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Fuse; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/


using System;
using Gtk;

namespace Fuse
{
	
	/// <summary>
	/// A class for conveniently creating dialogs.
	/// </summary>
	public abstract class Dialogs
	{
		
		// load the next dialog with the last folder opened
		static string last_folder;
		
		
		/// <summary>
		/// Create a dialog to choose a folder.
		/// </summary>
		public static string ChooseFolder ()
		{
			string[] ret = ChooseDialog (FileChooserAction.SelectFolder, "Select Folder", false);
			if (ret != null && ret.Length > 0)
				return ret[0];
			return null;
		}
		
		
		/// <summary>
		/// Create a dialog to choose files.
		/// </summary>
		public static string[] ChooseFiles ()
		{
			return ChooseDialog (FileChooserAction.Open, "Select Files", true);
		}
		
		
		
		/// <summary>
		/// Create a dialog to choose a file with a custom header.
		/// </summary>
		public static string ChooseFile (string title)
		{
			string[] ret = ChooseDialog (FileChooserAction.Open, title, false);
			if (ret != null && ret.Length > 0)
				return ret[0];
			return null;
		}
		
		
		
		// creates a custom choose dialog
		static string[] ChooseDialog (FileChooserAction action, string title, bool many)
		{
			FileChooserDialog dialog = new FileChooserDialog (title, null, action);
			
			if (last_folder != null)
				dialog.SetCurrentFolder (last_folder);
			if (many)
				dialog.SelectMultiple = true;
			
			dialog.AddButton (Stock.Cancel, ResponseType.Cancel);
			dialog.AddButton (Stock.Ok, ResponseType.Ok);
			
			int response = dialog.Run ();
			string[] ret = null;
			
			if ((ResponseType) response == ResponseType.Ok)
				ret = dialog.Filenames;
			if ((ResponseType) response == ResponseType.Cancel)
				ret = null;
			
			last_folder = dialog.CurrentFolder;
			dialog.Destroy ();
			
			if(ret != null && ret.Length == 0) ret = null;
			return ret;
		}
		
	}
}
