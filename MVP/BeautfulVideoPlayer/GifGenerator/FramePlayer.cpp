//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//

#include "pch.h"
#include "FramePlayer.h"
#include <d3dmanagerlock.hxx>
#include "..\DirectXTex\DirectXTex\DirectXTex.h"
#include <ppltasks.h>

using namespace std;
using namespace Microsoft::WRL;
using namespace Windows::System::Threading;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::UI::Core;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::Foundation;
using namespace concurrency;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Input;
using namespace Windows::Storage;
using namespace concurrency;
//using namespace Logging;
#include <wincodec.h>

// MediaEngineNotify: Implements the callback for Media Engine event notification.
class MediaEngineNotify : public IMFMediaEngineNotify
{
	long m_cRef;
	Platform::Agile<Windows::UI::Core::CoreWindow> m_cWindow;
	MediaEngineNotifyCallback^ m_pCB;
	
public:
	MediaEngineNotify(Platform::Agile<Windows::UI::Core::CoreWindow> cWindow) : m_cWindow(cWindow), m_cRef(1), m_pCB(nullptr)
	{
		Logging::Logger::Info("FramePlayer::MediaEngineNotify");
	}

	STDMETHODIMP QueryInterface(REFIID riid, void** ppv)
	{
		if (__uuidof(IMFMediaEngineNotify) == riid)
		{
			*ppv = static_cast<IMFMediaEngineNotify*>(this);
		}
		else
		{
			*ppv = nullptr;
			return E_NOINTERFACE;
		}

		AddRef();

		return S_OK;
	}

	STDMETHODIMP_(ULONG) AddRef()
	{
		return InterlockedIncrement(&m_cRef);
	}

	STDMETHODIMP_(ULONG) Release()
	{
		LONG cRef = InterlockedDecrement(&m_cRef);
		if (cRef == 0)
		{
			delete this;
		}
		return cRef;
	}

	// EventNotify is called when the Media Engine sends an event.
	STDMETHODIMP EventNotify(DWORD meEvent, DWORD_PTR param1, DWORD param2)
	{
		Logging::Logger::Debug("FramePlayer::EventNotify");

		if (meEvent == MF_MEDIA_ENGINE_EVENT_NOTIFYSTABLESTATE)
		{
			SetEvent(reinterpret_cast<HANDLE>(param1));
		}
		else
		{
			m_pCB->OnMediaEngineEvent(meEvent);
		}

		Logging::Logger::Debug("FramePlayer::~EventNotify");

		return S_OK;
	}

	void MediaEngineNotifyCallback(MediaEngineNotifyCallback^ pCB)
	{
		m_pCB = pCB;
	}
};

FramePlayer::FramePlayer() :
m_spDX11Device(nullptr),
m_spDX11DeviceContext(nullptr),
m_spDXGIOutput(nullptr),
m_spDX11SwapChain(nullptr),
m_spDXGIManager(nullptr),
m_spMediaEngine(nullptr),
m_spEngineEx(nullptr),
m_bstrURL(nullptr),
m_TimerThreadHandle(nullptr),
m_fPlaying(FALSE),
m_fLoop(FALSE),
m_fEOS(FALSE),
m_fStopTimer(TRUE),
m_bInitiialized(FALSE),
m_d3dFormat(DXGI_FORMAT_B8G8R8A8_UNORM)
{
	Logging::Logger::Info("FramePlayer::FramePlayer");

	memset(&m_bkgColor, 0, sizeof(MFARGB));

	InitializeCriticalSectionEx(&m_critSec, 0, 0);

	Windows::ApplicationModel::Core::CoreApplication::Suspending +=
		ref new Windows::Foundation::EventHandler<SuspendingEventArgs^>(this, &FramePlayer::OnSuspending);

	Logging::Logger::Info("FramePlayer::~FramePlayer");
}

FramePlayer::~FramePlayer()
{
	Logging::Logger::Info("FramePlayer::~FramePlayer");

	Shutdown();

	//MFShutdown();

	DeleteCriticalSection(&m_critSec);

	Logging::Logger::Info("~~FramePlayer::FramePlayer");
}


void FramePlayer::OnSuspending(
	_In_ Platform::Object^ sender,
	_In_ SuspendingEventArgs^ args
	)
{
	Logging::Logger::Info("FramePlayer::OnSuspending");
	int x = -1;
	Logging::Logger::Info("FramePlayer::~OnSuspending");
}

