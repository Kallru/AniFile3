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
	_outputBuffer(nullptr)
{
	_commandMap =
	{
		{ toLower("StartDownload"), CommandType(&EngineInterface::StartDownload) }
	};
}

EngineInterface::~EngineInterface()
{
	Uninitialize();
}

void EngineInterface::Initialize(const std::string& savePath)
{
	settings_pack settings;
	settings.set_str(settings_pack::listen_interfaces, "0.0.0.0:6881");
	_pSession = new lt::session(settings);
}

void EngineInterface::Uninitialize()
{
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
	auto tesCase1_2 = input.as<std::tuple<std::string, std::string>>();

	std::tuple<bool, std::string, int> test(true, "hello", 20);
	PackOutputBuffer(test);

	return true;
}