using Medical.Domain.Models;
using MyOrm.Core;
using Spectre.Console;

namespace Medical.ConsoleApp.App
{
    public static class Seed
    {
        public static void EnsureDoctorsSeeded(OrmContext ctx)
        {
            ArgumentNullException.ThrowIfNull(ctx);

            if (DoctorsExist(ctx))
            {
                AnsiConsole.MarkupLine("[grey]Doctors already seeded.[/]");
                return;
            }

            foreach (var doctor in GetSeedDoctors())
                ctx.Insert(doctor);

            AnsiConsole.MarkupLine("[green]Seeded doctors (first run only).[/]");
        }

        private static bool DoctorsExist(OrmContext ctx) =>
            ctx.SelectWhere<Doctor>("1=1 LIMIT 1").Count > 0;

        private static IReadOnlyList<Doctor> GetSeedDoctors() =>
            [
                new Doctor { FirstName = "Marko", LastName = "Medic", Specialization = "CARDIO" },
                new Doctor { FirstName = "Ivana", LastName = "Spec", Specialization = "RADIO" },
                new Doctor { FirstName = "Petra", LastName = "Derm", Specialization = "DERM" }
            ];
    }
}
