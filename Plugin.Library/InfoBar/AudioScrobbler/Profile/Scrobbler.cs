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
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;
using Gtk;

namespace Fuse.Plugin.Library.Info.AudioScrobbler.Profile
{
	
	
	public enum ScrobblerStatus { OK, ClientBanned, BadAuthentication, BadTimeStamp, BadSession, Failed, ConnectionFailed };
	
	
	/// <summary>
	/// Tracks the music played in audioscrobbler.
	/// </summary>
	public class Scrobbler
	{
		//TODO: this whole class needs more comments. rather complicated.
		
		private Timer timer = new Timer ();
		private Timer failed_timer = new Timer ();
		private int timeout;
		private int counter;
		private int failed_timeout = 1;
		private int failed_counter;
		private int hard_fail_counter;
		private string play_timestamp;
		
		private string current_playing_url;
		
		private string username;
		private string password;
		private string session_id;
		private string now_playing_url;
		private string submission_url;
		private bool online;
		private bool retrying;
		private ScrobblerStatus status;
		private ScrobblerQueue submit_queue = new ScrobblerQueue ();
		
		
		public delegate void FailedHandler (string reason);
		public delegate void FailedRetryHandler (int counter, int timeout);
		public delegate void StatusChangedHandler (ScrobblerStatus status);
		
		public event FailedHandler Failed;
		public event FailedRetryHandler FailedRetry;
		public event StatusChangedHandler StatusChanged;
		
		
		//audioscrobbler constants
		private const string CLIENT_ID = "fmc";
        private const string CLIENT_VERSION = "0.1";
        private const string POST_URL = "http://post.audioscrobbler.com/";
        private const string POST_VERSION = "1.2";
		
		
		
		public Scrobbler ()
		{
			timer.Elapsed += timer_count;
			failed_timer.Elapsed += failed_timer_count;
		}
		
		
		
		public bool Online { get{ return online; } }
		public bool Retrying { get{ return retrying; } }
		public string Username { get{ return username; } }
		public string Password { get{ return password; } }
		public ScrobblerStatus Status { get{ return status; } }
		
		
		
		//start recording activity
		public void Start ()
		{
			Global.Core.Fuse.MediaControls.MediaEngine.StateChanged += state_changed;
		}
		
		//stop everything
		public void Stop ()
		{
			Global.Core.Fuse.MediaControls.MediaEngine.StateChanged -= state_changed;
			timer.Stop ();
			failed_timer.Stop ();
			
			timeout = 0; counter = 0;
			failed_timeout = 1; failed_counter = 0;
		}
		
		
		
		//log in and wait for it to finish
		public void Login (string user, string pass, bool pass_hashed)
		{
			if (!pass_hashed)
				pass = md5sum (pass);
			
			threadLoad (delegate{ handshake (user, pass); });
		}
		
		
		
		//the engine state has changed
		private void state_changed (StateEventArgs args)
		{
			if (args.State == MediaStatus.Loaded)
			{
				Media media = Global.Core.Library.MediaTree.CurrentMedia;
				
				Thread thread = new Thread (delegate (){ nowPlaying (media); });
				thread.Start ();
				
				
				timeout = 240;
				if (media.Duration.TotalSeconds > 30)
				{
					int duration = (int) (media.Duration.TotalSeconds / 2);
					if (duration < timeout)
						timeout = duration;
				}
				
				play_timestamp = unixTimestamp ().ToString ();
			}
			
			else if (args.State == MediaStatus.Playing)
				timer.Start (1000);
			
			else if (args.State == MediaStatus.Paused)
				timer.Stop ();
			
			else
			{
				timer.Stop ();
				counter = 0;
				timeout = 0;
			}
		}
		
		
		
		//when the time is right submit the media file
		private void timer_count ()
		{
			counter++;
			if (counter < timeout)
				return;
			
			Media media = Global.Core.Library.MediaTree.CurrentMedia;
			
			Thread thread = new Thread (delegate (){ submit (media); });
			thread.Start ();
			
			timer.Stop ();
			counter = 0;
		}
		
		
		
		//handshake timer for when loggin in is failing
		private void failed_timer_count ()
		{
			failed_counter++;
			if (FailedRetry != null)
				FailedRetry (failed_counter, failed_timeout);
			
			if (failed_counter < failed_timeout)
				return;
			
			
			failed_timeout = failed_timeout*2;
			failed_counter = 0;
			
			Thread thread = new Thread (delegate (){ handshake (username, password); });
			thread.Start ();
			
			failed_timer.Stop ();
		}
		
		
		
		//throw a hard failed message
		private void throwFailed (string message)
		{
			this.status = ScrobblerStatus.Failed;
			string reason = null;
			
			if (message.StartsWith ("FAILED"))
				reason = message.Substring (7);
			
			if (Failed != null)
				Failed (reason);
		}
		
		
		
		//changes the status and throws the event
		private void changeStatus (ScrobblerStatus status)
		{
			this.status = status;
			Application.Invoke (delegate{
				if (StatusChanged != null)
					StatusChanged (status);
			});
		}
		
		
		
		//keep trying to login
		private void retryHandshake ()
		{
			if (failed_timeout > 128)
			{
				failed_timeout = 1;
				return;
			}
			
			failed_timer.Start (60000);
			Application.Invoke (delegate{
				if (FailedRetry != null)
					FailedRetry (failed_counter, failed_timeout);
			});
		}
		
		
		
