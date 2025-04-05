using CommandLine;

public class Options
{

    [Option('f', "from", Required = true, HelpText = "Date from invoices.")]
    public DateTime From { get; set; }

    [Option('t', "to", Required = true, HelpText = "Date to invoices.")]
    public DateTime To { get; set; }
}
