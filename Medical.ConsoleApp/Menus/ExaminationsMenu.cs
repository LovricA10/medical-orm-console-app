using Medical.ConsoleApp.Crud;
using Medical.ConsoleApp.Ui;
using Medical.Domain.Models;
using MyOrm.Core;
using Spectre.Console;

namespace Medical.ConsoleApp.Menus
{
    internal sealed class ExaminationsMenu
    {
        private readonly OrmContext _db;

        public ExaminationsMenu(OrmContext db)
        {
            ArgumentNullException.ThrowIfNull(db);
            _db = db;
        }

        public void Run() =>
            CrudMenu.Run(
                title: "Examinations",
                list: List,
                add: Add,
                update: Update,
                delete: Delete);

        private void List()
        {
            var rows = _db.SelectAll<Examination>();

            var t = new Table().RoundedBorder()
                .AddColumn("Id")
                .AddColumn("PatientId")
                .AddColumn("DoctorId")
                .AddColumn("Type")
                .AddColumn("Scheduled");

            foreach (var x in rows)
                t.AddRow(
                    x.Id.ToString(),
                    x.PatientId.ToString(),
                    x.DoctorId.ToString(),
                    x.Type,
                    x.ScheduledAt.ToString("yyyy-MM-dd HH:mm"));

            AnsiConsole.Write(t);
        }

        private void Add()
        {
            var x = new Examination
            {
                PatientId = UiPrompts.AskInt("PatientId"),
                DoctorId = UiPrompts.AskInt("DoctorId (specialist)"),
                Type = UiPrompts.AskText("Type (CT/MR/ULTRA/...)"),
                ScheduledAt = UiPrompts.AskDateTime("ScheduledAt (yyyy-mm-dd HH:mm)")
            };

            _db.Insert(x);
            UiMessages.Ok("Inserted examination.");
        }

        private void Update()
        {
            var id = UiPrompts.AskInt("Id");
            var x = _db.GetById<Examination>(id);
            if (x is null) { UiMessages.Warn("Not found."); return; }

            x.Type = UiPrompts.AskOptional("Type", x.Type);
            x.ScheduledAt = UiPrompts.AskOptionalDateTime("ScheduledAt", x.ScheduledAt);

            var doc = AnsiConsole.Prompt(
                 new TextPrompt<string>($"DoctorId ([grey]{x.DoctorId}[/]) (enter to keep):")
                .AllowEmpty()
);
            if (!string.IsNullOrWhiteSpace(doc))
                x.DoctorId = int.Parse(doc);

            _db.Update(x);
            UiMessages.Ok("Updated examination.");
        }

        private void Delete()
        {
            var id = UiPrompts.AskInt("Id");
            if (!UiPrompts.Confirm("Delete this examination?")) return;

            _db.DeleteById<Examination>(id);
            UiMessages.Ok("Deleted examination.");
        }
    }
}

