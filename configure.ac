AC_PREREQ([2.54])
AC_INIT([Fuse], [0.2])
AM_INIT_AUTOMAKE([foreign])
AM_PROG_LIBTOOL
AC_PROG_INSTALL




AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi




AC_PATH_PROG(GMCS, gmcs, no)
if test "x$GMCS" = "xno"; then
        AC_MSG_ERROR([gmcs Not found])
fi


PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0])
PKG_CHECK_MODULES([GECKO_SHARP_20], [gecko-sharp-2.0])
PKG_CHECK_MODULES([TAGLIB_SHARP], [taglib-sharp])
PKG_CHECK_MODULES(gstreamer, [gstreamer-0.10 >= 0.10])

AC_SUBST(gstreamer_CFLAGS)
AC_SUBST(gstreamer_LIBS)



AC_ARG_VAR(FUSE_DIR, The path that holds all the user data)
AC_SUBST(FUSE_DIR)



AC_CONFIG_FILES([

Makefile
Fuse/Makefile
Fuse/fuse

Plugin/Makefile
Plugin.Library/Makefile
Plugin.News/Makefile
Plugin.Theatre/Makefile
Plugin.TrayIcon/Makefile

GStreamerPlaybin/Makefile
MediaEngine.GStreamer/Makefile

RssReader/Makefile
Nini/Makefile

])

AC_OUTPUT

echo "Fuse is ready to be compiled."
