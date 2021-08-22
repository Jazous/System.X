using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data
{
    public enum QueryMode
    {
        Equal = 0,
        NotEqual,
        NotNull,
        Between,
        StartWith,
        EndWith,
        Contains,
        ElementIn,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }
}