using Spectre.Console;

namespace Medical.ConsoleApp.Ui
{
    internal static class UiMessages
    {
        public static void Ok(string msg) =>
            AnsiConsole.MarkupLine($"[green] {Markup.Escape(msg)}[/]");

        public static void Warn(string msg) =>
            AnsiConsole.MarkupLine($"[yellow]! {Markup.Escape(msg)}[/]");

        public static void Error(Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Operation failed.[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenTypes);
        }

        public static void Wait()
        {
            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
    }
}
