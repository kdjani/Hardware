using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml.Media.Imaging;

namespace AnimationLibrary
{
    public sealed class AnimationItem
    {
        public AnimationItem()
        {

        }

        public AnimationItem(string id, string folderName, AnimationType type)
        {
            this.id = id;
            this.folderName = folderName;
            this.type = type;
        }

        
        private AnimationType type;

        //private object animatedGif;

        //private List<WriteableBitmap> frames;

        
        private string id;

        
        private string folderName;

        public string GetKey()
        {
            return id + "_" + type.ToString();
        }

        public static string GenerateKey(string id, AnimationType type)
        {
            return id + "_" + type.ToString();
        }

        [XmlAttribute]
        public string AnimationPath
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        [XmlAttribute]
        public string FolderName
        {
            get
            {
                return this.folderName;
            }
            set
            {
                this.folderName = value;
            }
        }

        [XmlAttribute]
        public AnimationType TypeOfAnimation
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}
