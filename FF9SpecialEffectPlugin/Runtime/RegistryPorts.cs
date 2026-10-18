using System.Globalization;
using System.Runtime.Versioning;

namespace Recompiler.Runtime;

/// <summary>
/// Deterministic registry for tests and non-Windows hosts: values live in sorted dictionaries, keys are
/// case-insensitive like the real registry, and <see cref="Dump"/> renders the content for assertions.
/// </summary>
public sealed class InMemoryRegistry : IRegistry
{
    private readonly SortedDictionary<string, SortedDictionary<string, object>> _keys = new(StringComparer.OrdinalIgnoreCase);

    public string? ReadString(RegistryRoot root, string key, string name) =>
        Read(root, key, name) is { } value ? Convert.ToString(value, CultureInfo.InvariantCulture) : null;

    public uint? ReadDword(RegistryRoot root, string key, string name) => Read(root, key, name) switch
    {
        uint dword => dword,
        string text when uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint parsed) => parsed,
        _ => null,
    };

    public void WriteString(RegistryRoot root, string key, string name, string value) => Values(root, key, create: true)![name] = value;

    public void WriteDword(RegistryRoot root, string key, string name, uint value) => Values(root, key, create: true)![name] = value;

    public bool DeleteValue(RegistryRoot root, string key, string name) => Values(root, key, create: false)?.Remove(name) ?? false;

    public bool KeyExists(RegistryRoot root, string key) => _keys.ContainsKey(Path(root, key));

    public byte[]? ReadBinary(RegistryRoot root, string key, string name) => Read(root, key, name) is byte[] bytes ? (byte[])bytes.Clone() : null;

    public void WriteBinary(RegistryRoot root, string key, string name, ReadOnlySpan<byte> value) => Values(root, key, create: true)![name] = value.ToArray();

    public IReadOnlyList<string> SubKeyNames(RegistryRoot root, string key)
    {
        string prefix = Path(root, key) + "\\";
        return _keys.Keys
            .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(k => k[prefix.Length..].Split('\\')[0])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public IReadOnlyList<string> ValueNames(RegistryRoot root, string key) => Values(root, key, create: false)?.Keys.ToList() ?? [];

    public RegistryValue? ReadValue(RegistryRoot root, string key, string name) => Read(root, key, name) switch
    {
        null => null,
        RegistryValue value => value with { Data = (byte[])value.Data.Clone() },
        string text => RegistryValue.FromString(text),
        uint dword => RegistryValue.FromDword(dword),
        byte[] bytes => new RegistryValue(RegistryValueType.Binary, (byte[])bytes.Clone()),
        object other => RegistryValue.FromString(Convert.ToString(other, CultureInfo.InvariantCulture) ?? string.Empty),
    };

    /// <summary>Stores a value of any kind (<c>REG_EXPAND_SZ</c>, <c>REG_MULTI_SZ</c>, <c>REG_QWORD</c>, ...) as raw bytes, for tests of <see cref="ReadValue"/>.</summary>
    public void WriteValue(RegistryRoot root, string key, string name, RegistryValue value) => Values(root, key, create: true)![name] = value with { Data = (byte[])value.Data.Clone() };

    /// <summary>One <c>root\key!name=value</c> line per value, sorted; DWORDs print as <c>0x</c> hex.</summary>
    public IReadOnlyList<string> Dump()
    {
        var lines = new List<string>();
        foreach ((string path, SortedDictionary<string, object> values) in _keys)
        {
            foreach ((string name, object value) in values)
            {
                lines.Add($"{path}!{name}={value switch { uint dword => "0x" + dword.ToString("x", CultureInfo.InvariantCulture), byte[] bytes => "hex:" + Convert.ToHexString(bytes).ToLowerInvariant(), RegistryValue raw => $"hex({(int)raw.Kind}):" + Convert.ToHexString(raw.Data).ToLowerInvariant(), _ => value }}");
            }
        }

        return lines;
    }

    private object? Read(RegistryRoot root, string key, string name) =>
        Values(root, key, create: false) is { } values && values.TryGetValue(name, out object? value) ? value : null;

    private SortedDictionary<string, object>? Values(RegistryRoot root, string key, bool create)
    {
        string path = Path(root, key);
        if (_keys.TryGetValue(path, out SortedDictionary<string, object>? values))
        {
            return values;
        }

        if (!create)
        {
            return null;
        }

        values = new SortedDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        _keys[path] = values;
        return values;
    }

    private static string Path(RegistryRoot root, string key) => root + "\\" + key.Trim('\\');
}

