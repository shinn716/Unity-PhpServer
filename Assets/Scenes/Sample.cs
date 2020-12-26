using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public PhpServerManager phpServerManager;
    public RawImage rimg_qrcode;

    void Start()
    {
        //rimg_qrcode.texture = Shinn.QRcodeHelper.GetQRcodeTexture("12345");
        phpServerManager.eventUploadCallback += UploadEvent;
    }

    private void OnApplicationQuit()
    {
        phpServerManager.eventUploadCallback -= UploadEvent;
    }

    [ContextMenu("DownloadTest")]
    void DownloadTest()
    {
        StartCoroutine(phpServerManager.Download("http://psquare.io/test/api/upload/01.jpg"));
    }

    [ContextMenu("UploadTest")]
    void UploadTest()
    {
        rimg_qrcode.gameObject.transform.GetChild(0).GetComponent<Text>().text = "LOADING...";
        var file = Path.Combine(phpServerManager.m_uploadPath, "03.jpg");
        StartCoroutine(phpServerManager.Upload(file, "http://psquare.io/test/api/upload.php"));
    }

    void UploadEvent()
    {
        if (phpServerManager.UploadComplete() != null)
        {
            print("GetUploadEvent complete! " + phpServerManager.UploadComplete());
            rimg_qrcode.texture = Shinn.QRcodeHelper.GetQRcodeTexture(phpServerManager.UploadComplete());
            rimg_qrcode.gameObject.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
        }
    }
}
