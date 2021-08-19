using System.Collections.Specialized;
using System.Text;

namespace System.IO
{
    public sealed class Profile
    {
        /// <summary>
        /// 将指定的键和值写到指定的节点，如果已经存在则替换
        /// </summary>
        /// <param name="fileName">文件路径。</param>
        /// <param name="section">节点名称。</param>
        /// <param name="key">键名称。如果为 null，则删除指定的节点及其所有的项目。</param>
        /// <param name="value">值内容。如果为 null，则删除指定节点中指定的键。</param>
        /// <returns>操作是否成功</returns>
        public bool Write(string fileName, string section, string key, string value)
        {
            return WritePrivateProfileString(section, key, value, fileName) != 0;
        }
        public bool Write(string fileName, string section, NameValueCollection collection)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(collection.GetKey(0));
            sb.Append("=");
            sb.Append(collection[0]);
            for (int i = 1; i < collection.Count; i++)
            {
                sb.Append('\0');
                sb.Append(collection.GetKey(i));
                sb.Append("=");
                sb.Append(collection[i]);
            }
            return WritePrivateProfileSection(section, sb.ToString(), fileName) != 0;
        }
        public NameValueCollection ReadSection(string fileName, string section)
        {
            NameValueCollection items = new NameValueCollection();
            byte[] buffer = new byte[0x8000];
            long length = GetPrivateProfileSection(section, buffer, buffer.GetUpperBound(0), fileName);
            if (length <= 0)
            {
                return items;
            }
            StringBuilder sb = new StringBuilder();
            byte b;
            char ch;
            string key = null;
            for (int i = 0; i < length; i++)
            {
                b = buffer[i];
                if (b != 0)
                {
                    ch = (char)b;
                    if (ch == '=')
                    {
                        key = sb.ToString();
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        items.Add(key, sb.ToString());
                        sb.Clear();
                    }
                }
            }
            return items;
        }
        public string Read(string fileName, string section, string key)
        {
            return Read(fileName, section, key, string.Empty);
        }
        public string Read(string fileName, string section, string key, string defaultValue)
        {
            StringBuilder buffer = new StringBuilder();
            GetPrivateProfileString(section, key, defaultValue, buffer, int.MaxValue, fileName);
            return buffer.ToString();
        }
        public bool ClearSection(string fileName, string section)
        {
            return WritePrivateProfileSection(section, string.Empty, fileName) != 0;
        }
        public bool RemoveSection(string fileName, string section)
        {
            return WritePrivateProfileSection(section, null, fileName) != 0;
        }

        /// <summary>
        /// 将指定的键值对写到指定的节点，如果已经存在则替换。
        /// </summary>
        /// <param name="lpAppName">节点，如果不存在此节点，则创建此节点</param>
        /// <param name="lpString">Item 键值对，多个用 \0 分隔,形如 key1=value1\0key2=value2
        /// <para>如果为 string.Empty，则删除指定节点下的所有内容，保留节点</para>
        /// <para>如果为 null，则删除指定节点下的所有内容，并且删除该节点</para>
        /// </param>
        /// <param name="lpFileName">INI文件</param>
        /// <returns>是否成功写入</returns>
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern long WritePrivateProfileSection(string lpAppName, string lpString, string lpFileName);
        /// <summary>
        /// Windows API 对INI文件写方法
        /// </summary>
        /// <param name="lpAppName">要在其中写入新字串的小节名称。这个字串不区分大小写</param>
        /// <param name="lpKeyName">要设置的项名或条目名。这个字串不区分大小写。用null可删除这个小节的所有设置项</param>
        /// <param name="lpString">指定为这个项写入的字串值。用null表示删除这个项现有的字串</param>
        /// <param name="lpFileName">初始化文件的名字。如果没有指定完整路径名，则windows会在windows目录查找文件。如果文件没有找到，则函数会创建它</param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern long WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);
        /// <summary>
        /// Windows API 对INI文件读方法
        /// </summary>
        /// <param name="lpAppName">欲在其中查找条目的小节名称。这个字串不区分大小写。如设为null，就在lpReturnedString缓冲区内装载这个ini文件所有小节的列表</param>
        /// <param name="lpKeyName">欲获取的项名或条目名。这个字串不区分大小写。如设为null，就在lpReturnedString缓冲区内装载指定小节所有项的列表</param>
        /// <param name="lpDefault">指定的条目没有找到时返回的默认值。可设为空（""）</param>
        /// <param name="lpReturnedString">指定一个字串缓冲区，长度至少为nSize</param>
        /// <param name="nSize">指定装载到lpReturnedString缓冲区的最大字符数量</param>
        /// <param name="lpFileName">初始化文件的名字。如没有指定一个完整路径名，windows就在Windows目录中查找文件</param>
        /// 注意：如lpKeyName参数为null，那么lpReturnedString缓冲区会载入指定小节所有设置项的一个列表。
        /// 每个项都用一个NULL字符分隔，最后一个项用两个NULL字符中止。也请参考GetPrivateProfileInt函数的注解
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern long GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, System.Text.StringBuilder lpReturnedString, int nSize, string lpFileName);
        [System.Runtime.InteropServices.DllImport("kernel32")]
        static extern long GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string lpFileName);
    }
}