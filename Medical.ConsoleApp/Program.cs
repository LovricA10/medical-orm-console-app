using Medical.ConsoleApp.App;
using Medical.ConsoleApp.Config;
using Medical.Domain.Models;
using MyOrm.Core;
using MyOrm.Migrations;
using Spectre.Console;

try
{
    using var db = new OrmContext(AppConfig.ConnectionString);

    var generator = new MigrationGenerator(db.Connection);
    var migration = generator.Generate("auto",
    [
    typeof(Patient),
    typeof(Doctor),
    typeof(Examination),
    typeof(Diagnosis),
    typeof(Medication),
    typeof(MedicalHistory),
    typeof(Therapy),
    ]);

    migration = AutoMigrations.ApplyDeterministicName(migration);

    if (migration.Up.Count > 0)
        new MigrationApplier(db.Connection).ApplyUp(migration);

    Seed.EnsureDoctorsSeeded(db);

    new ConsoleMenu(db).Run();
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes);
}