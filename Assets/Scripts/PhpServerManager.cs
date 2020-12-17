using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class PhpServerManager : MonoBehaviour
{
    public string m_downloadPath { get; set; } = string.Empty;
    public string m_uploadPath { get; set; } = string.Empty;

    public event UploadCallback eventUploadCallback;
    public delegate void UploadCallback();

    private string m_uploadUrl = string.Empty;

    private void Start()
    {
        m_downloadPath = Path.Combine(Application.streamingAssetsPath, "download");
        m_uploadPath = Path.Combine(Application.streamingAssetsPath, "upload");
    }

    public string UploadComplete()
    {
        return m_uploadUrl;
    }

    public IEnumerator Download(string downloadloc)
    {
        var uri = new Uri(downloadloc);
        var path = Path.Combine(m_downloadPath, Path.GetFileName(uri.LocalPath));
        if (File.Exists(path))
        {
            Debug.Log("[File exist] " + path);
            yield return null;
        }
        else
        {
            var uwr = new UnityWebRequest(downloadloc)
            {
                method = UnityWebRequest.kHttpVerbGET
            };

            var dh = new DownloadHandlerFile(path)
            {
                removeFileOnAbort = true
            };

            uwr.downloadHandler = dh;
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                Debug.Log("[Process]" + uwr.downloadProgress);
                yield return null;
            }

            if (uwr.isNetworkError || uwr.isHttpError)
                Debug.Log("[Error]，" + uwr.error);
            else
                Debug.Log("[Download success!]");
        }
    }

    //public IEnumerator Upload(string uloadurl, string serverUploadPhp = "https://shinnyid123.000webhostapp.com/test/api/upload.php")
    //{
    //    byte[] myData = System.Text.Encoding.UTF8.GetBytes(uloadurl);
    //    UnityWebRequest uwr = UnityWebRequest.Put(serverUploadPhp, myData);
    //    yield return uwr.SendWebRequest();

    //    if (uwr.isNetworkError || uwr.isHttpError)
    //        Debug.Log("[Error]" + uwr.error);
    //    else
    //    {
    //        Debug.Log("[Upload complete!]");
    //        m_uploadUrl = Path.Combine(Path.GetDirectoryName(serverUploadPhp), "upload", Path.GetFileName(uloadurl));
    //        eventUploadCallback?.Invoke();                     // net 4.0
    //    }
    //}

    public IEnumerator Upload(string uloadurl, string serverUploadPhp = "https://shinnyid123.000webhostapp.com/test/api/upload.php")
    {
        WWW localFile = new WWW("file:///" + uloadurl);
        yield return localFile;

        if (localFile.error == null)
            Debug.Log("[Loaded success!]");
        else
        {
            Debug.Log("[Error]" + localFile.error);
            yield break; // stop the coroutine here
        }
        WWWForm postForm = new WWWForm();
        // version 1
        //postForm.AddBinaryData("theFile",localFile.bytes);
        // version 2
        postForm.AddBinaryData("theFile", localFile.bytes, uloadurl, "text/plain");
        WWW upload = new WWW(serverUploadPhp, postForm);

        yield return upload;

        if (upload.error == null)
        {
            Debug.Log("[Upload done]" + upload.text);
            m_uploadUrl = Path.Combine(Path.GetDirectoryName(serverUploadPhp), "upload", Path.GetFileName(uloadurl));
            eventUploadCallback?.Invoke();                     // net 4.0
        }
        else
            Debug.Log("[Error]" + upload.error);
    }    
}
