using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFolders
{
    public enum Sorting
    {
        None,
        DateModified,
        DateModifiedDescending,
        DateCreated,
        DateCreatedDescending,
        Size,
        SizeDescending,
        Name,
        NameDescending,
        Resolution,
        ResolutionDescending,
        MostPlayed,
        MostPlayedDesc
    }
}
