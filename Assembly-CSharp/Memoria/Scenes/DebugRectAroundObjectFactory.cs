using Memoria.Prime;
using System;
using System.Threading;
using UnityEngine;
using Object = System.Object;

namespace Memoria.Scenes
{
	public sealed class DebugRectAroundObjectFactory
	{
		private static readonly DebugRectAroundObjectFactory s_instance = new DebugRectAroundObjectFactory();

		public static void Run()
		{
			Log.Message("~[DebugRectAroundObjectFactory]");
			s_instance.RunInternal();
		}

		private volatile Timer _timer;

		private void RunInternal()
		{
			lock (this)
			{
				if (_timer != null)
					return;

				_timer = new Timer(OnTimer, null, 0, 3);
			}
		}

		private void OnTimer(Object state)
		{
			if (!Monitor.TryEnter(_timer, 0))
				return;

			try
			{
				GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
				foreach (GameObject obj in allGameObjects)
				{
					if (!obj.activeInHierarchy)
						continue;

					if (obj.name.StartsWith("Block"))
						continue;

					if (obj.name.StartsWith("bone"))
						continue;

					if (obj.name.StartsWith("SPS_"))
						continue;

					if (obj.name.StartsWith("Overlay_"))
						continue;

					//if (obj.name == "TriPosObj" || obj.name == "Player")
					if (obj.name.StartsWith("obj") && !obj.name.EndsWith("_Shadow"))
					{
						DebugRectAroundObject component = obj.GetComponent<DebugRectAroundObject>();
						if (component != null)
							continue;

						component = obj.AddComponent<DebugRectAroundObject>();
						component.Initialize(obj);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
			finally
			{
				Monitor.Exit(_timer);
			}
		}
	}

	public sealed class DebugRectAroundObject : MonoBehaviour
	{
		private volatile GameObject _obj;
		private volatile FieldMapActorController _controler;

		public void Initialize(GameObject obj)
		{
			_obj = obj;
		}

		public void OnGUI()
		{
			try
			{
				if (_obj == null)
					return;

				if (_controler == null)
				{
					_controler = FindObjectOfType<FieldMapActorController>();
					if (_controler == null)
						return;
				}

				Vector3 a = _obj.transform.position;

				Camera mainCamera = _controler.fieldMap.GetMainCamera();
				BGCAM_DEF currentBgCamera = _controler.fieldMap.GetCurrentBgCamera();
				Vector3 position = PSX.CalculateGTE_RTPT(a + new Vector3(0f, 0, 0f), Matrix4x4.identity, currentBgCamera.GetMatrixRT(), currentBgCamera.GetViewDistance(), _controler.fieldMap.GetProjectionOffset());
				position = mainCamera.WorldToScreenPoint(position);
				position.y = (Single)Screen.height - position.y;
				Int32 targetMarkSize = 40;
				Rect position2 = new Rect(position.x - targetMarkSize / 2f, position.y - targetMarkSize, targetMarkSize, targetMarkSize);
				GUI.Label(position2, _obj.name);

				Component[] components = _obj.GetComponents<Component>();
				Log.Message(_obj.name);
				foreach (Component cmp in components)
				{
					Log.Message(cmp.name + '_' + cmp.GetType());
				}
				for (Int32 i = 0; i < _obj.transform.childCount; i++)
				{
					GameObject child = _obj.GetChild(i);
					Log.Message("child: " + child.name);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex);
			}
		}
	}
}
