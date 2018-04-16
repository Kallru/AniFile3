#include "stdafx.h"
#include "libtorrent/peer_info.hpp"

#include <boost/algorithm/string.hpp>

#define SAFT_DELETE(x) if (x != nullptr){ delete x; x = nullptr; }

std::string toLower(std::string str)
{
	boost::algorithm::to_lower(str);
	return str;
}

EngineInterface::EngineInterface()
	:_pSession(nullptr),
	_outputBuffer(nullptr)
{
	_commandMap =
	{
		{ toLower("StartDownload"), CommandType(&EngineInterface::StartDownload) },
		{ toLower("Stop"), CommandType(&EngineInterface::Stop) },
		{ toLower("Resume"), CommandType(&EngineInterface::Resume) },
		{ toLower("QueryInfo"), CommandType(&EngineInterface::QueryInfo) },
		{ toLower("QueryState"), CommandType(&EngineInterface::QueryState) },
		{ toLower("DestroyId"), CommandType(&EngineInterface::DestroyId) }
	};
}

EngineInterface::~EngineInterface()
{
	Uninitialize();
}

void EngineInterface::Initialize(CShapCallback callback)
{
	lt::settings_pack settings;

	settings.set_str(lt::settings_pack::user_agent, "richgrass");
	settings.set_int(lt::settings_pack::alert_mask
			, lt::alert::error_notification
			| lt::alert::storage_notification
			| lt::alert::status_notification);

	settings.set_bool(lt::settings_pack::announce_to_all_trackers, true);
	settings.set_bool(lt::settings_pack::announce_to_all_tiers, true);
	settings.set_bool(lt::settings_pack::smooth_connects, false);
	settings.set_int(lt::settings_pack::connection_speed, 200);
	
	/*
	settings.set_bool(lt::settings_pack::strict_end_game_mode, false);
	settings.set_bool(lt::settings_pack::allow_multiple_connections_per_ip, true);
	settings.set_bool(lt::settings_pack::close_redundant_connections, true);

	settings.set_int(lt::settings_pack::mixed_mode_algorithm
		, lt::settings_pack::bandwidth_mixed_algo_t::prefer_tcp);
	settings.set_int(lt::settings_pack::max_queued_disk_bytes, 1024 * 1024);

	settings.set_int(lt::settings_pack::recv_socket_buffer_size, 20 * 1024 * 1024);

	settings.set_bool(lt::settings_pack::rate_limit_ip_overhead, false);
	
	settings.set_bool(lt::settings_pack::low_prio_disk, false);
	
	settings.set_int(lt::settings_pack::upload_rate_limit, 20 * 1024 * 1024);
	settings.set_int(lt::settings_pack::download_rate_limit, 20 * 1024 * 1024);
	settings.set_int(lt::settings_pack::local_download_rate_limit, 20 * 1024 * 1024);
	settings.set_int(lt::settings_pack::local_upload_rate_limit, 20 * 1024 * 1024);

	settings.set_bool(lt::settings_pack::ignore_limits_on_local_network, true);
	settings.set_bool(lt::settings_pack::ban_web_seeds, true);
	settings.set_int(lt::settings_pack::active_loaded_limit, 2000);
	settings.set_int(lt::settings_pack::active_seeds, 2000);
	settings.set_int(lt::settings_pack::tick_interval, 150);
	settings.set_int(lt::settings_pack::urlseed_wait_retry, 60);
	settings.set_int(lt::settings_pack::min_announce_interval, 30);
	settings.set_int(lt::settings_pack::local_service_announce_interval, 30);
	settings.set_int(lt::settings_pack::max_rejects, 10);
	settings.set_int(lt::settings_pack::file_pool_size, 20);
	settings.set_int(lt::settings_pack::request_timeout, 10);
	settings.set_int(lt::settings_pack::peer_timeout, 20);
	settings.set_int(lt::settings_pack::inactivity_timeout, 20);
	settings.set_int(lt::settings_pack::max_failcount, 20);
	/**/
	
	_pSession = new lt::session(settings);

	_cshapCallback = callback;
	_updateRoutine = UpdateTorrent();
}

void EngineInterface::Uninitialize()
{
	_bRunRoutine = false;
	_updateRoutine.wait();

	for (auto element : _torrents)
	{
		SAFT_DELETE(element.second);
	}

	_torrents.clear();

	SAFT_DELETE(_pSession);
	SAFT_DELETE(_outputBuffer);
	_outputBufferSize = 0;
}

bool EngineInterface::ProcessRequest(boost::int64_t id, const std::string& message, const msgpack::object& request)
{
	SAFT_DELETE(_outputBuffer);
	_outputBufferSize = 0;

	return _commandMap[toLower(message)](this, id, request);
}

