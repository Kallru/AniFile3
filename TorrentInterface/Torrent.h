#pragma once

namespace lt = libtorrent;
class Torrent
{
public:
	Torrent(EngineInterface* pEngine, boost::int64_t id);
	~Torrent();

	void Start();

	lt::torrent_handle GetHandle() { return _handle; }

private:
	std::future<void> UpdateTorrent();

private:
	EngineInterface* _pEngine;
	lt::torrent_handle _handle;
	std::future<void> _updateRoutine;
	std::atomic<bool> _bRunRoutine;
	boost::int64_t _id;
};

