#include "stdafx.h"

Torrent::Torrent(EngineInterface* pEngine, boost::int64_t id)
	:_pEngine(pEngine),
	_id(id)
{
}

Torrent::~Torrent()
{
	_bRunRoutine = false;	
	_updateRoutine.wait();
	_pEngine = nullptr;
}

void Torrent::Start()
{
	// 업데이트 루틴, 컨트롤을 위해 저장해둠
	_updateRoutine = UpdateTorrent();
}

std::future<void> Torrent::UpdateTorrent()
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
			_pEngine->_pSession->pop_alerts(&alerts);

			for (lt::alert const* alert : alerts)
			{
				if (auto at = lt::alert_cast<lt::add_torrent_alert>(alert))
				{
					_handle = at->handle;
					auto hash = _handle.info_hash();
					_pEngine->SendToCShap(_id, "AddTorrentHandle", hash.to_string());
				}

				// if we receive the finished alert or an error, we're done
				if (lt::alert_cast<lt::torrent_finished_alert>(alert))
				{
					_handle.save_resume_data();
					_pEngine->OnFinishedTorrent();
					return;
				}

				if (auto pErrorAlert = lt::alert_cast<lt::torrent_error_alert>(alert))
				{
					_pEngine->OnErrorTorrent(pErrorAlert);
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
			_pEngine->_pSession->post_torrent_updates();

			// save resume data once every 30 seconds
			if (clk::now() - lastSaveResume > std::chrono::seconds(30))
			{
				_handle.save_resume_data();
				lastSaveResume = clk::now();
			}
		}
	});
}