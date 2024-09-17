using FF9;
using NCalc;
using SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParametricMovement
{
    private class MovementSetup
    {
        public class Piece
        {
            public Expression[] origin = new Expression[3] { null, null, null };
            public Expression[] dest = new Expression[3] { null, null, null };
            public InterpolateType[] interpolate = new InterpolateType[3] { InterpolateType.Linear, InterpolateType.Linear, InterpolateType.Linear };
            public Int32 min = 0;
            public Int32 max = 1;
        }

        public List<Piece> piece = new List<Piece>();
    }

    private MovementSetup setup = null;
    private MovementSetup.Piece currentPiece = null;
    private Int32 currentPieceID = -1;
    public Vector3 currentOrigin = default(Vector3);
    public Vector3 currentDest = default(Vector3);

    public ParametricMovement()
    {
        setup = null;
        currentPiece = null;
        currentPieceID = -1;
        currentOrigin = default(Vector3);
        currentDest = default(Vector3);
    }

    public ParametricMovement(ParametricMovement basis)
    {
        setup = basis.setup;
        currentPiece = null;
        currentPieceID = -1;
        currentOrigin = default(Vector3);
        currentDest = default(Vector3);
    }

    public void LoadFromJSON(JSONNode node)
    {
        setup = new MovementSetup();
        if (node == null || (node is not JSONArray && node is not JSONClass))
            return;
        Int32 pieceID = 0;
        JSONClass[] classList;
        if (node is JSONArray)
        {
            classList = new JSONClass[node.AsArray.Count];
            for (Int32 i = 0; i < node.AsArray.Count; i++)
                classList[i] = node.AsArray[i] is JSONClass ? node.AsArray[i] as JSONClass : null;
        }
        else
        {
            classList = new JSONClass[] { node as JSONClass };
        }
        Int32 duration;
        MovementSetup.Piece lastPiece = null;
        for (Int32 i = 0; i < classList.Length; i++)
        {
            if (classList[i] == null)
                continue;
            MovementSetup.Piece p = new MovementSetup.Piece();
            duration = classList[i]["Duration"] != null ? classList[i]["Duration"].AsInt : 1;
            if (classList[i]["OriginX"] != null)
            {
                p.origin[0] = new Expression(classList[i]["OriginX"]);
                p.origin[0].EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                p.origin[0].EvaluateParameter += NCalcUtility.commonNCalcParameters;
            }
            else if (lastPiece != null)
                p.origin[0] = lastPiece.dest[0];
            if (classList[i]["OriginY"] != null)
            {
                p.origin[1] = new Expression(classList[i]["OriginY"]);
                p.origin[1].EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                p.origin[1].EvaluateParameter += NCalcUtility.commonNCalcParameters;
            }
            else if (lastPiece != null)
                p.origin[1] = lastPiece.dest[1];
            if (classList[i]["OriginZ"] != null)
            {
                p.origin[2] = new Expression(classList[i]["OriginZ"]);
                p.origin[2].EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                p.origin[2].EvaluateParameter += NCalcUtility.commonNCalcParameters;
            }
            else if (lastPiece != null)
                p.origin[2] = lastPiece.dest[2];
            if (classList[i]["DestinationX"] != null)
            {
                p.dest[0] = new Expression(classList[i]["DestinationX"]);
                p.dest[0].EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                p.dest[0].EvaluateParameter += NCalcUtility.commonNCalcParameters;
            }
            if (classList[i]["DestinationY"] != null)
            {
                p.dest[1] = new Expression(classList[i]["DestinationY"]);
                p.dest[1].EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                p.dest[1].EvaluateParameter += NCalcUtility.commonNCalcParameters;
            }
            if (classList[i]["DestinationZ"] != null)
            {
                p.dest[2] = new Expression(classList[i]["DestinationZ"]);
                p.dest[2].EvaluateFunction += NCalcUtility.commonNCalcFunctions;
                p.dest[2].EvaluateParameter += NCalcUtility.commonNCalcParameters;
            }
            if (classList[i]["InterpolationTypeX"] != null)
                TryParseInterpolateType(classList[i]["InterpolationTypeX"], out p.interpolate[0]);
            if (classList[i]["InterpolationTypeY"] != null)
                TryParseInterpolateType(classList[i]["InterpolationTypeY"], out p.interpolate[1]);
            if (classList[i]["InterpolationTypeZ"] != null)
                TryParseInterpolateType(classList[i]["InterpolationTypeZ"], out p.interpolate[2]);
            p.min = pieceID;
            p.max = pieceID + duration;
            setup.piece.Add(p);
            pieceID += duration;
            lastPiece = p;
        }
    }

    private static void InitExpressions(Expression[] init, Dictionary<Int32, Single> customParam, BTL_DATA caster, BTL_DATA target, Vector3 avgPos)
    {
        foreach (Expression e in init)
        {
            if (e == null)
                continue;
            Matrix4x4 crootmat = Matrix4x4.identity, ctargmat = Matrix4x4.identity, cweapmat = Matrix4x4.identity;
            Matrix4x4 trootmat = Matrix4x4.identity, ttargmat = Matrix4x4.identity, tweapmat = Matrix4x4.identity;
            if (caster != null)
            {
                crootmat = caster.gameObject.transform.GetChildByName("bone000")?.localToWorldMatrix ?? Matrix4x4.identity;
                ctargmat = caster.gameObject.transform.GetChildByName("bone" + caster.tar_bone.ToString("D3"))?.localToWorldMatrix ?? Matrix4x4.identity;
                cweapmat = caster.gameObject.transform.GetChildByName("bone" + caster.weapon_bone.ToString("D3"))?.localToWorldMatrix ?? Matrix4x4.identity;
            }
            if (target != null)
            {
                trootmat = target.gameObject.transform.GetChildByName("bone000")?.localToWorldMatrix ?? Matrix4x4.identity;
                ttargmat = target.gameObject.transform.GetChildByName("bone" + target.tar_bone.ToString("D3"))?.localToWorldMatrix ?? Matrix4x4.identity;
                tweapmat = target.gameObject.transform.GetChildByName("bone" + target.weapon_bone.ToString("D3"))?.localToWorldMatrix ?? Matrix4x4.identity;
            }
            e.Parameters["CasterPositionX"] = caster != null ? caster.pos.x : 0f;
            e.Parameters["CasterPositionY"] = caster != null ? caster.pos.y : 0f;
            e.Parameters["CasterPositionZ"] = caster != null ? caster.pos.z : 0f;
            e.Parameters["CasterRootX"] = crootmat.m03;
            e.Parameters["CasterRootY"] = crootmat.m13;
            e.Parameters["CasterRootZ"] = crootmat.m23;
            e.Parameters["CasterMainX"] = ctargmat.m03;
            e.Parameters["CasterMainY"] = ctargmat.m13;
            e.Parameters["CasterMainZ"] = ctargmat.m23;
            e.Parameters["CasterWeaponX"] = cweapmat.m03;
            e.Parameters["CasterWeaponY"] = cweapmat.m13;
            e.Parameters["CasterWeaponZ"] = cweapmat.m23;
            e.Parameters["CasterAngleX"] = caster != null ? caster.rot.eulerAngles.x : 0f;
            e.Parameters["CasterAngleY"] = caster != null ? caster.rot.eulerAngles.y : 0f;
            e.Parameters["CasterAngleZ"] = caster != null ? caster.rot.eulerAngles.z : 0f;
            e.Parameters["CasterRadius"] = caster != null ? (caster.bi.player == 0 ? caster.radius_effect : 256f) : 0f;
            e.Parameters["CasterHeight"] = caster != null ? (caster.bi.player == 0 ? 1 : 2) * caster.radius_collision : 0f;
            e.Parameters["CasterIsPlayer"] = caster != null ? caster.bi.player != 0 : false;
            e.Parameters["TargetPositionX"] = target != null ? target.pos.x : 0f;
            e.Parameters["TargetPositionY"] = target != null ? target.pos.y : 0f;
            e.Parameters["TargetPositionZ"] = target != null ? target.pos.z : 0f;
            e.Parameters["TargetRootX"] = trootmat.m03;
            e.Parameters["TargetRootY"] = trootmat.m13;
            e.Parameters["TargetRootZ"] = trootmat.m23;
            e.Parameters["TargetMainX"] = ttargmat.m03;
            e.Parameters["TargetMainY"] = ttargmat.m13;
            e.Parameters["TargetMainZ"] = ttargmat.m23;
            e.Parameters["TargetWeaponX"] = tweapmat.m03;
            e.Parameters["TargetWeaponY"] = tweapmat.m13;
            e.Parameters["TargetWeaponZ"] = tweapmat.m23;
            e.Parameters["TargetAngleX"] = target != null ? target.rot.eulerAngles.x : 0f;
            e.Parameters["TargetAngleY"] = target != null ? target.rot.eulerAngles.y : 0f;
            e.Parameters["TargetAngleZ"] = target != null ? target.rot.eulerAngles.z : 0f;
            e.Parameters["TargetRadius"] = target != null ? (target.bi.player == 0 ? target.radius_effect : 256f) : 0f;
            e.Parameters["TargetHeight"] = target != null ? (target.bi.player == 0 ? 1 : 2) * target.radius_collision : 0f;
            e.Parameters["TargetIsPlayer"] = target != null ? target.bi.player != 0 : false;
            e.Parameters["TargetAveragePositionX"] = avgPos.x;
            e.Parameters["TargetAveragePositionY"] = avgPos.y;
            e.Parameters["TargetAveragePositionZ"] = avgPos.z;
            if (customParam != null)
                foreach (KeyValuePair<Int32, Single> cp in customParam)
                    e.Parameters["Parameter" + cp.Key] = cp.Value;
        }
    }

    public Vector3 GetPosition(Int32 frame, Dictionary<Int32, Single> customParam = null, BTL_DATA caster = null, BTL_DATA target = null, Vector3 avgPos = default(Vector3))
    {
        Int32 pieceID;
        for (pieceID = 0; pieceID < setup.piece.Count; pieceID++)
            if (setup.piece[pieceID].min <= frame && setup.piece[pieceID].max >= frame)
                break;
        if (pieceID < setup.piece.Count && pieceID != currentPieceID)
        {
            MovementSetup.Piece p = setup.piece[pieceID];
            InitExpressions(p.origin, customParam, caster, target, avgPos);
            InitExpressions(p.dest, customParam, caster, target, avgPos);
            for (Int32 i = 0; i < 3; i++)
                currentOrigin[i] = p.origin[i] != null ? NCalcUtility.ConvertNCalcResult(p.origin[i].Evaluate(), 0f) : currentOrigin[i];
            for (Int32 i = 0; i < 3; i++)
                currentDest[i] = p.dest[i] != null ? NCalcUtility.ConvertNCalcResult(p.dest[i].Evaluate(), 0f) : currentDest[i];
            currentPieceID = pieceID;
            currentPiece = p;
        }
        if (currentPiece == null)
            return default(Vector3);
        Single[] r = new Single[3];
        for (Int32 i = 0; i < 3; i++)
        {
            Single max = currentPiece.max - currentPiece.min;
            Single cur = frame - currentPiece.min;
            if (currentPiece.interpolate[i] == InterpolateType.Turning1 || currentPiece.interpolate[i] == InterpolateType.Turning2)
            {
                Single baseAngle;
                customParam.TryGetValue(0, out baseAngle);
                cur += max * baseAngle / 360f;
            }
            else
            {
                cur = Math.Min(cur, max);
            }
            r[i] = Interpolate(cur, max, currentOrigin[i], currentDest[i], currentPiece.interpolate[i]);
        }
        return new Vector3(r[0], r[1], r[2]);
    }

    public enum InterpolateType
    {
        Constant, // origin -> origin
        Linear, // origin -> destination
        Sinus, // origin -> destination
        SinusIn, // origin -> destination
        SinusOut, // origin -> destination
        Turning1, // origin -> destination -> origin
        Turning2 // mid -> destination -> origin -> mid
    }

    public static Boolean TryParseInterpolateType(String s, out InterpolateType t)
    {
        try
        {
            t = (InterpolateType)Enum.Parse(typeof(InterpolateType), s);
            return true;
        }
        catch (Exception)
        {
            t = InterpolateType.Constant;
            return false;
        }
    }

    public static Single Factor1(Single cur, Single max, InterpolateType type = InterpolateType.Linear)
    {
        switch (type)
        {
            case InterpolateType.Constant: return 1f;
            case InterpolateType.Linear: return (max - cur) / max;
            case InterpolateType.Sinus: return (Single)(0.5 * (1 - Math.Cos(Math.PI * (max - cur) / max)));
            case InterpolateType.SinusIn: return (Single)Math.Sin(0.5 * Math.PI * (max - cur) / max);
            case InterpolateType.SinusOut: return (Single)(1 - Math.Cos(0.5 * Math.PI * (max - cur) / max));
            case InterpolateType.Turning1: return (Single)(0.5 * (1 + Math.Cos(2 * Math.PI * cur / max)));
            case InterpolateType.Turning2: return (Single)(0.5 * (1 + Math.Sin(2 * Math.PI * cur / max)));
        }
        return (max - cur) / max;
    }
    public static Single Factor2(Single cur, Single max, InterpolateType type = InterpolateType.Linear)
    {
        switch (type)
        {
            case InterpolateType.Constant: return 0f;
            case InterpolateType.Linear: return cur / max;
            case InterpolateType.Sinus: return (Single)(0.5 * (1 - Math.Cos(Math.PI * cur / max)));
            case InterpolateType.SinusIn: return (Single)(1 - Math.Cos(0.5 * Math.PI * cur / max));
            case InterpolateType.SinusOut: return (Single)Math.Sin(0.5 * Math.PI * cur / max);
            case InterpolateType.Turning1: return (Single)(0.5 * (1 - Math.Cos(2 * Math.PI * cur / max)));
            case InterpolateType.Turning2: return (Single)(0.5 * (1 - Math.Sin(2 * Math.PI * cur / max)));
        }
        return cur / max;
    }
    public static Single Interpolate(Single cur, Single max, Single origin, Single dest, InterpolateType type = InterpolateType.Linear)
    {
        return Factor1(cur, max, type) * origin + Factor2(cur, max, type) * dest;
    }
    public static Vector4 Interpolate(Single cur, Single max, Vector4 origin, Vector4 dest, InterpolateType type = InterpolateType.Linear)
    {
        return Factor1(cur, max, type) * origin + Factor2(cur, max, type) * dest;
    }

    public static T GetInterpolatedDictionaryValue<T>(Func<Single, T, Single, T, T> interpolationFunction, Dictionary<Int32, T> dic, Int32 frame, T defaultValue, InterpolateType iTypeDefault = InterpolateType.Linear, InterpolateType[] iTypeArray = null)
    {
        if (dic.Count == 0)
            return defaultValue;
        T before = defaultValue;
        T after = defaultValue;
        Int32 frameBefore = Int32.MinValue;
        Int32 frameAfter = Int32.MaxValue;
        Int32 countBefore = -1;
        if (dic.TryGetValue(frame, out before))
            return before;
        foreach (KeyValuePair<Int32, T> p in dic)
        {
            if (p.Key <= frame)
            {
                countBefore++;
                if (p.Key > frameBefore)
                {
                    frameBefore = p.Key;
                    before = p.Value;
                }
            }
            if (p.Key >= frame && p.Key < frameAfter)
            {
                frameAfter = p.Key;
                after = p.Value;
            }
        }
        if (frameBefore == Int32.MinValue)
            return after;
        if (frameAfter == Int32.MaxValue)
            return before;
        if (frameAfter == frameBefore)
            return before;
        InterpolateType iType = iTypeArray != null && countBefore < iTypeArray.Length ? iTypeArray[countBefore] : iTypeDefault;
        return interpolationFunction(Factor1(frame - frameBefore, frameAfter - frameBefore, iType), before, Factor2(frame - frameBefore, frameAfter - frameBefore, iType), after);
    }
}