//+-------------------------------------------------------------------------
//
//  Function:   CreateDX11Device()
//
//  Synopsis:   creates a default device
//
//--------------------------------------------------------------------------
void FramePlayer::CreateDX11Device()
{
	Logging::Logger::Info("FramePlayer::CreateDX11Device");

	static const D3D_FEATURE_LEVEL levels[] = {
		
		D3D_FEATURE_LEVEL_11_1,
		D3D_FEATURE_LEVEL_11_0,
		D3D_FEATURE_LEVEL_10_1,
		D3D_FEATURE_LEVEL_10_0,
		D3D_FEATURE_LEVEL_9_3,
		D3D_FEATURE_LEVEL_9_2,
		D3D_FEATURE_LEVEL_9_1
	};

	D3D_FEATURE_LEVEL FeatureLevel;
	MEDIA::ThrowIfFailed(D3D11CreateDevice(
		nullptr,
		D3D_DRIVER_TYPE_HARDWARE,
		nullptr,
		D3D11_CREATE_DEVICE_VIDEO_SUPPORT | D3D11_CREATE_DEVICE_BGRA_SUPPORT,
		levels,
		ARRAYSIZE(levels),
		D3D11_SDK_VERSION,
		&m_spDX11Device,
		&FeatureLevel,
		&m_spDX11DeviceContext
		));

	switch (FeatureLevel) {
	case D3D_FEATURE_LEVEL_11_1:
		break;
	case D3D_FEATURE_LEVEL_11_0:
		break;
	case D3D_FEATURE_LEVEL_10_1:
		break;
	case D3D_FEATURE_LEVEL_10_0:
		break;
	case D3D_FEATURE_LEVEL_9_3:
		break;
	case D3D_FEATURE_LEVEL_9_2:
		break;
	case D3D_FEATURE_LEVEL_9_1:
		break;
	}

	ComPtr<ID3D10Multithread> spMultithread;
	MEDIA::ThrowIfFailed(
		m_spDX11Device.Get()->QueryInterface(IID_PPV_ARGS(&spMultithread))
		);
	spMultithread->SetMultithreadProtected(TRUE);

	Logging::Logger::Info("FramePlayer::~CreateDX11Device");

	return;
}

//+-----------------------------------------------------------------------------
//
//  Function:   CreateBackBuffers
//
//  Synopsis:   Creates the D3D back buffers
//
//------------------------------------------------------------------------------
void FramePlayer::CreateBackBuffers()
{
	Logging::Logger::Info("FramePlayer::CreateBackBuffers");

	EnterCriticalSection(&m_critSec);

	Logging::Logger::Info("FramePlayer::CreateBackBuffers - Aquired Critical section");

	// make sure everything is released first;    
	if (m_spDX11Device)
	{
		// Acquire the DXGIdevice lock 
		CAutoDXGILock DXGILock(m_spDXGIManager);

		ComPtr<ID3D11Device> spDevice;
		MEDIA::ThrowIfFailed(
			DXGILock.LockDevice(/*out*/spDevice)
			);

		// swap chain does not exist - so create it
		if (m_spDX11SwapChain == nullptr)
		{
			DXGI_SWAP_CHAIN_DESC1 swapChainDesc = { 0 };

			// Don't use Multi-sampling
			swapChainDesc.SampleDesc.Count = 1;
			swapChainDesc.SampleDesc.Quality = 0;

			swapChainDesc.BufferUsage = DXGI_USAGE_BACK_BUFFER | DXGI_USAGE_RENDER_TARGET_OUTPUT;
			swapChainDesc.Scaling = DXGI_SCALING_NONE;
			swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;

			// Use more than 1 buffer to enable Flip effect.
			swapChainDesc.BufferCount = 4;

			// Most common swapchain format is DXGI_FORMAT_R8G8B8A8-UNORM
			swapChainDesc.Format = m_d3dFormat;
			//swapChainDesc.Width = m_rcTarget.right;
			//swapChainDesc.Height = m_rcTarget.bottom;
			swapChainDesc.Width = (LONG)m_imageWidth;
			swapChainDesc.Height = (LONG)m_imageHeight;

			// long QI chain to get DXGIFactory from the device
			ComPtr<IDXGIDevice2> spDXGIDevice;
			MEDIA::ThrowIfFailed(
				spDevice.Get()->QueryInterface(IID_PPV_ARGS(&spDXGIDevice))
				);

			// Ensure that DXGI does not queue more than one frame at a time. This both reduces 
			// latency and ensures that the application will only render after each VSync, minimizing 
			// power consumption.
			MEDIA::ThrowIfFailed(
				spDXGIDevice->SetMaximumFrameLatency(1)
				);

			ComPtr<IDXGIAdapter> spDXGIAdapter;
			MEDIA::ThrowIfFailed(
				spDXGIDevice->GetParent(IID_PPV_ARGS(&spDXGIAdapter))
				);

			ComPtr<IDXGIFactory2> spDXGIFactory;
			MEDIA::ThrowIfFailed(
				spDXGIAdapter->GetParent(IID_PPV_ARGS(&spDXGIFactory))
				);

			ComPtr<ID3D11VideoDevice> pDX11VideoDevice;
			MEDIA::ThrowIfFailed(
				spDevice.Get()->QueryInterface(__uuidof(ID3D11VideoDevice), (void**)&pDX11VideoDevice)
				);

			// and then we pass the device to the factory...
			MEDIA::ThrowIfFailed(
				spDXGIFactory.Get()->CreateSwapChainForCoreWindow(spDevice.Get(), reinterpret_cast<IUnknown*>((Windows::UI::Core::CoreWindow^)m_window.Get()), &swapChainDesc, nullptr, &m_spDX11SwapChain)
				);
		}
		else
		{
			// otherwise just resize it
			MEDIA::ThrowIfFailed(m_spDX11SwapChain->ResizeBuffers(
				4,
				m_rcTarget.right,
				m_rcTarget.bottom,
				m_d3dFormat,
				0
				));
		}
	}

	LeaveCriticalSection(&m_critSec);

	Logging::Logger::Info("FramePlayer::~CreateBackBuffers");

	return;
}

