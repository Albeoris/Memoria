using System.IO;

namespace Memoria.Assets
{
    public interface ITextFormatter
    {
        ITextWriter GetWriter();
        ITextReader GetReader();
    }
}
