#pragma once

#include <msgpack.hpp>
#include "libtorrent/session.hpp"

using namespace libtorrent;
namespace lt = libtorrent;

struct ResponseInfo
{
	std::string First;
	std::string Second;
	int Thrid;
	float Forth;
	unsigned int Five;

	MSGPACK_DEFINE(First, Second, Thrid, Forth, Five);
};

class EngineInterface
{
private:
	typedef std::function<bool(EngineInterface*, const msgpack::object& request)> CommandType;
	typedef std::map<std::string, CommandType> CommandMap;

public:
	EngineInterface();
	~EngineInterface();

	void Initialize(const std::string& savePath);
	void Uninitialize();

	void* GetOutputBuffer(){ return _outputBuffer; }
	int GetOutputBufferSize() { return _outputBufferSize; }

	bool ProcessRequest(const std::string& message, const msgpack::object& request);

	bool StartDownload(const msgpack::object& request);

private:
	template<typename T> void PackOutputBuffer(const T& some);

private:
	lt::session* _pSession;
	CommandMap _commandMap;

	void* _outputBuffer;
	int _outputBufferSize;
};