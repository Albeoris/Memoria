using System;
using FF9;

namespace Memoria.Field
{
    public static class SFieldCalculator
    {
        public static Boolean FieldCalcMain(PLAYER caster, PLAYER target, AA_DATA tbl, Byte scriptId, UInt32 cursor)
        {
            ItemActionData tbl1 = new ItemActionData(tbl);
            return FieldCalcMain(caster, target, tbl1, scriptId, cursor);
        }

        public static Boolean FieldCalcMain(PLAYER caster, PLAYER target, ITEM_DATA tbl, Byte scriptId, UInt32 cursor)
        {
            ItemActionData tbl1 = new ItemActionData(tbl);
            return FieldCalcMain(caster, target, tbl1, scriptId, cursor);
        }

        private static Boolean FieldCalcMain(PLAYER caster, PLAYER target, ItemActionData tbl, Byte scriptId, UInt32 cursor)
        {
            Context v = new Context
            {
                Caster = caster,
                Target = target,
                Tbl = tbl,
                Cursor = cursor,
                Flags = 0
            };
            v.TargetHp = v.TargetMp = 0;
            Byte num = scriptId;
            switch (num)
            {
                case 62:
                case 73:
                    if (tbl.Info.vfx_no == 289)
                        return PersistenSingleton<UIManager>.Instance.ItemScene.FF9FItem_Vegetable();
                    FldCalcSub_305(v);
                    goto case 64;
                case 64:
                case 75:
                    label_29:
                    return FieldCalcResult(v);
                case 69:
                    if (FldCalcSub_12A(v))
                    {
                        if (target.cur.hp == target.max.hp)
                        {
                            v.Flags |= 1;
                            goto case 64;
                        }
                        else
                        {
                            FldCalcSub_171(v);
                            FldCalcSub_202(v);
                            goto case 64;
                        }
                    }
                    else
                        goto case 64;
                case 70:
                    if (FldCalcSub_12A(v))
                    {
                        if (target.cur.mp == target.max.mp)
                        {
                            v.Flags |= 1;
                            goto case 64;
                        }
                        else
                        {
                            FldCalcSub_171(v);
                            FldCalcSub_21E(v);
                            goto case 64;
                        }
                    }
                    else
                        goto case 64;
                case 71:
                    if (FldCalcSub_12A(v))
                    {
                        if (target.cur.hp == target.max.hp && target.cur.mp == target.max.mp)
                        {
                            v.Flags |= 1;
                            goto case 64;
                        }
                        else
                        {
                            FldCalcSub_21F(v);
                            goto case 64;
                        }
                    }
                    else
                        goto case 64;
                case 72:
                    if (FldCalcSub_12B(v))
                    {
                        FldCalcSub_220(v);
                        goto case 64;
                    }
                    else
                        goto case 64;
                case 74:
                    return PersistenSingleton<UIManager>.Instance.ItemScene.FF9FItem_Vegetable();
                case 76:
                    if (FldCalcSub_12A(v))
                    {
                        FldCalcSub_223(v);
                        goto case 64;
                    }
                    else
                        goto case 64;
                default:
                    switch (num)
                    {
                        case 10:
                            if (FldCalcSub_12A(v))
                            {
                                if (target.cur.hp == target.max.hp)
                                {
                                    v.Flags |= 1;
                                    goto label_29;
                                }
                                else
                                {
                                    FldCalcSub_137(v);
                                    FldCalcSub_146(v);
                                    FldCalcSub_144(v);
                                    FldCalcSub_156(v);
                                    FldCalcSub_202(v);
                                    goto label_29;
                                }
                            }
                            else
                                goto label_29;
                        case 12:
                            FldCalcSub_302(v);
                            goto label_29;
                        case 13:
                            if (FldCalcSub_12B(v))
                            {
                                FldCalcSub_204(v);
                                goto label_29;
                            }
                            else
                                goto label_29;
                        default:
                            goto label_29;
                    }
            }
        }

        private static Boolean FieldCalcResult(Context v)
        {
            if ((v.Flags & 1) != 0)
            {
                v.TargetInfo |= 32;
                return false;
            }
            if (v.TargetHp > 0)
                FieldSetRecover(v.Target, v.TargetHp);
            if (v.TargetMp > 0)
                FieldSetMpRecover(v.Target, v.TargetMp);
            return true;
        }

        private static Boolean FldCalcSub_12A(Context v)
        {
            if (!FieldCheckStatus(v.Target, 65) && v.Target.cur.hp != 0)
                return true;
            v.Flags |= 1;
            return false;
        }

        private static Boolean FldCalcSub_12B(Context v)
        {
            if (!FieldCheckStatus(v.Target, 1) && v.Target.cur.hp <= 0 && (!FieldCheckStatus(v.Target, 64) || v.Target.cur.hp != 0))
                return true;
            v.Flags |= 1;
            return false;
        }

        private static void FldCalcSub_137(Context v)
        {
            v.AttackNumber = (Int16)(v.Caster.elem.mgc + Comn.random16() % (1 + (v.Caster.level + v.Caster.elem.mgc >> 3)));
            v.AttackPower = v.Tbl.Ref.power;
            v.DefencePower = v.Target.defence.m_def;
        }

