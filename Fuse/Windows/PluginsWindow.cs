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
using System.Collections.Generic;
using Gtk;
using Fuse.Interfaces;

namespace Fuse
{
	
	/// <summary>
	/// The window in which the user selects plugins.
	/// </summary>
	public class PluginsWindow : DialogBase, IPluginOptions
	{
		FuseApp fuse;
		List <Plugin> plugin_list = new List <Plugin> ();
		
		// user interface widgets
		ListStore store = new ListStore (typeof (Plugin));
		TreeView tree = new TreeView ();
		Notebook pages = new Notebook ();
		
		Label name = new Label ();
		Label description = new Label ();
		Label version = new Label ();
		Label author = new Label ();
		Label website = new Label ();
		
		
		
		// creates the plugins user interface
		public PluginsWindow (FuseApp fuse, List <Plugin> plugin_list) : base (fuse.Window, "Plugins")
		{
			this.fuse = fuse;
			this.plugin_list = plugin_list;
			
			Frame frame = new Frame ();
			frame.Add (tree);
			frame.ShadowType = ShadowType.In;
			
			HBox backbone = new HBox (false, 5);
			backbone.PackStart (frame, false, false, 0);
			backbone.PackStart (pages, true, true, 0);
			
			
			// tree layout
			tree.Model = store;
			CellRendererToggle crt = new CellRendererToggle ();
			tree.AppendColumn ("Plugin", new CellRendererText (), new TreeCellDataFunc (renderPluginName));
			tree.AppendColumn ("Enabled", crt, new TreeCellDataFunc (renderPluginEnabled));
			
			tree.Selection.Changed += plugin_selected;
			crt.Toggled += plugin_toggled;
			
			
			// overview
			VBox overview = new VBox (false, 0);
			overview.PackStart (name, false, false, 0);
			overview.PackStart (description, false, false, 0);
			overview.PackStart (version, false, false, 0);
			overview.PackStart (author, false, false, 0);
			overview.PackStart (website, false, false, 0);
			pages.AppendPage (overview, new Label ("Overview"));
			
			
			loadPluginList ();
			
			backbone.BorderWidth = 10;
			this.Resize (500, 200);
			this.SkipPagerHint = true;
			this.SkipTaskbarHint = true;
			this.Add (backbone);
		}
		
		
		// renders the plugin name into the tree cell
		void renderPluginName (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Plugin plugin = (Plugin) store.GetValue (iter, 0);
			(cell as CellRendererText).Text = plugin.Instance.Name;
		}
		
		// renders the checkbox depending on whether or not the plugin is enabled
		void renderPluginEnabled (TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
		{
			Plugin plugin = (Plugin) store.GetValue (iter, 0);
			(cell as CellRendererToggle).Active = plugin.Enabled;
		}
		
		
		// when a user selects a plugin from the list
		void plugin_selected (object o, EventArgs args)
		{
			TreeIter iter;
			tree.Selection.GetSelected (out iter);
			Plugin plugin = (Plugin) store.GetValue (iter, 0);
			
			name.Markup = "<b>" + plugin.Instance.Name + "</b>";
			version.Text = "Version  " + plugin.Instance.Version;
			description.Text = plugin.Instance.Description;
			author.Text = plugin.Instance.Author;
			website.Text = plugin.Instance.Website;
			
			if (pages.NPages == 2)
				pages.RemovePage (1);
			
			plugin.Instance.InitiateOptions (this);
		}
		
		// when a user enables the plugin from the list
		void plugin_toggled (object o, ToggledArgs args)
		{
			TreeIter iter;
			store.GetIter (out iter, new TreePath (args.Path));
			Plugin plugin = (Plugin) store.GetValue (iter, 0);
			plugin.Enabled = !plugin.Enabled;
			
			if (plugin.Enabled)
				plugin.Instance.Initiate (fuse);
			else
				plugin.Instance.Deinitiate (fuse);
		}
		
		
		// loads all the available plugins into the treeview
		void loadPluginList ()
		{
			string dir = AppDomain.CurrentDomain.BaseDirectory;
			if (dir.Length == 0) return;
			
			dir = System.IO.Path.Combine (dir, "plugins");
			if (!System.IO.Directory.Exists (dir)) return;
			
			
			foreach (string file in System.IO.Directory.GetFiles (dir, "*.dll"))
			{
				bool exists = false;
				foreach (Plugin plugin in plugin_list)
				{
					if (plugin.Path == file)
					{
						exists = true;
						store.AppendValues (plugin);
					}
				}
				
				if (!exists)
				{
					Plugin plugin = new Plugin (file);
					if (plugin.Load ())
					{
						store.AppendValues (plugin);
						plugin_list.Add (plugin);
					}
				}
			}
		}
		
		
		/// <summary>
		/// Adds a widget into the options area of the plugin window
		/// </summary>
		public void AddOptionsWidget (Widget widget)
		{
			pages.AppendPage (widget, new Label ("Options"));
			pages.ShowAll ();
		}
		
		
	}
}
