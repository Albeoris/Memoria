using System;
using System.IO;
using System.Collections.Generic;

namespace Memoria.Compiler
{
	internal static class Program
	{
		private const String DefaultReferencesPath64 = "../../../x64/FF9_Data/Managed/";
		private const String DefaultReferencesPath86 = "../../../x86/FF9_Data/Managed/";
		private const String DefaultSourcesPath = "../Sources/";
		private const String DefaultOutputPath = "../";
		private const String DefaultOutputName = "Memoria.Scripts.dll";
		private const String GameRootPath = "../../../";
		private const String ModSourcesPath = "/StreamingAssets/Scripts/Sources/";
		private const String ModOutputPath = "/StreamingAssets/Scripts/";

		private static Int32 modSelectionIndex = -1;

		internal static void Main(String[] args)
		{
			Boolean loop = true;
			while (loop)
			{
				loop = false;
				try
				{
					Console.Clear();
					Compile();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					Console.WriteLine("Fail!");
					Console.WriteLine("Press R to retry or any other key to exit...");
					ConsoleKey key = Console.ReadKey(true).Key;
					loop = key == ConsoleKey.R;
				}
			}
		}

		private static void Compile()
		{
			String referencesDirectoryPath;
			if (Directory.Exists(DefaultReferencesPath64))
				referencesDirectoryPath = Path.GetFullPath(DefaultReferencesPath64);
			else if (Directory.Exists(DefaultReferencesPath86))
				referencesDirectoryPath = Path.GetFullPath(DefaultReferencesPath86);
			else
				throw new DirectoryNotFoundException("Cannot find the directory \"FF9_Data/Managed/\"");

			String sourcesDirectoryPath = Path.GetFullPath(DefaultSourcesPath);
			if (!Directory.Exists(sourcesDirectoryPath))
				throw new DirectoryNotFoundException("Directory not found: " + sourcesDirectoryPath);

			String outputDirectoryPath = Path.GetFullPath(DefaultOutputPath);
			if (!Directory.Exists(outputDirectoryPath))
				throw new DirectoryNotFoundException("Directory not found: " + outputDirectoryPath);

			String outputFileName = DefaultOutputName;
			if (String.IsNullOrWhiteSpace(outputFileName))
				throw new InvalidDataException("You must specify a name for output file.");

			List<String> modFolder = new List<String>();
			foreach (String folder in Directory.GetDirectories(Path.GetFullPath(GameRootPath)))
				if (Directory.Exists(folder + ModSourcesPath))
					modFolder.Add(folder);

			if (modFolder.Count == 0)
			{
				Compiler compiler = new Compiler(referencesDirectoryPath, sourcesDirectoryPath, outputDirectoryPath, outputFileName);
				compiler.Compile();
				Console.WriteLine($"Compilation of the main sources succeeded.");
				Console.WriteLine("Press enter to exit...");
				Console.ReadLine();
				return;
			}

			if (modSelectionIndex >= modFolder.Count)
				modSelectionIndex = -1;
			Console.WriteLine($"Pick which script(s) to compile by pressing a key.");
			Console.WriteLine($"(A) Compile all");
			Int32 selectionRow = Console.CursorTop;
			if (modSelectionIndex < 0)
				Console.WriteLine($"(C) Compile the main sources (use ↓ and ↑ to select others)");
			else
				Console.WriteLine($"(C) Compile the sources of the mod {Path.GetFileName(modFolder[modSelectionIndex])} (use ↓ and ↑ to select others)");
			Console.WriteLine($"(Q) Quit");

			Boolean validKey;
			ConsoleKey key;
			do
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);
				key = keyInfo.Key;
				validKey = key == ConsoleKey.A || key == ConsoleKey.C || key == ConsoleKey.Q;
				if (key == ConsoleKey.DownArrow || key == ConsoleKey.UpArrow)
				{
					if (key == ConsoleKey.DownArrow)
						modSelectionIndex++;
					else
						modSelectionIndex--;
					if (modSelectionIndex <= -2)
						modSelectionIndex = modFolder.Count - 1;
					else if (modSelectionIndex >= modFolder.Count)
						modSelectionIndex = -1;
					Int32 cursorPosX = Console.CursorLeft;
					Int32 cursorPosY = Console.CursorTop;
					Console.SetCursorPosition(0, selectionRow);
					Console.Write(new String(' ', Console.BufferWidth));
					Console.SetCursorPosition(0, selectionRow);
					if (modSelectionIndex < 0)
						Console.WriteLine($"(C) Compile the main sources (use ↓ and ↑ to select others)");
					else
						Console.WriteLine($"(C) Compile the sources of the mod {Path.GetFileName(modFolder[modSelectionIndex])} (use ↓ and ↑ to select others)");
					Console.SetCursorPosition(cursorPosX, cursorPosY);
				}
			}
			while (!validKey);

			if (key == ConsoleKey.Q)
				return;

			List<String> compileList = new List<String>();
			if (key == ConsoleKey.A)
			{
				compileList.Add(String.Empty);
				compileList.AddRange(modFolder);
			}
			else
			{
				compileList.Add(modSelectionIndex >= 0 ? modFolder[modSelectionIndex] : String.Empty);
			}

			foreach (String folder in compileList)
			{
				if (String.IsNullOrEmpty(folder))
				{
					Console.WriteLine($"Compiling main sources...");
					Compiler compiler = new Compiler(referencesDirectoryPath, sourcesDirectoryPath, outputDirectoryPath, outputFileName);
					compiler.Compile();
					Console.WriteLine($"Compilation of the main sources succeeded.");
					Console.WriteLine();
				}
				else
				{
					String modName = Path.GetFileName(folder);
					Console.WriteLine($"Compiling sources of {modName}...");
					Compiler modCompiler = new Compiler(referencesDirectoryPath, folder + ModSourcesPath, folder + ModOutputPath, $"Memoria.Scripts.{modName}.dll");
					modCompiler.Compile();
					Console.WriteLine($"Compilation of the {modName} sources succeeded.");
					Console.WriteLine();
				}
			}
			Console.WriteLine("Press enter to exit...");
			Console.ReadLine();
		}
	}
}