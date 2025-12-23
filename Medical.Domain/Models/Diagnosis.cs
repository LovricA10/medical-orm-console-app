using MyOrm.Attributes;

namespace Medical.Domain.Models
{
    [Table("diagnoses")]
    public class Diagnosis
    {
        [Key(AutoIncrement = true)]
        [Column("id", DbType.Int, IsNullable = false)]
        public int Id { get; set; }

        [Column("name", DbType.Varchar, Length = 200, IsNullable = false, IsUnique = true)]
        public string Name { get; set; } = "";
    }
}
