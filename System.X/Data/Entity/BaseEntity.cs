using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.Entity
{
    public abstract class BaseEntity : IEntity
    {
        public DateTime CreateTime { get; set; }
    }
    public abstract class BaseEntity<T> : BaseEntity, IEntity
    {
        public T Id { get; set; }
    }
}