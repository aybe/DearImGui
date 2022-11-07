// imgui.cpp : Defines the exported functions for the DLL.
//

#include "framework.h"
#include "imgui.h"


// This is an example of an exported variable
IMGUI_API int nimgui=0;

// This is an example of an exported function.
IMGUI_API int fnimgui(void)
{
    return 0;
}

// This is the constructor of a class that has been exported.
Cimgui::Cimgui()
{
    return;
}
