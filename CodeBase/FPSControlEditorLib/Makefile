
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/FPSControlEditorLib.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../FPSControlLib/bin/Debug/FPSControlLib.dll
BUILD_DIR = bin/Debug

FPSCONTROLEDITORLIB_DLL_MDB_SOURCE=bin/Debug/FPSControlEditorLib.dll.mdb
FPSCONTROLEDITORLIB_DLL_MDB=$(BUILD_DIR)/FPSControlEditorLib.dll.mdb
UNITYEDITOR_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll
UNITYENGINE_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll
NUNIT_FRAMEWORK_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/nunit.framework.dll

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/FPSControlEditorLib.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../FPSControlLib/bin/Release/FPSControlLib.dll
BUILD_DIR = bin/Release

FPSCONTROLEDITORLIB_DLL_MDB=
UNITYEDITOR_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll
UNITYENGINE_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll
NUNIT_FRAMEWORK_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/nunit.framework.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FPSCONTROLEDITORLIB_DLL_MDB) \
	$(UNITYEDITOR_DLL) \
	$(UNITYENGINE_DLL) \
	$(NUNIT_FRAMEWORK_DLL)  

LINUX_PKGCONFIG = \
	$(FPSCONTROLEDITORLIB_PC)  


RESGEN=resgen2

UNITYEDITOR_DLL = $(BUILD_DIR)/UnityEditor.dll
UNITYENGINE_DLL = $(BUILD_DIR)/UnityEngine.dll
NUNIT_FRAMEWORK_DLL = $(BUILD_DIR)/nunit.framework.dll
FPSCONTROLEDITORLIB_PC = $(BUILD_DIR)/fpscontroleditorlib.pc

FILES = \
	AssemblyInfo.cs \
	Core/Utils/Knobs.cs \
	Core/FPSControlEditorModule.cs \
	Core/UnavailableModule.cs \
	Main\ Editor/FPSControlMainEditor.cs \
	Login/LoginModule.cs \
	Footstep\ Control/FootstepControlModule.cs \
	Footstep\ Control/FootstepControlNameDialogue.cs \
	Game\ Settings/GameSettingsModule.cs \
	Mission\ Control/MissionControlModule.cs \
	Mission\ Control/MeshCombineUtility.cs \
	Music\ Control/MusicControlModule.cs \
	Music\ Control/MusicControlInspector.cs \
	Teamwork/TeamworkModule.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	fpscontroleditorlib.pc.in 

REFERENCES =  \
	System

DLL_REFERENCES =  \
	/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEditor.dll \
	/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,UNITYEDITOR_DLL))
$(eval $(call emit-deploy-target,UNITYENGINE_DLL))
$(eval $(call emit-deploy-target,NUNIT_FRAMEWORK_DLL))
$(eval $(call emit-deploy-wrapper,FPSCONTROLEDITORLIB_PC,fpscontroleditorlib.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(FPSCONTROLEDITORLIB_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(UNITYEDITOR_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(UNITYENGINE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(FPSCONTROLEDITORLIB_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FPSCONTROLEDITORLIB_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(UNITYEDITOR_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(UNITYENGINE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(NUNIT_FRAMEWORK_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FPSCONTROLEDITORLIB_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
