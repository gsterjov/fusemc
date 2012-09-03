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
	/// The window in which the user selects media engines.
	/// </summary>
	public class EnginesWindow : DialogBase
	{
		FuseApp fuse;
		
		ListStore store = new ListStore (typeof (MediaEngine), typeof (string));
		MediaEngine engine;
		
		ComboBox combo;
		
		// content labels
		Label title = new Label ();
		Label version = new Label ();
		Label description = new Label ();
		Label author = new Label ();
		Label website = new Label ();
		
		
		
		// creates the media engine window
		public EnginesWindow (FuseApp fuse) : base (fuse.Window, "Media Engines")
		{
			this.fuse = fuse;
			
			// main widgets
			VBox backbone = new VBox (false, 2);
			backbone.BorderWidth = 10;
			
			Table details = new Table (5, 4, false);
			details.ColumnSpacing = 5;
			combo = new ComboBox (store);
			
			Button cancel = new Button (Stock.Cancel);
			Button save = new Button (Stock.Save);
			HBox button_box = new HBox (false, 2);
			
			
			// basic setup
			button_box.PackEnd (save);
			button_box.PackEnd (cancel);
			
			CellRendererText renderer = new CellRendererText ();
			combo.PackStart (renderer, true);
			combo.AddAttribute (renderer, "text", 1);
			
			
			
			// header labels
			Label instruction = new Label ("Select a media engine from the list below:");
			Label title_header = new Label ();
			Label version_header = new Label ();
			Label description_header = new Label ();
			Label author_header = new Label ();
			Label website_header = new Label ();
			
			
			// add header content and styles
			title_header.Markup = "<small><b>Title:</b></small>";
			version_header.Markup = "<small><b>Version:</b></small>";
			description_header.Markup = "<small><b>Description:</b></small>";
			author_header.Markup = "<small><b>Author:</b></small>";
			website_header.Markup = "<small><b>Website:</b></small>";
			
			
			// change label alignment
			title_header.Xalign = 0;
			version_header.Xalign = 0;
			description_header.Xalign = 0;
			author_header.Xalign = 0;
			website_header.Xalign = 0;
			
			title.Xalign = 0;
			version.Xalign = 0;
			description.Xalign = 0;
			author.Xalign = 0;
			website.Xalign = 0;
			
			
			// attach the labels to the details table
			details.Attach (title_header, 0, 1, 0, 1);
			details.Attach (version_header, 0, 1, 1, 2);
			details.Attach (author_header, 0, 1, 2, 3);
			details.Attach (website_header, 0, 1, 3, 4);
			details.Attach (description_header, 0, 1, 4, 5);
			
			details.Attach (title, 1, 2, 0, 1);
			details.Attach (version, 1, 2, 1, 2);
			details.Attach (author, 1, 2, 2, 3);
			details.Attach (website, 1, 2, 3, 4);
			details.Attach (description, 1, 2, 4, 5);
			
			
			// widget events
			combo.Changed += combo_changed;
			save.Clicked += save_clicked;
			cancel.Clicked += cancel_clicked;
			
			
			// add main widgets to the backbone
			backbone.PackStart (instruction, false, false, 5);
			backbone.PackStart (combo, false, false, 0);
			backbone.PackStart (details, false, false, 5);
			backbone.PackStart (button_box, false, false, 0);
			
			this.Resizable = false;
			this.SkipTaskbarHint = true;
			this.SkipPagerHint = true;
			this.Add (backbone);
			
			loadEngineList ();
			
			
			
			TreeIter first_iter;
			bool not_empty = combo.Model.GetIterFirst (out first_iter);
			combo.Sensitive = not_empty;
			if (not_empty) combo.SetActiveIter (first_iter);
			
			
			// select the previously selected engine
			store.Foreach (delegate (TreeModel model, TreePath path, TreeIter iter) {
				MediaEngine engine = (MediaEngine) model.GetValue (iter, 0);
				if (engine.Path == fuse.ChosenEngine)
				{
					combo.SetActiveIter (iter);
					return true;
				}
				return false;
			});
		}
		
		
		
		/// <summary>
		/// The selected media engine.
		/// </summary>
		public MediaEngine Engine
		{
			get{ return engine; }
		}
		
		
		
		// loads all the available media engines into the combo box
		void loadEngineList ()
		{
			string dir = AppDomain.CurrentDomain.BaseDirectory;
			if (dir.Length == 0) return;
			dir = System.IO.Path.Combine (dir, "plugins");
			
			foreach (string file in System.IO.Directory.GetFiles (dir, "*.dll"))
			{
				MediaEngine engine = new MediaEngine (file);
				if (engine.Load ())
					store.AppendValues (engine, engine.Instance.Name);
			}
		}
		
		
		
		
		// when the user has selected a media engine from the combo box
		void combo_changed (object o, EventArgs args) {
			TreeIter iter;
			if (combo.GetActiveIter (out iter)) {
				MediaEngine engine = (MediaEngine) store.GetValue (iter, 0);
				this.engine = engine;
				
				title.Markup = "<small>" + engine.Instance.Name + "</small>";
				version.Markup = "<small>" + engine.Instance.Version + "</small>";
				description.Markup = "<small>" + engine.Instance.Description + "</small>";
				author.Markup = "<small>" + engine.Instance.Author + "</small>";
				website.Markup = "<small>" + engine.Instance.Website + "</small>";
			}
		}
		
		
		// when the user saves their preference
		void save_clicked (object o, EventArgs args) {
			fuse.Controls.Engine = this.engine;
			fuse.ChosenEngine = engine.Path;
			
			this.Destroy ();
			retVal = 1;
		}
		
		
		// the user canceled the selection
		void cancel_clicked (object o, EventArgs args) {
			this.Destroy ();
			retVal = 0;
		}
		
	}
}
