## Process this file with automake to produce Makefile.in


AM_CFLAGS = $(gstreamer_CFLAGS) -Wall -g

pkglib_LTLIBRARIES = libgstreamer_playbin.la
libgstreamer_playbin_la_SOURCES = main.c
libgstreamer_playbin_la_LDFLAGS = $(gstreamer_LIBS) -lgstinterfaces-0.10




COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ../build/release/GStreamerPlaybin.dll
COMPILE_TARGET = library



REFERENCES =  \
	$(GLIB_SHARP_20_LIBS)
	

SOURCES = \
	AssemblyInfo.cs \
	Engine.cs \
	Events.cs



all: $(ASSEMBLY) 


$(ASSEMBLY):
	mkdir -p $(dir $(ASSEMBLY))
	$(COMPILER_COMMAND) $(REFERENCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)


EXTRA_DIST = $(SOURCES) main.c
CLEANFILES = $(ASSEMBLY)

include $(top_srcdir)/Makefile.include