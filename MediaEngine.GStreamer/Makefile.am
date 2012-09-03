

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
PLUGIN = ../build/release/plugins/MediaEngine.GStreamer.dll
COMPILE_TARGET = library



REFERENCES =  \
	-r:System \
	-r:../build/release/Plugin.dll \
	-r:../build/release/GStreamerPlaybin.dll


SOURCES = \
	AssemblyInfo.cs \
	Engine.cs \
	VideoInfo.cs \
	Tag.cs




all: $(PLUGIN)


$(PLUGIN):
	mkdir -p $(dir $(PLUGIN))
	$(COMPILER_COMMAND) $(REFERENCES) $(RESOURCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)



EXTRA_DIST = $(SOURCES)
CLEANFILES = $(PLUGIN)



include $(top_srcdir)/Makefile.include
