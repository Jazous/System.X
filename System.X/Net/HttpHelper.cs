using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace System.X.Net
{
    public sealed class HttpHelper
    {
        internal static readonly HttpHelper Instance = new HttpHelper();

        private HttpHelper() { }

        public Task<string> GET(string url)
        {
            return GetAsync(url, null, 20);
        }
        public Task<string> GET(string url, string token)
        {
            return GetAsync(url, token, 20);
        }
        public async Task<byte[]> GET_ToBytes(string url, string token)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        public async Task<string> GET_ToFile(string url, string token)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(20);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string fileName = Fn.NewDir() + response.Content.Headers.ContentDisposition.FileName;
                using (var fs = System.IO.File.OpenWrite(fileName))
                    await response.Content.CopyToAsync(fs);

                return fileName;
            }
        }
        async Task<string> GetAsync(string url, string token, int timeout)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }


        public Task<string> POST(string url, string jsonData)
        {
            var content = new StringContent(jsonData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            return PostAsync(url, content, 20);
        }
        public Task<string> POST(string url, string token, string jsonData)
        {
            var content = new StringContent(jsonData);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            return PostAsync(url, content, 20);
        }
        public Task<string> POST(string url, string token, IEnumerable<NameValue> formData)
        {
            return POST(url, token, formData, null);
        }
        public Task<string> POST(string url, string token, IEnumerable<NameValue> formData, byte[] fileBytes, string originFileName)
        {
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(fileBytes), originFileName, "files");
            if (formData != null && formData.Any())
                foreach (var data in formData)
                    content.Add(new StringContent(data.Value, Encoding.UTF8), data.Name);

            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));

            return PostAsync(url, content, 120);
        }
        public Task<string> POST(string url, string token, IEnumerable<NameValue> formData, params FileInfo[] files)
        {
            var content = new MultipartFormDataContent();
            if (files != null)
                foreach (var item in files)
                    content.Add(new ByteArrayContent(File.ReadAllBytes(item.FullName)), item.Name, "files");
            if (formData != null)
                foreach (var data in formData)
                    content.Add(new StringContent(data.Value, Encoding.UTF8), data.Name);

            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));

            return PostAsync(url, content, 120);
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="timemout">Set the seconds to wait before the request times out.</param>
        /// <returns>The task string representing the asynchronous operation.</returns>
        async Task<string> PostAsync(string url, HttpContent content, int timemout)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timemout);
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<byte[]> POST_ToBytes(string url, string token, string jsonData)
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmss"));
            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        public async Task<string> POST_ToFile(string url, string token, string jsonData)
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmss"));
            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string fileName = Fn.NewDir() + response.Content.Headers.ContentDisposition.FileName;
                using (var fs = System.IO.File.OpenWrite(fileName))
                    await response.Content.CopyToAsync(fs);

                return fileName;
            }
        }
    }
}