// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the IMPLOT_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// IMPLOT_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef IMPLOT_EXPORTS
#define IMPLOT_API __declspec(dllexport)
#else
#define IMPLOT_API __declspec(dllimport)
#endif

// This class is exported from the dll
class IMPLOT_API Cimplot {
public:
	Cimplot(void);
	// TODO: add your methods here.
};

extern IMPLOT_API int nimplot;

IMPLOT_API int fnimplot(void);
