using System;
using System.IO;
using System.Linq;
using Memoria;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable EmptyConstructor
// ReSharper disable UnusedMember.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace FF9
{
    [ExportedType("5¨ÿĶńńńń8!!!èzĤ·ĉxċĝĺSÐÌĻ¯A¸Êðð}wğÿBVjġ9TCĳìċÀ!éôLK¢ŀªĕĹNµkčĿ?®zĺĶĠĽÎ`iüĔ©Ğþ~Ĳ¨NãÂěÐ$)eĕ¦ĦÖŃæeÕÍÒĽ!o/!!!ĄÀļěī;čd¥Ğ2onŁ¶ĔėíĎu}tĄðj>ÃßđĮmUė$ÖDĊÃÇćĢÙpď¯Y3Ý¾Zĳįńńńń")]
    public class ff9abil
    {
        //private const Byte FF9ABIL_PLAYER_MAX = 8;
        //private const Byte FF9ABIL_EQUIP_MAX = 3;
        //private const Byte FF9ABIL_NONE = 0;
        //private const Byte FF9ABIL_PA_SLOT_MAX = 16;
        //private const Byte FF9ABIL_AA_START = 0;
        //private const Byte FF9ABIL_AA_MAX = 192;
        //private const Byte FF9ABIL_SA_START = 192;
        //private const Byte FF9ABIL_SA_MAX = 64;
        //private const Byte FF9ABIL_SA_SIZE = 8;
        //private const Byte FF9ABIL_SA_HP10 = 197;
        //private const Byte FF9ABIL_SA_HP20 = 198;
        //private const Byte FF9ABIL_SA_MP10 = 199;
        //private const Byte FF9ABIL_SA_MP20 = 200;
        //private const Byte FF9ABIL_SA_FIELD_MAX = 4;
        //private const Byte FF9ABIL_SA_UP_EXP = 235;
        //private const Byte FF9ABIL_SA_UP_AP = 236;
        //private const Byte FF9ABIL_SA_UP_GIL = 237;
        //private const Byte FF9ABIL_SA_ESCAPE_GIL = 238;
        //private const Byte FF9ABIL_EVENT_SUMMON_GARNET = 18;

        public static PA_DATA[][] _FF9Abil_PaData;
        public static SA_DATA[] _FF9Abil_SaData;

        static ff9abil()
        {
            _FF9Abil_PaData = LoadCharacterAbilities();
            _FF9Abil_SaData = new SA_DATA[64]
            {
                new SA_DATA(0, 15, 0, 0, 8301),
                new SA_DATA(0, 6, 13, 43, 8301),
                new SA_DATA(0, 9, 24, 84, 8301),
                new SA_DATA(0, 10, 35, 125, 8301),
                new SA_DATA(0, 12, 46, 166, 8301),
                new SA_DATA(0, 4, 56, 206, 4218),
                new SA_DATA(0, 8, 63, 227, 4218),
                new SA_DATA(0, 4, 70, 248, 4219),
                new SA_DATA(0, 8, 77, 269, 4219),
                new SA_DATA(0, 2, 84, 290, 8292),
                new SA_DATA(0, 5, 94, 323, 8330),
                new SA_DATA(0, 16, 103, 364, 8317),
                new SA_DATA(0, 5, 114, 397, 8311),
                new SA_DATA(0, 3, 124, 434, 8303),
                new SA_DATA(0, 2, 136, 473, 8303),
                new SA_DATA(0, 4, 147, 505, 8303),
                new SA_DATA(0, 2, 160, 543, 8303),
                new SA_DATA(0, 3, 174, 582, 8303),
                new SA_DATA(0, 2, 188, 614, 8303),
                new SA_DATA(0, 4, 201, 645, 8303),
                new SA_DATA(0, 2, 214, 676, 8303),
                new SA_DATA(0, 4, 224, 707, 8306),
                new SA_DATA(0, 5, 234, 747, 4202),
                new SA_DATA(0, 5, 247, 767, 4238),
                new SA_DATA(0, 2, 257, 795, 4220),
                new SA_DATA(0, 3, 264, 817, 8354),
                new SA_DATA(0, 1, 275, 871, 4259),
                new SA_DATA(0, 4, 290, 904, 8323),
                new SA_DATA(0, 19, 298, 944, 8302),
                new SA_DATA(0, 3, 310, 974, 8302),
                new SA_DATA(0, 7, 319, 1005, 8280),
                new SA_DATA(0, 17, 332, 1040, 8325),
                new SA_DATA(0, 13, 342, 1093, 4224),
                new SA_DATA(0, 10, 356, 1118, 8302),
                new SA_DATA(0, 11, 368, 1149, 8301),
                new SA_DATA(0, 8, 376, 1180, 4258),
                new SA_DATA(0, 8, 386, 1209, 8311),
                new SA_DATA(0, 6, 394, 1250, 8301),
                new SA_DATA(0, 4, 400, 1287, 8301),
                new SA_DATA(0, 5, 414, 1323, 8282),
                new SA_DATA(0, 4, 424, 1355, 4250),
                new SA_DATA(0, 4, 434, 1389, 4231),
                new SA_DATA(0, 5, 440, 1412, 8310),
                new SA_DATA(0, 7, 451, 1447, 4252),
                new SA_DATA(0, 3, 460, 1475, 8289),
                new SA_DATA(0, 5, 471, 1510, 8285),
                new SA_DATA(0, 3, 483, 1541, 8316),
                new SA_DATA(0, 3, 492, 1584, 8294),
                new SA_DATA(0, 5, 505, 1617, 4186),
                new SA_DATA(0, 4, 515, 1637, 4253),
                new SA_DATA(0, 4, 524, 1672, 4207),
                new SA_DATA(0, 4, 536, 1695, 4195),
                new SA_DATA(0, 8, 546, 1717, 8339),
                new SA_DATA(0, 4, 557, 1764, 8307),
                new SA_DATA(0, 9, 563, 1810, 8300),
                new SA_DATA(0, 6, 576, 1839, 4251),
                new SA_DATA(0, 3, 586, 1865, 8334),
                new SA_DATA(0, 4, 598, 1909, 4181),
                new SA_DATA(0, 5, 609, 1928, 4208),
                new SA_DATA(0, 12, 622, 1952, 4254),
                new SA_DATA(0, 5, 628, 1981, 4241),
                new SA_DATA(0, 3, 641, 2008, 8280),
                new SA_DATA(0, 5, 645, 2038, 8306),
                new SA_DATA(0, 20, 652, 2068, 4096)
            };

            //String[] skillNames = EmbadedSentenseLoader.LoadSentense(EmbadedTextResources.SkillNames);
            //using (var writer = new CsvWriter("D:\\SkillCrys.csv"))
            //{

            //}
        }

        public ff9abil()
        {
        }

        public static bool FF9Abil_IsEnableSA(uint[] sa, int abil_id)
        {
            int num = abil_id - 192;
            return (sa[num >> 5] & 1 << num) != 0L;
        }

        public static void FF9Abil_SetEnableSA(int slot_id, int abil_id, bool enable)
        {
            uint[] numArray = FF9StateSystem.Common.FF9.player[slot_id].sa;
            int num = abil_id - 192;
            if (enable)
                numArray[num >> 5] |= (uint)(1 << num);
            else
                numArray[num >> 5] &= (uint)~(1 << num);
        }

        public static bool FF9Abil_GetEnableSA(int slot_id, int abil_id)
        {
            return FF9Abil_IsEnableSA(FF9StateSystem.Common.FF9.player[slot_id].sa, abil_id);
        }

        public static int FF9Abil_GetAp(int slot_id, int abil_id)
        {
            int index = FF9Abil_GetIndex(slot_id, abil_id);
            if (index < 0)
                return -1;
            return FF9StateSystem.Common.FF9.player[slot_id].pa[index];
        }

        public static int FF9Abil_SetAp(int slot_id, int abil_id, int ap)
        {
            int index = FF9Abil_GetIndex(slot_id, abil_id);
            if (index < 0)
                return -1;
            int num = FF9StateSystem.Common.FF9.player[slot_id].pa[index];
            FF9StateSystem.Common.FF9.player[slot_id].pa[index] = (byte)ap;
            return num;
        }

        public static int FF9Abil_ClearAp(int slot_id, int abil_id)
        {
            return FF9Abil_SetAp(slot_id, abil_id, 0);
        }

        public static int FF9Abil_GetMax(int slot_id, int abil_id)
        {
            int index = FF9Abil_GetIndex(slot_id, abil_id);
            if (index < 0)
                return 0;

            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            return _FF9Abil_PaData[player.info.menu_type][index].max_ap;
        }

        public static bool FF9Abil_HasAp(PLAYER play)
        {
            if ((play.category & 16) == 0 && play.info.menu_type < 8)
                return true;

            if (play.info.menu_type >= _FF9Abil_PaData.Length)
                return false;

            return _FF9Abil_PaData[play.info.menu_type].Any(pa => pa.max_ap > 0);
        }

        public static int FF9Abil_GetIndex(int slot_id, int abil_id)
        {
            if (slot_id >= 9)
                return -1;

            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            if (player.info.menu_type >= 16)
                return -1;

            PA_DATA[] array = _FF9Abil_PaData[player.info.menu_type];
            for (int index = 0; index < array.Length; ++index)
            {
                if (_FF9Abil_PaData[player.info.menu_type][index].id == abil_id)
                    return index;
            }

            return -1;
        }

        public static bool FF9Abil_IsMaster(int slot_id, int abil_id)
        {
            int ap = FF9Abil_GetAp(slot_id, abil_id);
            int max = FF9Abil_GetMax(slot_id, abil_id);
            return ap >= 0 && max >= 0 && ap >= max;
        }

        public static bool FF9Abil_SetMaster(int slot_id, int abil_id)
        {
            int index = FF9Abil_GetIndex(slot_id, abil_id);
            if (index < 0)
                return false;

            PLAYER player = FF9StateSystem.Common.FF9.player[slot_id];
            player.pa[index] = _FF9Abil_PaData[player.info.menu_type][index].max_ap;
            return true;
        }

        private static PA_DATA[][] LoadCharacterAbilities()
        {
            try
            {
                if (!Directory.Exists(DataResources.CharacterAbilitiesDirectory))
                    throw new DirectoryNotFoundException($"[ff9abil] Cannot load character abilities because a directory does not exist: [{DataResources.CharacterAbilitiesDirectory}].");

                CharacterPresetId[] presetIds = (CharacterPresetId[])Enum.GetValues(typeof(CharacterPresetId));
                PA_DATA[][] result = new PA_DATA[presetIds.Length][];
                for (Int32 id = 0; id < result.Length; id++)
                {
                    CharacterPresetId presetId = presetIds[id];
                    result[id] = LoadCharacterAbilities(presetId);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ff9abil] Load character abilities failed.");
                UIManager.Input.ConfirmQuit();
                return null;
            }
        }

        private static PA_DATA[] LoadCharacterAbilities(CharacterPresetId presetId)
        {
            try
            {
                String inputPath = DataResources.GetCsvCharacterAbilitiesPath(presetId);

                if (!File.Exists(inputPath))
                    throw new FileNotFoundException($"File with {presetId}'s abilities not found: [{inputPath}]");

                CharacterAbility[] abilities = CsvReader.Read<CharacterAbility>(inputPath);
                if (abilities.Length != 48)
                    throw new NotSupportedException($"You must set 48 abilities, but there {abilities.Length}. Any number of abilities will be available after a game stabilization."); // TODO Json, Player, SettingsState, EqupUI, ff9feqp

                PA_DATA[] result = new PA_DATA[abilities.Length];
                for (int i = 0; i < abilities.Length; i++)
                {
                    CharacterAbility source = abilities[i];
                    result[i] = new PA_DATA(source.Id, source.Ap);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"Failed to load {presetId}'s  learnable abilities.", ex);
            }
        }
    }
}