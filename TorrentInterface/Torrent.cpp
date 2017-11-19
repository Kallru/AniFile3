#include "stdafx.h"

Torrent::Torrent(const lt::torrent_handle& handle, boost::int64_t id)
	:_handle(handle)
	,_id(id)
{
}

Torrent::~Torrent()
{
}