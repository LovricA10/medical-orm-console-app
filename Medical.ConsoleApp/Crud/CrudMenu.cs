using Medical.ConsoleApp.Ui;
using Spectre.Console;

namespace Medical.ConsoleApp.Crud
{
    internal static class CrudMenu
    {
        public static void Run(
            string title,
            Action list,
            Action add,
            Action update,
            Action delete,
            IDictionary<string, Action>? extra = null)
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule($"[bold]{title}[/]") { Style = Style.Parse("grey") });

                var choices = new List<string> { "List", "Add", "Update", "Delete" };
                if (extra is not null) choices.AddRange(extra.Keys);
                choices.Add("Back");

                var actions = new Dictionary<string, Action>
                {
                    ["List"] = list,
                    ["Add"] = add,
                    ["Update"] = update,
                    ["Delete"] = delete
                };

                if (extra is not null)
                {
                    foreach (var kv in extra)
                        actions[kv.Key] = kv.Value;
                }

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose operation")
                        .AddChoices(choices)
                        .PageSize(10));

                if (choice == "Back")
                    return;

                try
                {
                    actions[choice]();
                }
                catch (Exception ex)
                {
                    UiMessages.Error(ex);
                }

                UiMessages.Wait();
            }
        }
    }
}
