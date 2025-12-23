using Medical.ConsoleApp.Crud;
using Medical.ConsoleApp.Menus;
using Medical.ConsoleApp.Ui;
using Medical.Domain.Models;
using MyOrm.Core;
using Spectre.Console;

namespace Medical.ConsoleApp.App
{
    public sealed class ConsoleMenu
    {
        private readonly OrmContext _db;
        private readonly IDictionary<string, Action> _main;

        public ConsoleMenu(OrmContext db)
        {
            _db = db;

            _main = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
            {
                ["Patients"] = () => new PatientsMenu(_db).Run(),
                ["Diagnoses"] = () => CrudNameOnly.Run<Diagnosis>(_db, "Diagnoses", x => x.Name, (x, v) => x.Name = v),
                ["Medications"] = () => CrudNameOnly.Run<Medication>(_db, "Medications", x => x.Name, (x, v) => x.Name = v),
                ["Medical history"] = () => new MedicalHistoryMenu(_db).Run(),
                ["Therapies"] = () => new TherapiesMenu(_db).Run(),
                ["Examinations"] = () => new ExaminationsMenu(_db).Run(),
                ["EAGER demo"] = () => DemoQueries.PrintPatientEager(_db, UiPrompts.AskInt("PatientId")),
                ["Change tracking demo"] = () => DemoQueries.ChangeTrackingDemo(_db)
            };
        }

        public void Run()
        {
            while (true)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Medical Console[/] — choose an action")
                        .PageSize(10)
                        .AddChoices(_main.Keys.Concat(["Exit"]))
                );

                if (choice == "Exit")
                    return;

                _main[choice]();
                UiMessages.Wait();
            }
        }
    }
}
