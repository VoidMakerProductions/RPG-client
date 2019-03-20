using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Talker : MonoBehaviour
{
    public static readonly string APIprefix = "https://demonroll.herokuapp.com/";

    public UserResponse User;
    public InputField username;
    public InputField password;
    public Text Greeting;
    public GameObject LoginPanel;
    public GameObject ChatPanel;
    public RoomListItem[] roomList;
    public Dropdown ChatSelect;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetAuth());
        
    }

    IEnumerator GetAuth() {
        using (UnityWebRequest request = UnityWebRequest.Get(APIprefix + "auth_state")) {
            if (PlayerPrefs.HasKey("Token")) {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();


            if (!request.isNetworkError) {
                //Debug.Log(request.downloadHandler.text);
                User = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);
                
                Greeting.text =User.is_authenticated ? "Greetings, " + User.username+" !" : "";
                
                LoginPanel.SetActive(!User.is_authenticated);
                ChatPanel.SetActive(User.is_authenticated);
            }
            
        }
    }


    public void Login() {
        //Debug.Log("Login attempt satrted!");
        StartCoroutine(LoginCoroutine(username.text, password.text));
    }

    public void SignUp() {
        StartCoroutine(SignUpCoroutine(username.text, password.text));
    }

    public void updateChats() {
        StartCoroutine(GetChatList());
    }


    IEnumerator GetChatList() {
        using (UnityWebRequest request = UnityWebRequest.Get(APIprefix + "chat/list")) {
            if (PlayerPrefs.HasKey("Token"))
            {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();

            if (!request.isNetworkError)
            {
                Debug.Log(request.downloadHandler.text);
                if (!request.isHttpError)
                {
                    
                    roomList = JsonUtility.FromJson<RoomList>(request.downloadHandler.text).data;
                    List<string> names = new List<string>();
                    foreach (var item in roomList) {
                        names.Add(item.room.name);
                    }
                    ChatSelect.AddOptions(names);
                }

            }
        }

    }




    IEnumerator SignUpCoroutine(string uname,string password) {
        WWWForm form = new WWWForm();
        form.AddField("username", uname);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix + "signup", form)) {

            yield return request.SendWebRequest();

            if (!request.isNetworkError) {
                if (!request.isHttpError) {
                    StartCoroutine(LoginCoroutine(uname, password));
                }

            }

        }
    }
    IEnumerator LoginCoroutine(string uname, string password) {
        WWWForm form = new WWWForm();
        form.AddField("username", uname);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix+"login",form)) {

            yield return request.SendWebRequest();

            if (!request.isNetworkError) {
                //Debug.Log(request.downloadHandler.text);
                if (!request.isHttpError) {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                    PlayerPrefs.SetString("Token", response.token);
                    StartCoroutine(GetAuth());
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class RoomList {
    public RoomListItem[] data;
}
[System.Serializable]
public class RoomListItem {
    public Room room;
}

[System.Serializable]
public class Room {
    public int id;
    public string name;
}

[System.Serializable]
public class LoginResponse {
    public string token;
    public string status;
    public string error;
}

[System.Serializable]
public class UserResponse {
    public int id;
    public bool is_authenticated;
    public string username;
    public int max_games_played;
    public int max_games_ruled;
}

