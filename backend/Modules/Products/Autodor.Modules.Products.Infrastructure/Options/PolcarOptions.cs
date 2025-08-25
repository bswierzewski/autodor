namespace Autodor.Modules.Products.Infrastructure.Options;

public class PolcarOptions
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string DistributorCode { get; set; } = null!;
    public int BranchId { get; set; }
    public int LanguageId { get; set; }
    public RetryPolicyOptions RetryPolicy { get; set; } = new();
}

public class RetryPolicyOptions
{
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(10);
}