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

        public async Task<string> GET(string url, string token = null, int timeout = 60)
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
        public async Task<byte[]> GET_ToBytes(string url, string token = null, int timeout = 60)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        public async Task<string> GET_ToFile(string url, string token = null, int timeout = 60)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string fileName = Fn.NewDir() + response.Content.Headers.ContentDisposition.FileName;
                using (var fs = System.IO.File.OpenWrite(fileName))
                    await response.Content.CopyToAsync(fs);

                return fileName;
            }
        }


        public Task<string> POST(string url, string jsonData, string token = null, int timeout = 60)
        {
            var content = new StringContent(jsonData);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            return PostAsync(url, content, timeout);
        }
        public Task<string> POST(string url, IEnumerable<NameValue> formData, string token = null, int timeout = 60)
        {
            return POST(url, formData, (string)null, token, timeout);
        }
        public Task<string> POST(string url, IEnumerable<NameValue> formData, string srcFileName, string token = null, int timeout = 60)
        {
            var content = new MultipartFormDataContent();
            if (!string.IsNullOrEmpty(srcFileName))
                content.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(srcFileName)), Path.GetFileName(srcFileName), "files");
            if (formData != null && formData.Any())
                foreach (var data in formData)
                    content.Add(new StringContent(data.Value, Encoding.UTF8), data.Name);

            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));

            return PostAsync(url, content, timeout);
        }
        public Task<string> POST(string url, IEnumerable<NameValue> formData, IEnumerable<FileSystemInfo> files, string token = null, int timeout = 60)
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
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));

            return PostAsync(url, content, timeout);
        }
        public async Task<byte[]> POST_ToBytes(string url, string jsonData, string token = null, int timeout = 60)
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        public async Task<string> POST_ToFile(string url, string jsonData, string token = null, int timeout = 60)
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string fileName = Fn.NewDir() + response.Content.Headers.ContentDisposition.FileName;
                using (var fs = System.IO.File.OpenWrite(fileName))
                    await response.Content.CopyToAsync(fs);

                return fileName;
            }
        }
        public async Task<Stream> POST_ToStream(string url, string jsonData, string token = null, int timeout = 60)
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(timeout);
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
        }
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

        public async Task<string> GET_SSL(string url, string cert, string token = null, int timeout = 60)
        {
            using (var handler = new System.Net.Http.HttpClientHandler())
            using (var x509cert = GetX509Certificate(cert))
            {
                handler.ClientCertificates.Add(x509cert);
                using (var client = new System.Net.Http.HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(timeout);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        public Task<string> POST_SSL(string url, string jsonData, string cert, string token = null, int timeout = 60)
        {
            var content = new StringContent(jsonData);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            return PostAsync(url, content, cert, timeout);
        }
        public Task<string> POST_SSL(string url, IEnumerable<NameValue> formData, string cert, string token = null, int timeout = 60)
        {
            var content = new MultipartFormDataContent();
            if (formData != null)
                foreach (var data in formData)
                    content.Add(new StringContent(data.Value, Encoding.UTF8), data.Name);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
            return PostAsync(url, content, cert, timeout);
        }
        async Task<string> PostAsync(string url, HttpContent content, string cert, int timemout)
        {
            using (var handler = new System.Net.Http.HttpClientHandler())
            using (var x509cert = GetX509Certificate(cert))
            {
                handler.ClientCertificates.Add(x509cert);
                using (var client = new System.Net.Http.HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(timemout);
                    var response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        Security.Cryptography.X509Certificates.X509Certificate GetX509Certificate(string cert)
        {
            if (cert.EndsWith(".cer"))
                return Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(cert);

            using (var store = new Security.Cryptography.X509Certificates.X509Store(Security.Cryptography.X509Certificates.StoreName.Root, Security.Cryptography.X509Certificates.StoreLocation.LocalMachine))
            {
                store.Open(Security.Cryptography.X509Certificates.OpenFlags.ReadOnly | Security.Cryptography.X509Certificates.OpenFlags.OpenExistingOnly);
                var collection = store.Certificates.Find(Security.Cryptography.X509Certificates.X509FindType.FindBySubjectName, cert, false);
                if (collection != null && collection.Count > 0)
                    return collection[0];
            }
            return null;
        }
    }
}