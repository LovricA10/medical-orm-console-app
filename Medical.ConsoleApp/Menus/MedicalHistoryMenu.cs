using Medical.ConsoleApp.Crud;
using Medical.ConsoleApp.Ui;
using Medical.Domain.Models;
using MyOrm.Core;
using Spectre.Console;

namespace Medical.ConsoleApp.Menus
{
    internal sealed class MedicalHistoryMenu
    {
        private readonly OrmContext _db;

        public MedicalHistoryMenu(OrmContext db) => _db = db;

        public void Run()
        {
            CrudMenu.Run(
                title: "Medical history",
                list: List,
                add: Add,
                update: Update,
                delete: Delete);
        }

        private void List()
        {
            var rows = _db.SelectAll<MedicalHistory>();
            var t = new Table().RoundedBorder()
                .AddColumn("Id")
                .AddColumn("PatientId")
                .AddColumn("DiagnosisId")
                .AddColumn("Start")
                .AddColumn("End");

            foreach (var h in rows)
                t.AddRow(
                    h.Id.ToString(),
                    h.PatientId.ToString(),
                    h.DiagnosisId.ToString(),
                    h.StartDate.ToString("yyyy-MM-dd"),
                    h.EndDate?.ToString("yyyy-MM-dd") ?? "");

            AnsiConsole.Write(t);
        }

        private void Add()
        {
            var h = new MedicalHistory
            {
                PatientId = UiPrompts.AskInt("PatientId"),
                DiagnosisId = UiPrompts.AskInt("DiagnosisId"),
                StartDate = UiPrompts.AskDate("Start (yyyy-mm-dd)"),
                EndDate = UiPrompts.AskNullableDate("End (yyyy-mm-dd) (enter for null)")
            };

            _db.Insert(h);
            UiMessages.Ok("Inserted medical history row.");
        }

        private void Update()
        {
            var id = UiPrompts.AskInt("Id");
            var h = _db.GetById<MedicalHistory>(id);
            if (h is null) { UiMessages.Warn("Not found."); return; }

            h.StartDate = UiPrompts.AskOptionalDate("Start", h.StartDate);
            h.EndDate = UiPrompts.AskOptionalNullableDate("End", h.EndDate);

            _db.Update(h);
            UiMessages.Ok("Updated medical history row.");
        }

        private void Delete()
        {
            var id = UiPrompts.AskInt("Id");
            if (!UiPrompts.Confirm("Delete this row?")) return;

            _db.DeleteById<MedicalHistory>(id);
            UiMessages.Ok("Deleted row.");
        }
    }
}
