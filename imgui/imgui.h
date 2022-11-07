// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the IMGUI_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// IMGUI_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef IMGUI_EXPORTS
#define IMGUI_API __declspec(dllexport)
#else
#define IMGUI_API __declspec(dllimport)
#endif

// This class is exported from the dll
class IMGUI_API Cimgui {
public:
	Cimgui(void);
	// TODO: add your methods here.
};

extern IMGUI_API int nimgui;

IMGUI_API int fnimgui(void);