// Create a new instance of the Media Engine.
void FramePlayer::Initialize(Windows::UI::Core::CoreWindow^ window)
{
	Logging::Logger::Info("FramePlayer::Initialize");

	ComPtr<IMFMediaEngineClassFactory> spFactory;
	ComPtr<IMFAttributes> spAttributes;
	ComPtr<MediaEngineNotify> spNotify;

	HRESULT hr = S_OK;

	m_window = window;

	// Get the bounding rectangle of the window.
	m_rcTarget.left = 0;
	m_rcTarget.top = 0;
	m_rcTarget.right = (LONG)m_imageWidth;//m_window->Bounds.Width;
	m_rcTarget.bottom = (LONG)m_imageHeight;//m_window->Bounds.Height;
	//m_rcTarget.right = (LONG)m_window->Bounds.Width;
	//m_rcTarget.bottom = (LONG)m_window->Bounds.Height;

	MEDIA::ThrowIfFailed(MFStartup(MF_VERSION));

	EnterCriticalSection(&m_critSec);

	Logging::Logger::Info("FramePlayer::Initialize - Aquired critical section");

	if (!this->m_bInitiialized)
	{
		// Create DX11 device.    
		CreateDX11Device();
		m_bInitiialized = TRUE;
	}

	UINT resetToken;
	MEDIA::ThrowIfFailed(
		MFCreateDXGIDeviceManager(&resetToken, &m_spDXGIManager)
		);

	MEDIA::ThrowIfFailed(
		m_spDXGIManager->ResetDevice(m_spDX11Device.Get(), resetToken)
		);

	// Create our event callback object.
	spNotify = new MediaEngineNotify(m_window);
	if (spNotify == nullptr)
	{
		MEDIA::ThrowIfFailed(E_OUTOFMEMORY);
	}

	spNotify->MediaEngineNotifyCallback(this);

	// Create the class factory for the Media Engine.
	MEDIA::ThrowIfFailed(
		CoCreateInstance(CLSID_MFMediaEngineClassFactory, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&spFactory))
		);

	// Set configuration attribiutes.
	MEDIA::ThrowIfFailed(
		MFCreateAttributes(&spAttributes, 1)
		);

	MEDIA::ThrowIfFailed(
		spAttributes->SetUnknown(MF_MEDIA_ENGINE_DXGI_MANAGER, (IUnknown*)m_spDXGIManager.Get())
		);

	MEDIA::ThrowIfFailed(
		spAttributes->SetUnknown(MF_MEDIA_ENGINE_CALLBACK, (IUnknown*)spNotify.Get())
		);

	MEDIA::ThrowIfFailed(
		spAttributes->SetUINT32(MF_MEDIA_ENGINE_VIDEO_OUTPUT_FORMAT, m_d3dFormat)
		);

	// Create the Media Engine.
	const DWORD flags = MF_MEDIA_ENGINE_WAITFORSTABLE_STATE;
	MEDIA::ThrowIfFailed(
		spFactory->CreateInstance(flags, spAttributes.Get(), &m_spMediaEngine)
		);

	MEDIA::ThrowIfFailed(
		m_spMediaEngine.Get()->QueryInterface(__uuidof(IMFMediaEngine), (void**)&m_spEngineEx)
		);

	// Create/Update swap chain
	UpdateForWindowSizeChange();

	LeaveCriticalSection(&m_critSec);

	Logging::Logger::Info("FramePlayer::~Initialize");

	return;
}

