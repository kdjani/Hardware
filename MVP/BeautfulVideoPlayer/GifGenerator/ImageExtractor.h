#pragma once

#include "FramePlayer.h"

using namespace Windows::Storage;

namespace ImageProcessing
{
	public ref class ImageExtractor sealed
    {
	private:
		FramePlayer^ m_player;

		bool m_initialized;

    public:
		ImageExtractor();

		void GenerateThumbnails(StorageFile^ file, Platform::String^ hash);

		bool IsGenerationFinished();

		bool IsUnrecoverableError();
    };
}