bool EngineInterface::StartDownload(boost::int64_t id, const msgpack::object& input)
{
	// 1.Magent Link
	// 2.SavePath
	auto requestInfo = input.as<std::tuple<std::string, std::string>>();

	lt::add_torrent_params torrentParams;
	torrentParams.max_uploads = 0;
	torrentParams.upload_limit = 0;

	// load resume data from disk and pass it in as we add the magnet link
	std::ifstream ifs(".resume_file", std::ios_base::binary);
	ifs.unsetf(std::ios_base::skipws);
	torrentParams.resume_data.assign(std::istream_iterator<char>(ifs), std::istream_iterator<char>());

	std::tie(torrentParams.url, torrentParams.save_path) = requestInfo;
	
	auto handle = _pSession->add_torrent(torrentParams);

	_torrents.insert({ id, new Torrent(handle, id) });
	
	return true;
}

bool EngineInterface::Stop(boost::int64_t id, const msgpack::object& input)
{
	return true;
}

bool EngineInterface::Resume(boost::int64_t id, const msgpack::object& input)
{
	return true;
}

bool EngineInterface::QueryState(boost::int64_t id, const msgpack::object& input)
{
	auto handle = GetHandle(id);
	if (handle.is_valid())
	{
		auto status = handle.status();

		// 모든 단위는 kB
		StateInfo stateInfo = {};
		stateInfo.State = status.state;
		stateInfo.DownloadPayloadRate = status.download_payload_rate / 1000.f;
		stateInfo.TotalDone = status.total_done / 1000;
		stateInfo.TotalWanted = status.total_wanted / 1000;
		stateInfo.Progress = status.progress_ppm / 10000;

		// 핸들 또는 세션에 유효성 체크 필요
		PackOutputBuffer(stateInfo);
	}
	return true;
}

bool EngineInterface::DestroyId(boost::int64_t id, const msgpack::object& input)
{
	if (_pSession != nullptr)
	{
		auto handle = GetHandle(id);
		_pSession->remove_torrent(handle);
	}

	SAFT_DELETE(_torrents[id]);
	_torrents.erase(id);
	return true;
}

bool EngineInterface::QueryInfo(boost::int64_t id, const msgpack::object& input)
{
	auto handle = GetHandle(id);
	if (handle.is_valid())
	{
		//_torrentHandle.status
		const lt::torrent_info* pTorrentInfo = handle.torrent_file().get();
		if (pTorrentInfo != nullptr)
		{
			QueryResponseInfo info = {};
			info.TotalSize = pTorrentInfo->total_size();

			PackOutputBuffer(info);
			return true;
		}
	}
	return false;
}

void EngineInterface::OnFinishedTorrent()
{

}

void EngineInterface::OnErrorTorrent(const lt::torrent_error_alert* pAlert)
{

}

lt::torrent_handle EngineInterface::GetHandle(boost::int64_t id)
{
	assert(_torrents.find(id) != _torrents.end());
	return _torrents[id]->GetHandle();
}

std::future<void> EngineInterface::UpdateTorrent()
{
	return std::async(std::launch::async, [=]()
	{
		using clk = std::chrono::steady_clock;
		clk::time_point lastSaveResume = clk::now();

		// this is the handle we'll set once we get the notification of it being
		// added
		while (_bRunRoutine)
		{
			std::vector<lt::alert*> alerts;
			_pSession->pop_alerts(&alerts);

			for (lt::alert const* alert : alerts)
			{
				if (auto at = lt::alert_cast<lt::add_torrent_alert>(alert))
				{
					auto handle = at->handle;

					for (const auto& pair : _torrents)
					{
						if (pair.second->GetHandle() == handle)
						{
							auto hash = handle.info_hash();
							SendToCShap(pair.first, "AddTorrentHandle", hash.to_string());
							break;
						}
					}
				}

				// if we receive the finished alert or an error, we're done
				if (auto at = lt::alert_cast<lt::torrent_finished_alert>(alert))
				{
					at->handle.save_resume_data();
					OnFinishedTorrent();
					return;
				}

				if (auto pErrorAlert = lt::alert_cast<lt::torrent_error_alert>(alert))
				{
					OnErrorTorrent(pErrorAlert);
					return;
				}

				// when resume data is ready, save it
				if (auto rd = lt::alert_cast<lt::save_resume_data_alert>(alert))
				{
					std::ofstream of(".resume_file", std::ios_base::binary);
					of.unsetf(std::ios_base::skipws);
					lt::bencode(std::ostream_iterator<char>(of), *rd->resume_data);
				}
			}

			std::this_thread::sleep_for(std::chrono::milliseconds(200));

			// ask the session to post a state_update_alert, to update our
			// state output for the torrent
			_pSession->post_torrent_updates();

			// save resume data once every 30 seconds
			if (clk::now() - lastSaveResume > std::chrono::seconds(30))
			{
				auto torrents = _pSession->get_torrents();

				for (const auto& handle : torrents)
				{
					handle.save_resume_data();
				}
				lastSaveResume = clk::now();
			}
		}
	});
}