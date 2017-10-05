#include "stdafx.h"
#include "EngineInterface.h"


EngineInterface::EngineInterface()
	:_pSession(nullptr)
{

}

EngineInterface::~EngineInterface()
{
	Uninitialize();
}

void EngineInterface::Initialize(std::string savePath)
{
	settings_pack settings;
	settings.set_str(settings_pack::listen_interfaces, "0.0.0.0:6881");
	_pSession = new lt::session(settings);
}

void EngineInterface::Uninitialize()
{
	if (_pSession != nullptr)
	{
		delete _pSession;
		_pSession = nullptr;
	}
}