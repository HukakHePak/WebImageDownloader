using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace WebImageDownloader
{
    internal class WebImageDownloader
    {
        string path = "С:\\";
        Action<int> OnLoad;
        int progress = 0;
        List<KeyValuePair<string, string>> links;
        readonly HttpClient client = new HttpClient();
        CancellationToken token;


        async Task ParseHTML(string url)
        {
            var html = await client.GetStringAsync(url);

            var chunks = html.Split(new string[] { "<img" }, StringSplitOptions.None);

            for(int i = 1; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                int start = chunk.IndexOf("src") + 5;
                int end = chunk.IndexOf("\"", start);

                if (start > -1)
                {
                    var fileUrl= chunk.Substring(start, end - start);

                    if (fileUrl[0] == '/')
                    {
                        fileUrl = url + fileUrl;
                    }

                    var crumbs = fileUrl.Split('/');

                    links.Add(new KeyValuePair<string, string>(fileUrl, path + '\\' + crumbs.Last()));
                }
            }
        }

        public async Task Download(string url)
        {
            progress = 0;
            OnLoad?.Invoke(progress);

            links = new List<KeyValuePair<string, string>>();
            await ParseHTML(url);

            var tasks = new Task[links.Count];

            for(int i = 0; i < tasks.Length; i++)
            {
                var pair = links[i];

                tasks[i] = Task.Run(async () =>
                {
                    await SaveImage(pair.Key, pair.Value);

                    progress += 100 / links.Count;

                    OnLoad?.Invoke(progress);
                }, token);
            }

            await Task.WhenAll(tasks);

            progress = 100;
            OnLoad?.Invoke(progress);
        }

        async Task SaveImage(string url, string path)
        {
            try
            {
                var img = await client.GetByteArrayAsync(url);                
                var file = File.OpenWrite(path);
       
                file.Write(img, 0, img.Length);
                file.Close();
            }
            catch { };
        }

        public void SetPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            this.path = path;
        }

        public void Stop()
        {
            token = new CancellationToken(true);
        }

        public void OnDownloadProgress(Action<int> action)
        {
            OnLoad += action;
        }



    }
}
