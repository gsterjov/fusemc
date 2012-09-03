

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
PLUGIN = ../build/release/plugins/Plugin.Theatre.dll
COMPILE_TARGET = library



RESOURCES = -resource:../pixmaps/fuse-logo-theatre.png 


REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	-r:System \
	-r:System.Xml \
	-r:System.Data \
	-r:Mono.Data.SqliteClient \
	-r:../build/release/Plugin.dll \
	-r:../build/release/Nini.dll


SOURCES = \
	Widgets/Fullscreen.cs \
	Widgets/FullscreenControls.cs \
	Widgets/Theatre.cs \
	Widgets/TopBar.cs \
	Widgets/VideoWidget.cs \
	Widgets/VideoContextMenu.cs \
	Global.cs \
	Core.cs \
	DataManager.cs \
	Media.cs \
	Config.cs \
	AssemblyInfo.cs




all: $(PLUGIN)


$(PLUGIN):
	mkdir -p $(dir $(PLUGIN))
	$(COMPILER_COMMAND) $(REFERENCES) $(RESOURCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)



EXTRA_DIST = $(SOURCES)
CLEANFILES = $(PLUGIN)



include $(top_srcdir)/Makefile.include
