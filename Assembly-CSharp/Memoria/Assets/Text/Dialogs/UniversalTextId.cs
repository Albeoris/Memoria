using System;
using System.Collections.Generic;
using Memoria.Prime.Collections;

namespace Memoria.Assets
{
    // This class is a way to deal with the fact that text databases are sorted differently depending on the language
    // TODO: Handle battle texts (Ragtime Mouse)
    public static class UniversalTextId
    {
        public static Int32 GetMatchingTextId(String from, String to, Int32 fieldTextArea, Int32 textId)
        {
            Int32 universalId = GetUniversalTextId(from, fieldTextArea, textId);
            if (universalId < 0)
                return -1;
            return GetTextIdFromUniversalId(to, fieldTextArea, universalId);
        }

        public static Int32 GetUniversalTextId(String lang, Int32 fieldTextArea, Int32 textId)
        {
            TwoWayDictionary<Int32, Int32> dict = GetDictionary(lang, fieldTextArea);
            if (dict != null && dict.TryGetKey(textId, out Int32 universalId))
                return universalId;
            return textId;
        }

        public static Int32 GetTextIdFromUniversalId(String lang, Int32 fieldTextArea, Int32 universalId)
        {
            TwoWayDictionary<Int32, Int32> dict = GetDictionary(lang, fieldTextArea);
            if (dict != null && dict.TryGetValue(universalId, out Int32 textId))
                return textId;
            return universalId;
        }