// Shut down the player and release all interface pointers.
void FramePlayer::Shutdown()
{
	Logging::Logger::Info("FramePlayer::Shutdown");

	EnterCriticalSection(&m_critSec);

	Logging::Logger::Info("FramePlayer::Shutdown - Aquired critical section");

	StopTimer();

	if (m_spMediaEngine)
	{
		m_spMediaEngine->Shutdown();
	}

	if (m_spDX11SwapChain != nullptr)
	{
		//m_spDX11SwapChain->Release();
		m_spDX11SwapChain = nullptr;
	}
	if (m_spDX11DeviceContext != NULL)
	{
		//m_spDX11DeviceContext->Release();
		m_spDX11DeviceContext = nullptr;
	}
	if (m_spDX11Device != nullptr)
	{
		//m_spDX11Device->Release();
		m_spDX11Device = nullptr;
	}
	if (m_spDXGIManager != nullptr)
	{
		//m_spDXGIManager->CloseDeviceHandle();
		//m_spDXGIManager->Release();
		m_spDXGIManager = nullptr;
	}

	MFShutdown();
	
	this->m_bInitiialized = FALSE;

	if (nullptr != m_bstrURL)
	{
		//::CoTaskMemFree(m_bstrURL);
	}

	LeaveCriticalSection(&m_critSec);

	Logging::Logger::Info("FramePlayer::~Shutdown");

	return;
}

// Open File
void FramePlayer::OpenFile(Windows::Storage::StorageFile^ fileHandle)
{
	Logging::Logger::Info("FramePlayer::OpenFile");

	Logging::Logger::Info("Opening file" + fileHandle->Name);

	if (IsPlaying())
	{
		Pause();
	}
	if (m_fEOS)
	{
		StopTimer();
	}

	/*
	FileOpenPicker^ getVidFile = ref new FileOpenPicker();

	getVidFile->SuggestedStartLocation = PickerLocationId::VideosLibrary;
	getVidFile->ViewMode = PickerViewMode::Thumbnail;

	getVidFile->FileTypeFilter->Clear();
	getVidFile->FileTypeFilter->Append(".mp4");
	getVidFile->FileTypeFilter->Append(".m4v");
	getVidFile->FileTypeFilter->Append(".mts");
	getVidFile->FileTypeFilter->Append(".mov");
	getVidFile->FileTypeFilter->Append(".wmv");
	getVidFile->FileTypeFilter->Append(".wma");
	getVidFile->FileTypeFilter->Append(".avi");
	getVidFile->FileTypeFilter->Append(".wav");
	getVidFile->FileTypeFilter->Append(".mp3");

	m_pickFileTask = task<StorageFile^>(getVidFile->PickSingleFileAsync(), m_tcs.get_token());
	*/
	//m_pickFileTask.then([this](StorageFile^ fileHandle)
	{
		try
		{
			if (!fileHandle)
			{
				MEDIA::ThrowIfFailed(E_OUTOFMEMORY);
			}

			task<IRandomAccessStream^> fOpenStreamTask(fileHandle->OpenAsync(Windows::Storage::FileAccessMode::Read));

			SetURL(fileHandle->Path);

			auto vidPlayer = this;
			fOpenStreamTask.then([vidPlayer](IRandomAccessStream^ streamHandle)
			{
				try
				{
					vidPlayer->SetBytestream(streamHandle);
				}
				catch (Platform::Exception^ e)
				{
					MEDIA::ThrowIfFailed(E_UNEXPECTED);
				}
			});

		}
		catch (Platform::Exception^ e) 
		{
		}
	};

	Logging::Logger::Info("FramePlayer::~OpenFile");

	return;
}

// Set a URL
void FramePlayer::SetURL(Platform::String^ szURL)
{
	Logging::Logger::Info("FramePlayer::SetURL");

	if (nullptr != m_bstrURL)
	{
		::CoTaskMemFree(m_bstrURL);
		m_bstrURL = nullptr;
	}

	size_t cchAllocationSize = 1 + ::wcslen(szURL->Data());
	m_bstrURL = (LPWSTR)::CoTaskMemAlloc(sizeof(WCHAR)*(cchAllocationSize));

	if (m_bstrURL == 0)
	{
		MEDIA::ThrowIfFailed(E_OUTOFMEMORY);
	}

	StringCchCopyW(m_bstrURL, cchAllocationSize, szURL->Data());

	Logging::Logger::Info("FramePlayer::~SetURL");

	return;
}

