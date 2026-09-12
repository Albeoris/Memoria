namespace ModCatalogValidator;

internal static class Program
{
    private static async Task<Int32> Main(String[] arguments)
    {
        try
        {
            ValidatorOptions options = ValidatorOptions.Parse(arguments);
            CatalogValidationApplication application = new(new GitRepository(), new CatalogParser(), new CatalogStructureValidator());
            return await application.RunAsync(options);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Catalog validation could not start: {exception.Message}");
            return 1;
        }
    }
}
