namespace ModCatalogValidator;

internal sealed record ValidatorOptions(String CatalogPath, String BaseRevision, String HeadRevision)
{
    public static ValidatorOptions Parse(String[] arguments)
    {
        if (arguments.Length != 3)
            throw new ArgumentException("Usage: ModCatalogValidator <catalog-path> <base-revision> <head-revision>.");

        String catalogPath = Path.GetFullPath(arguments[0]);
        if (!File.Exists(catalogPath))
            throw new FileNotFoundException("The catalog file does not exist.", catalogPath);

        return new ValidatorOptions(catalogPath, arguments[1], arguments[2]);
    }
}