// Set Bytestream
void FramePlayer::SetBytestream(IRandomAccessStream^ streamHandle)
{
	Logging::Logger::Info("FramePlayer::SetBytestream");

	HRESULT hr = S_OK;
	ComPtr<IMFByteStream> spMFByteStream = nullptr;

	MEDIA::ThrowIfFailed(
		MFCreateMFByteStreamOnStreamEx((IUnknown*)streamHandle, &spMFByteStream)
		);

	MEDIA::ThrowIfFailed(
		m_spEngineEx->SetSourceFromByteStream(spMFByteStream.Get(), m_bstrURL)
		);

	Logging::Logger::Info("FramePlayer::~SetBytestream");

	return;
}

void FramePlayer::OnMediaEngineEvent(DWORD meEvent)
{
	Logging::Logger::Debug("FramePlayer::OnMediaEngineEvent");

	switch (meEvent)
	{
	case MF_MEDIA_ENGINE_EVENT_LOADEDMETADATA:
	{
												 m_fEOS = FALSE;
	}
		break;
	case MF_MEDIA_ENGINE_EVENT_CANPLAY:
	{
										  // Start the Playback
										  double duration = m_spMediaEngine->GetDuration();
										  this->m_VideoStart = (float)(duration * this->m_startPercent/100);
										  SetPlaybackPosition(m_VideoStart);
										  Play();
	}
		break;
	case MF_MEDIA_ENGINE_EVENT_PLAY:
		m_fPlaying = TRUE;
		break;
	case MF_MEDIA_ENGINE_EVENT_PAUSE:
		m_fPlaying = FALSE;
		break;
	case MF_MEDIA_ENGINE_EVENT_ENDED:

		if (m_spMediaEngine->HasVideo())
		{
			StopTimer();
		}
		m_fEOS = TRUE;
		break;
	case MF_MEDIA_ENGINE_EVENT_TIMEUPDATE:
		break;
	case MF_MEDIA_ENGINE_EVENT_ERROR:
		break;
	}

	Logging::Logger::Debug("FramePlayer::~OnMediaEngineEvent");

	return;
}

// Start playback.
void FramePlayer::Play()
{
	Logging::Logger::Info("FramePlayer::Play");

	m_bEnd = false;

	if (m_spMediaEngine)
	{
		if (m_spMediaEngine->HasVideo())
		{
			if (m_fStopTimer)
			{
				// Start the Timer thread
				StartTimer();
			}
		}
		else
		{
			m_bEnd = true;
			m_bUnrecoverableError = true;
		}

		if (m_fEOS)
		{
			SetPlaybackPosition(0);
			m_fPlaying = TRUE;
		}
		else
		{
			MEDIA::ThrowIfFailed(
				m_spMediaEngine->Play()
				);

			MEDIA::ThrowIfFailed(
				m_spMediaEngine->SetVolume(0.0)
				);
		}

		m_fEOS = FALSE;
	}

	Logging::Logger::Info("FramePlayer::~Play");

	return;
}

// Pause playback.
void FramePlayer::Pause()
{
	Logging::Logger::Info("FramePlayer::Pause");

	if (m_spMediaEngine)
	{
		MEDIA::ThrowIfFailed(
			m_spMediaEngine->Pause()
			);
	}

	Logging::Logger::Info("FramePlayer::~Pause");

	return;
}

// Set the audio volume.
void FramePlayer::SetVolume(float fVol)
{
	Logging::Logger::Info("FramePlayer::SetVolume");

	if (m_spMediaEngine)
	{
		MEDIA::ThrowIfFailed(
			m_spMediaEngine->SetVolume(fVol)
			);
	}
	
	Logging::Logger::Info("FramePlayer::~SetVolume");

	return;
}

// Set the audio balance.
void FramePlayer::SetBalance(float fBal)
{
	Logging::Logger::Info("FramePlayer::SetBalance");

	if (m_spEngineEx)
	{
		MEDIA::ThrowIfFailed(
			m_spEngineEx->SetBalance(fBal)
			);
	}
	
	Logging::Logger::Info("FramePlayer::~SetBalance");

	return;
}

// Mute the audio.
void FramePlayer::Mute(BOOL mute)
{
	Logging::Logger::Info("FramePlayer::Mute");

	if (m_spMediaEngine)
	{
		MEDIA::ThrowIfFailed(
			m_spMediaEngine->SetMuted(mute)
			);
	}

	Logging::Logger::Info("FramePlayer::~Mute");

	return;
}

