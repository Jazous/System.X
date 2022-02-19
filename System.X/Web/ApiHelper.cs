namespace System.X.Web;

public sealed class ApiHelper
{
    internal static readonly ApiHelper Instance = new ApiHelper();

    private ApiHelper() { }

    public async Task<string> Get(string url, string? token, int timeout = 60)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            client.DefaultRequestHeaders.Add("Accept", "text/plain");
            client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
    public async Task<string> GetJson(string url, string? token, int timeout = 60)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
    public async Task<T> GetJson<T>(string url, string? token, int timeout = 60)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                using (var data = response.Content.ReadAsStream())
                    return System.Text.Json.JsonSerializer.Deserialize<T>(data);
            }
        }
    }
    public async Task<byte[]> GetBytes(string url, string? token, int timeout = 60)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
    }
    public async Task<Stream> GetStream(string url, string? token, int timeout = 60)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
        }
    }
    public async Task<string> GetFile(string url, string? token, int timeout = 60)
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();
                string fileName = Fn.NewTempDir() + response.Content.Headers.ContentDisposition.FileName;
                using (var fs = File.OpenWrite(fileName))
                {
                    await response.Content.CopyToAsync(fs);
                    fs.Flush();
                    return fileName;
                }
            }
        }
    }


    public Task<string> PostJson(string url, string? token, string jsonData, int timeout = 60)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8); 
        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
        return PostAsync(url, content, timeout);
    }
    public async Task<T> PostJson<T>(string url, string? token, string jsonData, int timeout = 60)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8);
        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));

        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;

        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
            client.Timeout = TimeSpan.FromSeconds(timeout);
            using (var response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                using (var data = response.Content.ReadAsStream())
                    return System.Text.Json.JsonSerializer.Deserialize<T>(data);
            }
        }
    }
    public Task<string> PostJsonWithSSL(string url, string? token, string jsonData, string cert, int timeout = 60)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8);
        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
        return PostAsync(url, content, cert, timeout);
    }
    public Task<string> Post(string url, string? token, IList<NameValue> formData, int timeout = 60)
    {
        var content = new MultipartFormDataContent();
        if (formData != null)
            for (int i = 0; i < formData.Count; i++)
                content.Add(new StringContent(formData[i].Value, System.Text.Encoding.UTF8), formData[i].Name);

        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
        return PostAsync(url, content, timeout);
    }
    public Task<string> PostFiles(string url, string? token, IList<NameValue> formData, IList<FileSystemInfo> files, int timeout = 60)
    {
        var content = new MultipartFormDataContent();
        if (formData != null)
            for (int i = 0; i < formData.Count; i++)
                content.Add(new StringContent(formData[i].Value, System.Text.Encoding.UTF8), formData[i].Name);

        for (int i = 0; i < files.Count; i++)
            content.Add(new ByteArrayContent(File.ReadAllBytes(files[i].FullName)), files[i].Name, "files");

        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
        return PostAsync(url, content, timeout);
    }
    public async Task<byte[]> PostJsonAsBytes(string url, string? token, string jsonData, int timeout = 60)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8);
        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            using (var response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
    }
    public async Task<Stream> PostJsonAsStream(string url, string? token, string jsonData, int timeout = 60)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8);
        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            using (var response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
        }
    }
    public async Task<string> PostJsonAsFile(string url, string? token, string jsonData, int timeout = 60)
    {
        var content = new StringContent(jsonData, System.Text.Encoding.UTF8);
        content.Headers.TryAddWithoutValidation("Authorization", token);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        content.Headers.TryAddWithoutValidation("Timestamp", DateTime.Now.ToString("yyyyMMddHHmmssttt"));
        using (var client = new System.Net.Http.HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            using (var response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                string fileName = Fn.NewTempDir() + response.Content.Headers.ContentDisposition.FileName;
                using (var fs = File.OpenWrite(fileName))
                {
                    await response.Content.CopyToAsync(fs);
                    fs.Flush();
                    return fileName;
                }
            }
        }
    }
    static async Task<string> PostAsync(string url, HttpContent content, int timeout)
    {
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;

        using (var client = new System.Net.Http.HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
            client.Timeout = TimeSpan.FromSeconds(timeout);
            using (var response = await client.PostAsync(url, content))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
    static async Task<string> PostAsync(string url, HttpContent content, string cert, int timeout)
    {
        if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;

        using (var handler = new System.Net.Http.HttpClientHandler())
        using (var x509cert = GetX509Certificate(cert))
        {
            handler.ClientCertificates.Add(x509cert);
            using (var client = new System.Net.Http.HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.AcceptCharset.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("utf-8"));
                client.Timeout = TimeSpan.FromSeconds(timeout);
                using (var response = await client.PostAsync(url, content))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }

    static Security.Cryptography.X509Certificates.X509Certificate GetX509Certificate(string cert)
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

    public string MineType(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            return "application/octet-stream";

        var ext = extension.ToLower();
        switch (ext)
        {
            case ".apk": return "application/vnd.android.package-archive";
            case ".jar": return "application/java-archive";
            case ".js": return "application/x-javascript";
            case ".mpc": return "application/vnd.mpohun.certificate";
            case ".acx": return "application/internet-property-stream";
            case ".crt":
            case ".cer": return "application/x-x509-ca-cert";
            case ".spc": return "application/x-pkcs7-certificates";
            case ".dll": return "application/x-msdownload";
            case ".sh": return "application/x-sh";
            case ".iii": return "application/x-iphone";

            case ".doc": return "application/msword";
            case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            case ".pdf": return "application/pdf";
            case ".rtf": return "application/rtf";
            case ".xla":
            case ".xlc":
            case ".xlm":
            case ".xlt":
            case ".xlw":
            case ".xls": return "application/vnd.ms-excel";
            case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            case ".pps":
            case ".ppt": return "application/vnd.ms-powerpoint";
            case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.prese";
            case ".wcm":
            case ".wdb":
            case ".wks":
            case ".wps": return "application/vnd.ms-works";
            case ".msg": return "application/vnd.ms-outlook";
            case ".mmp": return "application/vnd.ms-project";

            case ".rar": return "application/x-rar-compressed";
            case ".zip": return "application/x-zip-compressed";
            case ".gtar": return "application/x-gtar";
            case ".tar": return "application/x-tar";
            case ".gz": return "application/x-gzip";
            case ".z": return "application/x-compress";
            case ".tgz": return "application/x-compressed";

            case ".css": return "text/css";
            case ".html":
            case ".htm":
            case ".shtml": return "text/html";
            case ".c":
            case ".cpp":
            case ".h":
            case ".java":
            case ".class":
            case ".py":
            case ".cfg":
            case ".conf":
            case ".config":
            case ".cs":
            case ".log":
            case ".txt":
            case ".ini":
            case ".md":
            case ".xml": return "text/plain";
            case ".rtx": return "text/richtext";

            case ".kar":
            case ".midi":
            case ".mid": return "audio/midit";
            case ".mp2":
            case ".mp3": return "audio/mpeg";
            case ".ogg": return "audio/ogg";
            case ".m4a":
            case ".m4b":
            case ".m4p": return "audio/x-m4a-latm";
            case ".wav": return "audio/x-wav";
            case ".wma": return "audio/x-ms-wma";
            case ".wmv": return "audio/x-ms-wmv";
            case ".ra": return "audio/x-realaudio";
            case ".m3u": return "audio/x-mpegurl";

            case ".gif": return "image/gif";
            case ".jpe":
            case ".jpg":
            case ".jpeg": return "image/jpeg";
            case ".png": return "image/png";
            case ".tif":
            case ".tiff": return "image/tiff";
            case ".wbmp": return "image/vnd.wap.wbmp";
            case ".ico": return "image/x-icon";
            case ".jng": return "image/x-jng";
            case ".bmp": return "image/x-ms-bmp";
            case ".svg":
            case ".svgz": return "image/svg+xml";
            case ".webp": return "image/webp";

            case ".3gpp":
            case ".3gp": return "video/3gpp";
            case ".mp4": return "video/mp4";
            case ".mpv2":
            case ".mpe":
            case ".mpeg":
            case ".mpg":
            case ".mpga": return "video/mpeg";
            case ".mov": return "video/quicktime";
            case ".m4u": return "video/x-m4v";
            case ".webm": return "video/webm";
            case ".flv": return "video/x-flv";
            case ".m4v": return "video/x-m4v";
            case ".asf": return "video/x-ms-asf";
            case ".avi": return "video/x-msvideo";
            case ".rmvb": return "audio/x-pn-realaudio";
            case ".swf": return "application/x-shockwave-flash";

            default: return "application/octet-stream";
        }
    }
}