		//handshake with audioscrobbler to log in
		private void handshake (string user, string pass)
		{
			retrying = failed_timeout <= 128;
			
			string timestamp = unixTimestamp ().ToString ();
			string auth_token = md5sum (pass + timestamp);
			
			
			string format = "{0}?hs=true&p={1}&c={2}&v={3}&u={4}&t={5}&a={6}";
			string handshake = String.Format (format,
			                                  POST_URL,
			                                  POST_VERSION,
			                                  CLIENT_ID,
			                                  CLIENT_VERSION,
			                                  HttpUtility.UrlEncode (user),
			                                  timestamp,
			                                  auth_token);
			
			
			string[] result = loadQuery (handshake, "GET");
			bool handled = false;
			
			
			if (result == null)
			{
				changeStatus (ScrobblerStatus.ConnectionFailed);
				retryHandshake ();
				return;
			}
			
			
			switch (result[0])
			{
				case "OK":
					session_id = result[1];
					now_playing_url = result[2];
					submission_url = result[3];
					this.username = user;
					this.password = pass;
					this.online = true;
					
					hard_fail_counter = 0;
					changeStatus (ScrobblerStatus.OK);
					handled = true;
					break;
					
				case "BANNED":
					changeStatus (ScrobblerStatus.ClientBanned);
					handled = true;
					break;
					
				case "BADAUTH":
					changeStatus (ScrobblerStatus.BadAuthentication);
					handled = true;
					break;
					
				case "BADTIME":
					changeStatus (ScrobblerStatus.BadTimeStamp);
					handled = true;
					break;
			}
			
			
			
			//a hard fail occured
			if (!handled)
			{
				throwFailed (result[0]);
				retryHandshake ();
			}
			
		}
		
		
		
		
		//sends out a now playing message
		private void nowPlaying (Media media)
		{
			if (hard_fail_counter > 3)
				return;
			
			if (string.IsNullOrEmpty (media.Artist) || string.IsNullOrEmpty (media.Title))
				return;
			
			
			string duration = Math.Ceiling (media.Duration.TotalSeconds).ToString ();
			string format = "?s={0}&a={1}&t={2}&b={3}&l={4}&n={5}&m=";
			
			string url = String.Format (format,
			                            session_id,
			                            HttpUtility.UrlEncode (media.Artist),
			                            HttpUtility.UrlEncode (media.Title),
			                            HttpUtility.UrlEncode (media.Album),
			                            duration,
			                            media.TrackNumber);
			
			url = now_playing_url + url;
			if (current_playing_url == url)
				return;
			
			current_playing_url = url;
			string[] result = loadQuery (url, "POST");
			
			
			if (result == null)
			{
				changeStatus (ScrobblerStatus.ConnectionFailed);
				
				hard_fail_counter++;
				
				if (hard_fail_counter > 3)
					handshake (username, password);
				
				return;
			}
			
			
			
			if (result[0] == "OK")
				hard_fail_counter = 0;
			
			else if (result[0] == "BADSESSION")
			{
				changeStatus (ScrobblerStatus.BadSession);
				handshake (username, password);
			}
			
		}
		
		
		
		//submits the current media playing
		private void submit (Media media)
		{
			if (string.IsNullOrEmpty (media.Artist) || string.IsNullOrEmpty (media.Title))
				return;
			
			submit_queue.Add (media, play_timestamp);
			
			if (hard_fail_counter > 3)
				return;
			
			
			string url = submit_queue.GetQuery (session_id, submission_url);
			string[] result = loadQuery (url, "POST");
			
			
			//check the results
			if (result == null)
			{
				changeStatus (ScrobblerStatus.ConnectionFailed);
				
				hard_fail_counter++;
				
				if (hard_fail_counter > 3)
					handshake (username, password);
				
				return;
			}
			
			
			
			//the possible results
			if (result[0] == "OK")
			{
				hard_fail_counter = 0;
				submit_queue.Save ();
			}
			else if (result[0] == "BADSESSION")
			{
				changeStatus (ScrobblerStatus.BadSession);
				handshake (username, password);
			}
			else
			{
				throwFailed (result[0]);
				hard_fail_counter++;
				
				if (hard_fail_counter > 3)
					handshake (username, password);
			}
			
		}
		
		
		
		//generates the unix timestamp
		private double unixTimestamp ()
		{
			DateTime origin = new DateTime (1970, 1, 1, 0, 0, 0, 0);
			TimeSpan diff = DateTime.UtcNow - origin;
			return Math.Floor (diff.TotalSeconds);
		}
		
		
		//create an md5 sum out of the password
		private string md5sum (string text)
		{
			MD5 sum = MD5.Create ();
			byte[] hash = sum.ComputeHash (Encoding.ASCII.GetBytes (text));
			
			return BitConverter.ToString(hash).Replace("-","").ToLower();
		}
		
		
		
		//make a worker thread
		private void threadLoad (EventHandler func)
		{
			//FIXME: thread initilization broken
			//Thread thread = new Thread (new ThreadStart (func));
			//thread.Start ();
		}
		
		
		
		//load the query in a separate thread
		private string[] loadQuery (string query, string method)
		{
			List <string> list = new List <string> ();
			
			Stream stream = null;
			
			HttpWebRequest request = null;
			HttpWebResponse response = null;
			
			//catch any errors. ugly, but works.
			try
			{
				request = (HttpWebRequest) HttpWebRequest.Create (query);
				request.Method = method;
				
				response = (HttpWebResponse) request.GetResponse ();
				stream = response.GetResponseStream ();
				
				StreamReader reader = new StreamReader (stream);
				
				while (!reader.EndOfStream)
					list.Add (reader.ReadLine ());
			}
			catch
			{ stream = null; }
			finally
			{
				if (response != null)
					response.Close ();
				
				if (stream != null)
				{
					stream.Close ();
					stream.Dispose ();
				}
			}
			
			return list.Count > 0 ? list.ToArray () : null;
		}
		
		
		
	}
	
	
	
}
