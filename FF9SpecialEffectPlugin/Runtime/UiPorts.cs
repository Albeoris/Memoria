using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Recompiler.Runtime;

/// <summary>Deterministic UI for tests and headless runs: records every call, answers every question the same way.</summary>
public sealed class RecordingUi : IUserInterface
{
    private readonly List<string> _events = [];

    /// <summary>The answer <see cref="Confirm"/> returns.</summary>
    public bool ConfirmAnswer { get; set; } = true;

    /// <summary><c>message|confirm|notify: caption: text</c> lines in call order.</summary>
    public IReadOnlyList<string> Events => _events;

    public void MessageBox(string text, string caption) => _events.Add($"message: {caption}: {text}");

    public bool Confirm(string text, string caption)
    {
        _events.Add($"confirm: {caption}: {text}");
        return ConfirmAnswer;
    }

    public void Notify(string text) => _events.Add($"notify: {text}");

    /// <summary>The button <see cref="MessageBox(nint, string, string, uint)"/> answers; <c>null</c> (default) derives it from <see cref="ConfirmAnswer"/> and the button group.</summary>
    public int? MessageBoxAnswer { get; set; }

    public int MessageBox(nint owner, string text, string caption, uint flags)
    {
        _events.Add($"messagebox(0x{flags:x}): {caption}: {text}");
        if (MessageBoxAnswer is { } answer)
        {
            return answer;
        }

        return (flags & 0xF) switch
        {
            MessageBoxButtons.YesNo or MessageBoxButtons.YesNoCancel => ConfirmAnswer ? MessageBoxButtons.IdYes : MessageBoxButtons.IdNo,
            MessageBoxButtons.OkCancel => ConfirmAnswer ? MessageBoxButtons.IdOk : MessageBoxButtons.IdCancel,
            MessageBoxButtons.RetryCancel => ConfirmAnswer ? MessageBoxButtons.IdRetry : MessageBoxButtons.IdCancel,
            MessageBoxButtons.AbortRetryIgnore => ConfirmAnswer ? MessageBoxButtons.IdIgnore : MessageBoxButtons.IdAbort,
            _ => MessageBoxButtons.IdOk,
        };
    }
}

/// <summary>UI routed to caller-supplied callbacks (a shell that owns a window, a test that scripts answers per question).</summary>
public sealed class CallbackUi(Action<string, string> messageBox, Func<string, string, bool> confirm, Action<string> notify) : IUserInterface
{
    public void MessageBox(string text, string caption) => messageBox(text, caption);

    public bool Confirm(string text, string caption) => confirm(text, caption);

    public void Notify(string text) => notify(text);
}

/// <summary>Win32 <c>MessageBoxW</c>; <see cref="Notify"/> writes to the debugger output stream so it never blocks.</summary>
[SupportedOSPlatform("windows")]
public sealed partial class Win32Ui : IUserInterface
{
    private const uint MbOk = 0x0;
    private const uint MbYesNo = 0x4;
    private const uint MbIconInformation = 0x40;
    private const uint MbIconQuestion = 0x20;
    private const int IdYes = 6;

    public void MessageBox(string text, string caption) => MessageBoxW(0, text, caption, MbOk | MbIconInformation);

    public bool Confirm(string text, string caption) => MessageBoxW(0, text, caption, MbYesNo | MbIconQuestion) == IdYes;

    public void Notify(string text) => OutputDebugStringW(text);

    public int MessageBox(nint owner, string text, string caption, uint flags) => MessageBoxW(owner, text, caption, flags);

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int MessageBoxW(nint owner, string text, string caption, uint type);

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf16)]
    private static partial void OutputDebugStringW(string text);
}
