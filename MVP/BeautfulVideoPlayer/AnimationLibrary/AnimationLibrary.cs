using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TCD.Serialization.Xml;
using Windows.Storage;

namespace AnimationLibrary
{
    public sealed class AnimationLibrary
    {
        public AnimationLibrary()
        {
            this.myLock = new object();
            this.IdToHashItems = new Dictionary<string, AnimationItem>();
            this.listOfAnimations = new List<AnimationItem>();
            this.LoadAsync();
        }

        /// <summary>
        /// Mapping File ID to the ID of stored animation
        /// </summary>
        private Dictionary<string, AnimationItem> IdToHashItems;

        List<AnimationItem> listOfAnimations;

        private object myLock;

        public void AddItem(AnimationItem item)
        {
            lock(myLock)
            {
                IdToHashItems[item.GetKey()] = item;
                this.listOfAnimations.Add(item);
            };

            SaveAsync();
        }

        public AnimationItem GetItem(string Id, AnimationType type)
        {
            lock (myLock)
            {
                return IdToHashItems[AnimationItem.GenerateKey(Id, type)];
            };
        }

        public bool DoesItemExist(string Id, AnimationType type)
        {
            lock (myLock)
            {
                if(IdToHashItems.ContainsKey(AnimationItem.GenerateKey(Id, type)))
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            };
        }

        void RemoveItem(string Id)
        {
            lock (myLock)
            {

            };
        }

        void DisposeItem(AnimationItem item)
        {

        }

        void GetItems(string Id, AnimationType type)
        {
            lock (myLock)
            {

            };
        }

        void RemoveItems(string folderName)
        {
            lock (myLock)
            {

            };
        }

        void RemoveAll()
        {
            lock (myLock)
            {

            };
        }

        private async void SaveAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder; 
            StorageFile file = await localFolder.CreateFileAsync("Animations.xml", CreationCollisionOption.OpenIfExists);
            try
            {

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    XmlSerializer xmlIzer = new XmlSerializer(typeof(List<AnimationItem>));
                    lock (myLock)
                    {
                        xmlIzer.Serialize(stream, this.listOfAnimations);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private  async Task LoadAsync()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync("Animations.xml", CreationCollisionOption.OpenIfExists);
                if (file != null)
                {
                    try
                    {
                        using (var stream = await file.OpenStreamForReadAsync())
                        {
                            lock (myLock)
                            {
                                if (stream.Length != 0)
                                {
                                    this.listOfAnimations = XmlDeSerializer.DeserializeFromStream(stream, typeof(List<AnimationItem>)) as List<AnimationItem>;
                                    foreach (var item in this.listOfAnimations)
                                    {
                                        IdToHashItems[item.GetKey()] = item;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
            }
                
        }
    }
}
