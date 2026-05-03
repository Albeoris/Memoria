using Assets.Scripts.Common;
using Assets.Sources.Scripts.UI.Common;
using FF9;
using Memoria.Assets;
using Memoria.Data;
using Memoria.Prime;
using Memoria.Prime.Text;
using NCalc;
using NCalc.Domain;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Memoria
{
    public class TextPatcher
    {
        public static void PatchTexts(String[] patchCode)
        {
            TextPatcher patcher = null;
            FindAndReplacer finder = null;
            Appender appender = null;
            foreach (String line in patchCode)
            {
                if (line.StartsWith("//"))
                    continue;
                List<TextPatcher> list = IsPatcherDeclaration(line);
                if (list != null)
                {
                    patcher = new TextPatcher();
                    list.Add(patcher);
                    finder = null;
                    appender = null;
                    continue;
                }
                if (patcher == null)
                    continue;
                if (line.StartsWith("FindAndReplace"))
                {
                    finder = new FindAndReplacer();
                    appender = null;
                    patcher.Modifiers.Add(finder);
                }
                else if (line.StartsWith("Append"))
                {
                    finder = null;
                    appender = new Appender();
                    appender.IsAppend = true;
                    patcher.Modifiers.Add(appender);
                }
                else if (line.StartsWith("Prepend"))
                {
                    finder = null;
                    appender = new Appender();
                    appender.IsAppend = false;
                    patcher.Modifiers.Add(appender);
                }
                else if (line.StartsWith("[code=Condition]") && line.EndsWith("[/code]"))
                {
                    String condition = line.Substring("[code=Condition]".Length, line.Length - "[code=Condition][/code]".Length).Trim();
                    if (finder != null)
                        finder.Condition = condition;
                    else if (appender != null)
                        appender.Condition = condition;
                    else
                        patcher.Condition = condition;
                }
                else if (line.StartsWith("Languages: "))
                {
                    String[] langs = line.Substring("Languages: ".Length).Split(',');
                    HashSet<String> hash = finder != null ? finder.Languages
                                         : appender != null ? appender.Languages
                                         : patcher.Languages;
                    foreach (String lang in langs)
                        hash.Add(lang.Trim());
                }
                else if (finder != null && line.StartsWith("Find: "))
                {
                    finder.Find = new Regex(line.Substring("Find: ".Length), RegexOptions.Multiline);
                }
                else if (finder != null && line.StartsWith("Replace: "))
                {
                    finder.Replace = line.Substring("Replace: ".Length).Replace("\\n", "\n");
                    finder.AsExpression = false;
                }
                else if (finder != null && line.StartsWith("[code=Replace]") && line.EndsWith("[/code]"))
                {
                    finder.Replace = line.Substring("[code=Replace]".Length, line.Length - "[code=Replace][/code]".Length).Trim();
                    finder.AsExpression = true;
                }
                else if (appender != null && line.StartsWith("Text: "))
                {
                    appender.Text = line.Substring("Text: ".Length).Replace("\\n", "\n");
                    appender.AsExpression = false;
                }
                else if (appender != null && line.StartsWith("[code=Text]") && line.EndsWith("[/code]"))
                {
                    appender.Text = line.Substring("[code=Text]".Length, line.Length - "[code=Text][/code]".Length).Trim();
                    appender.AsExpression = true;
                }
            }
        }

        private static List<TextPatcher> IsPatcherDeclaration(String line)
        {
            if (line.StartsWith(">DIALOG"))
                return DialogPatchers;
            if (line.StartsWith(">BATTLE"))
                return BattleDialogPatchers;
            if (line.StartsWith(">INTERFACE"))
                return InterfacePatchers;
            if (line.StartsWith(">DATABASE"))
                return DatabasePatchers;
            return null;
        }

        public static String PatchDialogString(String str, Dialog dialog)
        {
            try
            {
                if (DialogPatchers.Count == 0)
                    return str;
                Int32 lineCount = str.OccurenceCount("\n") + 1;
                ExpressionInitializer ncalcInit = delegate (ref Expression expr)
                {
                    expr.Parameters["RawText"] = str;
                    expr.Parameters["FieldZoneId"] = FF9TextTool.FieldZoneId;
                    expr.Parameters["TextId"] = dialog.TextId;
                    expr.Parameters["DialogFlags"] = ETb.StylesToFlag(dialog.Style, dialog.CapType);
                    expr.Parameters["SpeakerModelId"] = dialog.Po?.model ?? -1;
                    expr.Parameters["SpeakerAnimationId"] = dialog.Po?.anim ?? -1;
                    expr.Parameters["SpeakerIdleAnimationId"] = (dialog.Po as Actor)?.idle ?? -1;
                    expr.Parameters["LineCount"] = lineCount;
                    expr.EvaluateFunction += delegate (String name, FunctionArgs args)
                    {
                        if (name == "SearchCount" && args.Parameters.Length >= 1)
                        {
                            String search = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String from = args.Parameters.Length >= 2 ? NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate()) : str;
                            args.Result = String.IsNullOrEmpty(search) ? 0 : from.OccurenceCount(search);
                        }
                        else if (name == "SpriteExists" && args.Parameters.Length == 2)
                        {
                            String atlasName = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String spriteName = NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate());
                            args.Result = FF9UIDataTool.GetSpriteSize(atlasName, spriteName) != Vector2.zero;
                        }
                        else if (name == "GetTextVariable" && args.Parameters.Length == 1)
                        {
                            Int32 scriptId = (Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), -1);
                            args.Result = scriptId >= 0 && scriptId < ETb.gMesValue.Length ? ETb.gMesValue[scriptId] : 0;
                        }
                    };
                };
                foreach (TextPatcher patcher in DialogPatchers)
                    if (patcher.ApplyPatch(ref str, ncalcInit))
                        lineCount = str.OccurenceCount("\n") + 1;
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return str;
        }

        public static String PatchBattleDialogString(String str, Boolean isTitle, Int32 priority, CMD_DATA cmd)
        {
            try
            {
                if (BattleDialogPatchers.Count == 0)
                    return str;
                Int32 lineCount = str.OccurenceCount("\n") + 1;
                ExpressionInitializer ncalcInit = delegate (ref Expression expr)
                {
                    expr.Parameters["RawText"] = str;
                    expr.Parameters["IsCommandTitle"] = isTitle;
                    expr.Parameters["MessagePriority"] = priority;
                    expr.Parameters["CommandId"] = (Int32)(cmd?.cmd_no ?? BattleCommandId.None);
                    expr.Parameters["AbilityId"] = (Int32)(cmd != null ? btl_util.GetCommandMainActionIndex(cmd) : BattleAbilityId.Void);
                    expr.Parameters["ScriptId"] = cmd?.ScriptId ?? 0;
                    expr.Parameters["LineCount"] = lineCount;
                    expr.EvaluateFunction += delegate (String name, FunctionArgs args)
                    {
                        if (name == "SearchCount" && args.Parameters.Length >= 1)
                        {
                            String search = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String from = args.Parameters.Length >= 2 ? NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate()) : str;
                            args.Result = String.IsNullOrEmpty(search) ? 0 : from.OccurenceCount(search);
                        }
                        else if (name == "SpriteExists" && args.Parameters.Length == 2)
                        {
                            String atlasName = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String spriteName = NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate());
                            args.Result = FF9UIDataTool.GetSpriteSize(atlasName, spriteName) != Vector2.zero;
                        }
                        else if (name == "GetTextVariable" && args.Parameters.Length == 1)
                        {
                            Int32 scriptId = (Int32)NCalcUtility.ConvertNCalcResult(args.Parameters[0].Evaluate(), -1);
                            args.Result = scriptId >= 0 && scriptId < ETb.gMesValue.Length ? ETb.gMesValue[scriptId] : 0;
                        }
                    };
                };
                foreach (TextPatcher patcher in BattleDialogPatchers)
                    if (patcher.ApplyPatch(ref str, ncalcInit))
                        lineCount = str.OccurenceCount("\n") + 1;
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return str;
        }

        public static String PatchInterfaceString(String str, UILabel label)
        {
            try
            {
                if (InterfacePatchers.Count == 0)
                    return str;
                Int32 lineCount = str.OccurenceCount("\n") + 1;
                ExpressionInitializer ncalcInit = delegate (ref Expression expr)
                {
                    expr.Parameters["RawText"] = str;
                    expr.Parameters["TextKey"] = label.GetComponent<UILocalize>()?.key ?? "";
                    expr.Parameters["LabelAlignment"] = label.alignment.ToString();
                    expr.Parameters["LabelName"] = label.gameObject.name;
                    expr.Parameters["CurrentScene"] = PersistenSingleton<SceneDirector>.Instance.CurrentScene;
                    expr.EvaluateFunction += delegate (String name, FunctionArgs args)
                    {
                        if (name == "SearchCount" && args.Parameters.Length >= 1)
                        {
                            String search = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String from = args.Parameters.Length >= 2 ? NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate()) : str;
                            args.Result = String.IsNullOrEmpty(search) ? 0 : from.OccurenceCount(search);
                        }
                        else if (name == "SpriteExists" && args.Parameters.Length == 2)
                        {
                            String atlasName = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String spriteName = NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate());
                            args.Result = FF9UIDataTool.GetSpriteSize(atlasName, spriteName) != Vector2.zero;
                        }
                    };
                };
                foreach (TextPatcher patcher in InterfacePatchers)
                    if (patcher.ApplyPatch(ref str, ncalcInit))
                        lineCount = str.OccurenceCount("\n") + 1;
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return str;
        }

        public static String PatchDatabaseString(String str, String databaseName, Int32 id, Boolean isName, Boolean isHelp)
        {
            try
            {
                if (DatabasePatchers.Count == 0)
                    return str;
                Int32 lineCount = str.OccurenceCount("\n") + 1;
                ExpressionInitializer ncalcInit = delegate (ref Expression expr)
                {
                    expr.Parameters["RawText"] = str;
                    expr.Parameters["Database"] = databaseName;
                    expr.Parameters["EntryId"] = id;
                    expr.Parameters["IsNameEntry"] = isName;
                    expr.Parameters["IsHelpEntry"] = isHelp;
                    expr.EvaluateFunction += delegate (String name, FunctionArgs args)
                    {
                        if (name == "SearchCount" && args.Parameters.Length >= 1)
                        {
                            String search = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String from = args.Parameters.Length >= 2 ? NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate()) : str;
                            args.Result = String.IsNullOrEmpty(search) ? 0 : from.OccurenceCount(search);
                        }
                        else if (name == "SpriteExists" && args.Parameters.Length == 2)
                        {
                            String atlasName = NCalcUtility.EvaluateNCalcString(args.Parameters[0].Evaluate());
                            String spriteName = NCalcUtility.EvaluateNCalcString(args.Parameters[1].Evaluate());
                            args.Result = FF9UIDataTool.GetSpriteSize(atlasName, spriteName) != Vector2.zero;
                        }
                    };
                };
                foreach (TextPatcher patcher in DatabasePatchers)
                    if (patcher.ApplyPatch(ref str, ncalcInit))
                        lineCount = str.OccurenceCount("\n") + 1;
            }
            catch (Exception err)
            {
                Log.Error(err);
            }
            return str;
        }

        private Expression GetExpression(ref String condition, ExpressionInitializer ncalcInitializer)
        {
            if (!expressionCache.TryGetValue(condition, out LogicalExpression exp))
            {
                exp = Expression.Compile(condition, true);
                expressionCache[condition] = exp;
            }
            Expression c = new Expression(exp);
            ncalcInitializer(ref c);
            c.EvaluateFunction += NCalcUtility.commonNCalcFunctions;
            c.EvaluateParameter += NCalcUtility.commonNCalcParameters;
            return c;
        }

        private Boolean ApplyPatch(ref String str, ExpressionInitializer ncalcInitializer)
        {
            Single patchStart = Time.realtimeSinceStartup;
            if (Languages.Count > 0 && !Languages.Contains(Localization.CurrentDisplaySymbol))
                return false;
            if (!String.IsNullOrEmpty(Condition))
            {
                if (!NCalcUtility.EvaluateNCalcCondition(GetExpression(ref Condition, ncalcInitializer).Evaluate()))
                    return false;
            }
            foreach (Modifier modifier in Modifiers)
            {
                if (modifier.Languages.Count > 0 && !modifier.Languages.Contains(Localization.CurrentDisplaySymbol))
                    continue;
                if (!String.IsNullOrEmpty(modifier.Condition))
                {
                    if (!NCalcUtility.EvaluateNCalcCondition(GetExpression(ref modifier.Condition, ncalcInitializer).Evaluate()))
                        continue;
                }
                if (modifier is FindAndReplacer replacer && replacer.Find != null)
                {
                    String replace = replacer.Replace;
                    if (replacer.AsExpression && !String.IsNullOrEmpty(replace))
                    {
                        replace = NCalcUtility.EvaluateNCalcString(GetExpression(ref replace, ncalcInitializer).Evaluate());
                    }
                    str = replacer.Find.Replace(str, replace);
                }
                else if (modifier is Appender appender)
                {
                    String add = appender.Text;
                    if (appender.AsExpression && !String.IsNullOrEmpty(add))
                    {
                        add = NCalcUtility.EvaluateNCalcString(GetExpression(ref add, ncalcInitializer).Evaluate());
                    }
                    appender.Apply(ref str, add);
                }
                return true;
            }
            return false;
        }

        public static Dictionary<String, LogicalExpression> expressionCache = new Dictionary<String, LogicalExpression>();

        private delegate void ExpressionInitializer(ref Expression expr);

        private static List<TextPatcher> DialogPatchers = new List<TextPatcher>();
        private static List<TextPatcher> BattleDialogPatchers = new List<TextPatcher>();
        private static List<TextPatcher> InterfacePatchers = new List<TextPatcher>();
        private static List<TextPatcher> DatabasePatchers = new List<TextPatcher>();

        private String Condition = String.Empty;
        private HashSet<String> Languages = new HashSet<String>();
        private List<Modifier> Modifiers = new List<Modifier>();

        private class Modifier
        {
            public String Condition = String.Empty;
            public HashSet<String> Languages = new HashSet<String>();
        }

        private class FindAndReplacer : Modifier
        {
            public Regex Find = null;
            public String Replace = String.Empty;
            public Boolean AsExpression = false;
        }

        private class Appender : Modifier
        {
            public String Text = String.Empty;
            public Boolean AsExpression = false;
            public Boolean IsAppend = true;

            public void Apply(ref String str, String add)
            {
                if (IsAppend)
                {
                    if (str.Length > 0 && str[str.Length - 1] == ']')
                    {
                        Int32 bStart = str.LastIndexOf('[');
                        if (bStart >= 0)
                        {
                            String lastCode = str.Substring(bStart);
                            if (lastCode == "[ENDN]" || lastCode.StartsWith("[TIME="))
                            {
                                str = str.Substring(0, bStart) + add + lastCode;
                                return;
                            }
                        }
                    }
                    str += add;
                }
                else
                {
                    str = add + str;
                }
            }
        }
    }
}
