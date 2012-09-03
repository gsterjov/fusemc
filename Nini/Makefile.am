

COMPILER_COMMAND = gmcs
COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ../build/release/Nini.dll
COMPILE_TARGET = library


REFERENCES =  \
	-r:System \
	-r:System.Xml


SOURCES = \
	AssemblyInfo.cs \
	Config/AliasText.cs \
	Config/ArgvConfigSource.cs \
	Config/ConfigBase.cs \
	Config/ConfigCollection.cs \
	Config/ConfigSourceBase.cs \
	Config/DotNetConfigSource.cs \
	Config/IConfig.cs \
	Config/IConfigSource.cs \
	Config/IniConfig.cs \
	Config/IniConfigSource.cs \
	Config/RegistryConfigSource.cs \
	Config/XmlConfigSource.cs \
	Ini/IniDocument.cs \
	Ini/IniException.cs \
	Ini/IniItem.cs \
	Ini/IniReader.cs \
	Ini/IniSection.cs \
	Ini/IniSectionCollection.cs \
	Ini/IniWriter.cs \
	Util/ArgvParser.cs \
	Util/OrderedList.cs \
	Util/OrderedListEnumerator.cs



all: $(ASSEMBLY) 


$(ASSEMBLY):
	mkdir -p $(dir $(ASSEMBLY))
	$(COMPILER_COMMAND) $(REFERENCES) $(COMPILER_FLAGS) -target:$(COMPILE_TARGET) -out:$@ $(SOURCES)


EXTRA_DIST = $(SOURCES)
CLEANFILES = $(ASSEMBLY)


include $(top_srcdir)/Makefile.include