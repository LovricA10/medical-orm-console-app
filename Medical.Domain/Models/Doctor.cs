using MyOrm.Attributes;


namespace Medical.Domain.Models
{
    [Table("doctors")]
    public class Doctor
    {
        [Key(AutoIncrement = true)]
        [Column("id", DbType.Int, IsNullable = false)]
        public int Id { get; set; }

        [Column("first_name", DbType.Varchar, Length = 100, IsNullable = false)]
        public string FirstName { get; set; } = "";

        [Column("last_name", DbType.Varchar, Length = 100, IsNullable = false)]
        public string LastName { get; set; } = "";

        [Column("specialization", DbType.Varchar, Length = 100, IsNullable = false)]
        public string Specialization { get; set; } = "";

        [Ignore] public ICollection<Examination> Examinations { get; set; } = [];
    }
}
