#include "stdafx.h"

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
		{ toLower("QueryState"), CommandType(&EngineInterface::QueryState) }
	};
}

EngineInterface::~EngineInterface()
{
	Uninitialize();
}

void EngineInterface::Initialize(CShapCallback callback)
{
	lt::settings_pack settings;

	settings.set_int(lt::settings_pack::alert_mask
			, lt::alert::error_notification
			| lt::alert::storage_notification
			| lt::alert::status_notification);

	_pSession = new lt::session(settings);

	_cshapCallback = callback;
}

void EngineInterface::Uninitialize()
{
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

	// load resume data from disk and pass it in as we add the magnet link
	std::ifstream ifs(".resume_file", std::ios_base::binary);
	ifs.unsetf(std::ios_base::skipws);
	torrentParams.resume_data.assign(std::istream_iterator<char>(ifs), std::istream_iterator<char>());

	std::tie(torrentParams.url, torrentParams.save_path) = requestInfo;
		
	_pSession->async_add_torrent(torrentParams);

	auto pTorrent = new Torrent(this, id);
	_torrents.insert({ id, pTorrent });
	pTorrent->Start();
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