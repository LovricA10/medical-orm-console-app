using Medical.ConsoleApp.App;
using Medical.ConsoleApp.Config;
using MyOrm.Core;
using Spectre.Console;

try
{
    using var db = new OrmContext(AppConfig.ConnectionString);

    Seed.EnsureDoctorsSeeded(db);

    new ConsoleMenu(db).Run();
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes);
}