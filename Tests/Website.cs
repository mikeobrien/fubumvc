using System;
using System.IO;
using System.Net;
using FubuCore;
using Microsoft.Web.Administration;

namespace Tests
{
    public class Website
    {
        private static readonly Random Random = new Random();
        private string _name;
        private int _port;

        public void Create(string name, string path)
        {
            if (!Path.IsPathRooted(path)) path = Path.GetFullPath(path);
            _port = Random.Next(30000, 30100);
            using (var manager = new ServerManager())
            {
                _name = name + Guid.NewGuid().ToString("N");
                manager.Sites.Add(_name, "http", "*:{0}:".ToFormat(_port), path);
                manager.CommitChanges();
            } 
        }

        public void Remove()
        {
            using (var manager = new ServerManager())
            {
                var site = manager.Sites[_name];
                site.Stop();
                manager.Sites.Remove(site);
                manager.CommitChanges();
            }
        }

        public string DownloadString(string url = "", string contentType = null)
        {
            try
            {
                using (var client = new WebClient())
                {
                    if (contentType != null) client.Headers.Add("accept", contentType);
                    url = "http://localhost:{0}/{1}".ToFormat(_port, url);
                    Console.WriteLine("Downloading {0}", url);
                    return client.DownloadString(url);
                }
            }
            catch (WebException exception)
            {
                var response = exception.Response as HttpWebResponse;
                if (response != null)
                {
                    var responseStream = response.GetResponseStream();
                    Console.WriteLine("{0}: {1}", response.StatusCode, response.StatusDescription);
                    Console.WriteLine(responseStream != null ? new StreamReader(responseStream).ReadToEnd() : "No response from web server.");
                }
                throw;
            }
        }
    }
}