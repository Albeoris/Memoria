#nullable disable

namespace Memoria.Launcher.Utils.ModValidation
{
    internal sealed class D3D9X5213MultiplyFixer : D3D9X5213BinaryOperationFixer
    {
        public D3D9X5213MultiplyFixer()
            : base("mul", "Direct3D 9 ps_2_0 X5213 multiply fixer")
        {
        }
    }
}
