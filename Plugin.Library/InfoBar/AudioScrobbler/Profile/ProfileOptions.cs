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
using System.Threading;
using Gtk;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	/// <summary>
	/// Shows options of the profile panel.
	/// </summary>
	public class ProfileOptions
	{
		
		private Profile main;
		private Scrobbler scrobbler = new Scrobbler ();
		
		
		//widgets
		private Button login_button = new Button ();
		private Label status_label = new Label ();
		private Label details_label = new Label ();
		private Entry username_entry = new Entry ();
		private Entry password_entry = new Entry ();
		private CheckButton auto_login = new CheckButton ("Automatically Login");
		
		private VBox box = new VBox (false, 0);
		
		private bool connecting;
		private string status_format = "<small>Status: <b>{0}</b></small>";
		
		
		
		public ProfileOptions (Profile main)
		{
			this.main = main;
			
			auto_login.Active = Global.Core.Config.AudioScrobbler.GetBoolean ("Auto Login", false);
			username_entry.Text = Global.Core.Config.AudioScrobbler.GetString ("Username", "");
			
			Table login_table = new Table (3, 2, false);
			
			login_table.Attach (new Label ("Username:"), 0, 1, 0, 1);
			login_table.Attach (new Label ("Password:"), 0, 1, 1, 2);
			login_table.Attach (username_entry, 1, 2, 0, 1);
			login_table.Attach (password_entry, 1, 2, 1, 2);
			login_table.Attach (login_button, 1, 2, 2, 3);
			
			login_table.ColumnSpacing = 5;
			password_entry.Visibility = false;
			
			username_entry.SetSizeRequest (100, -1);
			password_entry.SetSizeRequest (100, -1);
			
			
			VBox login_box = new VBox (false, 0);
			login_box.PackStart (login_table, false, false, 0);
			
			Alignment auto_align = new Alignment (1, 0.5f, 0, 0);
			auto_align.Add (auto_login);
			login_box.PackStart (auto_align, false, false, 0);
			
			
			Label title = new Label ();
			Label details1 = new Label ();
			Label details2 = new Label ();
			
			
			title.Markup = "<b><big><big>Last.fm Profile</big></big></b>";
			
			details1.Markup = "This panel displays information about your Last.fm profile";
			details2.Markup = "To start using this panel, log in below";
			
			details1.Wrap = true;
			details2.Wrap = true;
			
			
			status_label.Markup = String.Format (status_format, "Offline");
			login_button.Image = new Image (Stock.Connect, IconSize.Button);
			login_button.Label = "Sign In";
			
			
			box.PackStart (title, false, false, 0);
			box.PackStart (new HSeparator (), false, false, 5);
			
			
			Alignment login_align = new Alignment (0.5f, 0.5f, 0, 0);
			login_align.Add (login_box);
			
			VBox details_box = new VBox (false, 0);
			details_box.PackStart (details1, false, false, 0);
			details_box.PackStart (details2, false, false, 2);
			details_box.PackStart (login_align, false, false, 20);
			details_box.PackStart (status_label, false, false, 0);
			details_box.PackStart (details_label, false, false, 5);
			
			Alignment details_align = new Alignment (0.5f, 0.5f, 0, 0);
			details_align.Add (details_box);
			
			
			
			box.PackStart (details_align, true, true, 0);
			box.PackStart (new Image (null, "lastfm-logo.png"), false, false, 0);
			
			
			login_button.Clicked += login_clicked;
			username_entry.Activated += login_clicked;
			password_entry.Activated += login_clicked;
			auto_login.Toggled += auto_login_toggled;
			
			scrobbler.Failed += failed;
			scrobbler.FailedRetry += failed_retry;
			scrobbler.StatusChanged += status_changed;
			
			Global.Core.PluginLoaded += plugin_loaded;
		}
		
		
		
		/// <summary>The main display widget.</summary>
		public Widget DisplayWidget { get{ return box; } }
		
		/// <summary>The username of the profile.</summary>
		public string Username { get{ return scrobbler.Username; } }
		
		
		
		/// <summary>
		/// Sign the user out.
		///</summary>
		public void SignOut ()
		{
			scrobbler.Stop ();
			enableWidgets ();
			status_label.Markup = String.Format (status_format, "Offline");
			details_label.Markup = "";
			connecting = false;
		}
		
		
		
		//handshake with the audioscrobbler server
		private void signIn (bool pass_hashed)
		{
			connecting = true;
			
			status_label.Markup = String.Format (status_format, "Signing In");
			username_entry.Sensitive = false;
			password_entry.Sensitive = false;
			
			login_button.Image = new Image (Stock.Cancel, IconSize.Button);
			login_button.Label = "Cancel";
			
			string username = username_entry.Text;
			string password = pass_hashed ? Global.Core.Config.AudioScrobbler.GetString ("Password") : password_entry.Text;
			
			scrobbler.Login (username, password, pass_hashed);
		}
		
		
		
		
		//enable the login widgets
		private void enableWidgets ()
		{
			username_entry.Sensitive = true;
			password_entry.Sensitive = true;
			login_button.Image = new Image (Stock.Connect, IconSize.Button);
			login_button.Label = "Sign In";
			login_button.Sensitive = true;
			connecting = false;
		}
		
		
		//saves the auto login status
		private void saveAutoLogin ()
		{
			string username = auto_login.Active ? scrobbler.Username : "";
			string password = auto_login.Active ? scrobbler.Password : "";
			
			Global.Core.Config.AudioScrobbler.Set ("Auto Login", auto_login.Active);
			Global.Core.Config.AudioScrobbler.Set ("Username", username);
			Global.Core.Config.AudioScrobbler.Set ("Password", password);
			Global.Core.Config.Save ();
		}
		
		
		
		//the plugin has been completely loaded
		private void plugin_loaded (object o, EventArgs args)
		{
			if (auto_login.Active)
				signIn (true);
		}
		
		
		//log in to the service
		private void login_clicked (object o, EventArgs args)
		{
			if (connecting)
				SignOut ();
			else
				signIn (false);
		}
		
		
		
		//whether or not to automatically log in
		private void auto_login_toggled (object o, EventArgs args)
		{
			if (!auto_login.Active || scrobbler.Online)
				saveAutoLogin ();
		}
		
		
		
		//the scrobbler failed somewhere
		private void failed (string reason)
		{
			status_label.Markup = String.Format (status_format, "Server Failure");
		}
		
		
		//the current situation in the login retry
		private void failed_retry (int counter, int timeout)
		{
			int time_left = timeout - counter;
			
			if (time_left > 0)
			{
				string format = "<small>Attempting retry in {0} minutes</small>";
				details_label.Markup = String.Format (format, time_left);
			}
			else
			{
				status_label.Markup = String.Format (status_format, "Signing In");
				details_label.Markup = "";
			}
		}
		
		
		//the status of the scrobbler has changed
		private void status_changed (ScrobblerStatus status)
		{
			switch (status)
			{
				case ScrobblerStatus.BadAuthentication:
					status_label.Markup = String.Format (status_format, "Authentication Failed");
					enableWidgets ();
					break;
					
				case ScrobblerStatus.ClientBanned:
					status_label.Markup = String.Format (status_format, "Client Banned");
					details_label.Markup = "<small>Upgrade to a newer version of Fuse Media Centre</small>";
					break;
					
				case ScrobblerStatus.BadTimeStamp:
					status_label.Markup = String.Format (status_format, "Invalid Timestamp");
					details_label.Markup = "</small>Correct your system time</small>";
					enableWidgets ();
					break;
					
				case ScrobblerStatus.ConnectionFailed:
					status_label.Markup = String.Format (status_format, "Connection Failed");
					
					if (!scrobbler.Retrying)
						enableWidgets ();
					break;
					
				case ScrobblerStatus.OK:
					saveAutoLogin ();
					status_label.Markup = String.Format (status_format, "Online");
					
					login_button.Image = new Image (Stock.Disconnect, IconSize.Button);
					login_button.Label = "Sign Off";
					
					scrobbler.Start ();
					main.LoadProfile ();
					break;
			}
		}
		
		
	}
	
	
	
}
