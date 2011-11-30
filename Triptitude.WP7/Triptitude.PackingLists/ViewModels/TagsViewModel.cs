using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using Newtonsoft.Json;

namespace Triptitude.PackingLists.ViewModels
{
    public class TagsViewModel
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
            TryLoadFromIsolatedStorage();

            request = WebRequest.Create(@"http://www.triptitude.com/api/v1/tags") as HttpWebRequest;
            request.BeginGetResponse(RequestCallback, null);
        }

        private void RequestCallback(IAsyncResult result)
        {
            try
            {
                string json;
                using (var response = request.EndGetResponse(result))
                using (StreamReader sd = new StreamReader(response.GetResponseStream()))
                {
                    json = sd.ReadToEnd();
                }

                SaveToIsolatedStorage(json);
                TryLoadFromIsolatedStorage();
            }
            catch { }
        }

        private void SaveToIsolatedStorage(string tagJson)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["tagJson"] = tagJson;
        }

        private void TryLoadFromIsolatedStorage()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            string tagJson;

            if (!settings.TryGetValue("tagJson", out tagJson))
            {
                // Fallback to tags.json content file
                var sri = Application.GetResourceStream(new Uri("ViewModels/tags.json", UriKind.Relative));
                using (sri.Stream)
                using (StreamReader streamReader = new StreamReader(sri.Stream))
                {
                    tagJson = streamReader.ReadToEnd();
                }
            }

            APIResponse parsed = JsonConvert.DeserializeObject<APIResponse>(tagJson);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Tags.Clear();
                foreach (var tag in parsed.Tags.Where(t => t.Items.Count() > 1))
                {
                    Tags.Add(tag);
                }
            });
            IsDataLoaded = true;
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
