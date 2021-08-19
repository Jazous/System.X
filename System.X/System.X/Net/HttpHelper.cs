﻿using System;
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
        async Task<string> GET(string url, string token)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> GET_Download(string url, string token)
        {
            string path = Path.GetTempFileName();
            await GET_Download(url, token, path);
            return path;
        }
        async Task GET_Download(string url, string token, string path)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                using (var fileStream = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                    fileStream.Flush();
                }
            }
        }
        async Task<byte[]> GET_DownloadBytes(string url, string token)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }


        public async Task<string> POST(string url, string token, string jsonData)
        {
            var content = new StringContent(jsonData);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmss"));
            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        public async Task<string> POST_Form(string url, string token, IEnumerable<NameValue> formData, params FileInfo[] files)
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
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmss"));

            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
        public async Task<string> POST_Download(string url, string token, string jsonData)
        {
            string path = Path.GetTempFileName();
            await POST_Download(url, token, jsonData, path);
            return path;
        }
        async Task POST_Download(string url, string token, string jsonData, string path)
        {
            var content = new StringContent(jsonData, Encoding.UTF8);
            content.Headers.TryAddWithoutValidation("Authorization", token);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.TryAddWithoutValidation("TimeStamp", DateTime.Now.ToString("yyyyMMddHHmmss"));
            using (var client = new System.Net.Http.HttpClient())
            {
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                using (var fileStream = System.IO.File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                    fileStream.Flush();
                }
            }
        }
        async Task<byte[]> POST_DownloadBytes(string url, string token, string jsonData)
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
    }
}