#include "stdafx.h"
#include "EngineInterface.h"

#include <boost/algorithm/string.hpp>

#define SAFT_DELETE(x) if (x != nullptr){ delete x; x = nullptr; }

std::string toLower(std::string str)
{
	boost::algorithm::to_lower(str);
	return str;
}

EngineInterface::EngineInterface()
	:_pSession(nullptr),
	_outputBuffer(nullptr),
	_currentStateInfo({}),
	bAbort(false)
{
	_commandMap =
	{
		{ toLower("StartDownload"), CommandType(&EngineInterface::StartDownload) },
		{ toLower("Stop"), CommandType(&EngineInterface::Stop) },
		{ toLower("Resume"), CommandType(&EngineInterface::Resume) },
		{ toLower("QueryInfo"), CommandType(&EngineInterface::QueryInfo) },
		{ toLower("QueryState"), CommandType(&EngineInterface::QueryState) }
	};
}

EngineInterface::~EngineInterface()
{
	Uninitialize();
}

void EngineInterface::Initialize()
{
	settings_pack settings;

	settings.set_int(lt::settings_pack::alert_mask
			, lt::alert::error_notification
			| lt::alert::storage_notification
			| lt::alert::status_notification);

	_pSession = new lt::session(settings);
}

void EngineInterface::Uninitialize()
{
	bAbort = true;
	_updateRoutine.wait();

	SAFT_DELETE(_pSession);
	SAFT_DELETE(_outputBuffer);
	_outputBufferSize = 0;
}

bool EngineInterface::ProcessRequest(const std::string& message, const msgpack::object& request)
{
	SAFT_DELETE(_outputBuffer);
	_outputBufferSize = 0;

	return _commandMap[toLower(message)](this, request);
}

template<typename T>
void EngineInterface::PackOutputBuffer(const T& some)
{
	std::stringstream buffer;
	msgpack::pack(buffer, some);

	_outputBufferSize = buffer.str().size();
	_outputBuffer = new char[_outputBufferSize];
	memcpy(_outputBuffer, buffer.str().data(), _outputBufferSize);
}

bool EngineInterface::StartDownload(const msgpack::object& input)
{
	// 1.Magent Link
	// 2.SavePath
	auto requestInfo = input.as<std::tuple<std::string, std::string>>();

	lt::add_torrent_params torrentParams;

	// load resume data from disk and pass it in as we add the magnet link
	std::ifstream ifs(".resume_file", std::ios_base::binary);
	ifs.unsetf(std::ios_base::skipws);
	torrentParams.resume_data.assign(std::istream_iterator<char>(ifs), std::istream_iterator<char>());

	std::tie(torrentParams.url, torrentParams.save_path) = requestInfo;

	_pSession->async_add_torrent(torrentParams);

	_updateRoutine = UpdateTorrent();

	//_updateRoutine.
	//bool result = _updateRoutine.get();
	//bool result = await _updateRoutine;
	return true;
}

struct Someawit
{
	bool await_ready()
	{
		return false;
	}

	void await_suspend(std::experimental::coroutine_handle<>)
	{
	}

	void await_resume()
	{
	}
};

std::future<void> EngineInterface::UpdateTorrent()
{
	return std::async(std::launch::async, [=]() 
	{
		using clk = std::chrono::steady_clock;
		clk::time_point lastSaveResume = clk::now();

		// this is the handle we'll set once we get the notification of it being
		// added
		while (bAbort == false)
		{
			std::vector<lt::alert*> alerts;
			_pSession->pop_alerts(&alerts);

			for (lt::alert const* alert : alerts)
			{
				if (auto at = lt::alert_cast<lt::add_torrent_alert>(alert))
				{
					_torrentHandle = at->handle;
				}

				// if we receive the finished alert or an error, we're done
				if (lt::alert_cast<lt::torrent_finished_alert>(alert))
				{
					_torrentHandle.save_resume_data();
					OnFinishedTorrent();
					return ;
				}

				if (auto pErrorAlert = lt::alert_cast<lt::torrent_error_alert>(alert))
				{
					OnErrorTorrent(pErrorAlert);
					return ;
				}

				// when resume data is ready, save it
				if (auto rd = lt::alert_cast<lt::save_resume_data_alert>(alert))
				{
					std::ofstream of(".resume_file", std::ios_base::binary);
					of.unsetf(std::ios_base::skipws);
					lt::bencode(std::ostream_iterator<char>(of), *rd->resume_data);
				}

				if (auto st = lt::alert_cast<lt::state_update_alert>(alert))
				{
					if (st->status.empty()) continue;

					// we only have a single torrent, so we know which one
					// the status is for
					lt::torrent_status const& s = st->status[0];

					// 모든 단위는 kB
					_currentStateInfo.State = s.state;
					_currentStateInfo.DownloadPayloadRate = s.download_payload_rate / 1000;
					_currentStateInfo.Total = s.total_done / 1000;
					_currentStateInfo.Progress = s.progress_ppm / 10000;
				}
			}
			
			std::this_thread::sleep_for(std::chrono::milliseconds(200));

			// ask the session to post a state_update_alert, to update our
			// state output for the torrent
			_pSession->post_torrent_updates();

			// save resume data once every 30 seconds
			if (clk::now() - lastSaveResume > std::chrono::seconds(30))
			{
				_torrentHandle.save_resume_data();
				lastSaveResume = clk::now();
			}
		}
	});
}

bool EngineInterface::Stop(const msgpack::object& request)
{
	return true;
}

bool EngineInterface::Resume(const msgpack::object& request)
{
	return true;
}

bool EngineInterface::QueryState(const msgpack::object& request)
{
	if (_torrentHandle.is_valid())
	{
		// 핸들 또는 세션에 유효성 체크 필요
		PackOutputBuffer(_currentStateInfo);
	}
	return true;
}

bool EngineInterface::QueryInfo(const msgpack::object& request)
{
	if (_torrentHandle.is_valid())
	{
		const lt::torrent_info* pSome = _torrentHandle.torrent_file().get();
	}
	return true;
}

void EngineInterface::OnFinishedTorrent()
{

}

void EngineInterface::OnErrorTorrent(const lt::torrent_error_alert* pAlert)
{

}