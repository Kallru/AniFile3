#pragma once

#include <msgpack.hpp>
#include <future>

#include "libtorrent/session.hpp"

namespace lt = libtorrent;

struct StateInfo
{
	int State;
	float DownloadPayloadRate;
	long long TotalDone;
	long long TotalWanted;
	int Progress;

	MSGPACK_DEFINE(State, DownloadPayloadRate, TotalDone, TotalWanted, Progress);
};

struct QueryResponseInfo
{
	long long TotalSize;

	MSGPACK_DEFINE(TotalSize);
};

class EngineInterface
{
	friend class Torrent;

private:
	typedef std::function<bool(EngineInterface*, boost::int64_t id, const msgpack::object& input)> CommandType;
	typedef std::map<std::string, CommandType> CommandMap;
	typedef std::function<void __stdcall(boost::int64_t id, const char* message, const char* pRequest, unsigned int bufferSize)> CShapCallback;

public:
	EngineInterface();
	~EngineInterface();

	void Initialize(CShapCallback callback);
	void Uninitialize();

	void* GetOutputBuffer(){ return _outputBuffer; }
	int GetOutputBufferSize() { return _outputBufferSize; }

	bool ProcessRequest(boost::int64_t id, const std::string& message, const msgpack::object& input);

	bool StartDownload(boost::int64_t id, const msgpack::object& input);
	bool Stop(boost::int64_t id, const msgpack::object& input);
	bool Resume(boost::int64_t id, const msgpack::object& input);
	bool QueryInfo(boost::int64_t id, const msgpack::object& input);
	bool QueryState(boost::int64_t id, const msgpack::object& input);
	bool DestroyId(boost::int64_t id, const msgpack::object& input);

private:
	template<typename T>
	void SendToCShap(boost::int64_t id, const std::string& message, const T& data)
	{
		std::stringstream buffer;
		msgpack::pack(buffer, data);

		int size = buffer.str().size();
		char* outputBuffer = new char[size];
		memcpy(outputBuffer, buffer.str().data(), size);

		_cshapCallback(id, message.data(), outputBuffer, size);
		delete outputBuffer;
	}

	template<typename T>
	void PackOutputBuffer(const T& data)
	{
		std::stringstream buffer;
		msgpack::pack(buffer, data);

		_outputBufferSize = buffer.str().size();
		_outputBuffer = new char[_outputBufferSize];
		memcpy(_outputBuffer, buffer.str().data(), _outputBufferSize);
	}

	void OnFinishedTorrent();
	void OnErrorTorrent(const lt::torrent_error_alert* pAlert);
	std::future<void> UpdateTorrent();

	lt::torrent_handle GetHandle(boost::int64_t id);

private:
	lt::session* _pSession;
	CommandMap _commandMap;

	// C++ to C#
	CShapCallback _cshapCallback;

	std::map<boost::int64_t, Torrent*> _torrents;

	void* _outputBuffer;
	int _outputBufferSize;

	// for update Routine
	std::atomic<bool> _bRunRoutine;
	std::future<void> _updateRoutine;
};