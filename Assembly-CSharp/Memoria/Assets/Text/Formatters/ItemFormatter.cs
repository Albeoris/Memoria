using Memoria.Prime.Text;
using System;
using System.Text;

namespace Memoria.Assets
{
	public static class ItemFormatter
	{
		private const String Separator = "------";

		private static readonly TextReplacements Replacements = new TextReplacements
		{
			{"[A85038][HSHD]", "{b}"},
			{"[383838][HSHD]", "{/b}"}
		}.Commit();

		public static TxtEntry[] Build(String prefix, String[] itemNames, String[] itemHelps, String[] itemBattle)
		{
			TxtEntry[] abilities = new TxtEntry[Math.Max(itemNames.Length, itemHelps.Length)];
			for (Int32 i = 0; i < abilities.Length; i++)
			{
				String name = i < itemNames.Length ? itemNames[i] : String.Empty;
				String help = i < itemHelps.Length ? itemHelps[i] : String.Empty;
				String bttl = i < itemBattle.Length ? itemBattle[i] : String.Empty;
				abilities[i] = Build(prefix, name, help, bttl);
			}
			return abilities;
		}

		private static TxtEntry Build(String prefix, String itemName, String itemHelp, String itemBattle)
		{
			return new TxtEntry
			{
				Prefix = prefix,
				Value = FormatValue(itemName, itemHelp, itemBattle)
			};
		}

		private static String FormatValue(String itemName, String itemHelp, String itemBattle)
		{
			StringBuilder sb = new StringBuilder(itemName.Length + itemHelp.Length + itemBattle.Length + 8);
			sb.AppendLine(itemName);
			sb.AppendLine(itemHelp.ReplaceAll(Replacements.Forward));
			sb.AppendLine(Separator);
			sb.Append(itemBattle.ReplaceAll(Replacements.Forward));
			return sb.ToString();
		}

		public static void Parse(TxtEntry[] entreis, out String[] itemNames, out String[] itemHelps, out String[] itemBattle)
		{
			itemNames = new String[entreis.Length];
			itemHelps = new String[entreis.Length];
			itemBattle = new String[entreis.Length];

			for (Int32 i = 0; i < entreis.Length; i++)
			{
				String value = entreis[i].Value;
				Int32 newLineIndex = value.IndexOf('\n');
				Int32 helpIndex = newLineIndex + 1;
				if (newLineIndex < 0 || helpIndex >= value.Length)
				{
					itemNames[i] = value;
					itemHelps[i] = String.Empty;
					itemBattle[i] = String.Empty;
					continue;
				}

				if (newLineIndex == 0)
				{
					itemNames[i] = String.Empty;
				}
				else
				{
					itemNames[i] = value.Substring(0, newLineIndex);
				}

				Int32 separatorIndex = value.IndexOf(Separator, helpIndex, StringComparison.Ordinal) - 1;
				if (separatorIndex < 0)
				{
					itemHelps[i] = value.Substring(helpIndex).ReplaceAll(Replacements.Backward);
					itemBattle[i] = String.Empty;
					continue;
				}

				if (separatorIndex == 0)
					itemHelps[i] = String.Empty;
				else
					itemHelps[i] = value.Substring(helpIndex, separatorIndex - helpIndex).ReplaceAll(Replacements.Backward);

				Int32 battleIndex = separatorIndex + Separator.Length + 2;
				if (battleIndex >= value.Length)
					itemBattle[i] = String.Empty;
				else
					itemBattle[i] = value.Substring(battleIndex).ReplaceAll(Replacements.Backward);
			}
		}
	}
}
