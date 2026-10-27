using System;
using System.Collections.Generic;
using UnityEngine;

public static class fldmcf
{
    public const SByte LIGHT_TYPE_FLOOR = 0;
    public const SByte LIGHT_TYPE_DEFAULT = 1;
    public const SByte LIGHT_TYPE_LADDER = 2;

    public static Int32 SetCharColor(Int32 uid, Int32 ClrR, Int32 ClrG, Int32 ClrB)
    {
        return fldmcf.SetCharColor(FF9StateSystem.Common.FF9.charArray[uid], ClrR, ClrG, ClrB);
    }

    public static void SetMCFDefaultLight(SByte type, Color32 col, SByte shadowIntensity, SByte shadowScale)
    {
        EnsureLightConfig();
        DMSMapConf lightConfig = FF9StateSystem.Field.FF9Field.loc.map.mcfPtr.Value;
        Int32 lightIndex = GetOrCreateLight(lightConfig, type, -1);
        DMSMapLight light = lightConfig.DMSMapLight[lightIndex];
        light.clr = [(SByte)col.r, (SByte)col.g, (SByte)col.b];
        light.shadowI = shadowIntensity;
        light.shadowR = shadowScale;
    }

    public static void SetMCFFloorLight(SByte floorNdx, Color32 col, SByte shadowIntensity, SByte shadowScale)
    {
        EnsureLightConfig();
        DMSMapConf lightConfig = FF9StateSystem.Field.FF9Field.loc.map.mcfPtr.Value;
        Int32 lightIndex = GetOrCreateLight(lightConfig, LIGHT_TYPE_FLOOR, floorNdx);
        DMSMapLight light = lightConfig.DMSMapLight[lightIndex];
        light.clr = [(SByte)col.r, (SByte)col.g, (SByte)col.b];
        light.shadowI = shadowIntensity;
        light.shadowR = shadowScale;
    }

    public static void SetMCFModelLight(Int32 model, Color32 col, SByte shadowIntensity, SByte shadowScale)
    {
        EnsureLightConfig();
        DMSMapConf lightConfig = FF9StateSystem.Field.FF9Field.loc.map.mcfPtr.Value;
        Int32 lightIndex = GetOrCreateCharLight(lightConfig, model);
        DMSMapChar light = lightConfig.DMSMapChar[lightIndex];
        light.clr = [(SByte)col.r, (SByte)col.g, (SByte)col.b];
        light.shadowI = shadowIntensity;
        light.shadowR = shadowScale;
    }

    public static void FieldMCFService()
    {
        if (FF9StateSystem.Field.FF9Field.loc.map.mcfPtr == null)
            return;
        DMSMapConf lightConfig = FF9StateSystem.Field.FF9Field.loc.map.mcfPtr.Value;
        DMSMapChar[] allCharLights = lightConfig.DMSMapChar;
        if (allCharLights == null)
            return;
        DMSMapChar? defaultCharLight = fldmcf.GetCharLightByModelID(lightConfig, -1);
        fldmcf.GetLightDefaults(lightConfig, out DMSMapLight? defaultLight, out DMSMapLight? ladderLight);
        foreach (KeyValuePair<Int32, FF9Char> ch in FF9StateSystem.Common.FF9.charArray)
        {
            Int32 charUID = ch.Key;
            FF9Char character = ch.Value;
            Actor charActor = PersistenSingleton<EventEngine>.Instance.getActiveActorByUID(charUID);
            if (character != null && charActor != null)
            {
                FF9FieldCharState charState = FF9StateSystem.Field.FF9Field.loc.map.charStateArray[charUID];
                DMSMapChar? charLight = fldmcf.GetCharLightByModelID(lightConfig, (Int32)charActor.model);
                DMSMapLight? floorLight = fldmcf.GetLightByCharFloor(lightConfig, charUID, out Int16 charFloor);
                UInt32 isUsingLadder = character.attr & 4u;
                Boolean applyLight = (charState.floor & 0xFF) != (charFloor & 0xFF) || (isUsingLadder ^ (charState.attr & 4u)) != 0 || FF9Char.ff9charptr_attr_test(character, 0x100) == 0;
                if (applyLight)
                {
                    if (FF9Char.ff9charptr_attr_test(character, 0x400) == 0)
                    {
                        SByte[] charColor = charLight != null ? charLight.Value.clr : defaultCharLight.Value.clr;
                        SByte[] lightColor = [0, 0, 0];
                        if (isUsingLadder != 0 && ladderLight != null)
                            lightColor = ladderLight.Value.clr;
                        else if (floorLight != null)
                            lightColor = floorLight.Value.clr;
                        else if (defaultLight != null)
                            lightColor = defaultLight.Value.clr;
                        charState.clr[0] = new FF9FieldCharColor
                        {
                            r = (Byte)(charColor[0] + lightColor[0] << 3),
                            g = (Byte)(charColor[1] + lightColor[1] << 3),
                            b = (Byte)(charColor[2] + lightColor[2] << 3),
                            active = true
                        };
                        charState.clr[1] = charState.clr[0];
                    }
                    Int32 shadowIntensity;
                    Int32 shadowScale;
                    if (charLight != null)
                    {
                        shadowIntensity = charLight.Value.shadowI;
                        shadowScale = charLight.Value.shadowR;
                    }
                    else
                    {
                        shadowIntensity = defaultCharLight.Value.shadowI;
                        shadowScale = defaultCharLight.Value.shadowR;
                    }
                    if (floorLight != null)
                    {
                        shadowIntensity += floorLight.Value.shadowI;
                        shadowScale += floorLight.Value.shadowR;
                    }
                    else if (defaultLight != null)
                    {
                        shadowIntensity += defaultLight.Value.shadowI;
                        shadowScale += defaultLight.Value.shadowR;
                    }
                    shadowIntensity <<= 3;
                    ff9shadow.FF9ShadowSetAmpField(charUID, shadowIntensity);
                    ff9shadow.FF9ShadowSetScaleField(charUID, shadowScale, shadowScale);
                    charState.floor = (SByte)charFloor;
                    charState.attr = (charState.attr & 0xFFFFFFFF) | isUsingLadder;
                    FF9Char.ff9charptr_attr_set(character, 0x100);
                }
            }
        }
    }

