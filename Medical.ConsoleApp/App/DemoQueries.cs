using Medical.Domain.Models;
using MyOrm.Core;
using Spectre.Console;


namespace Medical.ConsoleApp.App
{
    public static class DemoQueries
    {
        public static void PrintPatientEager(OrmContext db, int patientId)
        {
            ArgumentNullException.ThrowIfNull(db);

            var patient = db.GetById<Patient>(patientId);
            if (patient is null)
            {
                AnsiConsole.MarkupLine("[red]Not found.[/]");
                return;
            }

            var diagnosesById = BuildDiagnosisMap(db);
            var medicationsById = BuildMedicationMap(db);

            var history = db.SelectWhere<MedicalHistory>("patient_id = @p0", patientId);
            var therapies = db.SelectWhere<Therapy>("patient_id = @p0", patientId);
            var exams = db.SelectWhere<Examination>("patient_id = @p0", patientId);
            var doctorsById = BuildDoctorMap(db);

            AnsiConsole.Write(BuildPatientPanel(patient));
            RenderHistory(history, diagnosesById);
            RenderTherapies(therapies, medicationsById);
            RenderExaminations(exams, doctorsById);

            AnsiConsole.MarkupLine("[blue]EAGER loading[/]: related data loaded explicitly (manual joins).");
        }

        public static void ChangeTrackingDemo(OrmContext db)
        {
            ArgumentNullException.ThrowIfNull(db);

            var patients = db.SelectAll<Patient>();
            if (patients.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No patients.[/]");
                return;
            }

            var patient = patients[^1];

            var oldFirstName = patient.FirstName;
            patient.FirstName = $"Tracked_{DateTime.Now:HHmmss}";

            var saved = db.SaveChanges();

            var panel = new Panel(
                $"[grey]PatientId:[/] {patient.Id}\n" +
                $"[grey]FirstName:[/] {oldFirstName} -> [bold]{patient.FirstName}[/]\n" +
                $"[grey]Saved updates:[/] [green]{saved}[/]")
            {
                Header = new PanelHeader("Change tracking demo", Justify.Left)
            };

            AnsiConsole.Write(panel);
        }

        private static IDictionary<int, string> BuildDiagnosisMap(OrmContext db)
        {
            var all = db.SelectAll<Diagnosis>();
            return all.ToDictionary(x => x.Id, x => x.Name);
        }

        private static IDictionary<int, string> BuildMedicationMap(OrmContext db)
        {
            var all = db.SelectAll<Medication>();
            return all.ToDictionary(x => x.Id, x => x.Name);
        }
        private static IDictionary<int, string> BuildDoctorMap(OrmContext db)
        {
            var all = db.SelectAll<Doctor>();
            return all.ToDictionary(x => x.Id, x => $"{x.FirstName} {x.LastName} ({x.Specialization})");
        }

        private static Panel BuildPatientPanel(Patient p) =>
            new(
                $"[bold]{p.FirstName} {p.LastName}[/]\n" +
                $"[grey]Id:[/] {p.Id}\n" +
                $"[grey]OIB:[/] {p.Oib}\n" +
                $"[grey]DOB:[/] {p.DateOfBirth:yyyy-MM-dd}\n" +
                $"[grey]Gender:[/] {p.Gender}\n" +
                $"[grey]Temp:[/] {p.TemporaryAddress}\n" +
                $"[grey]Perm:[/] {p.PermanentAddress}")
            {
                Header = new PanelHeader("Patient", Justify.Left)
            };

        private static void RenderHistory(IReadOnlyList<MedicalHistory> history, IDictionary<int, string> diagnosesById)
        {
            if (history.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No history rows.[/]");
                return;
            }

            var table = new Table().RoundedBorder().Title("History");
            table.AddColumn("Id");
            table.AddColumn("Diagnosis");
            table.AddColumn("Start");
            table.AddColumn("End");

            foreach (var h in history)
            {
                diagnosesById.TryGetValue(h.DiagnosisId, out var dxName);

                table.AddRow(
                    h.Id.ToString(),
                    dxName ?? "N/A",
                    h.StartDate.ToString("yyyy-MM-dd"),
                    h.EndDate?.ToString("yyyy-MM-dd") ?? "");
            }

            AnsiConsole.Write(table);
        }

        private static void RenderTherapies(IReadOnlyList<Therapy> therapies, IDictionary<int, string> medicationsById)
        {
            if (therapies.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No therapy rows.[/]");
                return;
            }

            var table = new Table().RoundedBorder().Title("Therapies");
            table.AddColumn("Id");
            table.AddColumn("Medication");
            table.AddColumn("Dose");
            table.AddColumn("Frequency");
            table.AddColumn("Start");
            table.AddColumn("End");

            foreach (var t in therapies)
            {
                medicationsById.TryGetValue(t.MedicationId, out var medName);

                table.AddRow(
                    t.Id.ToString(),
                    medName ?? "N/A",
                    $"{t.Dose}{t.Unit}",
                    t.Frequency,
                    t.StartDate.ToString("yyyy-MM-dd"),
                    t.EndDate?.ToString("yyyy-MM-dd") ?? "");
            }

            AnsiConsole.Write(table);
        }
        private static void RenderExaminations(IReadOnlyList<Examination> exams, IDictionary<int, string> doctorsById)
        {
            if (exams.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]No examinations.[/]");
                return;
            }

            var table = new Table().RoundedBorder().Title("Examinations");
            table.AddColumn("Id");
            table.AddColumn("Type");
            table.AddColumn("Scheduled");
            table.AddColumn("Doctor");

            foreach (var e in exams.OrderBy(x => x.ScheduledAt))
            {
                doctorsById.TryGetValue(e.DoctorId, out var docName);

                table.AddRow(
                    e.Id.ToString(),
                    e.Type,
                    e.ScheduledAt.ToString("yyyy-MM-dd HH:mm"),
                    docName ?? "N/A"
                );
            }

            AnsiConsole.Write(table);
        }

    }
}