// Step forward one frame.
void FramePlayer::FrameStep()
{
	Logging::Logger::Info("FramePlayer::FrameStep");

	if (m_spEngineEx)
	{
		MEDIA::ThrowIfFailed(
			m_spEngineEx->FrameStep(TRUE)
			);
	}

	Logging::Logger::Info("FramePlayer::~FrameStep");

	return;
}

// Get the duration of the content.
void FramePlayer::GetDuration(double *pDuration, BOOL *pbCanSeek)
{
	Logging::Logger::Info("FramePlayer::GetDuration");

	if (m_spMediaEngine)
	{
		double duration = m_spMediaEngine->GetDuration();

		// NOTE:
		// "duration != duration"
		// This tests if duration is NaN, because NaN != NaN

		if (duration != duration || duration == std::numeric_limits<float>::infinity())
		{
			*pDuration = 0;
			*pbCanSeek = FALSE;
		}
		else
		{
			*pDuration = duration;

			DWORD caps = 0;
			m_spEngineEx->GetResourceCharacteristics(&caps);
			*pbCanSeek = (caps & ME_CAN_SEEK) == ME_CAN_SEEK;
		}
	}
	else
	{
		MEDIA::ThrowIfFailed(E_FAIL);
	}

	Logging::Logger::Info("FramePlayer::~GetDuration");

	return;
}

// Get the current playback position.
double FramePlayer::GetPlaybackPosition()
{
	Logging::Logger::Info("FramePlayer::GetPlaybackPosition");

	if (m_spMediaEngine)
	{
		return m_spMediaEngine->GetCurrentTime();
	}
	else
	{
		return 0;
	}

	Logging::Logger::Info("FramePlayer::~GetPlaybackPosition");
}

// Seek to a new playback position.
void FramePlayer::SetPlaybackPosition(float pos)
{
	Logging::Logger::Info("FramePlayer::SetPlaybackPosition");

	if (m_spMediaEngine)
	{
		MEDIA::ThrowIfFailed(
			m_spMediaEngine->SetCurrentTime(pos)
			);
	}

	Logging::Logger::Info("FramePlayer::~SetPlaybackPosition");
}

// Is the player in the middle of a seek operation?
BOOL FramePlayer::IsSeeking()
{
	Logging::Logger::Info("FramePlayer::IsSeeking");

	if (m_spMediaEngine)
	{
		return m_spMediaEngine->IsSeeking();
	}
	else
	{
		return FALSE;
	}

	Logging::Logger::Info("FramePlayer::~IsSeeking");
}

void FramePlayer::EnableVideoEffect(BOOL enable)
{
	Logging::Logger::Info("FramePlayer::EnableVideoEffect");

	HRESULT hr = S_OK;

	if (m_spEngineEx)
	{
		MEDIA::ThrowIfFailed(m_spEngineEx->RemoveAllEffects());
		if (enable)
		{
			ComPtr<IMFActivate> spActivate;
			LPCWSTR szActivatableClassId = WindowsGetStringRawBuffer((HSTRING)Windows::Media::VideoEffects::VideoStabilization->Data(), nullptr);

			MEDIA::ThrowIfFailed(MFCreateMediaExtensionActivate(szActivatableClassId, nullptr, IID_PPV_ARGS(&spActivate)));

			MEDIA::ThrowIfFailed(m_spEngineEx->InsertVideoEffect(spActivate.Get(), FALSE));
		}
	}

	Logging::Logger::Info("FramePlayer::~EnableVideoEffect");

	return;
}


//+-----------------------------------------------------------------------------
//
//  Function:   DXGIDeviceTrim
//
//  Synopsis:   Calls IDXGIDevice3::Trim() (requirement when app is suspended)
//
//------------------------------------------------------------------------------
HRESULT FramePlayer::DXGIDeviceTrim()
{
	Logging::Logger::Info("FramePlayer::DXGIDeviceTrim");

	HRESULT hr = S_OK;
	if (m_fUseDX && m_spDX11Device != nullptr)
	{
		IDXGIDevice3 *pDXGIDevice;
		hr = m_spDX11Device.Get()->QueryInterface(__uuidof(IDXGIDevice3), (void **)&pDXGIDevice);
		if (hr == S_OK)
		{
			pDXGIDevice->Trim();
		}
	}

	Logging::Logger::Info("FramePlayer::~DXGIDeviceTrim");

	return hr;
}


//+-----------------------------------------------------------------------------
//
//  Function:   ExitApp
//
//  Synopsis:   Checks if there has been an error and indicates if the app
//				should exit.
//
//------------------------------------------------------------------------------
BOOL FramePlayer::ExitApp()
{
	Logging::Logger::Info("FramePlayer::ExitApp");

	return m_fExitApp;
}

