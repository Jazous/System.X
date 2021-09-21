using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data
{
    public enum QueryMode
    {
        [Newtonsoft.Json.JsonProperty("eq")]
        /// <summary>
        /// Equal
        /// </summary>
        Equal = 0,
        [Newtonsoft.Json.JsonProperty("nteq")]
        /// <summary>
        /// NotEqual
        /// </summary>
        NotEqual,
        [Newtonsoft.Json.JsonProperty("sw")]
        /// <summary>
        /// StartWith
        /// </summary>
        StartWith,
        [Newtonsoft.Json.JsonProperty("ew")]
        /// <summary>
        /// EndWith
        /// </summary>
        EndWith,
        [Newtonsoft.Json.JsonProperty("like")]
        /// <summary>
        /// Like
        /// </summary>
        Like,
        [Newtonsoft.Json.JsonProperty("in")]
        /// <summary>
        /// In
        /// </summary>
        In,
        [Newtonsoft.Json.JsonProperty("ntin")]
        /// <summary>
        /// NotIn
        /// </summary>
        NotIn,
        [Newtonsoft.Json.JsonProperty("gt")]
        /// <summary>
        /// GreaterThan
        /// </summary>
        GreaterThan,
        [Newtonsoft.Json.JsonProperty("gteq")]
        /// <summary>
        /// GreaterThanOrEqual
        /// </summary>
        GreaterThanOrEqual,
        [Newtonsoft.Json.JsonProperty("lt")]
        /// <summary>
        /// LessThan
        /// </summary>
        LessThan,
        [Newtonsoft.Json.JsonProperty("lteq")]
        /// <summary>
        /// LessThanOrEqual
        /// </summary>
        LessThanOrEqual
    }
}