namespace Application.Interfaces
{
    public interface IPDFGeneratorService
    {
        public byte[] Generate(string htmlContent);
    }
}
