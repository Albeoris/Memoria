using System.Collections.Generic;

namespace FF8.Core
{
    public interface IScriptExecuter
    {
        IEnumerable<IAwaitable> Execute(IServices services);
    }
}