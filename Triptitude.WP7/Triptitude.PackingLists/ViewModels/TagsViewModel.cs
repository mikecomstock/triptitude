using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Triptitude.PackingLists.ViewModels
{
    public class TagsViewModel : INotifyPropertyChanged
    {
        public bool IsDataLoaded { get; private set; }
        public ObservableCollection<Tag> Tags { get; private set; }

        public TagsViewModel()
        {
            Tags = new ObservableCollection<Tag>();
        }

        private HttpWebRequest request;
        public void LoadData()
        {
            request = WebRequest.Create(@"http://www.triptitude.com/api/v1/tags") as HttpWebRequest;
            request.BeginGetResponse(AfterRequest, null);
        }

        private void AfterRequest(IAsyncResult result)
        {
            var response = request.EndGetResponse(result);
            using (StreamReader sd = new StreamReader(response.GetResponseStream()))
            {
                string json = sd.ReadToEnd();
                APIResponse parsed = JsonConvert.DeserializeObject<APIResponse>(json);

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    foreach (var tag in parsed.Tags)
                    {
                        Tags.Add(tag);
                    }

                });

                response.Close();
            }

            NotifyPropertyChanged("Tags");
            IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class APIResponse
    {
        public IEnumerable<Tag> Tags { get; set; }
    }

    public class Tag : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private string _niceName;
        public string NiceName
        {
            get { return _niceName; }
            set
            {
                if (value != _niceName)
                {
                    _niceName = value;
                    NotifyPropertyChanged("NiceName");
                }
            }
        }

        public ObservableCollection<Item> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Item : INotifyPropertyChanged
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
