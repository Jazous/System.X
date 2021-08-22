using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace System.Data.Entity
{
    public class BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime UpdateTime { get; set; }
        public DateTime CreateTime { get; set; }
    }
}