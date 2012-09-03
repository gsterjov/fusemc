

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 "-main:Fuse.FuseApp"
ASSEMBLY = ../build/release/Fuse.exe
COMPILE_TARGET = exe


REFERENCES = \
	$(GTK_SHARP_20_LIBS) \
	-r:System \
	-r:../build/release/Plugin.dll \
	-r:../build/release/Nini.dll

RESOURCES = \
	-resource:../pixmaps/fuse-tray.png \
	-resource:../pixmaps/fuse-logo.png 

SOURCES = \
	Windows/MainWindow.cs \
	Windows/ExceptionWindow.cs \
	Windows/WarningWindow.cs \
	Windows/PluginsWindow.cs \
	Windows/EnginesWindow.cs \
	Widgets/Controls.cs \
	Widgets/StatusBar.cs \
	Widgets/MainMenu.cs \
	Extension.cs \
	MediaEngine.cs \
	Plugin.cs \
	CommandLineParser.cs \
	Config.cs \
	Fuse.cs



WRAPPER = ../build/release/fuse

	
all: $(ASSEMBLY)


$(ASSEMBLY):
	mkdir -p $(dir $(ASSEMBLY))
	$(COMPILER_COMMAND) $(REFERENCES) $(RESOURCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)



bin_SCRIPTS = $(WRAPPER)


$(WRAPPER): fuse
	mkdir -p ../build/release/
	cp '$<' '$@'


EXTRA_DIST = $(SOURCES) ./fuse.in ../pixmaps/fuse-tray.png ../pixmaps/fuse-logo.png
CLEANFILES = $(ASSEMBLY) $(WRAPPER)


include $(top_srcdir)/Makefile.include