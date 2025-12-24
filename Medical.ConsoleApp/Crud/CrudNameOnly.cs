using Medical.ConsoleApp.Ui;
using MyOrm.Core;
using Spectre.Console;

namespace Medical.ConsoleApp.Crud
{
    internal static class CrudNameOnly
    {
        public static void Run<T>(OrmContext db, string label, Func<T, string> getName, Action<T, string> setName)
            where T : new()
        {
            ArgumentNullException.ThrowIfNull(db);

            CrudMenu.Run(
                title: label,
                list: () =>
                {
                    var rows = db.SelectAll<T>();
                    var table = new Table().RoundedBorder().AddColumn("Id").AddColumn("Name");

                    foreach (var e in rows)
                        table.AddRow(GetId(e).ToString(), getName(e));

                    AnsiConsole.Write(table);
                },
                add: () =>
                {
                    var e = new T();
                    setName(e, UiPrompts.AskText("Name"));
                    db.Insert(e);
                    UiMessages.Ok("Inserted.");
                },
                update: () =>
                {
                    var id = UiPrompts.AskInt("Id");
                    var e = db.GetById<T>(id);
                    if (e is null) { UiMessages.Warn("Not found."); return; }

                    setName(e, UiPrompts.AskOptional("Name", getName(e)));
                    db.Update(e);
                    UiMessages.Ok("Updated.");
                },
                delete: () =>
                {
                    var id = UiPrompts.AskInt("Id");
                    if (!UiPrompts.Confirm("Delete?")) return;

                    db.DeleteById<T>(id);
                    UiMessages.Ok("Deleted.");
                });
        }

        private static int GetId<T>(T entity)
        {
            var p = entity!.GetType().GetProperty("Id");
            return (int)p?.GetValue(entity)!;
        }
    }
}
