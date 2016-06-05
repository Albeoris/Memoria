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

    internal static readonly LocalizationDictionary Provider = new LocalizationDictionary();

    public static bool localizationHasBeenSet;
    public static LoadFunction loadFunction;
    public static OnLocalizeNotification onLocalize;

    [Obsolete("Localization is now always active. You no longer need to check this property.")]
    public static Boolean isActive => true;

    public static String[] knownLanguages => Provider.ProvideLanguages();

    public static Dictionary<String, String[]> dictionary
    {
        get { return Provider.ProvideDictionary(); }
        set { Provider.SetDictionary(value); }
    }

    public static String language
    {
        get { return Provider.ProvideLanguage(); }
        set { Provider.SetLanguage(value); }
    }

    public static void Load(TextAsset asset)
    {
        ByteReader byteReader = new ByteReader(asset);
        Provider.Set(asset.name, byteReader.ReadDictionary());
    }

    public static void Set(string languageName, byte[] bytes)
    {
        ByteReader byteReader = new ByteReader(bytes);
        Provider.Set(languageName, byteReader.ReadDictionary());
    }

    public static void Set(String key, String value)
    {
        Provider.Set(key, value);
    }

    public static void ReplaceKey(string key, string val)
    {
        Provider.ReplaceKey(key, val);
    }

    public static void ClearReplacements()
    {
        Provider.ClearReplacements();
    }

    public static Boolean LoadCSV(TextAsset asset, Boolean merge = false)
    {
        return Provider.TryLoadCSV(asset.bytes, merge);
    }

    public static Boolean LoadCSV(byte[] bytes, Boolean merge = false)
    {
        return Provider.TryLoadCSV(bytes, merge);
    }

    public static void Set(String languageName, Dictionary<String, String> dic)
    {
        Provider.Set(languageName, dic);
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
        return Provider.Get(key);
    }

    public static String Format(String key, params object[] parameters)
    {
        return String.Format(Get(key), parameters);
    }

    public static bool Exists(string key)
    {
        return Provider.Exists(key);
    }

    [Obsolete("Use Localization.Get instead")]
    public static String Localize(String key)
    {
        return Get(key);
    }
}