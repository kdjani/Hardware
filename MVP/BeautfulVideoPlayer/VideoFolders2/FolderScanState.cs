using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFolders
{
    public enum FolderScanState
    {
        None,
        Scanned,
        PartiallyScanned,
        ShallowScanned,
        Error,
    }
}
