using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = System.Object;

public class GenQuad : MonoBehaviour
{
    private void Start()
    {
        this.fieldmap = base.GetComponent<FieldMap>();
        this.CreateMapList();
        this.CreateQuadList();
        this.currentQuad = 0;
    }

    private void Update()
    {
        if (this.isChange && this.fieldmap.mapName != this.fieldMapData[this.currentQuad].name)
        {
            this.fieldmap.ChangeFieldMap(this.fieldMapData[this.currentQuad].name);
            this.isChange = false;
        }
        this.SetQuad(this.fieldmap.scene, this.fieldmap.camIdx, this.fieldmap.offset);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, (Single)(Screen.height / 2), 100f, 100f), "<"))
        {
            this.currentQuad--;
            this.isChange = true;
            this.PrintCurrentQuad();
        }
        if (GUI.Button(new Rect((Single)(Screen.width - 100), (Single)(Screen.height / 2), 100f, 100f), ">"))
        {
            this.currentQuad++;
            this.isChange = true;
            this.PrintCurrentQuad();
        }
    }

    private void CreateMapList()
    {
        String path = "mapList";
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        String text = textAsset.text;
        String[] array = text.Split(new Char[]
        {
            "\n"[0]
        });
        Int32 num = 0;
        String[] array2 = array;
        for (Int32 i = 0; i < (Int32)array2.Length; i++)
        {
            String text2 = array2[i];
            if (text2 == String.Empty)
            {
                break;
            }
            this.mapList.Add(text2.Trim());
            num++;
        }
    }

    private void CreateQuadList()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("quadList2");
        this.fieldMapData = new List<GenQuad.FieldMapData>();
        String text = "noMap";
        Int32 num = 0;
        using (StringReader stringReader = new StringReader(textAsset.text))
        {
            String text2 = String.Empty;
            String text3;
            while ((text3 = stringReader.ReadLine()) != null)
            {
                num++;
                if (!(text3 == String.Empty))
                {
                    if (text3.IndexOf("setq") == -1)
                    {
                        text = "noMap";
                        if (text3.Contains("\\"))
                        {
                            String[] array = text3.Split(new Char[]
                            {
                                '\\'
                            });
                            for (Int32 i = 0; i < (Int32)array.Length; i++)
                            {
                                Int32 length;
                                if ((length = array[i].IndexOf(".ev")) != -1)
                                {
                                    text3 = array[i].Substring(0, length);
                                    foreach (String text4 in this.mapList)
                                    {
                                        if (text4.Contains(text3.ToUpper()))
                                        {
                                            if (text3.ToUpper() == "MS_MRR_1")
                                            {
                                                global::Debug.Log("beer: " + this.fieldMapData.Count);
                                            }
                                            text = text4;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (text == "noMap" && !text3.Contains(".bak") && !text3.Contains(".BAK"))
                            {
                                String text5 = text2;
                                text2 = String.Concat(new Object[]
                                {
                                    text5,
                                    num,
                                    ", ",
                                    text3,
                                    "\n"
                                });
                            }
                        }
                        else
                        {
                            String[] array2 = text3.Split(new Char[]
                            {
                                '/'
                            });
                            for (Int32 j = 0; j < (Int32)array2.Length; j++)
                            {
                                Int32 length2;
                                if ((length2 = array2[j].IndexOf(".h")) != -1)
                                {
                                    text3 = array2[j].Substring(0, length2);
                                    foreach (String text6 in this.mapList)
                                    {
                                        if (text6.Contains(text3.ToUpper()))
                                        {
                                            text = text6;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!(text == "noMap") || text3 != "<noMap>")
                            {
                            }
                        }
                    }
                    else if (!(text == "noMap"))
                    {
                        text3 = text3.Replace("setq", String.Empty);
                        Int32 length3 = text3.IndexOf(";");
                        text3 = text3.Substring(0, length3);
                        String[] array3 = text3.Split(new Char[]
                        {
                            ','
                        });
                        Int32[] array4 = new Int32[(Int32)array3.Length];
                        for (Int32 k = 0; k < (Int32)array4.Length; k++)
                        {
                            Int32 num2;
                            Boolean flag = Int32.TryParse(array3[k], out num2);
                            if (flag)
                            {
                                array4[k] = num2;
                            }
                            else if (array3[k].Contains("+"))
                            {
                                String[] array5 = array3[k].Split(new Char[]
                                {
                                    '+'
                                });
                                array4[k] = Int32.Parse(array5[0]) + Int32.Parse(array5[1]);
                            }
                            else if (array3[k].Contains("-"))
                            {
                                String[] array6 = array3[k].Split(new Char[]
                                {
                                    '-'
                                });
                                if ((Int32)array6.Length > 2)
                                {
                                    array4[k] = -Int32.Parse(array6[1]) - Int32.Parse(array6[2]);
                                }
                                else
                                {
                                    array4[k] = Int32.Parse(array6[0]) - Int32.Parse(array6[1]);
                                }
                            }
                        }
                        GenQuad.FieldMapData item;
                        item.name = text;
                        item.quad = array4;
                        this.fieldMapData.Add(item);
                    }
                }
            }
            global::Debug.Log("lineMissing: " + text2);
        }
        global::Debug.Log("quadCount: " + this.fieldMapData.Count);
    }

    private void PrintQuadList()
    {
        Int32 num = 0;
        foreach (GenQuad.FieldMapData fieldMapData in this.fieldMapData)
        {
            String text = String.Empty;
            for (Int32 i = 0; i < (Int32)fieldMapData.quad.Length; i++)
            {
                text = text + ", " + fieldMapData.quad[i].ToString();
            }
            global::Debug.Log(String.Concat(new Object[]
            {
                num,
                "| name: ",
                fieldMapData.name,
                ", quad",
                text
            }));
            num++;
        }
    }

    private void SetQuad(BGSCENE_DEF scene, Int32 camIdx, Vector2 offset)
    {
        Color[] array = new Color[]
        {
            Color.cyan,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.red,
            Color.white,
            Color.black,
            Color.grey
        };
        BGCAM_DEF bgcam_DEF = scene.cameraList[camIdx];
        Int32[] array2 = new Int32[]
        {
            2222,
            -5555,
            -2222,
            -5555,
            -2222,
            -4080,
            2222,
            -4080
        };
        Int32[] quad = this.fieldMapData[this.currentQuad].quad;
        String str = String.Empty;
        for (Int32 i = 0; i < (Int32)quad.Length; i++)
        {
            str = str + ", " + quad[i].ToString();
        }
        Vector3[] array3 = new Vector3[(Int32)quad.Length / 2];
        for (Int32 j = 0; j < (Int32)array3.Length; j++)
        {
            array3[j] = new Vector3((Single)quad[j * 2], 0f, (Single)quad[j * 2 + 1]);
            array3[j] = PSX.CalculateGTE_RTPT(array3[j], Matrix4x4.identity, bgcam_DEF.GetMatrixRT(), bgcam_DEF.GetViewDistance(), offset);
            Color color = array[j];
            Single num = 5f;
            Vector3 b = new Vector3(0f, 0f, (Single)scene.curZ);
            global::Debug.DrawLine(array3[j] + new Vector3(-num, -num, 0f) + b, array3[j] + new Vector3(num, num, 0f) + b, color, 0f, true);
            global::Debug.DrawLine(array3[j] + new Vector3(-num, num, 0f) + b, array3[j] + new Vector3(num, -num, 0f) + b, color, 0f, true);
        }
        for (Int32 k = 0; k < (Int32)array3.Length; k++)
        {
            Int32 num2 = (Int32)array3.Length - 1;
            Int32 num3 = (Int32)((k - 1 >= 0) ? (k - 1) : num2);
            Int32 num4 = (Int32)((k + 1 <= num2) ? (k + 1) : 0);
            Single num5 = Vector3.Angle(array3[k] - array3[num3], array3[k] - array3[num4]);
        }
        for (Int32 l = 0; l < (Int32)array3.Length - 1; l++)
        {
            global::Debug.DrawLine(array3[l], array3[l + 1], Color.cyan, 0f, true);
            global::Debug.DrawLine(array3[0], array3[l + 1], Color.cyan, 0f, true);
        }
    }

    private void PrintCurrentQuad()
    {
        String text = String.Empty;
        for (Int32 i = 0; i < (Int32)this.fieldMapData[this.currentQuad].quad.Length; i++)
        {
            text = text + ", " + this.fieldMapData[this.currentQuad].quad[i].ToString();
        }
        global::Debug.Log(String.Concat(new Object[]
        {
            this.currentQuad,
            "| name: ",
            this.fieldMapData[this.currentQuad].name,
            ", quad",
            text
        }));
    }

    public Int32 currentQuad;

    public List<GenQuad.FieldMapData> fieldMapData;

    public List<String> mapList;

    private Boolean isChange;

    public FieldMap fieldmap;

    private Int32 currentMap;

    public struct FieldMapData
    {
        public String name;

        public Int32[] quad;
    }
}
