using Assets.Sources.Scripts.Common;
using System;
using System.IO;
using UnityEngine;
using Object = System.Object;

public class FieldMapEditor : MonoBehaviour
{
    public static String GetFieldMapModName(String mapName)
    {
        return mapName + "_MOD";
    }

    public void Init(FieldMap fm)
    {
        this.fieldMap = fm;
        this._showUI = false;
    }

    private void OnGUI()
    {
        if (!this._showUI)
        {
            return;
        }
        Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
        DebugGuiSkin.ApplySkin();
        GUILayout.BeginArea(fullscreenRect);
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save", new GUILayoutOption[0]))
        {
            this.SaveFieldMap();
        }
        if (FieldMapEditor.useOriginalVersion)
        {
            if (GUILayout.Button("[Use Original          ]", new GUILayoutOption[0]))
            {
                FieldMapEditor.useOriginalVersion = !FieldMapEditor.useOriginalVersion;
            }
        }
        else if (GUILayout.Button("[Use Modified          ]", new GUILayoutOption[0]))
        {
            FieldMapEditor.useOriginalVersion = !FieldMapEditor.useOriginalVersion;
        }
        GUILayout.Button("Restore", new GUILayoutOption[0]);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void SaveFieldMap()
    {
        BGSCENE_DEF scene = this.fieldMap.scene;
        Byte[] ebgBin = scene.ebgBin;
        String mapName = this.fieldMap.mapName;
        String fieldMapModName = FieldMapEditor.GetFieldMapModName(mapName);
        String mapResourcePath = FieldMap.GetMapResourcePath(mapName);
        using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(ebgBin)))
        {
            for (Int32 i = 0; i < (Int32)scene.overlayCount; i++)
            {
                BGOVERLAY_DEF bgoverlay_DEF = scene.overlayList[i];
                UInt32 oriData = bgoverlay_DEF.oriData;
                UInt16 num = (UInt16)bgoverlay_DEF.transform.localPosition.z;
                UInt16 num2 = (UInt16)(oriData >> 8 & 4095u);
                UInt32 num3 = oriData;
                UInt32 num4 = 1048320u;
                num3 &= ~num4;
                num3 |= (UInt32)((Int64)((Int64)num << 8) & (Int64)((UInt64)num4));
                global::Debug.Log(String.Concat(new Object[]
                {
                    i,
                    " : data  :",
                    oriData,
                    ", curZ : ",
                    num,
                    ", oriZ : ",
                    num2,
                    ", res : ",
                    num3
                }));
                binaryWriter.BaseStream.Seek(bgoverlay_DEF.startOffset, SeekOrigin.Begin);
                binaryWriter.Write(num3);
            }
        }
        String path = "Assets/Resources/" + mapResourcePath + fieldMapModName + ".bgs.bytes";
        File.WriteAllBytes(path, ebgBin);
    }

    public static Boolean useOriginalVersion = true;

    public FieldMap fieldMap;

    private Boolean _showUI;
}
