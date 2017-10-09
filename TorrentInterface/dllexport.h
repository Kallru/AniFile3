#pragma once

#define DLL_EXPORT __declspec(dllexport)

// C++ to C# 콜백 호출, 각종 메세지 전달을 한다
typedef void(__stdcall *Win32HandleCallback)(long long id, const char* message, const char* pRequest, unsigned int bufferSize);

extern "C"
{
	DLL_EXPORT void InitializeEngine(Win32HandleCallback callback);
	DLL_EXPORT void UninitializeEngine();
	DLL_EXPORT bool Request(long long, const char* message, 
							const char* pRequest, 
							unsigned int bufferSize, 
							const void*& pResponse, 
							unsigned int& outputDataSize);
}