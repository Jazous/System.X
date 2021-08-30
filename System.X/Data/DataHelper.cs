using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System.X.Data
{
    public sealed class DataHelper
    {
        internal static readonly DataHelper Instance = new DataHelper();

        private DataHelper()
        {

        }

        public List<T> MapTo<T>(DataTable dataTable, IEnumerable<KeyValuePair<string, string>> headerMapping)
        {
            throw new NotImplementedException();
        }
        public List<TResult> MapTo<TResult, TSource>(IEnumerable<TSource> source)
        {
            throw new NotImplementedException();
        }
    }
}