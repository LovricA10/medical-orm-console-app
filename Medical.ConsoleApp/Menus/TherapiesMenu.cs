using Medical.ConsoleApp.Crud;
using Medical.ConsoleApp.Ui;
using Medical.Domain.Models;
using MyOrm.Core;
using Spectre.Console;

namespace Medical.ConsoleApp.Menus
{
    internal sealed class TherapiesMenu
    {
        private readonly OrmContext _db;

        public TherapiesMenu(OrmContext db)
        {
            ArgumentNullException.ThrowIfNull(db);
            _db = db;
        }

        public void Run() =>
            CrudMenu.Run(
                title: "Therapies",
                list: List,
                add: Add,
                update: Update,
                delete: Delete,
                extra: new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Tracked update"] = TrackedUpdate
                });

        private void List()
        {
            var rows = _db.SelectAll<Therapy>();

            var t = new Table().RoundedBorder()
                .AddColumn("Id")
                .AddColumn("PatientId")
                .AddColumn("MedicationId")
                .AddColumn("Dose")
                .AddColumn("Frequency");

            foreach (var x in rows)
                t.AddRow(
                    x.Id.ToString(),
                    x.PatientId.ToString(),
                    x.MedicationId.ToString(),
                    $"{x.Dose}{x.Unit}",
                    x.Frequency);

            AnsiConsole.Write(t);
        }

        private void Add()
        {
            var x = new Therapy
            {
                PatientId = UiPrompts.AskInt("PatientId"),
                MedicationId = UiPrompts.AskInt("MedicationId"),
                Dose = UiPrompts.AskDecimal("Dose"),
                Unit = UiPrompts.AskText("Dose unit (mg/tablets/...)"),
                Frequency = UiPrompts.AskText("Frequency (e.g. 3x daily)"),
                StartDate = UiPrompts.AskDate("Start (yyyy-mm-dd)"),
                EndDate = UiPrompts.AskNullableDate("End (yyyy-mm-dd) (enter for null)")
            };

            _db.Insert(x);
            UiMessages.Ok("Inserted therapy.");
        }

        private void Update()
        {
            var id = UiPrompts.AskInt("Id");
            var x = _db.GetById<Therapy>(id);
            if (x is null) { UiMessages.Warn("Not found."); return; }

            x.Frequency = UiPrompts.AskOptional("Frequency", x.Frequency);

            var d = AnsiConsole.Ask<string>($"Dose ([grey]{x.Dose}[/]) (enter to keep):");
            if (!string.IsNullOrWhiteSpace(d)) x.Dose = decimal.Parse(d);

            x.Unit = UiPrompts.AskOptional("Dose unit", x.Unit);
            x.StartDate = UiPrompts.AskOptionalDate("Start", x.StartDate);
            x.EndDate = UiPrompts.AskOptionalNullableDate("End", x.EndDate);

            _db.Update(x);
            UiMessages.Ok("Updated therapy.");
        }

        private void Delete()
        {
            var id = UiPrompts.AskInt("Id");
            if (!UiPrompts.Confirm("Delete this therapy?")) return;

            _db.DeleteById<Therapy>(id);
            UiMessages.Ok("Deleted therapy.");
        }

        private void TrackedUpdate()
        {
            var id = UiPrompts.AskInt("Id");
            var t = _db.GetById<Therapy>(id);
            if (t is null) { UiMessages.Warn("Not found."); return; }

            t.Frequency = $"tracked_{DateTime.Now:HHmmss}";
            var saved = _db.SaveChanges();

            AnsiConsole.MarkupLine($"[green]Saved updates:[/] {saved}");
        }
    }
}
