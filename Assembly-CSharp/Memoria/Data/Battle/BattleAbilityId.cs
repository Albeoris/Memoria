namespace Memoria.Data
{
    public enum BattleAbilityId : byte
    {
        ///<summary></summary>
        Void = 0000,

        ///<summary>Restores HP of single/multiple targets.</summary>
        Cure = 0001,

        ///<summary>Restores a lot of HP of single/multiple targets.</summary>
        Cura = 0002,

        ///<summary>Restores max HP of single/multiple targets.</summary>
        Curaga = 0003,

        ///<summary>Gradually restores HP.</summary>
        Regen = 0004,

        ///<summary>Recover from {b}KO{/b}.</summary>
        Life = 0005,

        ///<summary>Recover from {b}KO{/b} with full HP.</summary>
        FullLife = 0006,

        ///<summary>Scan enemy to determine HP, MP, and weaknesses.</summary>
        Scan = 0007,

        ///<summary>Removes {b}Venom{/b} and {b}Poison{/b}.</summary>
        Panacea = 0008,

        ///<summary>Removes {b}Petrify{/b} and {b}Gradual Petrify{/b}.</summary>
        Stona = 0009,

        ///<summary>Removes various abnormal status effects.</summary>
        Esuna = 0010,

        ///<summary>Reduces damage from magic attacks.</summary>
        Shell = 0011,

        ///<summary>Reduces damage from physical attacks.</summary>
        Protect = 0012,

        ///<summary>Speeds up ATB Gauge.</summary>
        Haste = 0013,

        ///<summary>Causes {b}Silence{/b}, which disables magic in single/multiple targets.</summary>
        Silence = 0014,

        ///<summary>Makes single/multiple targets smaller.</summary>
        Mini = 0015,

        ///<summary>Reflects magic attacks back onto caster.</summary>
        Reflect = 0016,

        ///<summary>Causes {b}Confuse{/b}, which makes single/multiple targets erratic.</summary>
        Confuse = 0017,

        ///<summary>Causes single/multiple targets to attack uncontrollably.</summary>
        Berserk = 0018,

        ///<summary>Causes {b}Darkness{/b}, which hinders accuracy of physical attacks.</summary>
        Blind = 0019,

        ///<summary>Allows single/multiple targets to float in the air.</summary>
        Float = 0020,

        ///<summary>Removes abnormal status caused by magic attacks.</summary>
        Dispel = 0021,

        ///<summary>Raises physical attack power.</summary>
        Might = 0022,

        ///<summary>Extracts Ore from a target.</summary>
        Jewel = 0023,

        ///<summary>Causes {b}Holy{/b} damage.</summary>
        Holy = 0024,

        ///<summary>Causes {b}Fire{/b} damage to single/multiple targets.</summary>
        Fire = 0025,

        ///<summary>Causes a lot of {b}Fire{/b} damage to single/multiple targets.</summary>
        Fira = 0026,

        ///<summary>Causes max {b}Fire{/b} damage to single/multiple targets.</summary>
        Firaga = 0027,

        ///<summary>Puts single/multiple targets to sleep.</summary>
        Sleep = 0028,

        ///<summary>Causes {b}Ice{/b} damage to single/multiple targets.</summary>
        Blizzard = 0029,

        ///<summary>Causes a lot of {b}Ice{/b} damage to single/multiple targets.</summary>
        Blizzara = 0030,

        ///<summary>Causes max {b}Ice{/b} damage to single/multiple targets.</summary>
        Blizzaga = 0031,

        ///<summary>Slows down ATB Gauge.</summary>
        Slow = 0032,

        ///<summary>Causes {b}Thunder{/b} damage to single/multiple targets.</summary>
        Thunder = 0033,

        ///<summary>Causes a lot of {b}Thunder{/b} damage to single/multiple targets.</summary>
        Thundara = 0034,

        ///<summary>Causes max {b}Thunder{/b} damage to single/multiple targets.</summary>
        Thundaga = 0035,

        ///<summary>Stops targets from taking any action.</summary>
        Stop = 0036,

        ///<summary>Causes {b}Poison{/b} to single/multiple targets.</summary>
        Poison = 0037,

        ///<summary>Causes {b}Non-elemental{/b} damage and {b}Poison{/b} to single/multiple targets.</summary>
        Bio = 0038,

        ///<summary>Absorbs MP from the target and transfers it to the spell caster.</summary>
        Osmose = 0039,

        ///<summary>Drains HP from the target and transfers it to the spell caster.</summary>
        Drain = 0040,

        ///<summary>Amount of damage depends on the target’s HP.</summary>
        Demi = 0041,

        ///<summary>Causes {b}Non-elemental{/b} damage.</summary>
        Comet = 0042,

        ///<summary>KOs the target.</summary>
        Death = 0043,

        ///<summary>Causes {b}Petrify{/b}.</summary>
        Break = 0044,

        ///<summary>Causes {b}Water{/b} damage to single/multiple targets.</summary>
        Water = 0045,

        ///<summary>Causes {b}Non-elemental{/b} damage to all enemies.</summary>
        Meteor = 0046,

        ///<summary>Causes {b}Non-elemental{/b} damage.</summary>
        Flare = 0047,

        ///<summary>Causes {b}Shadow{/b} damage to all targets.</summary>
        Doomsday = 0048,

        ///<summary>Causes {b}Ice{/b} damage to all enemies.</summary>
        Shiva = 0049,

        ///<summary>Strike the enemy with Fire Sword.</summary>
        FireSword = 0050,

        ///<summary>Causes {b}Fire{/b} damage to all enemies.</summary>
        Ifrit = 0051,

        ///<summary>Strike the enemy with Fira Sword.</summary>
        FiraSword = 0052,

        ///<summary>Causes {b}Thunder{/b} damage to all enemies.</summary>
        Ramuh = 0053,

        ///<summary>Strike the enemy with Firaga Sword.</summary>
        FiragaSword = 0054,

        ///<summary>Reduces all enemies’ HP. Amount of damage depends on targets’ HP.</summary>
        Atomos = 0055,

        ///<summary>Strike the enemy with Blizzard Sword.</summary>
        BlizzardSword = 0056,

        ///<summary>Strike the enemy with Blizzara Sword.</summary>
        BlizzaraSword = 0057,

        ///<summary>Causes KO to all enemies.</summary>
        Odin = 0058,

        ///<summary>Strike the enemy with Blizzaga Sword.</summary>
        BlizzagaSword = 0059,

        ///<summary>Causes {b}Water{/b} damage to all enemies.</summary>
        Leviathan = 0060,

        ///<summary>Strike the enemy with Thunder Sword.</summary>
        ThunderSword = 0061,

        ///<summary>Causes {b}Non-elemental{/b} damage to all enemies.</summary>
        Bahamut = 0062,

        ///<summary>Strike the enemy with Thundara Sword.</summary>
        ThundaraSword = 0063,

        ///<summary>Causes {b}Shadow{/b} damage to all enemies.</summary>
        Ark = 0064,

        ///<summary>Strike the enemy with Thundaga Sword.</summary>
        ThundagaSword = 0065,

        ///<summary>Causes {b}Earth{/b} damage to all enemies.</summary>
        Fenrir1 = 0066,

        ///<summary>Causes {b}Wind{/b} damage to all enemies.</summary>
        Fenrir2 = 0067,

        ///<summary>Casts {b}Reflect{/b} on all party members.</summary>
        Carbuncle1 = 0068,

        ///<summary>Casts {b}Haste{/b} on all party members.</summary>
        Carbuncle2 = 0069,

        ///<summary>Casts {b}Shell{/b} on all party members.</summary>
        Carbuncle3 = 0070,

        ///<summary>Casts {b}Vanish{/b} on all party members.</summary>
        Carbuncle4 = 0071,

        ///<summary>Causes {b}Fire{/b} damage to all enemies, and all party members recover from {b}KO{/b}.</summary>
        Phoenix = 0072,

        ///<summary></summary>
        RebirthFlame = 0073,

        ///<summary>Causes {b}Holy{/b} damage to all enemies.</summary>
        Madeen = 0074,

        ///<summary>Strike the enemy with Bio Sword.</summary>
        BioSword = 0075,

        ///<summary>Strike the enemy with Water Sword.</summary>
        WaterSword = 0076,

        ///<summary>Causes {b}Non-elemental{/b} damage to the enemy.</summary>
        GoblinPunch = 0077,

        ///<summary>KOs all enemies whose LEVELs are multiples of 5.</summary>
        Lv5Death = 0078,

        ///<summary>Causes {b}Holy{/b} damage to enemies whose LEVELs are multiples of 4.</summary>
        Lv4Holy = 0079,

        ///<summary>Reduces {b}Defence{/b} of enemies whose LEVELs are multiples of 3.</summary>
        Lv3DefLess = 0080,

        ///<summary>Casts {b}Doom{/b} on the target.</summary>
        Doom = 0081,

        ///<summary>Randomly KOs a target.</summary>
        Roulette = 0082,

        ///<summary>Causes {b}Water{/b} damage to all enemies.</summary>
        AquaBreath = 0083,

        ///<summary>Casts {b}Shell{/b} and {b}Protect{/b} on all party members.</summary>
        MightyGuard = 0084,

        ///<summary>Reduces the target’s HP to 1.</summary>
        MatraMagic = 0085,

        ///<summary>Causes {b}Confuse{/b}, {b}Darkness{/b}, {b}Poison{/b}, {b}Slow{/b}, and {b}Mini{/b} to the enemy.</summary>
        BadBreath = 0086,

        ///<summary>Causes {b}Non-elemental{/b} damage to the target when your HP is 1.</summary>
        LimitGlove = 0087,

        ///<summary>Reduces the enemy’s HP by 1,000.</summary>
        ThousandNeedles = 0088,

        ///<summary>Damages with the difference between your max HP and current HP.</summary>
        PumpkinHead = 0089,

        ///<summary>Causes {b}Sleep{/b} to all targets.</summary>
        Night = 0090,

        ///<summary>Causes {b}Wind{/b} damage to all enemies.</summary>
        Twister = 0091,

        ///<summary>Causes {b}Earth{/b} damage to all enemies.</summary>
        EarthShake = 0092,

        ///<summary>Uses Remedy on all party members.</summary>
        AngelSnack = 0093,

        ///<summary>Amount of damage depends on the number of frogs you have caught.</summary>
        FrogDrop = 0094,

        ///<summary>Restores HP of all party members.</summary>
        WhiteWind = 0095,

        ///<summary>Makes a party member disappear.</summary>
        Vanish = 0096,

        ///<summary>Causes {b}Freeze{/b} to the enemy.</summary>
        Freeze = 0097,

        ///<summary>Causes {b}Heat{/b} to the enemy.</summary>
        MustardBomb = 0098,

        ///<summary>Reduces the enemy’s MP.</summary>
        MagicHammer = 0099,

        ///<summary>Casts {b}Life{/b} when KO’d.</summary>
        AutoLife = 0100,

        ///<summary>Escape from battle with high probability.</summary>
        Flee1 = 0101,

        ///<summary>See the enemy’s items.</summary>
        Detect = 0102,

        ///<summary>Allows back attack.</summary>
        WhatIsThat = 0103,

        ///<summary>Draws out the hidden power in thief swords.</summary>
        SoulBlade = 0104,

        ///<summary>Causes {b}Trouble{/b} to the target.</summary>
        Annoy = 0105,

        ///<summary>Sacrifice yourself to restore HP and MP to the other party members.</summary>
        Sacrifice = 0106,

        ///<summary>Deals physical damage by luck.</summary>
        LuckySeven = 0107,

        ///<summary>Deals physical damage to the target.</summary>
        Thievery = 0108,

        ///<summary>Deals physical damage to the enemy.</summary>
        FreeEnergy = 0109,

        ///<summary>Deals physical damage to all enemies.</summary>
        TidalFlame = 0110,

        ///<summary>Deals physical damage to the enemy.</summary>
        ScoopArt = 0111,

        ///<summary>Deals physical damage to all enemies.</summary>
        ShiftBreak = 0112,

        ///<summary>Deals physical damage to the enemy.</summary>
        StellarCircle5 = 0113,

        ///<summary>Deals physical damage to all enemies.</summary>
        MeoTwister = 0114,

        ///<summary>Deals physical damage to the enemy.</summary>
        Solution9 = 0115,

        ///<summary>Deals physical damage to all enemies.</summary>
        GrandLethal = 0116,

        ///<summary>Reduces the enemy’s HP and MP.</summary>
        Lancer = 0117,

        ///<summary>Casts {b}Regen{/b} on all party members.</summary>
        ReisWind = 0118,

        ///<summary>Reduces HP of all enemies.</summary>
        DragonBreath = 0119,

        ///<summary>Restores MP of all party members.</summary>
        WhiteDraw = 0120,

        ///<summary>Causes {b}Berserk{/b} to all targets.</summary>
        Luna = 0121,

        ///<summary>See for yourself.</summary>
        SixDragons = 0122,

        ///<summary>Causes {b}Non-elemental{/b} damage to all enemies.</summary>
        CherryBlossom = 0123,

        ///<summary>Deals physical damage to the enemy.</summary>
        DragonCrest = 0124,

        ///<summary>Restores HP and MP of one party member.</summary>
        Chakra1 = 0125,

        ///<summary>Causes {b}Non-elemental{/b} damage to the enemy by using Gil.</summary>
        SpareChange1 = 0126,

        ///<summary>Causes {b}Non-elemental{/b} damage to the enemy.</summary>
        NoMercy1 = 0127,

        ///<summary>Casts {b}Auto-Life{/b} and {b}Regen{/b} on one party member.</summary>
        Aura1 = 0128,

        ///<summary>Makes the enemy weak against some elemental property.</summary>
        Curse1 = 0129,

        ///<summary>Recover from {b}KO{/b}.</summary>
        Revive1 = 0130,

        ///<summary>Amount of damage depends on the enemy’s HP.</summary>
        DemiShock1 = 0131,

        ///<summary>Casts {b}Doom{/b} on the enemy.</summary>
        Countdown1 = 0132,

        ///<summary>Restores HP and MP of all party members.</summary>
        Chakra2 = 0133,

        ///<summary>Causes {b}Non-elemental{b} damage to all enemies by using Gil.</summary>
        SpareChange2 = 0134,

        ///<summary>Causes {b}Non-elemental{b} damage to all enemies.</summary>
        NoMercy2 = 0135,

        ///<summary>Casts {b}Auto-Life{/b} and {b}Regen{/b} on all party members.</summary>
        Aura2 = 0136,

        ///<summary>Makes all enemies weak against some elemental property.</summary>
        Curse2 = 0137,

        ///<summary>Recover all party members from {b}KO{/b}.</summary>
        Revive2 = 0138,

        ///<summary>Deals damage to all enemies. Amount of damage depends on the enemies’ HP.</summary>
        DemiShock2 = 0139,

        ///<summary>Casts {b}Doom{/b} on all enemies.</summary>
        Countdown2 = 0140,

        ///<summary>Reduces your HP to cause {b}Shadow{/b} damage to the enemy.</summary>
        Darkside = 0141,

        ///<summary>Damages with the difference between your max HP and current HP.</summary>
        MinusStrike = 0142,

        ///<summary>Knocks Out the target.</summary>
        IaiStrike = 0143,

        ///<summary>Reduces the enemy’s {b}Attack Pwr{/b}.</summary>
        PowerBreak = 0144,

        ///<summary>Reduces the enemy’s {b}Defence{/b}.</summary>
        ArmourBreak = 0145,

        ///<summary>Reduces the enemy’s {b}Magic Def{/b}.</summary>
        MentalBreak = 0146,

        ///<summary>Reduces the enemy’s {b}Magic{/b}.</summary>
        MagicBreak = 0147,

        ///<summary>Makes all Near Death party members ‘Attack.’</summary>
        Charge = 0148,

        ///<summary>Causes {b}Thunder{/b} damage to the enemy.</summary>
        ThunderSlash = 0149,

        ///<summary>Causes {b}Non-elemental{/b} damage to all enemies.</summary>
        StockBreak = 0150,

        ///<summary>Causes {b}Non-elemental{/b} damage to all enemies.</summary>
        Climhazzard = 0151,

        ///<summary>Deals physical damage to the enemy.</summary>
        Shock = 0152,

        ///<summary>Causes {b}Ice{/b} damage to all enemies.</summary>
        DiamondDust = 0153,

        ///<summary>Causes {b}Fire{/b} damage to all enemies.</summary>
        FlamesofHell = 0154,

        ///<summary>Causes {b}Thunder{/b} damage to all enemies.</summary>
        JudgementBolt = 0155,

        ///<summary>Amount of damage depends on the enemies’ HP.</summary>
        WormHole = 0156,

        ///<summary>Causes {b}Non-elemental{/b} damage to all enemies.</summary>
        Zantetsuken = 0157,

        ///<summary>Causes {b}Water{/b} damage to all enemies.</summary>
        Tsunami = 0158,

        ///<summary>Causes {b}Non-elemental{/b} damage to all enemies.</summary>
        MegaFlare = 0159,

        ///<summary>Causes Shadow damage to all enemies.</summary>
        EternalDarkness = 0160,

        ///<summary></summary>
        None1 = 0161,

        ///<summary></summary>
        None2 = 0162,

        ///<summary></summary>
        None3 = 0163,

        ///<summary></summary>
        None4 = 0164,

        ///<summary></summary>
        None5 = 0165,

        ///<summary></summary>
        None6 = 0166,

        ///<summary></summary>
        None7 = 0167,

        ///<summary></summary>
        None8 = 0168,

        ///<summary></summary>
        None9 = 0169,

        ///<summary></summary>
        None10 = 0170,

        ///<summary></summary>
        None11 = 0171,

        ///<summary></summary>
        Jump1 = 0172,

        ///<summary>Powerful Magic.</summary>
        Pyro = 0173,

        ///<summary>Powerful Magic.</summary>
        Medeo = 0174,

        ///<summary>Powerful Magic.</summary>
        Poly = 0175,

        ///<summary></summary>
        Attack = 0176,

        ///<summary></summary>
        Steal = 0177,

        ///<summary></summary>
        Jump2 = 0178,

        ///<summary></summary>
        Defend = 0179,

        ///<summary></summary>
        Flee2 = 0180,

        ///<summary></summary>
        Mug = 0181,

        ///<summary></summary>
        Change = 0182,

        ///<summary></summary>
        Eat = 0183,

        ///<summary></summary>
        Cook = 0184,

        ///<summary></summary>
        Spear1 = 0185,

        ///<summary></summary>
        Spear2 = 0186,

        ///<summary>Causes {b}Holy{/b} damage to all enemies.</summary>
        TerraHoming = 0187,

        ///<summary></summary>
        Focus = 0188,

        ///<summary>Strike the enemy with Flare Sword.</summary>
        FlareSword = 0189,

        ///<summary></summary>
        Throw = 0190,

        ///<summary>Strike the enemy with Doomsday Sword.</summary>
        DoomsdaySword = 0191,
    }
}