namespace System.Data.Entity
{
    public class BaseEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime CreateTime { get; set; }
    }
}