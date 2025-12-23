using MyOrm.Attributes;


namespace Medical.Domain.Models
{
    [Table("therapies")]
    public class Therapy
    {
        [Key(AutoIncrement = true)]
        [Column("id", DbType.Int, IsNullable = false)]
        public int Id { get; set; }

        [Column("patient_id", DbType.Int, IsNullable = false)]
        [ForeignKey(typeof(Patient))]
        public int PatientId { get; set; }

        [Column("medication_id", DbType.Int, IsNullable = false)]
        [ForeignKey(typeof(Medication))]
        public int MedicationId { get; set; }

        [Column("dose", DbType.Decimal, IsNullable = false)]
        public decimal Dose { get; set; }

        [Column("unit", DbType.Varchar, Length = 20, IsNullable = false)]
        public string Unit { get; set; } = "";

        [Column("frequency", DbType.Varchar, Length = 100, IsNullable = false)]
        public string Frequency { get; set; } = "";

        [Column("start_date", DbType.TimestampWithoutTimezone, IsNullable = false)]
        public DateTime StartDate { get; set; }

        [Column("end_date", DbType.TimestampWithoutTimezone)]
        public DateTime? EndDate { get; set; }

        [Column("note", DbType.Text)]
        public string? Note { get; set; }

        [Ignore] public Patient? Patient { get; set; }
        [Ignore] public Medication? Medication { get; set; }
    }
}
