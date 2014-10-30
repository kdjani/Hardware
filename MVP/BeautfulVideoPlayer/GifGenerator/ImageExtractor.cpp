// Class1.cpp
#include "pch.h"
#include "ImageExtractor.h"

using namespace ImageProcessing;
using namespace Platform;

ImageExtractor::ImageExtractor()
{
	m_initialized = false;
}

void ImageExtractor::GenerateThumbnails(StorageFile^ file, Platform::String^ hash)
{
	if (!m_initialized)
	{
		m_player = ref new FramePlayer();
		m_player->Initialize(Windows::UI::Core::CoreWindow::GetForCurrentThread());
		m_player->AutoPlay(false);
		m_initialized = true;
	}
	else
	{  // Maybe create a brand new object?
		m_player->Shutdown();
		//delete m_player;
		//m_player = ref new FramePlayer();
		m_initialized = false;
		//m_player->Shutdown();
		m_player->Initialize(Windows::UI::Core::CoreWindow::GetForCurrentThread());
		m_player->AutoPlay(false);
		m_initialized = true;
	}

	//m_player->Initialize(Windows::UI::Core::CoreWindow::GetForCurrentThread());
	m_player->m_imageHeight = ApplicationConfiguration::Configuration::ImageHeight;
	m_player->m_imageHeight = ApplicationConfiguration::Configuration::ImageHeight;
	m_player->m_startPercent = ApplicationConfiguration::Configuration::Category1StartPercent;
	m_player->m_NumberOfFrames = ApplicationConfiguration::Configuration::Category1Frames;
	m_player->m_FrameInterval = (float)ApplicationConfiguration::Configuration::FrameInterval;

	//m_player->DXGIDeviceTrim();

	m_player->m_bEnd = false;
	m_player->m_bUnrecoverableError = false;
	m_player->m_FrameIndex = 0;
	
	
	m_player->SetFileHash(hash);
	m_player->OpenFile(file);
	//m_player->Play();
}

bool ImageExtractor::IsGenerationFinished()
{
	if (m_initialized)
	{
		return m_player->m_bEnd;
	}
	else
	{
		return false;
	}
}

bool ImageExtractor::IsUnrecoverableError()
{
	if (m_initialized)
	{
		return m_player->m_bUnrecoverableError;
	}
	else
	{
		return false;
	}
}