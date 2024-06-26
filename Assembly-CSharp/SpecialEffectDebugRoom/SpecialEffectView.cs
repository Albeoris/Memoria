using Assets.Scripts.Common;
using Assets.Sources.Scripts.Common;
using Memoria;
using Memoria.Data;
using System;
using UnityEngine;

namespace SpecialEffectDebugRoom
{
    public class SpecialEffectView : MonoBehaviour
    {
        private void Awake()
        {
            FPSManager.SetTargetFPS(15);
            FPSManager.SetMainLoopSpeed(15);
            this.effNum = 0;
            this.strEffNum = this.effNum.ToString();
            Camera.main.orthographic = false;
            Camera.main.nearClipPlane = 400f;
            Camera.main.farClipPlane = 16384f;
            Camera.main.fieldOfView = 45f;
            PSXTextureMgr.InitOnce();
            SFX.SetCameraPhase(0);
            SFX.StartDebugRoom();
        }

        private void Start()
        {
            this.effNum = 126;
        }

        private void OnGUI()
        {
            SFX.DebugOnGUI();
            DebugGuiSkin.ApplySkin();
            this.screenRect = DebugGuiSkin.GetFullscreenRect();
            this.OnUiTop();
            if (!this.isHideUI)
            {
                this.OnUiSpecialEffect();
            }
        }

