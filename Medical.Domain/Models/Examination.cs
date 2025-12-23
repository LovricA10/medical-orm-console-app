using MyOrm.Attributes;

namespace Medical.Domain.Models
{
    [Table("examinations")]
    public class Examination
    {
        [Key(AutoIncrement = true)]
        [Column("id", DbType.Int, IsNullable = false)]
        public int Id { get; set; }

        [Column("patient_id", DbType.Int, IsNullable = false)]
        [ForeignKey(typeof(Patient))]
        public int PatientId { get; set; }

        [Column("doctor_id", DbType.Int, IsNullable = false)]
        [ForeignKey(typeof(Doctor))]
        public int DoctorId { get; set; }

        [Column("type", DbType.Varchar, Length = 30, IsNullable = false)]
        public string Type { get; set; } = "";

        [Column("scheduled_at", DbType.TimestampWithoutTimezone, IsNullable = false)]
        public DateTime ScheduledAt { get; set; }

        [Ignore] public Patient? Patient { get; set; }
        [Ignore] public Doctor? Doctor { get; set; }
    }
}
