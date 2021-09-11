using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections;

namespace System.X.Data
{
    internal sealed class DataHelper
    {
        internal static readonly DataHelper Instance = new DataHelper();
        static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propcache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        private DataHelper()
        {
        }

        PropertyInfo[] GetPropertyInfos(Type type)
        {
            if (!_propcache.ContainsKey(type))
            {
                var properties = type.GetProperties();
                _propcache.AddOrUpdate(type, properties, (a, b) => properties);
                return properties;
            }
            return _propcache[type];
        }
        /// <summary>
        /// Map dataTable's data to special generic list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="mapping">DataTable's column name mapping T's property name.</param>
        /// <exception cref="System.InvalidCastException"></exception>
        /// <exception cref="System.AggregateException"></exception>
        /// <returns></returns>
        public List<T> MapTo<T>(DataTable dataTable, params NameValue[] mapping) where T : new()
        {
            PropertyInfo[] properties = GetPropertyInfos(typeof(T));

            int colCount = dataTable.Columns.Count;
            int rowCount = dataTable.Rows.Count;

            if (rowCount == 0)
                return new List<T>(0);

            List<T> result = new List<T>(rowCount);
            for (int i = 0; i < rowCount; i++)
                result.Add(new T());

            bool hasMap = mapping != null && mapping.Any();
            for (int i = 0; i < colCount; i++)
            {
                string cName = dataTable.Columns[i].ColumnName;
                if (hasMap)
                {
                    string mpName = mapping.FirstOrDefault(c => string.Equals(c.Name, cName, StringComparison.OrdinalIgnoreCase)).Value;
                    if (mpName != null)
                        cName = mpName;
                }
                var prop = properties.FirstOrDefault(c => string.Equals(c.Name, cName, StringComparison.OrdinalIgnoreCase));
                if (prop == null) continue;

                using (var cts = new Threading.CancellationTokenSource())
                {
                    System.Threading.Tasks.Parallel.For(0, rowCount, new ParallelOptions() { MaxDegreeOfParallelism = 10, CancellationToken = cts.Token }, j =>
                    {
                        if (cts.IsCancellationRequested)
                            return;

                        var cell = dataTable.Rows[j][i];
                        if (cell == null || cell == DBNull.Value)
                            return;

                        try
                        {
                            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                                prop.SetValue(result[j], Convert.ChangeType(cell, prop.PropertyType.GetGenericArguments()[0]), null);
                                //new System.ComponentModel.NullableConverter(prop.PropertyType).UnderlyingType;
                            else
                                prop.SetValue(result[j], Convert.ChangeType(cell, prop.PropertyType), null);
                        }
                        catch (Exception ex)
                        {
                            if (cts.IsCancellationRequested)
                                return;

                            cts.Cancel();
                            throw new InvalidCastException($"Convert value {cell} to type {prop.PropertyType.Name} failed on row {j + 1} column {i + 1}.", ex);
                        }
                    });
                }
            }
            return result;
        }

        public DataTable MapTo<T>(IEnumerable<T> source, params NameValue[] mapping)
        {
            var result = new DataTable();
            if (!source.Any())
                return result;

            var properties = GetPropertyInfos(typeof(T));
            if (properties.Length == 0)
                return result;

            bool hasMap = mapping != null && mapping.Any();
            if (hasMap)
            {
                var pNameList = mapping.Select(c => c.Name).ToList();
                properties = properties.Where(c => pNameList.Contains(c.Name, StringComparer.OrdinalIgnoreCase)).ToArray();
                if (properties.Length == 0)
                    return result;
            }

            foreach (var prop in properties)
            {
                var ptype = prop.PropertyType;
                if (ptype.IsGenericType && (ptype.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    ptype = ptype.GetGenericArguments()[0];
                var columnName = hasMap ? mapping.Single(c =>string.Equals( c.Name, prop.Name, StringComparison.OrdinalIgnoreCase)).Value : prop.Name;
                result.Columns.Add(new DataColumn(columnName, ptype));
            }
            foreach (var item in source)
            {
                var tempList = new ArrayList();
                foreach (var pi in properties)
                    tempList.Add(pi.GetValue(item, null));
                result.LoadDataRow(tempList.ToArray(), true);
            }
            return result;
        }
    }
}