// Window Event Handlers
void FramePlayer::UpdateForWindowSizeChange()
{
	Logging::Logger::Info("FramePlayer::UpdateForWindowSizeChange");

	//if ((m_window->Bounds.Width != m_rcTarget.right ||
	//	m_window->Bounds.Height != m_rcTarget.bottom) ||
	//	m_spDX11SwapChain == nullptr)
	{
		// Get the bounding rectangle of the window.    
		m_rcTarget.left = 0;
		m_rcTarget.top = 0;
		m_rcTarget.right = (LONG)m_imageWidth; //(LONG)m_window->Bounds.Width;
		m_rcTarget.bottom = (LONG)m_imageHeight; //(LONG)m_window->Bounds.Height;
		//m_rcTarget.right = (LONG)m_window->Bounds.Width;
		//m_rcTarget.bottom = (LONG)m_window->Bounds.Height;

		if (m_spEngineEx)
		{
			CreateBackBuffers();
		}
	}

	Logging::Logger::Info("FramePlayer::~UpdateForWindowSizeChange");

	return;
}

//Timer related

//+-----------------------------------------------------------------------------
//
//  Function:   StartTimer
//
//  Synopsis:   Our timer is based on the displays VBlank interval
//
//------------------------------------------------------------------------------
void FramePlayer::StartTimer()
{
	Logging::Logger::Info("FramePlayer::StartTimer");

	ComPtr<IDXGIFactory1> spFactory;
	MEDIA::ThrowIfFailed(
		CreateDXGIFactory1(IID_PPV_ARGS(&spFactory))
		);

	ComPtr<IDXGIAdapter> spAdapter;
	MEDIA::ThrowIfFailed(
		spFactory->EnumAdapters(0, &spAdapter)
		);

	ComPtr<IDXGIOutput> spOutput;
	MEDIA::ThrowIfFailed(
		spAdapter->EnumOutputs(0, &m_spDXGIOutput)
		);

	m_fStopTimer = FALSE;

	auto vidPlayer = this;
	task<void> workItem(ThreadPool::RunAsync(ref new WorkItemHandler([=](IAsyncAction^ /*sender*/){
		vidPlayer->RealVSyncTimer();
	}
	),
		WorkItemPriority::High
		));

	Logging::Logger::Info("FramePlayer::~StartTimer");

	return;
}

//+-----------------------------------------------------------------------------
//
//  Function:   StopTimer
//
//  Synopsis:   Stops the Timer and releases all its resources
//
//------------------------------------------------------------------------------
void FramePlayer::StopTimer()
{
	Logging::Logger::Info("FramePlayer::StopTimer");

	m_fStopTimer = TRUE;
	m_fPlaying = FALSE;

	Logging::Logger::Info("FramePlayer::~StopTimer");

	return;
}

//+-----------------------------------------------------------------------------
//
//  Function:   realVSyncTimer
//
//  Synopsis:   A real VSyncTimer - a timer that fires at approx 60 Hz 
//              synchronized with the display's real VBlank interrupt.
//
//------------------------------------------------------------------------------
DWORD FramePlayer::RealVSyncTimer()
{
	Logging::Logger::Info("FramePlayer::RealVSyncTimer");

	for (int i = 0; ;i++)
	{
		if (m_fStopTimer)
		{
			//Shutdown();
			break;
		}

		if (SUCCEEDED(m_spDXGIOutput->WaitForVBlank()))
		{
			OnTimer(i);
		}
		else
		{
			break;
		}
	}

	Logging::Logger::Info("FramePlayer::~RealVSyncTimer");

	return 0;
}

