using System;
using System.Collections.Generic;
using System.Globalization;

namespace Memoria.Launcher
{
    public sealed class GameLaunchOptions
    {
        public Boolean ModelViewer { get; private set; }
        public Boolean FieldCreator { get; private set; }
        public Int32? Battle { get; private set; }
        public Boolean Autoload { get; private set; }
        public Boolean Debugger { get; private set; }
        public Int32? Seed { get; private set; }
        public Int32[] Party { get; private set; }
        public Boolean HasStartupMode => ModelViewer || FieldCreator || Battle.HasValue || Autoload;

        public static GameLaunchOptions Parse(String[] arguments)
        {
            GameLaunchOptions options = new GameLaunchOptions();
            for (Int32 i = 0; i < arguments.Length; i++)
            {
                switch (arguments[i].ToLowerInvariant())
                {
                    case "-memoria-model-viewer":
                        options.ModelViewer = true;
                        break;
                    case "-memoria-field-creator":
                        options.FieldCreator = true;
                        break;
                    case "-memoria-battle":
                        options.Battle = ParseInteger(ReadValue(arguments, ref i), 0, Int16.MaxValue, "battle");
                        break;
                    case "-memoria-autoload":
                        options.Autoload = true;
                        break;
                    case "-memoria-debugger":
                        options.Debugger = true;
                        break;
                    case "-memoria-seed":
                        options.Seed = ParseInteger(ReadValue(arguments, ref i), Int32.MinValue, Int32.MaxValue, "seed");
                        break;
                    case "-memoria-party":
                        String[] members = ReadValue(arguments, ref i).Split(',');
                        if (members.Length < 1 || members.Length > 4)
                            throw new ArgumentException("The battle party must contain between one and four character IDs.");
                        options.Party = new Int32[members.Length];
                        for (Int32 member = 0; member < members.Length; member++)
                        {
                            options.Party[member] = ParseInteger(members[member], 0, 254, "party character ID");
                            for (Int32 previous = 0; previous < member; previous++)
                            {
                                if (options.Party[previous] == options.Party[member])
                                    throw new ArgumentException("The battle party cannot contain duplicate character IDs.");
                            }
                        }
                        break;
                }
            }
            if ((options.Party != null || options.Seed.HasValue) && !options.Battle.HasValue)
                throw new ArgumentException("Party and seed options require -memoria-battle.");
            return options;
        }

        public String ToGameArguments()
        {
            List<String> arguments = new List<String>();
            if (ModelViewer)
                arguments.Add("-memoria-model-viewer");
            if (FieldCreator)
                arguments.Add("-memoria-field-creator");
            if (Battle.HasValue)
                arguments.Add("-memoria-battle " + Battle.Value.ToString(CultureInfo.InvariantCulture));
            if (Autoload)
                arguments.Add("-memoria-autoload");
            if (Seed.HasValue)
                arguments.Add("-memoria-seed " + Seed.Value.ToString(CultureInfo.InvariantCulture));
            if (Party != null)
            {
                String[] members = new String[Party.Length];
                for (Int32 i = 0; i < Party.Length; i++)
                    members[i] = Party[i].ToString(CultureInfo.InvariantCulture);
                arguments.Add("-memoria-party " + String.Join(",", members));
            }
            return String.Join(" ", arguments.ToArray());
        }

        private static String ReadValue(String[] arguments, ref Int32 index)
        {
            if (++index >= arguments.Length)
                throw new ArgumentException("A value is required for " + arguments[index - 1] + ".");
            return arguments[index];
        }

        private static Int32 ParseInteger(String value, Int32 minimum, Int32 maximum, String name)
        {
            if (!Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out Int32 number) || number < minimum || number > maximum)
                throw new ArgumentException("Invalid " + name + ": " + value + ".");
            return number;
        }
    }
}
