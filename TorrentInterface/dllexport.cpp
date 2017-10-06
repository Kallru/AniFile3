#include "stdafx.h"
#include "dllexport.h"

#include <iostream>
#include <thread>
#include <chrono>
#include <fstream>

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
	//void InitializeEngine()
	//{
	//	namespace lt = libtorrent;
	//	using clk = std::chrono::steady_clock;

	//	int argc;
	//	char argv[2] = { 3, 2 };

	//	if (argc != 2) {
	//		std::cerr << "usage: " << argv[0] << " <magnet-url>" << std::endl;
	//		return;
	//	}

	//	lt::settings_pack pack;
	//	pack.set_int(lt::settings_pack::alert_mask
	//		, lt::alert::error_notification
	//		| lt::alert::storage_notification
	//		| lt::alert::status_notification);

	//	lt::session ses(pack);
	//	lt::add_torrent_params atp;
	//	clk::time_point last_save_resume = clk::now();

	//	// load resume data from disk and pass it in as we add the magnet link
	//	std::ifstream ifs(".resume_file", std::ios_base::binary);
	//	ifs.unsetf(std::ios_base::skipws);
	//	atp.resume_data.assign(std::istream_iterator<char>(ifs)
	//		, std::istream_iterator<char>());
	//	atp.url = argv[1];
	//	atp.save_path = "."; // save in current dir
	//	ses.async_add_torrent(atp);

	//	// this is the handle we'll set once we get the notification of it being
	//	// added
	//	lt::torrent_handle h;
	//	for (;;) {
	//		std::vector<lt::alert*> alerts;
	//		ses.pop_alerts(&alerts);

	//		for (lt::alert const* a : alerts) {
	//			if (auto at = lt::alert_cast<lt::add_torrent_alert>(a)) {
	//				h = at->handle;
	//			}
	//			// if we receive the finished alert or an error, we're done
	//			if (lt::alert_cast<lt::torrent_finished_alert>(a)) {
	//				h.save_resume_data();
	//				goto done;
	//			}
	//			if (lt::alert_cast<lt::torrent_error_alert>(a)) {
	//				std::cout << a->message() << std::endl;
	//				goto done;
	//			}

	//			// when resume data is ready, save it
	//			if (auto rd = lt::alert_cast<lt::save_resume_data_alert>(a)) {
	//				std::ofstream of(".resume_file", std::ios_base::binary);
	//				of.unsetf(std::ios_base::skipws);
	//				lt::bencode(std::ostream_iterator<char>(of)
	//					, *rd->resume_data);
	//			}

	//			if (auto st = lt::alert_cast<lt::state_update_alert>(a)) {
	//				if (st->status.empty()) continue;

	//				const lt::torrent_info* pSome = h.torrent_file().get();

	//				// we only have a single torrent, so we know which one
	//				// the status is for
	//				lt::torrent_status const& s = st->status[0];
	//				std::cout << "\r" << state(s.state) << " "
	//					<< (s.download_payload_rate / 1000) << " kB/s "
	//					<< (s.total_done / 1000) << " kB ("
	//					<< (s.progress_ppm / 10000) << "%) downloaded\x1b[K";
	//				std::cout.flush();
	//			}
	//		}
	//		std::this_thread::sleep_for(std::chrono::milliseconds(200));

	//		// ask the session to post a state_update_alert, to update our
	//		// state output for the torrent
	//		ses.post_torrent_updates();

	//		// save resume data once every 30 seconds
	//		if (clk::now() - last_save_resume > std::chrono::seconds(30)) {
	//			h.save_resume_data();
	//			last_save_resume = clk::now();
	//		}
	//	}

	//	// TODO: ideally we should save resume data here

	//done:
	//	std::cout << "\ndone, shutting down" << std::endl;
	//}

	void InitializeEngine()
	{
		g_pEngineInterface.reset(new EngineInterface());
	}

	bool Request(const char* message, const char* pRequest, unsigned int bufferSize, const void*& pResponse, unsigned int& outputDataSize)
	{
		if (g_pEngineInterface.get() != nullptr)
		{
			msgpack::object_handle handle = msgpack::unpack(pRequest, bufferSize);
			msgpack::object requestObject = handle.get();

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