//+-----------------------------------------------------------------------------
//
//  Function:   OnTimer
//
//  Synopsis:   Called at 60Hz - we simply call the media engine and draw
//              a new frame to the screen if told to do so.
//
//------------------------------------------------------------------------------
void FramePlayer::OnTimer(int i )
{

	Logging::Logger::Debug(i + "FramePlayer::OnTimer");

	EnterCriticalSection(&m_critSec);

	Logging::Logger::Debug(i + "FramePlayer::OnTimer - Aquired critical section");

	if (m_spMediaEngine != nullptr)
	{
		Logging::Logger::Debug(i + "FramePlayer::OnTimer - Media engine not null");

		LONGLONG pts;
		Logging::Logger::Debug(i + "FramePlayer::OnTimer - OnVideoStreamTick");
		HRESULT hr_abomination = m_spMediaEngine->OnVideoStreamTick(&pts);
		if (hr_abomination == S_OK)
		{
			Logging::Logger::Info(i + "FramePlayer:Abomination avoided...");

			Logging::Logger::Debug(i + "FramePlayer::OnTimer - GetCurrentTime");
			double currentTime = m_spMediaEngine->GetCurrentTime();
			Logging::Logger::Debug(i + "FramePlayer::OnTimer - GetStartTime");
			double startime = m_spMediaEngine->GetStartTime();
			Logging::Logger::Debug(i + "FramePlayer::OnTimer - GetDuration");
			double duration = m_spMediaEngine->GetDuration();

			Logging::Logger::Debug(i + "FramePlayer::OnTimer - Conditional");

			if ((int)this->m_FrameIndex >= this->m_NumberOfFrames)
			{
				Logging::Logger::Info(i + "FramePlayer::Done generating all images for file...");
				m_VideoLast = 0;
				m_bEnd = true;
			}
			else
			{
				Logging::Logger::Debug(i + "FramePlayer::OnTimer - Continuing");

				if (m_VideoLast == 0)
				{
					Logging::Logger::Debug(i + "FramePlayer::OnTimer - Last wideo time: 0");

					m_VideoLast = (float)currentTime;
				}
				else if (currentTime - m_VideoLast > m_FrameInterval)
				{
					Logging::Logger::Info(i + "FramePlayer::OnTimer Getting picture...");

					m_VideoLast = (float)currentTime;

					// new frame available at the media engine so get it 

					Logging::Logger::Debug(i + "FramePlayer::OnTimer - GetBuffer");

					ComPtr<ID3D11Texture2D> spTextureDst;
					MEDIA::ThrowIfFailed(
						m_spDX11SwapChain->GetBuffer(0, IID_PPV_ARGS(&spTextureDst))
						);

					auto rcNormalized = MFVideoNormalizedRect();
					rcNormalized.left = 0;
					rcNormalized.right = 1;
					rcNormalized.top = 0;
					rcNormalized.bottom = 1;

					Logging::Logger::Debug(i + "FramePlayer::OnTimer - TransferVideoFrame");

					HRESULT hr2 = m_spMediaEngine->TransferVideoFrame(spTextureDst.Get(), &rcNormalized, &m_rcTarget, &m_bkgColor);

					Logging::Logger::Debug(i + "FramePlayer::OnTimer - CaptureTexture");

					// capture an image from the DX11 texture
					DirectX::ScratchImage pImage;
				    HRESULT hr = DirectX::CaptureTexture(m_spDX11Device.Get(), m_spDX11DeviceContext.Get(), spTextureDst.Get(), pImage);

					if (SUCCEEDED(hr))
					{
						Logging::Logger::Debug(i + "FramePlayer::OnTimer - GetImage");

						// get the image object from the wrapper
						const DirectX::Image *pRealImage = pImage.GetImage(0, 0, 0);
						// set some place to save the image frame
						StorageFolder ^dataFolder = ApplicationData::Current->LocalFolder;
						m_FrameIndex++;
						wchar_t m_sFrameIndex[256];
						swprintf_s(m_sFrameIndex, L"%d", m_FrameIndex);
						Platform::String ^ x = ref new Platform::String(m_sFrameIndex);
						Platform::String ^szPath = dataFolder->Path + "\\" + m_hash + "\\" + x + ".png";

						hr = DirectX::SaveToWICFile(*pRealImage, DirectX::WIC_FLAGS_NONE, GUID_ContainerFormatPng, szPath->Data());
						Logging::Logger::Info(i + "FramePlayer::OnTimer Saved... " + szPath);	
					}

					Logging::Logger::Debug(i + "FramePlayer::OnTimer - Present");

					// and the present it to the screen
					MEDIA::ThrowIfFailed(
						m_spDX11SwapChain->Present(1, 0)
						);
				}
				else
				{
					Logging::Logger::Debug(i + "FramePlayer::OnTimer - Ignoring");
				}
			}
		}
		else
		{
			Logging::Logger::Debug(i + "FramePlayer::OnTimer - Ignoring abomination");
		}
	}

	LeaveCriticalSection(&m_critSec);

	if (m_bEnd)
	{
		m_fStopTimer = true;
	}

	Logging::Logger::Debug("FramePlayer::~OnTimer");

	return;
}


void FramePlayer::SetFileHash(Platform::String^ hash)
{
	Logging::Logger::Info("FramePlayer::SetFileHash");

	m_hash = hash;

	Logging::Logger::Info("FramePlayer::~SetFileHash");
}