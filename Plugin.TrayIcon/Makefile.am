

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
PLUGIN = ../build/release/plugins/Plugin.TrayIcon.dll
COMPILE_TARGET = library



REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	-r:System \
	-r:../build/release/Plugin.dll

RESOURCES = \
	-resource:../pixmaps/fuse-tray.png

SOURCES = \
	AssemblyInfo.cs \
	Plugin.cs \
	TrayIcon.cs \
	TrayLib.cs \
	TrayPopup.cs




all: $(PLUGIN)


$(PLUGIN):
	mkdir -p $(dir $(PLUGIN))
	$(COMPILER_COMMAND) $(REFERENCES) $(RESOURCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)



EXTRA_DIST = $(SOURCES) ../pixmaps/fuse-tray.png
CLEANFILES = $(PLUGIN)



include $(top_srcdir)/Makefile.include
