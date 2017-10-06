#pragma once

#define DLL_EXPORT __declspec(dllexport)

extern "C"
{
	DLL_EXPORT void InitializeEngine();
	DLL_EXPORT bool Request(const char* message, const char* pRequest, unsigned int bufferSize, const void*& pResponse, unsigned int& outputDataSize);
}