        private void OnUiTop()
        {
            Rect rect = this.screenRect;
            rect.height = this.screenRect.height * 0.5f;
            rect.y = 0f;
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("Back", new GUILayoutOption[0]))
            {
                SFX.isDebugViewport = false;
                SFX.isDebugMode = false;
                SFX.isDebugPng = false;
                SFX.isDebugLine = false;
                SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Debug UI", new GUILayoutOption[0]))
            {
                this.isHideUI = !this.isHideUI;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void OnUiSpecialEffect()
        {
            Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
            fullscreenRect.height *= 0.5f;
            fullscreenRect.y = fullscreenRect.height;
            GUILayout.BeginArea(fullscreenRect);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            this.OnUiSpecialEffectBottom1();
            this.OnUiSpecialEffectBottom0();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void OnUiSpecialEffectBottom1()
        {
            GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
            GUILayout.Label("Prim: " + SFXRender.primCount, new GUILayoutOption[0]);
            GUILayout.Label("Frame ID: " + SFX.frameIndex, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
        }

        private void OnUiSpecialEffectBottom0()
        {
            GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
            if (GUILayout.Button("Auto " + ((!SFX.isDebugAutoPlay) ? "Off" : "On"), new GUILayoutOption[0]))
            {
                SFX.isDebugAutoPlay = !SFX.isDebugAutoPlay;
            }
            if (GUILayout.Button("GC ", new GUILayoutOption[0]))
            {
                GC.Collect();
            }
            if (GUILayout.Button("Png " + ((!SFX.isDebugPng) ? "Off" : "On"), new GUILayoutOption[0]))
            {
                SFX.isDebugPng = !SFX.isDebugPng;
            }
            if (GUILayout.Button("Line " + ((!SFX.isDebugLine) ? "Off" : "On"), new GUILayoutOption[0]))
            {
                SFX.isDebugLine = !SFX.isDebugLine;
            }
            if (GUILayout.Button("View " + ((!SFX.isDebugViewport) ? "x 1" : "x 3"), new GUILayoutOption[0]))
            {
                SFX.isDebugViewport = !SFX.isDebugViewport;
            }
            if (GUILayout.Button("<", new GUILayoutOption[0]))
            {
                this.effNum--;
                if (this.effNum < 0)
                {
                    this.effNum = 510;
                }
                this.strEffNum = this.effNum.ToString();
            }
            String text = this.strEffNum;
            this.strEffNum = GUILayout.TextField(text, new GUILayoutOption[0]);
            if (this.strEffNum != text)
            {
                Int32 num = 0;
                if (Int32.TryParse(this.strEffNum, out num))
                {
                    if (num >= 0 && num < 511)
                    {
                        this.effNum = num;
                        this.strEffNum = this.effNum.ToString();
                    }
                }
                else if (this.strEffNum.Length == 0)
                {
                    this.effNum = 0;
                }
            }
            if (GUILayout.Button(">", new GUILayoutOption[0]))
            {
                this.effNum++;
                if (this.effNum >= 511)
                {
                    this.effNum = 0;
                }
                this.strEffNum = this.effNum.ToString();
            }
            Boolean enabled = true;
            GUI.enabled = enabled;
            if (GUILayout.Button("Play", new GUILayoutOption[0]))
            {
                global::Debug.Log("effect id = " + this.effNum.ToString());
                this.isPause = false;
                SFX.Begin(1, 0, new Byte[2], default(PSX_LIBGTE.VECTOR));
                SFX.SetDebug();
                SFX.Play((SpecialEffect)this.effNum);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            for (Int32 updateCount = 0; updateCount < FPSManager.MainLoopUpdateCount; updateCount++)
            {
                if (SFX.isDebugAutoPlay && !SFX.isRunning)
                {
                    this.effNum = SpecialEffectView.effTeble[UnityEngine.Random.Range(0, (Int32)SpecialEffectView.effTeble.Length)];
                    global::Debug.Log("effect id = " + this.effNum.ToString());
                    this.isPause = false;
                    SFX.Begin(0, 0, new Byte[2], default(PSX_LIBGTE.VECTOR));
                    SFX.SetDebug();
                    SFX.Play((SpecialEffect)this.effNum);
                }
                for (Int32 i = 0; i < (Int32)(SFX.isDebugAutoPlay ? 4 : 1); i++)
                {
                    if (!this.isPause)
                    {
                        SFX.UpdateCamera();
                        SFX.UpdatePlugin();
                    }
                }
            }
        }

        private void LateUpdate()
        {
            SFX.LateUpdatePlugin();
        }

        private void OnPostRender()
        {
            GL.Clear(true, true, new Color(0f, 0f, 0f));
            SFX.PostRender();
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            PSXTextureMgr.PostBlur(src, dest);
        }

        private void OnDestroy()
        {
            SFX.EndDebugRoom();
        }

        private const Int32 fps = 15;

        private Rect screenRect;

        private Boolean isHideUI;

        private Boolean isSummonUI;

        public Int32 effNum;

        private String strEffNum;

        private Single cumulativeTime;

        private Single frameTime = 0.06666667f;

        private Boolean isPause;

        private static Int32[] effTeble = new Int32[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            17,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31,
            32,
            33,
            34,
            35,
            36,
            38,
            40,
            41,
            42,
            43,
            44,
            45,
            46,
            47,
            48,
            49,
            50,
            51,
            52,
            53,
            54,
            55,
            56,
            57,
            58,
            59,
            60,
            61,
            62,
            63,
            64,
            65,
            66,
            67,
            68,
            69,
            70,
            71,
            72,
            73,
            74,
            75,
            76,
            77,
            78,
            79,
            81,
            82,
            83,
            85,
            86,
            87,
            88,
            89,
            90,
            92,
            93,
            94,
            95,
            96,
            97,
            98,
            99,
            120,
            121,
            122,
            123,
            124,
            125,
            126,
            127,
            128,
            129,
            130,
            131,
            132,
            133,
            134,
            135,
            136,
            137,
            138,
            139,
            140,
            141,
            142,
            143,
            144,
            145,
            146,
            147,
            148,
            149,
            150,
            151,
            152,
            153,
            154,
            155,
            156,
            157,
            158,
            159,
            160,
            161,
            162,
            163,
            164,
            165,
            166,
            167,
            168,
            169,
            170,
            171,
            172,
            173,
            174,
            175,
            176,
            177,
            178,
            179,
            180,
            181,
            182,
            183,
            184,
            185,
            186,
            187,
            188,
            189,
            190,
            191,
            192,
            193,
            194,
            195,
            196,
            197,
            198,
            199,
            201,
            202,
            203,
            204,
            205,
            206,
            207,
            208,
            209,
            210,
            211,
            212,
            213,
            214,
            215,
            216,
            217,
            218,
            219,
            220,
            221,
            222,
            223,
            224,
            225,
            226,
            227,
            228,
            229,
            230,
            231,
            232,
            233,
            234,
            235,
            236,
            237,
            238,
            239,
            240,
            241,
            242,
            243,
            244,
            245,
            246,
            247,
            248,
            249,
            250,
            251,
            252,
            253,
            254,
            255,
            256,
            257,
            258,
            259,
            260,
            261,
            262,
            274,
            275,
            276,
            277,
            278,
            279,
            280,
            281,
            282,
            283,
            284,
            285,
            286,
            287,
            288,
            289,
            290,
            291,
            292,
            293,
            294,
            295,
            296,
            297,
            298,
            299,
            300,
            301,
            302,
            303,
            304,
            305,
            306,
            307,
            308,
            309,
            310,
            311,
            312,
            377,
            378,
            381,
            382,
            383,
            384,
            385,
            386,
            387,
            388,
            389,
            390,
            391,
            392,
            394,
            395,
            396,
            397,
            398,
            399,
            400,
            401,
            402,
            403,
            404,
            405,
            406,
            407,
            408,
            409,
            410,
            411,
            412,
            413,
            414,
            415,
            416,
            417,
            418,
            419,
            420,
            421,
            422,
            423,
            424,
            425,
            427,
            428,
            429,
            431,
            432,
            433,
            434,
            435,
            436,
            431,
            432,
            433,
            434,
            435,
            436,
            443,
            445,
            446,
            447,
            457,
            458,
            459,
            460,
            489,
            490,
            491,
            492,
            493,
            494,
            495,
            496,
            497,
            498,
            499,
            500,
            501,
            502,
            503,
            504,
            505,
            506,
            507,
            508,
            509,
            510
        };
    }
}
