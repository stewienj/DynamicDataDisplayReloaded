// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

// Windows Header Files:
#include <windows.h>

// C RunTime Header Files
#include <stdlib.h>
#include <stdio.h>
#include <d3d9.h>
#include <D3D10_1.h>

#define IFC(x) { hr = (x); if (FAILED(hr)) { goto Cleanup; }}
#define ReleaseInterface(x) { if (NULL != x) { x->Release(); x = NULL; }}