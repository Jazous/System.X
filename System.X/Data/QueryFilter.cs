using System.Collections.Generic;

namespace System.Data
{
    [global::System.Runtime.Serialization.DataContract, global::System.Serializable]
    public sealed class QueryFilter
    {
        [global::System.Runtime.Serialization.DataMember]
        public List<QueryItem> Items { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int PageIndex { get; set; }
        [global::System.Runtime.Serialization.DataMember]
        public int PageSize { get; set; }

        public QueryFilter() : this(0, 50)
        {
        }
        public QueryFilter(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex < 0 ? 0 : pageIndex;
            this.PageSize = pageSize < 0 ? 50 : 0;
            this.Items = new List<QueryItem>();
        }

        public QueryFilter Add(QueryItem item)
        {
            this.Items.Add(item);
            return this;
        }
        public QueryFilter AddBetween(string name, int value1, int value2)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.Between, Values = new string[] { value1.ToString(), value2.ToString() } });
        }
        public QueryFilter AddContains(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Add(new QueryItem() { Name = name, Mode = QueryMode.Contains, Values = new string[] { value } });
            return this;
        }
        public QueryFilter AddEquals(string name, int value)
        {
            return AddEquals(name, value.ToString());
        }
        public QueryFilter AddEquals(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Add(new QueryItem() { Name = name, Mode = QueryMode.Equal, Values = new string[] { value } });
            return this;
        }
        public QueryFilter AddEquals(string name, params int[] values)
        {
            return AddEquals(name, Array.ConvertAll(values, c => c.ToString()));
        }
        public QueryFilter AddEquals(string name, params string[] values)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.Equal, Values = values });
        }
        public QueryFilter AddEndWith(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Add(new QueryItem() { Name = name, Mode = QueryMode.EndWith, Values = new string[] { value } });
            return this;
        }
        public QueryFilter AddExcept(string name, string[] value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.Except, Values = value });
        }
        public QueryFilter AddExcept(string name, int[] values)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.Except, Values = Array.ConvertAll(values, c => c.ToString()) });
        }
        public QueryFilter AddGreaterThan(string name, int value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.GreaterThan, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddGreaterThan(string name, DateTime value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.GreaterThan, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddGreaterThanOrEqual(string name, int value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.GreaterThanOrEqual, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddGreaterThanOrEqual(string name, DateTime value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.GreaterThanOrEqual, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddLessThan(string name, int value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.LessThan, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddLessThan(string name, DateTime value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.LessThan, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddLessThanOrEqual(string name, int value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.LessThanOrEqual, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddLessThanOrEqual(string name, DateTime value)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.LessThanOrEqual, Values = new string[] { value.ToString() } });
        }
        public QueryFilter AddNotEquals(string name, int[] values)
        {
            return AddNotEquals(name, Array.ConvertAll(values, c => c.ToString()));
        }
        public QueryFilter AddNotEquals(string name, string[] values)
        {
            return Add(new QueryItem() { Name = name, Mode = QueryMode.NotEqual, Values = values });
        }
        public QueryFilter AddStartWith(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Add(new QueryItem() { Name = name, Mode = QueryMode.StartWith, Values = new string[] { value } });
            return this;
        }
        public QueryFilter AddValueIn(string name, int[] values)
        {
            return AddValueIn(name, Array.ConvertAll(values, c => c.ToString()));
        }
        public QueryFilter AddValueIn(string name, string[] values)
        {
            return Add(new QueryItem() { Name = name, Mode = (values.Length == 1) ? QueryMode.Equal : QueryMode.In, Values = values });
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}