using System;
using SimpleJSON;
using UnityEngine;

public static class SettingUtils
{
	public static void LoadSettings()
	{
		TextAsset textAsset = AssetManager.Load<TextAsset>("EmbeddedAsset/Manifest/FieldMap/settingUtils.txt", false);
		if (textAsset == (UnityEngine.Object)null)
		{
			return;
		}
		SettingUtils.jsNode = JSON.Parse(textAsset.text);
		if (SettingUtils.jsNode == null)
		{
			return;
		}
		JSONNode jsonnode = SettingUtils.jsNode["FieldMapSettings"];
		if (jsonnode == null)
		{
			return;
		}
		if (jsonnode["enable"] != null)
		{
			SettingUtils.fieldMapSettings.enable = jsonnode["enable"].AsBool;
		}
		SettingUtils._ReadFieldMapSettingsFromJSONNode(jsonnode);
		if (jsonnode["activeProfileId"] != null)
		{
			SettingUtils.fieldMapSettings.activeProfileId = jsonnode["activeProfileId"].AsInt;
		}
		if (SettingUtils.fieldMapSettings.activeProfileId == -1)
		{
			return;
		}
		JSONNode jsonnode2 = jsonnode["debugProfile"];
		if (jsonnode2 == null)
		{
			return;
		}
		String aKey = "profile_" + SettingUtils.fieldMapSettings.activeProfileId;
		JSONNode jsonnode3 = jsonnode2[aKey];
		if (jsonnode3 == null)
		{
			return;
		}
		SettingUtils._ReadFieldMapSettingsFromJSONNode(jsonnode3);
	}

	public static Vector3 ReadVector3(JSONNode node, String key)
	{
		if (node[key] == null)
		{
			return Vector3.zero;
		}
		String value = node[key].Value;
		String[] array = value.Substring(1, value.Length - 2).Split(new Char[]
		{
			','
		});
		Single x = Single.Parse(array[0]);
		Single y = Single.Parse(array[1]);
		Single z = Single.Parse(array[2]);
		Vector3 result = new Vector3(x, y, z);
		return result;
	}

	private static void _ReadFieldMapSettingsFromJSONNode(JSONNode node)
	{
		if (node["language"] != null)
		{
			SettingUtils.fieldMapSettings.language = node["language"].Value;
		}
		if (node["fldMapNo"] != null)
		{
			SettingUtils.fieldMapSettings.fldMapNo = node["fldMapNo"].AsInt;
		}
		if (node["SC_COUNTER_SVR"] != null)
		{
			SettingUtils.fieldMapSettings.SC_COUNTER_SVR = node["SC_COUNTER_SVR"].AsInt;
		}
		if (node["MAP_INDEX_SVR"] != null)
		{
			SettingUtils.fieldMapSettings.MAP_INDEX_SVR = node["MAP_INDEX_SVR"].AsInt;
		}
		if (node["isDebugWalkMesh"] != null)
		{
			SettingUtils.fieldMapSettings.isDebugWalkMesh = node["isDebugWalkMesh"].AsBool;
		}
		if (node["debugObjName"] != null)
		{
			SettingUtils.fieldMapSettings.debugObjName = node["debugObjName"].Value;
		}
		if (node["debugTriIdx"] != null)
		{
			SettingUtils.fieldMapSettings.debugTriIdx = node["debugTriIdx"].AsInt;
		}
		for (Int32 i = 0; i < (Int32)SettingUtils.fieldMapSettings.debugPosMarker.Length; i++)
		{
			if (node["debugPosMarker" + i] != null)
			{
				SettingUtils.fieldMapSettings.debugPosMarker[i] = SettingUtils.ReadVector3(node, "debugPosMarker" + i);
			}
		}
		if (node["debugInt0"] != null)
		{
			SettingUtils.fieldMapSettings.debugInt0 = node["debugInt0"].AsInt;
		}
		if (node["debugFloat0"] != null)
		{
			SettingUtils.fieldMapSettings.debugFloat0 = node["debugFloat0"].AsFloat;
		}
	}

	public static JSONNode jsNode = (JSONNode)null;

	public static SettingUtils.FieldMapSettings fieldMapSettings = new SettingUtils.FieldMapSettings();

	public class FieldMapSettings
	{
		public FieldMapSettings()
		{
			this.enable = false;
			this.language = "English(US)";
			this.fldMapNo = 50;
			this.SC_COUNTER_SVR = 0;
			this.MAP_INDEX_SVR = 0;
			this.isDebugWalkMesh = false;
			this.debugObjName = "Player";
			this.debugTriIdx = -1;
			this.debugPosMarker = new Vector3[5];
			for (Int32 i = 0; i < (Int32)this.debugPosMarker.Length; i++)
			{
				this.debugPosMarker[i] = default(Vector3);
			}
			this.debugInt0 = 0;
			this.debugFloat0 = 0f;
			this.activeProfileId = -1;
		}

		public Boolean enable;

		public String language;

		public Int32 fldMapNo;

		public Int32 SC_COUNTER_SVR;

		public Int32 MAP_INDEX_SVR;

		public Boolean isDebugWalkMesh;

		public String debugObjName;

		public Int32 debugTriIdx;

		public Vector3[] debugPosMarker;

		public Int32 debugInt0;

		public Single debugFloat0;

		public Int32 activeProfileId;
	}
}
