using MyOrm.Attributes;


namespace Medical.Domain.Models
{
    [Table("patients")]
    public class Patient
    {

        [Key(AutoIncrement = true)]
        [Column("id", DbType.Int, IsNullable = false)]
        public int Id { get; set; }

        [Column("first_name", DbType.Varchar, Length = 100, IsNullable = false)]
        public string FirstName { get; set; } = "";

        [Column("last_name", DbType.Varchar, Length = 100, IsNullable = false)]
        public string LastName { get; set; } = "";

        [Column("oib", DbType.Char, Length = 11, IsNullable = false, IsUnique = true)]
        public string Oib { get; set; } = "";

        [Column("date_of_birth", DbType.TimestampWithoutTimezone, IsNullable = false)]
        public DateTime DateOfBirth { get; set; }

        [Column("gender", DbType.Char, Length = 1, IsNullable = false)]
        public string Gender { get; set; } = "M";

        [Column("temporary_address", DbType.Text, IsNullable = false)]
        public string TemporaryAddress { get; set; } = "";

        [Column("permanent_address", DbType.Text, IsNullable = false)]
        public string PermanentAddress { get; set; } = "";

        [Ignore] public ICollection<MedicalHistory> MedicalHistories { get; set; } = [];
        [Ignore] public ICollection<Therapy> Therapies { get; set; } = [];
        [Ignore] public ICollection<Examination> Examinations { get; set; } = [];
    }
}
