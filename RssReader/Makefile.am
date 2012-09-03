

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ../build/release/RssReader.dll
COMPILE_TARGET = library


REFERENCES =  \
	-r:System \
	-r:System.Xml


SOURCES = \
	AssemblyInfo.cs \
	RssChannel.cs \
	RssFeed.cs \
	RssItem.cs



all: $(ASSEMBLY) 


$(ASSEMBLY):
	mkdir -p $(dir $(ASSEMBLY))
	$(COMPILER_COMMAND) $(REFERENCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)


EXTRA_DIST = $(SOURCES)
CLEANFILES = $(ASSEMBLY)


include $(top_srcdir)/Makefile.include