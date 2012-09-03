

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ../build/release/Plugin.dll
COMPILE_TARGET = library


REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	-r:System


SOURCES = \
	Interfaces/IFuse.cs \
	Interfaces/IPlugin.cs \
	Interfaces/IPluginOptions.cs \
	Interfaces/IMediaEngine.cs \
	Interfaces/IMediaControls.cs \
	Interfaces/IVideoInfo.cs \
	Interfaces/IEngineTag.cs \
	AssemblyInfo.cs \
	Events.cs \
	DialogBase.cs \
	Dialogs.cs  \
	Utils.cs \
	Timer.cs \
	Communicator.cs \
	SeekBar.cs \
	DelegateQueue.cs \
	PagedList.cs \
	WebDownload.cs



all: $(ASSEMBLY) 


$(ASSEMBLY):
	mkdir -p $(dir $(ASSEMBLY))
	$(COMPILER_COMMAND) $(REFERENCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)


EXTRA_DIST = $(SOURCES)
CLEANFILES = $(ASSEMBLY)


include $(top_srcdir)/Makefile.include