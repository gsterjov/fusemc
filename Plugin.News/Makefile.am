

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
PLUGIN = ../build/release/plugins/Plugin.News.dll
COMPILE_TARGET = library



REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	$(GECKO_SHARP_20_LIBS) \
	-r:System \
	-r:System.Data \
	-r:Mono.Data.SqliteClient \
	-r:../build/release/Plugin.dll \
	-r:../build/release/RssReader.dll \
	-r:../build/release/Nini.dll


RESOURCES = -resource:./template.html 


SOURCES = \
	Windows/AddWindow.cs \
	Windows/NewsPopup.cs \
	Widgets/TopBar.cs \
	Widgets/News.cs \
	Widgets/NewsViewer.cs \
	Widgets/FeedContextMenu.cs \
	AssemblyInfo.cs \
	MainPage.cs \
	DataManager.cs \
	Feed.cs \
	Item.cs \
	Config.cs




all: $(PLUGIN)


$(PLUGIN):
	mkdir -p $(dir $(PLUGIN))
	$(COMPILER_COMMAND) $(REFERENCES) $(RESOURCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)



EXTRA_DIST = $(SOURCES) ./template.html
CLEANFILES = $(PLUGIN)



include $(top_srcdir)/Makefile.include
