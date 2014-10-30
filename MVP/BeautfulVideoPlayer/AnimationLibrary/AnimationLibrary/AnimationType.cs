using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnimationLibrary
{
    public enum AnimationType
    {
        None,

        /// <summary>
        /// Basic animation to be displayed
        /// </summary>
        [XmlEnum(Name = "1")]
        Category1Animation,

        /// <summary>
        /// To be displayed if more screen time
        /// </summary>
        [XmlEnum(Name = "2")]
        Category2Animation,

        /// <summary>
        /// To be displayed for even more screen time
        /// </summary>
        [XmlEnum(Name = "3")]
        Category3Animation,
    }
}
