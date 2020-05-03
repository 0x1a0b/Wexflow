﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Wexflow.Core;

namespace Wexflow.Tasks.HttpPost
{
    public class HttpPost : Task
    {
        public string Url { get; private set; }
        public string Payload { get; private set; }
        public string Authorization { get; private set; }
        public string Bearer { get; private set; }

        public HttpPost(XElement xe, Workflow wf) : base(xe, wf)
        {
            Url = GetSetting("url");
            Payload = GetSetting("payload");
            Authorization = GetSetting("authorization");
            Bearer = GetSetting("bearer");
        }

        public override TaskStatus Run()
        {
            Info("Executing POST request...");
            var status = Status.Success;
            try
            {
                var postTask = Post(Url, Authorization, Bearer, Payload);
                postTask.Wait();
                var result = postTask.Result;
                var destFile = Path.Combine(Workflow.WorkflowTempFolder, string.Format("HttpPost_{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
                File.WriteAllText(destFile, result);
                Files.Add(new FileInf(destFile, Id));
                InfoFormat("POST request {0} executed whith success -> {1}", Url, destFile);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception e)
            {
                ErrorFormat("An error occured while executing the POST request {0}: {1}", Url, e.Message);
                status = Status.Error;
            }
            Info("Task finished.");
            return new TaskStatus(status);
        }

        public async System.Threading.Tasks.Task<string> Post(string url, string auth, string bearer, string payload)
        {
            var httpContent = new StringContent(payload, Encoding.UTF8);
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrEmpty(auth))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
                }
                else if (!string.IsNullOrEmpty(bearer))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
                }
                var httpResponse = await httpClient.PostAsync(url, httpContent);
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    return responseContent;
                }
            }
            return string.Empty;
        }

    }
}
