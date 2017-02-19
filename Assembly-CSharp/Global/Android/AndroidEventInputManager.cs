using System;

public class AndroidEventInputManager : PersistenSingleton<AndroidEventInputManager>
{
	public Boolean GetKeyTrigger(Control key)
	{
		Boolean result = this.inputDown[(Int32)key];
		this.inputDown[(Int32)key] = false;
		return result;
	}

	public void Reset()
	{
		for (Int32 i = 0; i < 4; i++)
		{
			this.inputDown[i] = false;
		}
	}

	private void Update()
	{
		if (FF9StateSystem.AndroidPlatform && EventInput.IsKeyboardOrJoystickInput && PersistenSingleton<EventEngine>.Instance.gMode == 3 && EventInput.IsProcessingInput)
		{
			for (Int32 i = 0; i < 4; i++)
			{
				this.inputDown[i] = (this.inputDown[i] ? this.inputDown[i] : UIManager.Input.GetKeyTrigger((Control)i));
			}
		}
	}

	private Boolean[] inputDown = new Boolean[4];
}
