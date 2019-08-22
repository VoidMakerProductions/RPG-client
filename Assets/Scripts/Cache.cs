using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class Cache : MonoBehaviour
{
    static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    
    public static Cache instance;

    private void Awake()
    {
        instance = this;
        
    }

    public Sprite GetSprite(string url, System.Action<Sprite> readyCallback = null) {
        try{
            if (url==null)
                return null;
            return sprites[url];
        }
        catch (KeyNotFoundException) {
            if (File.Exists(butcher(url))) {
                //Debug.Log("File exists");
                Texture2D tex = new Texture2D(1,1);
                tex.LoadImage(File.ReadAllBytes(butcher(url)));
                sprites[url] = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero,tex.width/64f);
                return sprites[url];
            }

            StartCoroutine(LoadSprite(url, readyCallback));
            return null;
        }
        
    }

    IEnumerator LoadSprite(string url, System.Action<Sprite> readyCallback) {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();
            if (!request.isNetworkError && !request.isHttpError)
            {
                if (!Directory.Exists(Application.persistentDataPath + "/imgcache/"))
                    Directory.CreateDirectory(Application.persistentDataPath + "/imgcache/");
               
                Texture2D tex = DownloadHandlerTexture.GetContent(request);
                //Debug.Log(Directory.Exists(Application.persistentDataPath + "/imgcache/"));

                File.WriteAllBytes(butcher(url), request.downloadHandler.data);

                sprites[url] = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero, tex.width / 64f);
                readyCallback?.Invoke(sprites[url]);
            }

        }
    }

    public void Clear() {
        Directory.Delete(Application.persistentDataPath + "/imgcache/",true);
        sprites = new Dictionary<string, Sprite>();
    }

    string butcher(string url) {
        string ext = "";
        
        if (url.ToLower().Contains(".jpg"))
            ext = ".jpg";
        if (url.ToLower().Contains(".png"))
            ext = ".png";
        string stripped = url.Replace(@"/", string.Empty)
            .Replace(".", string.Empty)
            .Replace(":", string.Empty)
            .Replace("?", string.Empty);
        int start = Mathf.Max(0, stripped.Length - 24);
        stripped = stripped.Substring(start);
        return Application.persistentDataPath + "/imgcache/"
            +stripped
            +ext;
    }
}


