#include "stdafx.h"
#include "dllexport.h"

#include <thread>
#include <chrono>


#include "BinaryWriter.h"

#define DLL_EXPORT __declspec(dllexport)

extern "C"
{
	// return the name of a torrent status enum
	char const* state(lt::torrent_status::state_t s)
	{
		switch (s) {
		case lt::torrent_status::checking_files: return "checking";
		case lt::torrent_status::downloading_metadata: return "dl metadata";
		case lt::torrent_status::downloading: return "downloading";
		case lt::torrent_status::finished: return "finished";
		case lt::torrent_status::seeding: return "seeding";
		case lt::torrent_status::allocating: return "allocating";
		case lt::torrent_status::checking_resume_data: return "checking resume";
		default: return "<>";
		}
	}

	static std::shared_ptr<EngineInterface> g_pEngineInterface;	

	void InitializeEngine()
	{
		g_pEngineInterface.reset(new EngineInterface());
		g_pEngineInterface->Initialize();
	}

	void UninitializeEngine()
	{
		g_pEngineInterface.reset();
	}

	bool Request(const char* message, const char* pRequest, unsigned int bufferSize, const void*& pResponse, unsigned int& outputDataSize)
	{
		if (g_pEngineInterface.get() != nullptr)
		{
			msgpack::object requestObject;
			msgpack::object_handle handle;

			if (pRequest != nullptr)
			{
				handle = msgpack::unpack(pRequest, bufferSize);
				requestObject = handle.get();
			}

			bool bResult = g_pEngineInterface->ProcessRequest(std::string(message), requestObject);
			if (bResult)
			{
				pResponse = g_pEngineInterface->GetOutputBuffer();
				outputDataSize = g_pEngineInterface->GetOutputBufferSize();
				return true;
			}
		}
		return false;
	}
}