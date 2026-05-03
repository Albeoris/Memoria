using Assets.Scripts.Common;
using SiliconStudio;
using System;
using System.Text;
using UnityEngine;

public class CloudSaveDebug : MonoBehaviour
{
    private void Start()
    {
        SiliconStudio.Social.InitializeSocialPlatform();
        SiliconStudio.Social.Authenticate(false);
    }

    public void Back()
    {
        SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
    }

    public void ShowSelectSaveUIAndroid()
    {
    }

    public void CheckCloudAvailable()
    {
        global::Debug.Log("Is cloud active? => " + SiliconStudio.Social.IsCloudAvailable());
    }

    public void ButtonSave()
    {
        Byte[] localSaveData = this.GetLocalSaveData();
        TimeSpan playTime = new TimeSpan(1, 1, 1, 1);
        base.StartCoroutine(SiliconStudio.Social.Cloud_Save(this, localSaveData, playTime, new Action<Boolean, SiliconStudio.Social.ResponseData.Status>(this.CloudSaveCallBack)));
    }

    public void ButtonLoad()
    {
        base.StartCoroutine(SiliconStudio.Social.Cloud_Load(this, new Action<Byte[], SiliconStudio.Social.ResponseData.Status>(this.CloudLoadCallBack)));
    }

    public void ButtonCheckFileExist()
    {
        base.StartCoroutine(SiliconStudio.Social.Cloud_IsFileExist(this, new Action<Boolean, SiliconStudio.Social.ResponseData.Status>(this.CloudFileExistCallBack)));
    }

    private Byte[] GetLocalSaveData()
    {
        return Encoding.UTF8.GetBytes("CLOUD_DATA_TEST_" + DateTime.Now);
    }

    protected void CloudSaveCallBack(Boolean isSuccess, SiliconStudio.Social.ResponseData.Status status)
    {
        if (isSuccess)
        {
            global::Debug.Log("+++ CloudSaveCallBack");
        }
        global::Debug.Log("Result status:" + status);
    }

    protected void CloudLoadCallBack(Byte[] data, SiliconStudio.Social.ResponseData.Status status)
    {
        if (data == null)
        {
            return;
        }
        if (data.Length != 0)
        {
            global::Debug.Log("+++ CloudLoadCallBack : " + Encoding.UTF8.GetString(data));
        }
        global::Debug.Log("Result status: " + status);
    }

    protected void CloudFileExistCallBack(Boolean isExist, SiliconStudio.Social.ResponseData.Status status)
    {
        global::Debug.Log("+++ CloudFileExistCallBack : " + isExist);
        global::Debug.Log("Result status: " + status);
    }
}
