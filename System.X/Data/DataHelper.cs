using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace System.X.Data
{
    public sealed class DataHelper
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
        /// <returns></returns>
        public List<T> MapTo<T>(DataTable dataTable, params KeyValuePair<string, string>[] mapping) where T : new()
        {
            PropertyInfo[] properties = GetPropertyInfos(typeof(T));

            int colCount = dataTable.Columns.Count;
            int rowCount = dataTable.Rows.Count;

            List<T> result = new List<T>(rowCount);
            for (int i = 0; i < rowCount; i++)
                result.Add(new T());

            bool hasMap = mapping != null && mapping.Any();
            for (int i = 0; i < colCount; i++)
            {
                string cName = dataTable.Columns[i].ColumnName;
                if (hasMap)
                {
                    string mpName = mapping.FirstOrDefault(c => string.Equals(c.Key, cName, StringComparison.OrdinalIgnoreCase)).Value;
                    if (mpName != null)
                        cName = mpName;
                }
                var prop = properties.FirstOrDefault(c => string.Equals(c.Name, cName, StringComparison.OrdinalIgnoreCase));
                if (prop == null) continue;

                System.Threading.Tasks.Parallel.For(0, rowCount, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, j =>
                 {
                     var cell = dataTable.Rows[j][i];
                     if (cell == null)
                         return;
                     try
                     {
                         if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                             prop.SetValue(result[j], Convert.ChangeType(cell, prop.PropertyType.GetGenericArguments()[0]), null);
                         else
                             prop.SetValue(result[j], Convert.ChangeType(cell, prop.PropertyType), null);
                     }
                     catch (InvalidCastException ex)
                     {
                         throw new InvalidCastException($"Convert value {cell} to type {prop.PropertyType.Name} failed on row {j} column {i}.", ex);
                     }
                 });
                //for (int j = 0; j < rowCount; j++)
                //{
                //    var cell = dataTable.Rows[j][i];
                //    if (cell == null)
                //        continue;
                //    try
                //    {
                //        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                //            prop.SetValue(result[j], Convert.ChangeType(cell, prop.PropertyType.GetGenericArguments()[0]), null);
                //        else
                //            prop.SetValue(result[j], Convert.ChangeType(cell, prop.PropertyType), null);
                //    }
                //    catch (InvalidCastException ex)
                //    {
                //        throw new InvalidCastException($"Convert value {cell} to type {prop.PropertyType.Name} failed on row {j} column {i}.", ex);
                //    }
                //}
            }
            return result;
        }
        public List<TResult> MapTo<TResult, TSource>(IEnumerable<TSource> source, params KeyValuePair<string, string>[] mapping)
        {
            throw new NotImplementedException();
        }
    }
}