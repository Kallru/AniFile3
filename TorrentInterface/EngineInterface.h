#pragma once

#include "libtorrent/session.hpp"

using namespace libtorrent;
namespace lt = libtorrent;

class EngineInterface
{
private:	
	lt::session* _pSession;

public:
	EngineInterface();
	~EngineInterface();

	void Initialize(std::string savePath);
	void Uninitialize();
};

