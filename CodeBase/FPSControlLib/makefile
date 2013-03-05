
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/FPSControlLib.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

FPSCONTROLLIB_DLL_MDB_SOURCE=bin/Debug/FPSControlLib.dll.mdb
FPSCONTROLLIB_DLL_MDB=$(BUILD_DIR)/FPSControlLib.dll.mdb
UNITYENGINE_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/FPSControlLib.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

FPSCONTROLLIB_DLL_MDB=
UNITYENGINE_DLL_SOURCE=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(FPSCONTROLLIB_DLL_MDB) \
	$(UNITYENGINE_DLL)  

LINUX_PKGCONFIG = \
	$(FPSCONTROLLIB_PC)  


RESGEN=resgen2

UNITYENGINE_DLL = $(BUILD_DIR)/UnityEngine.dll
FPSCONTROLLIB_PC = $(BUILD_DIR)/fpscontrollib.pc

FILES = \
	AssemblyInfo.cs \
	Crosshair/IntelliCrosshair.cs \
	DamageSource.cs \
	Footstep\ Control/FootstepControl.cs \
	Footstep\ Control/FootstepControlDefinition.cs \
	Footstep\ Control/FootstepControlDefinitions.cs \
	Managers/Game.cs \
	Messenger\ Control/MessengerControl.cs \
	Messenger\ Control/MessengerControlListener.cs \
	Music\ Control/MusicControl.cs \
	Music\ Control/MusicControlTrack.cs \
	AIGunLogic.cs \
	Ambience.cs \
	CrosshairAnimator.cs \
	Damageable.cs \
	DataController.cs \
	Door.cs \
	FontColor.cs \
	GunLogic.cs \
	HurtIndicator.cs \
	InteractLogic.cs \
	OverrideAnimationSpeeds.cs \
	PickUpLogic.cs \
	ProgressBar.cs \
	RBFPSControllerLogic.cs \
	Sound\ Control/SoundControl.cs \
	Sound\ Control/SoundControlAudio.cs \
	BulletHoleLogic.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	fpscontrollib.pc.in 

REFERENCES =  \
	System

DLL_REFERENCES =  \
	/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,UNITYENGINE_DLL))
$(eval $(call emit-deploy-wrapper,FPSCONTROLLIB_PC,fpscontrollib.pc))


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
	$(call cp,$(FPSCONTROLLIB_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(UNITYENGINE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(FPSCONTROLLIB_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FPSCONTROLLIB_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(UNITYENGINE_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(FPSCONTROLLIB_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