/// <summary>The real registry through <c>Microsoft.Win32.Registry</c>; the only place rewritten programs touch it.</summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsRegistry : IRegistry
{
    public string? ReadString(RegistryRoot root, string key, string name)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: false);
        return opened?.GetValue(name) is { } value ? Convert.ToString(value, CultureInfo.InvariantCulture) : null;
    }

    public uint? ReadDword(RegistryRoot root, string key, string name)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: false);
        return opened?.GetValue(name) switch
        {
            int dword => unchecked((uint)dword),
            long qword => unchecked((uint)qword),
            string text when uint.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint parsed) => parsed,
            _ => null,
        };
    }

    public void WriteString(RegistryRoot root, string key, string name, string value)
    {
        using Microsoft.Win32.RegistryKey created = Base(root).CreateSubKey(key, writable: true);
        created.SetValue(name, value, Microsoft.Win32.RegistryValueKind.String);
    }

    public void WriteDword(RegistryRoot root, string key, string name, uint value)
    {
        using Microsoft.Win32.RegistryKey created = Base(root).CreateSubKey(key, writable: true);
        created.SetValue(name, unchecked((int)value), Microsoft.Win32.RegistryValueKind.DWord);
    }

    public bool DeleteValue(RegistryRoot root, string key, string name)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: true);
        if (opened?.GetValue(name) is null)
        {
            return false;
        }

        opened.DeleteValue(name, throwOnMissingValue: false);
        return true;
    }

    public bool KeyExists(RegistryRoot root, string key)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: false);
        return opened is not null;
    }

    public byte[]? ReadBinary(RegistryRoot root, string key, string name)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: false);
        return opened?.GetValue(name) as byte[];
    }

    public void WriteBinary(RegistryRoot root, string key, string name, ReadOnlySpan<byte> value)
    {
        using Microsoft.Win32.RegistryKey created = Base(root).CreateSubKey(key, writable: true);
        created.SetValue(name, value.ToArray(), Microsoft.Win32.RegistryValueKind.Binary);
    }

    public IReadOnlyList<string> SubKeyNames(RegistryRoot root, string key)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: false);
        return opened?.GetSubKeyNames().Order(StringComparer.OrdinalIgnoreCase).ToList() ?? [];
    }

    public IReadOnlyList<string> ValueNames(RegistryRoot root, string key)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: false);
        return opened?.GetValueNames().Order(StringComparer.OrdinalIgnoreCase).ToList() ?? [];
    }

    public RegistryValue? ReadValue(RegistryRoot root, string key, string name)
    {
        using Microsoft.Win32.RegistryKey? opened = Base(root).OpenSubKey(key, writable: false);
        object? value = opened?.GetValue(name, null, Microsoft.Win32.RegistryValueOptions.DoNotExpandEnvironmentNames);
        if (opened is null || value is null)
        {
            return null;
        }

        return opened.GetValueKind(name) switch
        {
            Microsoft.Win32.RegistryValueKind.String => RegistryValue.FromString((string)value),
            Microsoft.Win32.RegistryValueKind.ExpandString => RegistryValue.FromString((string)value, RegistryValueType.ExpandString),
            Microsoft.Win32.RegistryValueKind.MultiString => RegistryValue.FromStrings((string[])value),
            Microsoft.Win32.RegistryValueKind.DWord => RegistryValue.FromDword(unchecked((uint)(int)value)),
            Microsoft.Win32.RegistryValueKind.QWord => RegistryValue.FromQword(unchecked((ulong)(long)value)),
            Microsoft.Win32.RegistryValueKind.Binary => new RegistryValue(RegistryValueType.Binary, (byte[])value),
            Microsoft.Win32.RegistryValueKind.None => new RegistryValue(RegistryValueType.None, value as byte[] ?? []),
            _ => new RegistryValue(RegistryValueType.None, value as byte[] ?? []),
        };
    }

    private static Microsoft.Win32.RegistryKey Base(RegistryRoot root) => root switch
    {
        RegistryRoot.ClassesRoot => Microsoft.Win32.Registry.ClassesRoot,
        RegistryRoot.CurrentUser => Microsoft.Win32.Registry.CurrentUser,
        RegistryRoot.LocalMachine => Microsoft.Win32.Registry.LocalMachine,
        RegistryRoot.Users => Microsoft.Win32.Registry.Users,
        RegistryRoot.CurrentConfig => Microsoft.Win32.Registry.CurrentConfig,
        _ => throw new ArgumentOutOfRangeException(nameof(root), root, null),
    };
}
