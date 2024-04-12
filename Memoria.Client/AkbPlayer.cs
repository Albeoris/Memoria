using System.Runtime.ExceptionServices;

namespace Memoria.Client
{
	public sealed class AkbPlayer
	{
		public AkbPlayer()
		{
			Method();
		}

		[HandleProcessCorruptedStateExceptions()]
		private static void Method()
		{
			while (true)
			{
				//var instance = new SdLibAPIWithProLicense();
				//ISdLibAPIProxy.Instance = instance;
				//Int32 num = instance.SdSoundSystem_Create(String.Empty);
				//if (num < 0)
				//    return;

				//var musicPlayer = new MusicPlayer();
				//var soundPlayerList = new List<SoundPlayer>();
				//soundPlayerList.Add(musicPlayer);

				//SoundProfile soundProfile = new SoundProfile();
				//soundProfile.Code = 1.ToString();
				//soundProfile.Name = "music033";
				//soundProfile.SoundIndex = 1;
				//soundProfile.ResourceID = "Sounds01/BGM_/music033.ogg";
				//soundProfile.SoundProfileType = SoundProfileType.Music;

				//String path = @"StreamingAssets\Sounds\Sounds01\BGM_\music033";
				//DateTime writeTime = File.GetLastWriteTimeUtc(path);

				//Byte[] array = File.ReadAllBytes(path);

				//IntPtr intPtr = Marshal.AllocHGlobal((Int32)array.Length);
				//Marshal.Copy(array, 0, intPtr, (Int32)array.Length);
				//Int32 bankID = instance.SdSoundSystem_AddData(intPtr);
				//soundProfile.AkbBin = intPtr;
				//soundProfile.BankID = bankID;

				//try
				//{
				//    musicPlayer.soundDatabase.Create(soundProfile);
				//    musicPlayer.PlayMusic(soundProfile, 9);

				//    while (File.GetLastWriteTimeUtc(path) == writeTime)
				//        Thread.Sleep(1000);

				//    instance.SdSoundSystem_RemoveData(bankID);
				//    Marshal.FreeHGlobal(intPtr);
				//}
				//catch
				//{
				//    instance.SdSoundSystem_RemoveData(bankID);
				//    Marshal.FreeHGlobal(intPtr);

				//    while (File.GetLastWriteTimeUtc(path) == writeTime)
				//        Thread.Sleep(1000);
				//}
				//finally
				//{
				//}
			}
		}
	}
}
