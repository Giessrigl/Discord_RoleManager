using System.ComponentModel.DataAnnotations;

namespace DatabaseClasses
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
