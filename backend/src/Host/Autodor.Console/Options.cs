using CommandLine;

public class Options
{
    [Option('f', "from", Required = true, HelpText = "Date from for operations.")]
    public DateTime From { get; set; }

    [Option('t', "to", Required = true, HelpText = "Date to for operations.")]
    public DateTime To { get; set; }

    [Option('o', "operation", Required = false, HelpText = "Operation to perform (invoices, sync, etc).")]
    public string Operation { get; set; } = "invoices";
}