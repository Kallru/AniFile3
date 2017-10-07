#pragma once

#include <msgpack.hpp>
#include <future>

#include "libtorrent/session.hpp"

using namespace libtorrent;
namespace lt = libtorrent;

struct StateInfo
{
	int State;
	int DownloadPayloadRate;
	long long Total;
	int Progress;

	MSGPACK_DEFINE(State, DownloadPayloadRate, Total, Progress);
};

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

	void Initialize();
	void Uninitialize();

	void* GetOutputBuffer(){ return _outputBuffer; }
	int GetOutputBufferSize() { return _outputBufferSize; }

	bool ProcessRequest(const std::string& message, const msgpack::object& request);

	bool StartDownload(const msgpack::object& request);
	bool Stop(const msgpack::object& request);
	bool Resume(const msgpack::object& request);
	bool QueryInfo(const msgpack::object& request);
	bool QueryState(const msgpack::object& request);

private:
	template<typename T> void PackOutputBuffer(const T& some);
	std::future<void> UpdateTorrent();

	void OnFinishedTorrent();
	void OnErrorTorrent(const lt::torrent_error_alert* pAlert);

private:
	lt::session* _pSession;
	CommandMap _commandMap;

	lt::torrent_handle _torrentHandle;
	StateInfo _currentStateInfo;

	std::future<void> _updateRoutine;
	bool bAbort;

	void* _outputBuffer;
	int _outputBufferSize;
};