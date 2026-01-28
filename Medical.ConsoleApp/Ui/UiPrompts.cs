using Spectre.Console;

namespace Medical.ConsoleApp.Ui
{
    internal static class UiPrompts
    {
        public static bool Confirm(string msg) => AnsiConsole.Confirm(msg);

        public static int AskInt(string label) =>
            AnsiConsole.Prompt(new TextPrompt<int>($"{label}:").ValidationErrorMessage("[red]Enter a valid integer.[/]"));

        public static decimal AskDecimal(string label) =>
            AnsiConsole.Prompt(new TextPrompt<decimal>($"{label}:").ValidationErrorMessage("[red]Enter a valid decimal.[/]"));

        public static string AskText(string label) =>
            AnsiConsole.Prompt(
                new TextPrompt<string>($"{label}:")
                    .Validate(s => string.IsNullOrWhiteSpace(s)
                        ? ValidationResult.Error("[red]Required.[/]")
                        : ValidationResult.Success()));

        public static string AskOptional(string label, string current)
        {
            var s = AnsiConsole.Prompt(
                new TextPrompt<string>($"{label} ([grey]{Markup.Escape(current)}[/]) (enter to keep):")
                    .AllowEmpty());

            return string.IsNullOrWhiteSpace(s) ? current : s;
        }

        public static DateTime AskDate(string label) =>
            DateTime.Parse(
                AnsiConsole.Prompt(
                    new TextPrompt<string>($"{label}:").ValidationErrorMessage("[red]Use yyyy-mm-dd.[/]")));

        public static DateTime AskOptionalDate(string label, DateTime current)
        {
            var s = AnsiConsole.Prompt(
                new TextPrompt<string>($"{label} ([grey]{current:yyyy-MM-dd}[/]) (enter to keep):")
                .AllowEmpty()
            );

            return string.IsNullOrWhiteSpace(s) ? current : DateTime.Parse(s);
        }

        public static DateTime? AskNullableDate(string label)
        {
            var s = AnsiConsole.Ask<string>($"{label}:");
            return string.IsNullOrWhiteSpace(s) ? null : DateTime.Parse(s);
        }

        public static DateTime? AskOptionalNullableDate(string label, DateTime? current)
        {
            var shown = current.HasValue ? current.Value.ToString("yyyy-MM-dd") : "null";

            var s = AnsiConsole.Prompt(
                new TextPrompt<string>($"{label} ([grey]{shown}[/]) (enter to keep):")
                    .AllowEmpty()
            );

            if (string.IsNullOrWhiteSpace(s))
                return current;

            if (s.Equals("null", StringComparison.OrdinalIgnoreCase) || s == "-")
                return null;

            return DateTime.Parse(s);
        }

        public static DateTime AskDateTime(string label) =>
            DateTime.Parse(
                AnsiConsole.Prompt(
                    new TextPrompt<string>($"{label}:").ValidationErrorMessage("[red]Use yyyy-mm-dd HH:mm.[/]")));

        public static DateTime AskOptionalDateTime(string label, DateTime current)
        {
            var prompt = new TextPrompt<string>(
                $"{label} ([grey]{current:yyyy-MM-dd HH:mm}[/]) (enter to keep):")
                .AllowEmpty();

            var s = AnsiConsole.Prompt(prompt);

            return string.IsNullOrWhiteSpace(s)
                ? current
                : DateTime.Parse(s);
        }
    }
}
