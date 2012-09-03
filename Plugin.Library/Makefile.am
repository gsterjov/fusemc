

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
PLUGIN = ../build/release/plugins/Plugin.Library.dll
COMPILE_TARGET = library



REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	$(TAGLIB_SHARP_LIBS) \
	-r:System \
	-r:System.Xml \
	-r:System.Data \
	-r:System.Web \
	-r:Mono.Data.SqliteClient \
	-r:../build/release/Plugin.dll \
	-r:../build/release/Nini.dll


RESOURCES = -resource:../pixmaps/fuse-noart.png \
			-resource:../pixmaps/lyricwiki-logo.png \
			-resource:../pixmaps/audioscrobbler-logo.png \
			-resource:../pixmaps/lastfm-logo.png


SOURCES = \
	Windows/AddWindow.cs \
	Windows/MediaEdit.cs \
	Widgets/Library.cs \
	Widgets/OrganizerTree.cs \
	Widgets/TopBar.cs \
	Widgets/Progress.cs \
	Widgets/SortColumn.cs \
	Widgets/QuickLoad.cs \
	Widgets/BorderWidget.cs \
	Folders/FolderStore.cs \
	Folders/FolderTree.cs \
	Folders/FolderMonitor.cs \
	Folders/FolderContextMenu.cs \
	Folders/FolderDataManager.cs \
	Media/MediaStore.cs \
	Media/MediaContextMenu.cs \
	Media/MediaPopup.cs \
	Media/MediaTree.cs \
	Media/MediaDataManager.cs \
	MediaTypes/AudioCDMedia.cs \
	MediaTypes/FileMedia.cs \
	MediaTypes/FolderMedia.cs \
	MediaTypes/Media.cs \
	MediaTypes/PlaylistMedia.cs \
	MediaCollections/DynamicMedia.cs \
	MediaCollections/Folder.cs \
	MediaCollections/Playlist.cs \
	MediaCollections/MediaCollection.cs \
	Playlists/PlaylistStore.cs \
	Playlists/PlaylistTree.cs \
	Playlists/PlaylistContextMenu.cs \
	Playlists/PlaylistDataManager.cs \
	DynamicMedia/AudioCD.cs \
	DynamicMedia/DynamicTree.cs \
	DynamicMedia/MusicBrainzRelease.cs \
	InfoBar/AudioScrobbler/ArtistInfo/AlbumDetails/AlbumDetails.cs \
	InfoBar/AudioScrobbler/ArtistInfo/AlbumDetails/TrackInfo.cs \
	InfoBar/AudioScrobbler/ArtistInfo/SimilarArtists/SimilarArtist.cs \
	InfoBar/AudioScrobbler/ArtistInfo/SimilarArtists/SimilarArtistBox.cs \
	InfoBar/AudioScrobbler/ArtistInfo/SimilarArtists/SimilarArtists.cs \
	InfoBar/AudioScrobbler/ArtistInfo/SimilarTracks/SimilarTrack.cs \
	InfoBar/AudioScrobbler/ArtistInfo/SimilarTracks/SimilarTrackBox.cs \
	InfoBar/AudioScrobbler/ArtistInfo/SimilarTracks/SimilarTracks.cs \
	InfoBar/AudioScrobbler/ArtistInfo/TopAlbums/TopAlbum.cs \
	InfoBar/AudioScrobbler/ArtistInfo/TopAlbums/TopAlbumBox.cs \
	InfoBar/AudioScrobbler/ArtistInfo/TopAlbums/TopAlbums.cs \
	InfoBar/AudioScrobbler/ArtistInfo/TopTracks/TopTrack.cs \
	InfoBar/AudioScrobbler/ArtistInfo/TopTracks/TopTracks.cs \
	InfoBar/AudioScrobbler/ArtistInfo/ArtistBox.cs \
	InfoBar/AudioScrobbler/ArtistInfo/ArtistInfo.cs \
	InfoBar/AudioScrobbler/ArtistInfo/ArtistInfoHelp.cs \
	InfoBar/AudioScrobbler/Profile/Friends/Friend.cs \
	InfoBar/AudioScrobbler/Profile/Friends/FriendBox.cs \
	InfoBar/AudioScrobbler/Profile/Friends/Friends.cs \
	InfoBar/AudioScrobbler/Profile/RecommendedArtists/RecommendedArtist.cs \
	InfoBar/AudioScrobbler/Profile/RecommendedArtists/RecommendedArtists.cs \
	InfoBar/AudioScrobbler/Profile/TopArtists/TopArtist.cs \
	InfoBar/AudioScrobbler/Profile/TopArtists/TopArtistBox.cs \
	InfoBar/AudioScrobbler/Profile/TopArtists/TopArtists.cs \
	InfoBar/AudioScrobbler/Profile/Profile.cs \
	InfoBar/AudioScrobbler/Profile/ProfileInfo.cs \
	InfoBar/AudioScrobbler/Profile/ProfileOptions.cs \
	InfoBar/AudioScrobbler/Profile/Scrobbler.cs \
	InfoBar/AudioScrobbler/Profile/ScrobblerQueue.cs \
	InfoBar/LyricWiki/Album.cs \
	InfoBar/LyricWiki/AlbumBox.cs \
	InfoBar/LyricWiki/Lyrics.cs \
	InfoBar/LyricWiki/LyricsHelp.cs \
	InfoBar/LyricWiki/SearchArtist.cs \
	InfoBar/LyricWiki/SongLyrics.cs \
	InfoBar/Widgets/ContentButton.cs \
	InfoBar/Widgets/ContentButtonBox.cs \
	InfoBar/Widgets/ContentTabs.cs \
	InfoBar/Widgets/ImageBox.cs \
	InfoBar/Widgets/InfoButton.cs \
	InfoBar/Widgets/LinkLabel.cs \
	InfoBar/Widgets/PageNavigator.cs \
	InfoBar/Widgets/SearchBox.cs \
	InfoBar/Content.cs \
	InfoBar/InfoBar.cs \
	InfoBar/InfoPanel.cs \
	InfoBar/QueryInfo.cs \
	InfoBar/WebService.cs \
	Core.cs \
	Database.cs \
	Utils.cs \
	Loader.cs \
	Config.cs \
	Global.cs \
	AssemblyInfo.cs




all: $(PLUGIN)


$(PLUGIN):
	mkdir -p $(dir $(PLUGIN))
	$(COMPILER_COMMAND) $(REFERENCES) $(RESOURCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)



EXTRA_DIST = $(SOURCES) ../pixmaps/fuse-noart.png ../pixmaps/lyricwiki-logo.png ../pixmaps/audioscrobbler-logo.png ../pixmaps/lastfm-logo.png
CLEANFILES = $(PLUGIN)


include $(top_srcdir)/Makefile.include
