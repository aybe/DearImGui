// implot.cpp : Defines the exported functions for the DLL.
//

#include "framework.h"
#include "implot.h"


// This is an example of an exported variable
IMPLOT_API int nimplot=0;

// This is an example of an exported function.
IMPLOT_API int fnimplot(void)
{
    return 0;
}

// This is the constructor of a class that has been exported.
Cimplot::Cimplot()
{
    return;
}
