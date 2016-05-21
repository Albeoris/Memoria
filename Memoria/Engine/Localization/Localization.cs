using System;
using System.Collections.Generic;
using Memoria;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

[ExportedType("£ęĕ÷&!!!´pØĄ»MłĠa«_æYì¿À,!!!áÏø¾ĪĻ§Fãľ}XÀ×¥éÕçÁfłò±ét­İzÎđw¢J^ėË@êČf>!!!OĬÜq)Ðñĳ6}!Ę÷bďo«be¨ëô]Iī_Áå£ăc4{·4ĵ_êmĲôąĬĲAĵĬĊe4ĽÃĪÏbÞRĻyqOĖGrà«òUēĬĮÇ(ĥ£8­îÐ+=n$ēºFÙ(_ĉÐ7a@ÚĆķUâĞĹÔºfçûēŃ²±łã$!!!H0×ĺńńńńńńńń&!!!~đVĸ*äļÀĘnM´÷ĵÏ¡ńńńńłÅä~ńńńńńńńń&!!!~đVĸĜĂĥãÉ$q}Ç.JŃńńńń")]
public static class Localization
{
    public delegate Byte[] LoadFunction(String path);

    public delegate void OnLocalizeNotification();

    private static readonly LocalizationDictionary _provider = new LocalizationDictionary();

    public static bool localizationHasBeenSet;
    public static LoadFunction loadFunction;
    public static OnLocalizeNotification onLocalize;

    [Obsolete("Localization is now always active. You no longer need to check this property.")]
    public static Boolean isActive => true;

    public static String[] knownLanguages => _provider.ProvideLanguages();

    public static Dictionary<String, String[]> dictionary
    {
        get { return _provider.ProvideDictionary(); }
        set { _provider.SetDictionary(value); }
    }

    public static String language
    {
        get { return _provider.ProvideLanguage(); }
        set { _provider.SetLanguage(value); }
    }

    public static void Load(TextAsset asset)
    {
        ByteReader byteReader = new ByteReader(asset);
        _provider.Set(asset.name, byteReader.ReadDictionary());
    }

    public static void Set(string languageName, byte[] bytes)
    {
        ByteReader byteReader = new ByteReader(bytes);
        _provider.Set(languageName, byteReader.ReadDictionary());
    }

    public static void Set(String key, String value)
    {
        _provider.Set(key, value);
    }

    public static void ReplaceKey(string key, string val)
    {
        _provider.ReplaceKey(key, val);
    }

    public static void ClearReplacements()
    {
        _provider.ClearReplacements();
    }

    public static Boolean LoadCSV(TextAsset asset, Boolean merge = false)
    {
        return _provider.TryLoadCSV(asset.bytes, merge);
    }

    public static Boolean LoadCSV(byte[] bytes, Boolean merge = false)
    {
        return _provider.TryLoadCSV(bytes, merge);
    }

    public static void Set(String languageName, Dictionary<String, String> dic)
    {
        _provider.Set(languageName, dic);
    }

    public static String GetPath()
    {
        return "EmbeddedAsset/Text/" + GetSymbol();
    }

    public static String GetSymbol()
    {
        return Get("Symbol");
    }

    public static String Get(String key)
    {
        return _provider.Get(key);
    }

    public static String Format(String key, params object[] parameters)
    {
        return String.Format(Get(key), parameters);
    }

    public static bool Exists(string key)
    {
        return _provider.Exists(key);
    }

    [Obsolete("Use Localization.Get instead")]
    public static String Localize(String key)
    {
        return Get(key);
    }
}