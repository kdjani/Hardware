using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFolders
{
    public interface IScanner
    {
        void Initialize(FileLibrary fileLibrary);
        void StartScan();
        void PauseScan();
        void ResumeScan();
        void StopScan();
    }
}
