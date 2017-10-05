// Torrent_Test.cpp : 콘솔 응용 프로그램에 대한 진입점을 정의합니다.
//

#include "stdafx.h"

#include <boost/make_shared.hpp>
#include "libtorrent/entry.hpp"
#include "libtorrent/bencode.hpp"
#include "libtorrent/session.hpp"
#include "libtorrent/torrent_info.hpp"

int main(int argc, char* argv[])
{
	using namespace libtorrent;
	namespace lt = libtorrent;

	if (argc != 2)
	{
		fputs("usage: ./simple_client torrent-file\n"
			"to stop the client, press return.\n", stderr);
		return 1;
	}

	settings_pack sett;
	sett.set_str(settings_pack::listen_interfaces, "0.0.0.0:6881");
	lt::session s(sett);
	add_torrent_params p;
	p.save_path = "./";
	error_code ec;
	p.ti = boost::make_shared<torrent_info>(std::string(argv[1]), boost::ref(ec), 0);
	if (ec)
	{
		fprintf(stderr, "%s\n", ec.message().c_str());
		return 1;
	}
	s.add_torrent(p, ec);
	if (ec)
	{
		fprintf(stderr, "%s\n", ec.message().c_str());
		return 1;
	}

	// wait for the user to end
	char a;
	scanf("%c\n", &a);
    return 0;
}

