#pragma once

namespace lt = libtorrent;
class Torrent
{
public:
	Torrent(const lt::torrent_handle& handle, boost::int64_t id);
	~Torrent();

	lt::torrent_handle GetHandle() { return _handle; }

private:
	lt::torrent_handle _handle;
	boost::int64_t _id;
};