        private static void FldCalcSub_171(Context v)
        {
            v.AttackNumber = 10;
            v.AttackPower = v.Tbl.Ref.power;
            v.DefencePower = 0;
        }

        private static void FldCalcSub_144(Context v)
        {
            if (!FieldCheckStatus(v.Caster, 0))
                return;
            v.AttackNumber /= 2;
        }

        private static void FldCalcSub_146(Context v)
        {
            if (((Int32)v.Caster.sa[1] & 2) == 0)
                return;
            v.AttackNumber = (Int16)(v.AttackNumber * 3 >> 1);
        }

        private static void FldCalcSub_156(Context v)
        {
            if ((Int32)v.Cursor != 1 || v.Tbl.Info.cursor <= 2 || v.Tbl.Info.cursor >= 6)
                return;
            v.AttackNumber /= 2;
        }

        private static void FldCalcSub_202(Context v)
        {
            Int16 num = (Int16)(v.AttackPower * v.AttackNumber);
            if (num > 9999)
                num = 9999;
            v.TargetHp = num;
        }

        private static void FldCalcSub_204(Context v)
        {
            Int32 num1 = v.Target.max.hp * (v.Target.elem.wpr + v.Tbl.Ref.power);
            Int32 num2 = ((Int32)v.Caster.sa[1] & 2) == 0 ? num1 / 100 : num1 / 50;
            if (num2 > 9999)
                num2 = 9999;
            v.TargetHp = (Int16)num2;
        }

        private static void FldCalcSub_21E(Context v)
        {
            Int16 num = (Int16)(v.AttackPower * v.AttackNumber);
            if (num > 9999)
                num = 9999;
            v.TargetMp = num;
        }

        private static void FldCalcSub_21F(Context v)
        {
            v.Target.cur.hp = v.Target.max.hp;
            v.Target.cur.mp = v.Target.max.mp;
        }

        private static void FldCalcSub_220(Context v)
        {
            v.Target.cur.hp = (UInt16)(1 + Comn.random8() % 10);
        }

        private static void FldCalcSub_223(Context v)
        {
            v.TargetHp = (Int16)(v.Target.max.hp >> 1);
            v.TargetMp = (Int16)(v.Target.max.mp >> 1);
        }

        private static void FldCalcSub_302(Context v)
        {
            Byte[] numArray = new Byte[6] { 0, 0, 0, 2, 1, 24 };
            if ((Int32)FieldRemoveStatuses(v.Target, numArray[v.Tbl.AddNo]) == 2)
                return;
            v.Flags |= 1;
        }

        private static void FldCalcSub_305(Context v)
        {
            if ((Int32)FieldRemoveStatuses(v.Target, (Byte)v.Tbl.Status) == 2)
                return;
            v.Flags |= 1;
        }

        private static void FieldSetRecover(PLAYER player, Int32 recover)
        {
            if (FieldCheckStatus(player, 1))
                return;
            player.cur.hp += (UInt16)recover;
            if (player.cur.hp <= player.max.hp)
                return;
            player.cur.hp = player.max.hp;
        }

        private static void FieldSetMpRecover(PLAYER player, Int32 recover)
        {
            if (FieldCheckStatus(player, 1))
                return;
            player.cur.mp += (Int16)recover;
            if (player.cur.mp <= player.max.mp)
                return;
            player.cur.mp = player.max.mp;
        }

        public static UInt32 FieldRemoveStatus(PLAYER player, Byte status)
        {
            if ((player.status & status) == 0)
                return 1;
            player.status = (Byte)(player.status & ~status);
            return 2;
        }

        private static UInt32 FieldRemoveStatuses(PLAYER player, Byte statuses)
        {
            UInt32 num = 1;
            for (Int32 index = 0; index < 8; ++index)
            {
                Byte status = (Byte)(1 << index);
                if ((statuses & status) != 0 && (Int32)FieldRemoveStatus(player, status) == 2)
                    num = 2U;
            }
            return num;
        }

        private static Boolean FieldCheckStatus(PLAYER player, Byte status)
        {
            return (player.status & status) != 0;
        }

        private sealed class ItemActionData
        {
            public CMD_INFO Info;
            public BTL_REF Ref;
            public Byte Category;
            public Byte AddNo;
            public Byte MP;
            public Byte Type;
            public UInt16 Vfx2;
            public String Name;
            public UInt32 Status;

            public ItemActionData(ITEM_DATA item)
            {
                Info = item.info;
                Ref = item.Ref;
                Status = item.status;
            }

            public ItemActionData(AA_DATA aa)
            {
                Info = aa.Info;
                Ref = aa.Ref;
                Category = aa.Category;
                AddNo = aa.AddNo;
                MP = aa.MP;
                Type = aa.Type;
                Vfx2 = aa.Vfx2;
                Name = aa.Name;
            }
        }

        private sealed class Context
        {
            public PLAYER Caster;
            public PLAYER Target;
            public ItemActionData Tbl;
            public UInt32 Cursor;
            public Byte Flags;
            public Byte TargetInfo;
            public Int16 AttackPower;
            public Int16 DefencePower;
            public Int16 AttackNumber;
            public Int16 TargetHp;
            public Int16 TargetMp;
        }
    }
}