#pragma once

#define DLL_EXPORT __declspec(dllexport)

extern "C"
{
	DLL_EXPORT void InitializeEngine();
	DLL_EXPORT void Command();
}