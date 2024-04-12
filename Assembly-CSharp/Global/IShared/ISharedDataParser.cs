using SimpleJSON;
using UnityEngine;

public abstract class ISharedDataParser : MonoBehaviour
{
	public abstract void ParseFromFF9StateSystem();

	public abstract void ParseToFF9StateSystem(JSONClass rootNode);

	public JSONClass RootNodeInParser;
}