        private static TwoWayDictionary<Int32, Int32> GetDictionary(String lang, Int32 fieldTextArea)
        {
            switch (fieldTextArea)
            {
                case 121:
                    if (lang == "ES")
                        return SetupDictionary(ref _dictShrinesES, 166, [154]);
                    break;
                case 134:
                    if (lang == "JP")
                        return SetupDictionary(ref _dictPinnacleRocksJP, 297, null, null, Dict([[53, 52]]));
                    if (lang == "US" || lang == "UK" || lang == "ES")
                        return SetupDictionary(ref _dictPinnacleRocksUSUKES, 297, [143, 174, 223, 224, 225, 226, 252, 294, 295], null, Dict([[122, 121], [142, 140]]));
                    break;
                case 186:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictHildaGarde3USUK, 273, null, Dict([[91, 266], [95, 267], [98, 268], [207, 269], [208, 270], [209, 271], [225, 272]]));
                    break;
                case 187:
                    if (lang == "UK")
                        return SetupDictionary(ref _dictEndingUK, 193, [138, 147, 174]);
                    if (lang == "GR")
                        return SetupDictionary(ref _dictEndingGR, 193, [138, 147, 173]);
                    break;
                case 2:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictPrimaVistaUSUK, 387, [264, 265]);
                    break;
                case 22:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictLindblumCastle1USUK, 688, null, Dict([[211, 686], [677, 687]]));
                    if (lang == "GR")
                        return SetupDictionary(ref _dictLindblumCastle1GR, 688, [389, 390]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictLindblumCastle1FR, 688, [201, 202, 483, 484, 485, 486, 505]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictLindblumCastle1ES, 688, [175, 201, 202, 389, 390, 401, 483, 484, 485, 486, 505, 584, 597, 625], null, Dict([[353, 343]]));
                    break;
                case 23:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictMistGatesUSUK, 213, [104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 153, 154, 160, 189, 198, 199, 200, 201, 205]);
                    if (lang == "JP")
                        return SetupDictionary(ref _dictMistGatesJP, 213, null, null, Dict([[53, 52], [125, 123]]));
                    if (lang == "GR")
                        return SetupDictionary(ref _dictMistGatesGR, 213, [104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 153, 154, 160, 189, 198, 199, 200, 201, 205, 211], null, Dict([[174, 150]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictMistGatesFR, 213, [153, 154, 174, 189, 198, 199, 200, 201, 205, 211], null, Dict([[174, 171]]));
                    if (lang == "IT")
                        return SetupDictionary(ref _dictMistGatesIT, 213, [118, 119, 120, 121, 122, 123, 205], null, Dict([[125, 205]]));
                    if (lang == "ES")
                        return SetupDictionary(ref _dictMistGatesES, 213, [104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 153, 154, 205, 211]);
                    break;
                case 276:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictLindblum1USUK, 705, null, Dict([[389, 685], [371, 686], [372, 687], [373, 688], [374, 689], [375, 690], [376, 691], [377, 692], [378, 693], [379, 694], [380, 695], [296, 696], [229, 697], [230, 698], [231, 699], [233, 700], [234, 701], [232, 702], [236, 703], [240, 704]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictLindblum1FR, 705, [220, 221, 270, 271, 518, 519, 520, 521, 522, 523]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictLindblum1ES, 705, [146, 220, 221, 270, 271, 343, 352, 559, 609]);
                    break;
                case 290:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictCleyraStormlessUSUK, 402, [92, 93, 94, 95, 96, 97, 98, 99]);
                    break;
                case 358:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictMadainSari1USUK, 920, [52, 109, 171, 172, 289, 357, 382, 450, 538, 544, 549, 550, 571, 609, 620, 623, 627, 636, 637, 638, 639, 640, 645, 646, 647, 648, 649, 652, 653, 654, 663, 687, 764, 765, 790, 864, 865, 866, 867, 868, 869, 879, 880, 881, 882, 883, 884, 892, 893, 894, 895, 896, 918, 919], null, Dict([[502, 493], [671, 641]]));
                    if (lang == "JP" || lang == "FR")
                        return SetupDictionary(ref _dictMadainSari1JPFR, 920, [192, 570, 571, 573, 574, 602, 607, 609, 620, 627, 636, 637, 638, 639, 640, 645, 646, 647, 648, 649, 652, 653, 654, 864, 865, 866, 867, 868, 869, 879, 880, 881, 882, 883, 884, 892, 893, 894, 895, 896, 918, 919], Dict([[599, 599], [600, 600], [601, 594], [603, 595], [604, 596], [605, 597], [606, 598]]), Dict([[671, 650]]));
                    if (lang == "GR")
                        return SetupDictionary(ref _dictMadainSari1GR, 920, [171, 172, 663, 892, 893, 894, 895, 896], Dict([[699, 878], [700, 879], [701, 880], [561, 881], [188, 882], [189, 883], [194, 884], [806, 885], [870, 886], [871, 887], [872, 888], [873, 889], [874, 890], [875, 891], [876, 892], [885, 893], [886, 894], [887, 895], [888, 896], [889, 897], [890, 898], [891, 899], [198, 900], [199, 901], [562, 902], [563, 903], [918, 904], [641, 905], [642, 906], [650, 907], [655, 908], [656, 909], [919, 910], [657, 911]]));
                    if (lang == "IT")
                        return SetupDictionary(ref _dictMadainSari1IT, 920, [573, 607, 609, 620, 627, 636, 637, 638, 639, 645, 648, 649, 653, 864, 865, 866, 867, 868, 869, 879, 880, 881, 882, 883, 884, 918, 919], Dict([[599, 603], [600, 604], [601, 598], [603, 599], [604, 600], [605, 601], [606, 602], [646, 639], [647, 641], [650, 633], [651, 634], [652, 642], [654, 643], [655, 637], [656, 638]]), Dict([[671, 660]]));
                    if (lang == "ES")
                        return SetupDictionary(ref _dictMadainSari1ES, 920, [52, 109, 171, 172, 192, 357, 382, 450, 512, 538, 544, 570, 571, 573, 574, 602, 607, 609, 620, 627, 636, 637, 638, 639, 640, 645, 646, 647, 648, 649, 652, 653, 654, 663, 687, 764, 765, 790, 864, 865, 866, 867, 868, 869, 879, 880, 881, 882, 883, 884, 892, 893, 894, 895, 896, 918, 919], Dict([[599, 588], [600, 589], [601, 583], [603, 584], [604, 585], [605, 586], [606, 587]]), Dict([[502, 493], [671, 639]]));
                    break;
                case 359:
                    if (lang == "JP")
                        return SetupDictionary(ref _dictGarganRooJP, 205, null, null, Dict([[52, 51]]));
                    break;
                case 360:
                    if (lang == "US" || lang == "UK" || lang == "ES")
                        return SetupDictionary(ref _dictMadainSari2USUKES, 127, [76, 77, 78, 98]);
                    if (lang == "GR")
                        return SetupDictionary(ref _dictMadainSari2GR, 127, [77, 78]);
                    if (lang == "FR" || lang == "IT")
                        return SetupDictionary(ref _dictMadainSari2FRIT, 127, [76, 77, 78]);
                    break;
                case 361:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictFossilRooUSUK, 234, [223]);
                    if (lang == "JP")
                        return SetupDictionary(ref _dictFossilRooJP, 234, null, null, Dict([[67, 66]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictFossilRooFR, 234, [178, 179, 180, 223]);
                    if (lang == "IT" || lang == "ES")
                        return SetupDictionary(ref _dictFossilRooITES, 234, [178, 179, 180, 212, 223]);
                    break;
                case 38:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictMognetCentralUSUK, 96, [85], Dict([[84, 94]]));
                    break;
                case 4:
                    if (lang == "JP")
                        return SetupDictionary(ref _dictEvilForestJP, 256, null, null, Dict([[52, 51]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictEvilForestFR, 256, [155, 156, 157, 158, 159]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictEvilForestIT, 256, [107, 131, 133, 155, 156, 157, 158, 159, 183, 185]);
                    break;
                case 40:
                    if (lang == "JP")
                        return SetupDictionary(ref _dictPrimaVistaRuinJP, 412, null, null, Dict([[52, 51]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictPrimaVistaRuinFR, 412, [131, 245, 250, 257, 303, 304, 305, 306, 307, 308, 309, 320, 321, 344, 345, 346, 347, 355, 380, 381, 382, 383, 384, 385, 386, 387]);
                    break;
                case 44:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictCleyraUSUK, 302, [92, 93, 94, 95, 96, 97, 98, 99, 269], Dict([[271, 263], [273, 262]]), Dict([[272, 262]]));
                    if (lang == "JP" || lang == "FR" || lang == "ES")
                        return SetupDictionary(ref _dictCleyraJPFRES, 302, [269], null, Dict([[272, 271]]));
                    if (lang == "GR")
                        return SetupDictionary(ref _dictCleyraGR, 302, null, Dict([[271, 273], [273, 271]]));
                    if (lang == "IT")
                        return SetupDictionary(ref _dictCleyraIT, 302, [269], Dict([[271, 271], [273, 270]]), Dict([[272, 270]]));
                    break;
                case 47:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictDaliUSUK, 612, [192, 207, 212, 303, 320, 360, 430, 481, 482, 483, 485, 486], Dict([[143, 582], [146, 583], [147, 584], [148, 585], [409, 586], [410, 587], [411, 588], [412, 589], [413, 590], [142, 591], [144, 592], [523, 593], [141, 594], [258, 595], [145, 596], [609, 597], [610, 598], [611, 599]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictDaliFR, 612, [485, 486, 487]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictDaliES, 612, [192, 207, 212, 303, 320, 360, 430, 481, 482, 483, 485, 486, 487], null, Dict([[297, 296]]));
                    break;
                case 484:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictMountGulugUSUK, 253, [141, 142, 143, 144, 145, 146, 184, 187, 203, 204, 205, 240, 241], Dict([[88, 235], [89, 236], [90, 237], [91, 238], [92, 239]]));
                    if (lang == "JP")
                        return SetupDictionary(ref _dictMountGulugJP, 253, [252]);
                    if (lang == "GR")
                        return SetupDictionary(ref _dictMountGulugGR, 253, [240, 252]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictMountGulugFR, 253, [141, 142, 143, 144, 145, 184, 187, 203, 204, 205, 240]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictMountGulugIT, 253, [141, 142, 143, 144, 145, 184, 187, 203, 204, 205, 240, 241, 252]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictMountGulugES, 253, [141, 142, 143, 144, 145, 184, 187, 203, 204, 205, 240, 241, 242, 252]);
                    break;
                case 485:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictLindblumOccupiedUSUK, 393, [258]);
                    if (lang == "JP")
                        return SetupDictionary(ref _dictLindblumOccupiedJP, 393, [257, 258]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictLindblumOccupiedFR, 393, [152, 257, 258, 259, 260, 261, 267, 268, 269, 270, 271, 272, 273]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictLindblumOccupiedIT, 393, [257, 258, 259]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictLindblumOccupiedES, 393, [152, 247, 250, 257, 258, 267, 268, 269, 270, 271, 272, 273, 297, 381]);
                    break;
                case 50:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictDaliUndergroundUSUK, 218, [79, 80, 81, 82, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 182]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictDaliUndergroundFR, 218, [123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 182]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictDaliUndergroundES, 218, [80, 81, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137]);
                    break;
                case 51:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictGizamalukeGrottoUSUK, 205, null, Dict([[178, 201], [179, 202], [180, 203], [181, 204]]));
                    break;
                case 52:
                    if (lang == "FR")
                        return SetupDictionary(ref _dictBranBalFR, 422, [156, 158]);
                    break;
                case 525:
                    if (lang == "JP" || lang == "GR" || lang == "FR" || lang == "IT")
                        return SetupDictionary(ref _dictLindblumCastleOccupiedJPGRFRIT, 238, [202, 203, 204]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictLindblumCastleOccupiedES, 238, [129, 140, 141, 142, 202, 203, 204, 210, 219, 220]);
                    break;
                case 53:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictObservatoryMountainUSUK, 340, [226, 324]);
                    break;
                case 595:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictLindblum2USUK, 502, [161, 162, 171, 172, 178, 179, 180, 181, 182, 187, 188, 189, 190, 191, 192, 193]);
                    if (lang == "GR")
                        return SetupDictionary(ref _dictLindblum2GR, 502, [161, 162, 172]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictLindblum2FR, 502, [159, 160, 161, 162, 175, 176, 177, 178, 179, 180, 181, 182, 187, 188, 189, 190, 191, 192, 193, 201, 202, 205], null, Dict([[197, 177]]));
                    if (lang == "ES")
                        return SetupDictionary(ref _dictLindblum2ES, 502, [130, 159, 160, 161, 162, 171, 172, 175, 176, 177, 178, 179, 180, 181, 182, 187, 188, 189, 190, 191, 192, 193, 195, 205], null, Dict([[197, 173]]));
                    break;
                case 63:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictCargoShipUSUK, 327, [76, 77, 78, 79, 80, 131, 134, 137, 152, 153, 154, 155, 165, 166, 167, 168, 170, 189, 190, 191, 203, 204, 205, 206, 207, 217, 218, 259], null, Dict([[110, 104]]));
                    if (lang == "GR")
                        return SetupDictionary(ref _dictCargoShipGR, 327, [76, 77, 78, 79, 80, 87, 88, 131, 134, 137, 152, 153, 154, 155, 156, 165, 166, 167, 168, 170, 189, 190, 191, 207, 217, 218, 259], null, Dict([[110, 102]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictCargoShipFR, 327, [76, 77, 78, 79, 80, 131, 134, 137, 167, 168, 170, 189, 190, 191, 217, 218, 259]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictCargoShipIT, 327, [76, 77, 78, 79, 80, 165, 166, 167, 168, 170, 189, 190, 191, 207, 259]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictCargoShipES, 327, [77, 78, 165, 166, 167, 168, 170, 189, 190, 191, 207, 259]);
                    break;
                case 71:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictQuMarshUSUK, 394, [365, 374, 390, 391], Dict([[212, 258]]));
                    if (lang == "JP" || lang == "GR" || lang == "FR")
                        return SetupDictionary(ref _dictQuMarshJPGRFR, 394, [257]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictQuMarshIT, 394, [365, 390, 391]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictQuMarshES, 394, [365, 374, 390, 391]);
                    break;
                case 738:
                    if (lang == "US" || lang == "UK" || lang == "GR")
                        return SetupDictionary(ref _dictIifaTreeRootsUSUKGR, 359, [112, 135, 136, 138, 195, 210, 211, 223, 224, 225, 226, 227, 228, 229, 230, 231, 245, 265, 271, 279, 282, 336, 346, 347, 348]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictIifaTreeRootsFR, 359, [112, 138, 195, 210, 211, 223, 224, 225, 226, 227, 228, 229, 230, 231, 245, 279, 282]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictIifaTreeRootsIT, 359, [112, 135, 136, 138, 195, 211, 223, 224, 225, 226, 227, 228, 229, 230, 231, 245, 265, 271, 279, 282, 336, 346, 347, 348]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictIifaTreeRootsES, 359, [112, 135, 136, 138, 195, 210, 211, 224, 225, 226, 227, 228, 229, 230, 231, 245, 265, 271, 279, 282, 336, 346, 347, 348]);
                    break;
                case 74:
                    if (lang == "US" || lang == "UK" || lang == "ES")
                        return SetupDictionary(ref _dictSouthGateUSUKES, 552, [309, 373, 426, 480, 509, 511, 512, 513, 514, 537, 538], null, Dict([[498, 493]]));
                    if (lang == "GR")
                        return SetupDictionary(ref _dictSouthGateGR, 552, [373, 480, 509, 511, 512, 513, 514], null, Dict([[498, 495]]));
                    if (lang == "FR")
                        return SetupDictionary(ref _dictSouthGateFR, 552, [509, 511, 512, 513, 514, 537, 538]);
                    break;
                case 77:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictBurmeciaUSUK, 339, [295]);
                    if (lang == "JP" || lang == "FR" || lang == "IT" || lang == "ES")
                        return SetupDictionary(ref _dictBurmeciaJPFRITES, 339, [295, 337, 338]);
                    if (lang == "GR")
                        return SetupDictionary(ref _dictBurmeciaGR, 339, [337, 338], Dict([[134, 335], [213, 336]]));
                    break;
                case 8:
                    if (lang == "ES")
                        return SetupDictionary(ref _dictIceCavernES, 211, [97]);
                    break;
                case 90:
                    if (lang == "JP" || lang == "GR" || lang == "FR" || lang == "IT" || lang == "ES")
                        return SetupDictionary(ref _dictAlexandria2JPGRFRITES, 557, [262]);
                    break;
                case 944:
                    if (lang == "US" || lang == "UK" || lang == "ES")
                        return SetupDictionary(ref _dictIifaTreeUSUKES, 310, [83, 84, 85, 90, 122, 123, 175, 204, 218, 220, 258, 259, 260, 278, 279, 280, 281, 282, 299, 306]);
                    if (lang == "GR")
                        return SetupDictionary(ref _dictIifaTreeGR, 310, [83, 84, 122, 123, 175, 204, 218, 220, 258, 259, 260, 278, 279, 280, 281, 282, 299, 306]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictIifaTreeFR, 310, [83, 84, 85, 90, 122, 123, 175, 204, 218, 220, 259, 260, 278, 279, 280, 281, 299, 306]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictIifaTreeIT, 310, [83, 84, 85, 90, 122, 123, 204, 218, 220, 258, 259, 260, 278, 279, 280, 281, 282, 299, 306]);
                    break;
                case 945:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictChocoboPlacesUSUK, 467, [183, 257, 280, 285, 289, 293, 297, 363, 432]);
                    if (lang == "JP")
                        return SetupDictionary(ref _dictChocoboPlacesJP, 467, [89, 183]);
                    if (lang == "GR")
                        return SetupDictionary(ref _dictChocoboPlacesGR, 467, [89, 432]);
                    if (lang == "FR")
                        return SetupDictionary(ref _dictChocoboPlacesFR, 467, [183]);
                    if (lang == "IT")
                        return SetupDictionary(ref _dictChocoboPlacesIT, 467, [89]);
                    if (lang == "ES")
                        return SetupDictionary(ref _dictChocoboPlacesES, 467, [183], Dict([[89, 48]]));
                    break;
                case 946:
                    if (lang == "US" || lang == "UK")
                        return SetupDictionary(ref _dictAlexandriaRuinUSUK, 277, null, Dict([[245, 270], [246, 271], [247, 272], [248, 273], [249, 274], [256, 275], [255, 276]]));
                    break;
                case 68:
                    if (lang == "US" || lang == "UK" || lang == "JP" || lang == "FR")
                        return SetupDictionary(ref _dictWorldMapUSUKJPFR, 92, [88, 89]);
                    break;
            }
            return null;
        }

        private static Dictionary<Int32, Int32> Dict(Int32[][] entries)
        {
            Dictionary<Int32, Int32> result = new Dictionary<Int32, Int32>();
            foreach (Int32[] entry in entries)
                result[entry[0]] = entry[1];
            return result;
        }

        private static TwoWayDictionary<Int32, Int32> SetupDictionary(ref TwoWayDictionary<Int32, Int32> dict, Int32 max, HashSet<Int32> missing = null, Dictionary<Int32, Int32> swap = null, Dictionary<Int32, Int32> duplicate = null)
        {
            if (dict != null)
                return dict;
            HashSet<Int32> universalSkip = null;
            HashSet<Int32> indexSkip = null;
            Int32 indexKey = 0;
            dict = new TwoWayDictionary<Int32, Int32>(true);
            if (swap != null || duplicate != null)
            {
                universalSkip = new HashSet<Int32>();
                if (swap != null)
                {
                    indexSkip = new HashSet<Int32>();
                    foreach (KeyValuePair<Int32, Int32> kvp in swap)
                    {
                        dict[kvp.Key] = kvp.Value;
                        universalSkip.Add(kvp.Key);
                        indexSkip.Add(kvp.Value);
                    }
                    while (indexSkip.Contains(indexKey))
                        indexKey++;
                }
                if (duplicate != null)
                    foreach (Int32 indexUniversal in duplicate.Keys)
                        universalSkip.Add(indexUniversal);
            }
            for (Int32 indexUniversal = 0; indexUniversal < max; indexUniversal++)
            {
                if (missing != null && missing.Contains(indexUniversal))
                {
                    dict[indexUniversal] = -1;
                    continue;
                }
                if (universalSkip != null && universalSkip.Contains(indexUniversal))
                    continue;
                if (indexUniversal != indexKey)
                    dict[indexUniversal] = indexKey;
                indexKey++;
                while (indexSkip != null && indexSkip.Contains(indexKey))
                    indexKey++;
            }
            if (duplicate != null)
                foreach (KeyValuePair<Int32, Int32> kvp in duplicate)
                    dict[kvp.Key] = kvp.Value;
            return dict;
        }

        private static TwoWayDictionary<Int32, Int32> _dictShrinesES = null;
        private static TwoWayDictionary<Int32, Int32> _dictPinnacleRocksUSUKES = null;
        private static TwoWayDictionary<Int32, Int32> _dictPinnacleRocksJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictHildaGarde3USUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictEndingUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictEndingGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictPrimaVistaUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumCastle1USUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumCastle1GR = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumCastle1FR = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumCastle1ES = null;
        private static TwoWayDictionary<Int32, Int32> _dictMistGatesUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictMistGatesJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictMistGatesGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictMistGatesFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictMistGatesIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictMistGatesES = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblum1USUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblum1FR = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblum1ES = null;
        private static TwoWayDictionary<Int32, Int32> _dictCleyraStormlessUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari1USUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari1JPFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari1GR = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari1IT = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari1ES = null;
        private static TwoWayDictionary<Int32, Int32> _dictGarganRooJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari2USUKES = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari2GR = null;
        private static TwoWayDictionary<Int32, Int32> _dictMadainSari2FRIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictFossilRooUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictFossilRooJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictFossilRooFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictFossilRooITES = null;
        private static TwoWayDictionary<Int32, Int32> _dictMognetCentralUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictEvilForestJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictEvilForestFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictEvilForestIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictPrimaVistaRuinJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictPrimaVistaRuinFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictCleyraUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictCleyraJPFRES = null;
        private static TwoWayDictionary<Int32, Int32> _dictCleyraGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictCleyraIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictDaliUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictDaliFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictDaliES = null;
        private static TwoWayDictionary<Int32, Int32> _dictMountGulugUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictMountGulugJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictMountGulugGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictMountGulugFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictMountGulugIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictMountGulugES = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumOccupiedUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumOccupiedJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumOccupiedFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumOccupiedIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumOccupiedES = null;
        private static TwoWayDictionary<Int32, Int32> _dictDaliUndergroundUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictDaliUndergroundFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictDaliUndergroundES = null;
        private static TwoWayDictionary<Int32, Int32> _dictGizamalukeGrottoUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictBranBalFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumCastleOccupiedJPGRFRIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblumCastleOccupiedES = null;
        private static TwoWayDictionary<Int32, Int32> _dictObservatoryMountainUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblum2USUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblum2GR = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblum2FR = null;
        private static TwoWayDictionary<Int32, Int32> _dictLindblum2ES = null;
        private static TwoWayDictionary<Int32, Int32> _dictCargoShipUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictCargoShipGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictCargoShipFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictCargoShipIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictCargoShipES = null;
        private static TwoWayDictionary<Int32, Int32> _dictQuMarshUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictQuMarshJPGRFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictQuMarshIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictQuMarshES = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeRootsUSUKGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeRootsFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeRootsIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeRootsES = null;
        private static TwoWayDictionary<Int32, Int32> _dictSouthGateUSUKES = null;
        private static TwoWayDictionary<Int32, Int32> _dictSouthGateGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictSouthGateFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictBurmeciaUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictBurmeciaJPFRITES = null;
        private static TwoWayDictionary<Int32, Int32> _dictBurmeciaGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictIceCavernES = null;
        private static TwoWayDictionary<Int32, Int32> _dictAlexandria2JPGRFRITES = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeUSUKES = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictIifaTreeIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictChocoboPlacesUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictChocoboPlacesJP = null;
        private static TwoWayDictionary<Int32, Int32> _dictChocoboPlacesGR = null;
        private static TwoWayDictionary<Int32, Int32> _dictChocoboPlacesFR = null;
        private static TwoWayDictionary<Int32, Int32> _dictChocoboPlacesIT = null;
        private static TwoWayDictionary<Int32, Int32> _dictChocoboPlacesES = null;
        private static TwoWayDictionary<Int32, Int32> _dictAlexandriaRuinUSUK = null;
        private static TwoWayDictionary<Int32, Int32> _dictWorldMapUSUKJPFR = null;

        // 1073 - Black Mage Village (2): all good

        // 121 - Shrines: Total 166
        //  ES miss 154

        // 124 - Oeilvert: all good

        // 134 - Pinnacle Rocks: Total 297
        //  JP miss 53 (SPLIT of 52)
        //  US/UK/ES miss 122 (DUPLICATE of 121)
        //  US/UK/ES miss 142 (DUPLICATE of 141)
        //  US/UK/ES miss 143 (DUMMIED)
        //  US/UK/ES miss 174 (DUMMIED)
        //  US/UK/ES miss 223-226 (DUMMIED)
        //  US/UK/ES miss 252 (DUMMIED)
        //  US/UK/ES miss 294-295 (DUMMIED)

        // 166 - Daguerreo: all good

        // 18 - Red Rose: all good

        // 186 - Hilda Garde III: Total 273
        //  US/UK swap:
        //   91 -> 266
        //   95 -> 267
        //   98 -> 268
        //   207 -> 269
        //   208 -> 270
        //   209 -> 271
        //   225 -> 272

        // 187 - Ending: Total 193
        //  UK/GR miss 138 (DUMMIED)
        //  UK/GR miss 147 (DUMMIED)
        //  GR miss 173 (DUMMIED)
        //  UK miss 174 (DUMMIED)

        // 189 - Invincible: all good

        // 2 - Prima Vista: Total 387
        //  US/UK miss 264-265 (DUMMIED)

        // 22 - Lindblum Castle (1): Total 688
        //  ES miss 175 (DUMMIED)
        //  FR/ES miss 201-202 (DUMMIED)
        //  US/UK swap 211 -> 686
        //  ES miss 353 (DUPLICATE of 346)
        //  GR/ES miss 389-390 (DUMMIED)
        //  ES miss 401 (DUMMIED)
        //  FR/ES miss 483-486 (DUMMIED)
        //  FR/ES miss 505 (DUMMIED)
        //  ES miss 584 (DUMMIED)
        //  ES miss 597 (DUMMIED)
        //  ES miss 625 (DUMMIED)
        //  US/UK swap 677 -> 687

        // 223 - Treno & Supersoft: all good

        // 23 - Mist Gates: Total 212
        //  JP miss 53 (SPLIT of 52)
        //  US/UK/GR/ES miss 104-123 (DUMMIED)
        //  IT miss 118-123 (DUMMIED)
        //  JP miss 125 (DUPLICATE of 124)
        //  IT swap 125 -> 205 (DUPLICATE of 124)
        //  US/UK/GR/FR/ES miss 153-154 (DUMMIED)
        //  US/UK/GR miss 160 (DUMMIED)
        //  GR/FR miss 174 (DUPLICATE of 173)
        //  US/UK/GR/FR miss 189 (DUMMIED)
        //  US/UK/GR/FR miss 198-201 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 205 (DUMMIED)
        //  GR/FR/ES miss 211 (DUMMIED)

        // 276 - Lindblum (1): Total 705
        //  ES miss 146 (DUMMIED)
        //  FR/ES miss 220-221 (DUMMIED)
        //  FR/ES miss 270-271 (DUMMIED)
        //  ES miss 343 (DUMMIED)
        //  ES miss 352 (DUMMIED)
        //  FR miss 518-523 (DUMMIED)
        //  ES miss 559 (DUMMIED)
        //  ES miss 609 (DUMMIED)
        //  US/UK swap 389 -> 685
        //  US/UK swap 371 -> 686
        //  US/UK swap 372 -> 687
        //  US/UK swap 373 -> 688
        //  US/UK swap 374 -> 689
        //  US/UK swap 375 -> 690
        //  US/UK swap 376 -> 691
        //  US/UK swap 377 -> 692
        //  US/UK swap 378 -> 693
        //  US/UK swap 379 -> 694
        //  US/UK swap 380 -> 695
        //  US/UK swap 296 -> 696
        //  US/UK swap 229 -> 697
        //  US/UK swap 230 -> 698
        //  US/UK swap 231 -> 699
        //  US/UK swap 233 -> 700
        //  US/UK swap 234 -> 701
        //  US/UK swap 232 -> 702
        //  US/UK swap 236 -> 703
        //  US/UK swap 240 -> 704

        // 289 - Alexandria Castle (2): all good

        // 290 - Cleyra (Stormless): Total 402
        //  US/UK miss 92-99 (DUMMIED)

        // 3 - Alexandria Castle (1): all good

        // 30 - Terra: all good

        // 31 - Mountain Path: all good

        // 32 - Conde Petie: all good

        // 33 - Alexandria (1): all good

        // 344 - Pandemonium: all good

        // 358 - Madain Sari (1): Total 920
        //  US/UK/ES miss 52 (DUMMIED)
        //  US/UK/ES miss 109 (DUMMIED)
        //  US/UK/GR/ES miss 171-172 (DUMMIED)
        //  JP/FR/ES miss 192 (DUMMIED)
        //  US/UK miss 289 (DUMMIED)
        //  US/UK/ES miss 357 (DUMMIED)
        //  US/UK/ES miss 382 (DUMMIED)
        //  US/UK/ES miss 450 (DUMMIED)
        //  US/UK/ES miss 502 (DUPLICATE of 501)
        //  ES miss 512 (DUMMIED)
        //  US/UK/ES miss 538 (DUMMIED)
        //  US/UK/ES miss 544 (DUMMIED)
        //  US/UK miss 549-550 (DUMMIED)
        //  JP/FR/ES miss 570 (DUMMIED)
        //  US/UK/JP/FR/ES miss 571 (DUMMIED)
        //  JP/FR/IT/ES miss 573 (DUMMIED)
        //  JP/FR/ES miss 574 (DUMMIED)
        //  JP/FR/IT/ES swapx 599 -> 605
        //  JP/FR/IT/ES swapx 600 -> 606
        //  JP/FR/IT/ES swapx 601 -> 599
        //  JP/FR/ES miss 602 (DUMMIED)
        //  JP/FR/IT/ES swapx 603 -> 600 (IT 601)
        //  JP/FR/IT/ES swapx 604 -> 601 (IT 602)
        //  JP/FR/IT/ES swapx 605 -> 602 (IT 603)
        //  JP/FR/IT/ES swapx 606 -> 603 (IT 604)
        //  JP/FR/IT miss 607 (DUMMIED)
        //  JP/FR/IT/ES swapx 608 -> 608
        //  US/UK/JP/FR/IT/ES miss 609 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 620 (DUMMIED)
        //  US/UK miss 623 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 627 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 636-639 (DUMMIED)
        //  US/UK/JP/FR/ES miss 640 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 645-649 (DUMMIED) (IT -2)
        //  US/UK/JP/FR/IT/ES miss 652-654 (DUMMIED) (IT -2)
        //  IT swapx 646 -> 652 (part of dummied)
        //  IT swapx 647 -> 654 (part of dummied)
        //  IT swapx 650 -> 646
        //  IT swapx 651 -> 647
        //  IT swapx 652 -> 655 (part of dummied)
        //  IT swapx 654 -> 656 (part of dummied)
        //  IT swapx 655 -> 650
        //  IT swapx 656 -> 651
        //  US/UK/GR/ES miss 663 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 671 (DUPLICATE of 673)
        //  US/UK/ES miss 687 (DUMMIED)
        //  US/UK/ES miss 764-765 (DUMMIED)
        //  US/UK/ES miss 790 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 864-869 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 879-884 (DUMMIED)
        //  US/UK/JP/GR/FR/ES miss 892-896 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 918-919 (DUMMIED)
        //  GR swap 699 -> 878 Chimomo
        //  GR swap 700 -> 879 Chimomo
        //  GR swap 701 -> 880 Chimomo
        //  GR swap 561 -> 881 {Zidane}
        //  GR swap 188 -> 882 {Vivi}
        //  GR swap 189 -> 883 {Vivi}
        //  GR swap 194 -> 884 {Zidane}
        //  GR swap 806 -> 885 {Eiko}
        //  GR swap 870 -> 886 Rotschopf
        //  GR swap 871 -> 887 Rotschopf
        //  GR swap 872 -> 888 Rotschopf
        //  GR swap 873 -> 889 {Zidane}
        //  GR swap 874 -> 890 {Zidane}
        //  GR swap 875 -> 891 Rotschopf
        //  GR swap 876 -> 892 {Zidane}
        //  GR swap 885 -> 893 Rotschopf
        //  GR swap 886 -> 894 {Zidane}
        //  GR swap 887 -> 895 {Zidane}
        //  GR swap 888 -> 896 {Zidane}
        //  GR swap 889 -> 897 Rotschopf
        //  GR swap 890 -> 898 {Zidane}
        //  GR swap 891 -> 899 Rotschopf
        //  GR swap 198 -> 900 {Zidane}
        //  GR swap 199 -> 901 {Zidane}
        //  GR swap 562 -> 902 Moko
        //  GR swap 563 -> 903 Chimomo
        //  GR swap 918 -> 904 ??? (DUMMIED)
        //  GR swap 641 -> 905 Lani
        //  GR swap 642 -> 906 Lani
        //  GR swap 650 -> 907 Rotschopf
        //  GR swap 655 -> 908 Rotschopf
        //  GR swap 656 -> 909 Lani
        //  GR swap 919 -> 910 Rotschopf (DUMMIED)
        //  GR swap 657 -> 911 Rotschopf

        // 359 - Gargan Roo: Total 205
        //  JP miss 52 (SPLIT of 51)

        // 360 - Madain Sari (2): Total 127
        //  US/UK/FR/IT/ES miss 76 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 77-78 (DUMMIED)
        //  US/UK/ES miss 98 (DUMMIED)

        // 361 - Fossil Roo: Total 234
        //  JP miss 67 (SPLIT of 66)
        //  FR/IT/ES miss 178-180 (DUMMIED)
        //  IT/ES miss 212 (DUMMIED)
        //  US/UK/FR/IT/ES miss 223 (DUMMIED)

        // 37 - Black Mage Village (1): all good

        // 38 - Mognet Central: Total 96
        //  US/UK swap 84 -> 94
        //  US/UK miss 85 (DUMMIED)

        // 4 - Evil Forest: Total 256
        //  JP miss 52 (SPLIT of 51)
        //  IT miss 107 (DUMMIED)
        //  IT miss 131 (DUMMIED)
        //  IT miss 133 (DUMMIED)
        //  FR/IT miss 155-159 (DUMMIED)
        //  IT miss 183 (DUMMIED)
        //  IT miss 185 (DUMMIED)

        // 40 - Prima Vista (Ruin): Total 412
        //  JP miss 52 (SPLIT of 51)
        //  FR miss 131 (DUMMIED)
        //  FR miss 245 (DUMMIED)
        //  FR miss 250 (DUMMIED)
        //  FR miss 257 (DUMMIED)
        //  FR miss 303-309 (DUMMIED)
        //  FR miss 320-321 (DUMMIED)
        //  FR miss 344-347 (DUMMIED)
        //  FR miss 355 (DUMMIED)
        //  FR miss 380-387 (DUMMIED)

        // 42 - Quan's Dwelling: all good

        // 44 - Cleyra: Total 302
        //  US/UK miss 92-99 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 269 (DUPLICATE of 268)
        //  US/UK/GR/IT swapx 271 -> 273
        //  US/UK/JP/FR/IT/ES miss 272 (DUPLICATE of 271)
        //  US/UK/GR/IT swapx 273 -> 271

        // 47 - Dali: Total 612
        //  US/UK/ES miss 192 (DUMMIED)
        //  US/UK/ES miss 207 (DUMMIED)
        //  US/UK/ES miss 212 (DUMMIED)
        //  ES miss 297 (DUPLICATE of 299)
        //  US/UK/ES miss 303 (DUMMIED)
        //  US/UK/ES miss 320 (DUMMIED)
        //  US/UK/ES miss 360 (DUMMIED)
        //  US/UK/ES miss 430 (DUMMIED)
        //  US/UK/ES miss 481-483 (DUMMIED)
        //  US/UK/FR/ES miss 485-486 (DUMMIED)
        //  FR/ES miss 487 (DUMMIED)
        //  US/UK swap 143 -> 582 Innkeeper Hal
        //  US/UK swap 146 -> 583 ...?
        //  US/UK swap 147 -> 584 Innkeeper Hal
        //  US/UK swap 148 -> 585 Innkeeper Hal
        //  US/UK swap 409 -> 586 Useful Information
        //  US/UK swap 410 -> 587 Useful Information
        //  US/UK swap 411 -> 588 Useful Information
        //  US/UK swap 412 -> 589 Useful Information
        //  US/UK swap 413 -> 590 Please...
        //  US/UK swap 142 -> 591 ...Hmm?
        //  US/UK swap 144 -> 592 Innkeeper Hal
        //  US/UK swap 523 -> 593 Shopkeeper Eve
        //  US/UK swap 141 -> 594 {Zidane}
        //  US/UK swap 258 -> 595 Dutiful Daughter Slai
        //  US/UK swap 145 -> 596 (...Stay calm!)
        //  US/UK swap 609 -> 597 Positive Thinking
        //  US/UK swap 610 -> 598 {Zidane}
        //  US/UK swap 611 -> 599 sleep

        // 484 - Mount Gulug: Total 253
        //  US/UK/FR/IT/ES miss 141-145 (DUMMIED)
        //  US/UK miss 146 (DUMMIED)
        //  US/UK/FR/IT/ES miss 184 (DUMMIED)
        //  US/UK/FR/IT/ES miss 187 (DUMMIED)
        //  US/UK/FR/IT/ES miss 203-205 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 240 (DUMMIED)
        //  US/UK/IT/ES miss 241 (DUMMIED)
        //  ES miss 242 (DUMMIED)
        //  JP/GR/IT/ES miss 252 (DUMMIED)
        //  US/UK swap 88 -> 235
        //  US/UK swap 89 -> 236
        //  US/UK swap 90 -> 237
        //  US/UK swap 91 -> 238
        //  US/UK swap 92 -> 239

        // 485 - Lindblum (Occupied): Total 393
        //  FR/ES miss 152 (DUMMIED)
        //  ES miss 247 (DUMMIED)
        //  ES miss 250 (DUMMIED)
        //  JP/FR/IT/ES miss 257 (DUMMIED)
        //  US/UK/JP/FR/IT/ES miss 258 (DUMMIED)
        //  FR/IT miss 259 (DUMMIED)
        //  FR miss 260-261 (DUMMIED)
        //  FR/ES miss 267-273 (DUMMIED)
        //  ES miss 297 (DUMMIED)
        //  ES miss 381 (DUMMIED)

        // 50 - Dali Underground: Total 218
        //  US/UK miss 79 (DUMMIED)
        //  US/UK/ES miss 80-81 (DUMMIED)
        //  US/UK miss 82 (DUMMIED)
        //  US/UK/FR/ES miss 123-137 (DUMMIED)
        //  US/UK/FR miss 182 (DUMMIED)

        // 51 - Gizamaluke Grotto: Total 205
        //  US/UK swap 178 -> 201
        //  US/UK swap 179 -> 202
        //  US/UK swap 180 -> 203
        //  US/UK swap 181 -> 204

        // 52 - Bran Bal: Total 422
        //  FR miss 156 (DUMMIED)
        //  FR miss 158 (DUMMIED)

        // 525 - Lindblum Castle (Occupied): Total 238
        //  ES miss 129 (DUMMIED)
        //  ES miss 140-142 (DUMMIED)
        //  JP/GR/FR/IT/ES miss 202-204 (DUMMIED)
        //  ES miss 210 (DUMMIED)
        //  ES miss 219-220 (DUMMIED)

        // 53 - Observatory Mountain: Total 340
        //  US/UK miss 226 (DUMMIED)
        //  US/UK miss 324 (DUMMIED)

        // 595 - Lindblum (2): Total 502
        //  ES miss 130 (DUMMIED)
        //  FR/ES miss 159-160 (DUMMIED)
        //  US/UK/GR/FR/ES miss 161-162 (DUMMIED)
        //  US/UK/ES miss 171 (DUMMIED)
        //  US/UK/GR/ES miss 172 (DUMMIED)
        //  FR/ES miss 175-177 (DUMMIED)
        //  US/UK/FR/ES miss 178-182 (DUMMIED)
        //  US/UK/FR/ES miss 187-193 (DUMMIED)
        //  ES miss 195 (DUMMIED)
        //  FR/ES miss 197 (DUPLICATE of 196)
        //  FR miss 201-202 (DUMMIED)
        //  FR/ES miss 205 (DUMMIED)

        // 63 - Cargo Ship: Total 327
        //  US/UK/GR/FR/IT miss 76 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 77-78 (DUMMIED)
        //  US/UK/GR/FR/IT miss 79-80 (DUMMIED)
        //  GR miss 87-88 (DUMMIED)
        //  US/UK/GR miss 110 (DUPLICATE of 109)
        //  US/UK/GR/FR miss 131 (DUMMIED)
        //  US/UK/GR/FR miss 134 (DUMMIED)
        //  US/UK/GR/FR miss 137 (DUMMIED)
        //  US/UK/GR miss 152-155 (DUMMIED)
        //  GR miss 156 (DUMMIED)
        //  US/UK/GR/IT/ES miss 165-166 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 167-168 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 170 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 189-191 (DUMMIED)
        //  US/UK miss 203-206 (DUMMIED)
        //  US/UK/GR/IT/ES miss 207 (DUMMIED)
        //  US/UK/GR/FR miss 217-218 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 259 (DUMMIED)

        // 694 - Memoria: all good

        // 7 - Cleyra Trunk: all good

        // 70 - Treno (1): all good

        // 71 - Qu's Marsh: Total 394
        //  JP/GR/FR/IT/ES miss 257 (DUMMIED)
        //  US/UK swapx 212 -> 258
        //  US/UK/IT/ES miss 365 (DUMMIED)
        //  US/UK/ES miss 374 (DUMMIED)
        //  US/UK/IT/ES miss 390-391 (DUMMIED)

        // 738 - Iifa Tree Roots: Total 359
        //  US/UK/GR/FR/IT/ES miss 112 (DUMMIED)
        //  US/UK/GR/IT/ES miss 135-136 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 138 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 195 (DUMMIED)
        //  US/UK/GR/FR/ES miss 210 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 211 (DUMMIED)
        //  US/UK/GR/FR/IT miss 223 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 224-231 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 245 (DUMMIED)
        //  US/UK/GR/IT/ES miss 265 (DUMMIED)
        //  US/UK/GR/IT/ES miss 271 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 279 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 282 (DUMMIED)
        //  US/UK/GR/IT/ES miss 336 (DUMMIED)
        //  US/UK/GR/IT/ES miss 346-348 (DUMMIED)

        // 739 - Ipsen Castle: all good

        // 74 - South Gate: Total 552
        //  US/UK/ES miss 309 (DUMMIED)
        //  US/UK/GR/ES miss 373 (DUMMIED)
        //  US/UK/ES miss 426 (DUMMIED)
        //  US/UK/GR/ES miss 480 (DUMMIED)
        //  US/UK/GR/ES miss 498 (DUPLICATE of 497)
        //  US/UK/GR/FR/ES miss 509 (DUMMIED)
        //  US/UK/GR/FR/ES miss 511-514 (DUMMIED)
        //  US/UK/FR/ES miss 537-538 (DUMMIED)

        // 740 - Desert Palace: all good

        // 741 - Treno (2): all good

        // 754 - Alexandria Castle (Ruin): all good

        // 77 - Burmecia: Total 339
        //  US/UK/JP/FR/IT/ES miss 295 (DUMMIED)
        //  JP/GR/FR/IT/ES miss 337-338 (DUMMIED)
        //  GR swap 134 -> 335
        //  GR swap 213 -> 336

        // 8 - Ice Cavern: Total 211
        //  ES miss 97 (DUMMIED)

        // 88 - Alexandria Castle (3): all good

        // 89 - Alexander: all good

        // 90 - Alexandria (2): Total 557
        //  JP/GR/FR/IT/ES miss 262 (DUMMIED)

        // 908 - Esto Gaza: Total 191 but all good

        // 91 - Alexandria (Night): all good

        // 943 - Lindblum Castle (2): all good

        // 944 - Iifa Tree: Total 310
        //  US/UK/GR/FR/IT/ES miss 83-84 (DUMMIED)
        //  US/UK/FR/IT/ES miss 85 (DUMMIED)
        //  US/UK/FR/IT/ES miss 90 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 122-123 (DUMMIED)
        //  US/UK/GR/FR/ES miss 175 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 204 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 218 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 220 (DUMMIED)
        //  US/UK/GR/IT/ES miss 258 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 259-260 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 278-281 (DUMMIED)
        //  US/UK/GR/IT/ES miss 282 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 299 (DUMMIED)
        //  US/UK/GR/FR/IT/ES miss 306 (DUMMIED)

        // 945 - Chocobo Places: Total 467
        //  JP/GR/IT miss 89 (DUMMIED)
        //  ES swapx 89 -> 48
        //  US/UK/JP/FR/ES miss 183 (DUMMIED)
        //  US/UK miss 257 (DUMMIED)
        //  US/UK miss 280 (DUMMIED)
        //  US/UK miss 285 (DUMMIED)
        //  US/UK miss 289 (DUMMIED)
        //  US/UK miss 293 (DUMMIED)
        //  US/UK miss 297 (DUMMIED)
        //  US/UK miss 363 (DUMMIED)
        //  US/UK/GR miss 432 (DUMMIED)

        // 946 - Alexandria (Ruin): Total 277
        //  US/UK swap 245 -> 270
        //  US/UK swap 246 -> 271
        //  US/UK swap 247 -> 272
        //  US/UK swap 248 -> 273
        //  US/UK swap 249 -> 274
        //  US/UK swap 256 -> 275
        //  US/UK swap 255 -> 276

        // 68 - World Maps: Total 92
        //  US/UK/JP/FR miss 88-89 (DUMMIED)
    }
}
