using Medical.ConsoleApp.Crud;
using Medical.ConsoleApp.Ui;
using Medical.Domain.Models;
using MyOrm.Core;
using Spectre.Console;

namespace Medical.ConsoleApp.Menus
{
    internal sealed class PatientsMenu
    {
        private readonly OrmContext _db;

        public PatientsMenu(OrmContext db) => _db = db;

        public void Run()
        {
            CrudMenu.Run(
                title: "Patients",
                list: List,
                add: Add,
                update: Update,
                delete: Delete);
        }

        private void List()
        {
            var rows = _db.SelectAll<Patient>();

            var t = new Table().RoundedBorder().AddColumn("Id").AddColumn("Name").AddColumn("OIB");
            foreach (var p in rows)
                t.AddRow(p.Id.ToString(), $"{p.FirstName} {p.LastName}", p.Oib);

            AnsiConsole.Write(t);
        }

        private void Add()
        {
            var p = new Patient
            {
                FirstName = UiPrompts.AskText("First name"),
                LastName = UiPrompts.AskText("Last name"),
                Oib = UiPrompts.AskText("OIB (11)"),
                DateOfBirth = UiPrompts.AskDate("Date of birth (yyyy-mm-dd)"),
                Gender = UiPrompts.AskText("Gender (M/F)").ToUpperInvariant(),
                TemporaryAddress = UiPrompts.AskText("Temporary address"),
                PermanentAddress = UiPrompts.AskText("Permanent address")
            };

            _db.Insert(p);
            UiMessages.Ok("Inserted patient.");
        }

        private void Update()
        {
            var id = UiPrompts.AskInt("Id");
            var p = _db.GetById<Patient>(id);
            if (p is null) { UiMessages.Warn("Not found."); return; }

            p.FirstName = UiPrompts.AskOptional("First name", p.FirstName);
            p.LastName = UiPrompts.AskOptional("Last name", p.LastName);

            _db.Update(p);
            UiMessages.Ok("Updated patient.");
        }

        private void Delete()
        {
            var id = UiPrompts.AskInt("Id");
            if (!UiPrompts.Confirm("Delete this patient?")) return;

            _db.DeleteById<Patient>(id);
            UiMessages.Ok("Deleted patient.");
        }
    }
}
