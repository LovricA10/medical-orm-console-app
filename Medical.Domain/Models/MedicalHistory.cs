using MyOrm.Attributes;

namespace Medical.Domain.Models
{
    [Table("medical_history")]
    public class MedicalHistory
    {
        [Key(AutoIncrement = true)]
        [Column("id", DbType.Int, IsNullable = false)]
        public int Id { get; set; }

        [Column("patient_id", DbType.Int, IsNullable = false)]
        [ForeignKey(typeof(Patient))]
        public int PatientId { get; set; }

        [Column("diagnosis_id", DbType.Int, IsNullable = false)]
        [ForeignKey(typeof(Diagnosis))]
        public int DiagnosisId { get; set; }

        [Column("start_date", DbType.TimestampWithoutTimezone, IsNullable = false)]
        public DateTime StartDate { get; set; }

        [Column("end_date", DbType.TimestampWithoutTimezone)]
        public DateTime? EndDate { get; set; }

        [Ignore] public Patient? Patient { get; set; }
        [Ignore] public Diagnosis? Diagnosis { get; set; }
    }
}