    private static void GetLightDefaults(DMSMapConf lightConfig, out DMSMapLight? defaultLight, out DMSMapLight? ladderLight)
    {
        defaultLight = null;
        ladderLight = null;
        if (lightConfig.lightUse == 0)
            return;
        foreach (DMSMapLight light in lightConfig.DMSMapLight)
        {
            if (light.type == LIGHT_TYPE_DEFAULT)
                defaultLight = light;
            else if (light.type == LIGHT_TYPE_LADDER)
                ladderLight = light;
        }
    }

    private static DMSMapChar? GetCharLightByModelID(DMSMapConf lightConfig, Int32 model)
    {
        model &= 0xFFFF;
        if (lightConfig.charUse == 0)
            return null;
        foreach (DMSMapChar light in lightConfig.DMSMapChar)
            if (light.geoNo == model)
                return light;
        return null;
    }

    private static DMSMapLight? GetLightByCharFloor(DMSMapConf lightConfig, Int32 uid, out Int16 charFloor)
    {
        charFloor = 0;
        Int16 triIndex = 0;
        BGI.BGI_charGetInfo(uid, ref triIndex, ref charFloor);
        if (lightConfig.lightUse == 0)
            return null;
        if ((charFloor & 0xFF) == 0xFF)
            return null;
        foreach (DMSMapLight light in lightConfig.DMSMapLight)
            if (light.type == LIGHT_TYPE_FLOOR)
                for (Int32 i = 0; i < 4; i++)
                    if ((light.floor[i] & 0xFF) == charFloor)
                        return light;
        return null;
    }

    private static void EnsureLightConfig()
    {
        if (FF9StateSystem.Field.FF9Field.loc.map.mcfPtr == null)
        {
            FF9StateSystem.Field.FF9Field.loc.map.mcfPtr = new DMSMapConf()
            {
                attr = 0,
                version = 3,
                bgNo = 0,
                lightCount = 0,
                lightUse = 0,
                charCount = 0,
                charUse = 0,
                evtCount = 0,
                evtUse = 0,
                DMSMapLight = [],
                DMSMapChar = []
            };
        }
    }

    private static Int32 GetOrCreateLight(DMSMapConf lightConfig, SByte type, SByte floorIndex)
    {
        if (lightConfig.DMSMapLight == null)
            lightConfig.DMSMapLight = [];
        Int32 lightIndex = -1;
        for (Int32 i = 0; i < lightConfig.lightCount; i++)
        {
            DMSMapLight light = lightConfig.DMSMapLight[i];
            if (light.type == type)
                for (Int32 j = 0; j < 4; j++)
                    if ((light.floor[j] & 0xFF) == floorIndex)
                        lightIndex = i;
        }
        if (lightIndex < 0)
        {
            lightIndex = lightConfig.lightCount;
            lightConfig.lightCount++;
            Array.Resize(ref lightConfig.DMSMapLight, lightConfig.lightCount);
            lightConfig.DMSMapLight[lightIndex].type = type;
            lightConfig.DMSMapLight[lightIndex].floor = [floorIndex, -1, -1, -1];
            lightConfig.DMSMapLight[lightIndex].clr = [0, 0, 0];
        }
        return lightIndex;
    }

    private static Int32 GetOrCreateCharLight(DMSMapConf lightConfig, Int32 model)
    {
        if (lightConfig.DMSMapChar == null)
            lightConfig.DMSMapChar = [];
        Int32 lightIndex = -1;
        for (Int32 i = 0; i < lightConfig.charCount; i++)
        {
            DMSMapChar light = lightConfig.DMSMapChar[i];
            if (light.geoNo == model)
                lightIndex = i;
        }
        if (lightIndex < 0)
        {
            lightIndex = lightConfig.charCount;
            lightConfig.charCount++;
            Array.Resize(ref lightConfig.DMSMapChar, lightConfig.charCount);
            lightConfig.DMSMapChar[lightIndex].geoNo = (UInt16)model;
            lightConfig.DMSMapChar[lightIndex].clr = [0, 0, 0];
        }
        return lightIndex;
    }

    private static Int32 SetCharColor(FF9Char CharPtr, Int32 ClrR, Int32 ClrG, Int32 ClrB)
    {
        if (FF9Char.ff9charptr_attr_test(CharPtr, 0x800) == 0)
        {
            FF9FieldCharState charState = FF9StateSystem.Field.FF9Field.loc.map.charStateArray[CharPtr.evt.uid];
            charState.clr[0] = new FF9FieldCharColor
            {
                r = (Byte)ClrR,
                g = (Byte)ClrG,
                b = (Byte)ClrB,
                active = true
            };
            charState.clr[1] = charState.clr[0];
            FF9Char.ff9charptr_attr_set(CharPtr, 0x400);
            FF9Char.ff9charptr_attr_set(CharPtr, 0x800);
            return 1;
        }
        FF9Char.ff9charptr_attr_clear(CharPtr, 0x800);
        return 0;
    }
}
