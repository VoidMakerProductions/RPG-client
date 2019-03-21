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
    public Text ChatText;
    bool canManageRoom;
    [SerializeField]
    int currentRoom = 0;
    int currentIndex;
    float nextUpdate;
    public InputField InviteField;
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
                if (!request.isHttpError)
                {
                    User = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);

                    Greeting.text = User.is_authenticated ? "Greetings, " + User.username + " !" : "";

                    LoginPanel.SetActive(!User.is_authenticated);
                    ChatPanel.SetActive(User.is_authenticated);
                    if (User.is_authenticated) {
                        updateChats();
                    }
                }
                else {
                    LoginPanel.SetActive(true);
                    ChatPanel.SetActive(false);
                }
                
            }
            else
            {
                LoginPanel.SetActive(true);
                ChatPanel.SetActive(false);
            }

        }
    }

    public void SendChatMessage(string message) {
        StartCoroutine(SendMessageCoroutine(message));
    }
    public void ChangeRoom(int index) {
        currentRoom = roomList[index].room.id;
        canManageRoom = roomList[index].room.owner == User.id;
        currentIndex = index;
    }

    public void Login() {
        //Debug.Log("Login attempt satrted!");
        if (ValidateCreds()) StartCoroutine(LoginCoroutine(username.text, password.text));
    }

    public void SignUp() {
        if (ValidateCreds()) StartCoroutine(SignUpCoroutine(username.text, password.text));
    }

    bool ValidateCreds() {
        return (!string.IsNullOrWhiteSpace(username.text)) && (!string.IsNullOrEmpty(password.text));
    }


    public void updateChats() {
        StartCoroutine(GetChatList());
    }

    public void Invite(string name) {
        if (!string.IsNullOrWhiteSpace(name)) StartCoroutine(InviteCoroutine(name));
    }

    public void CreateRoom(string name) {
        if (!string.IsNullOrWhiteSpace(name)) StartCoroutine(CreateRoomCoroutine(name));
    }

    IEnumerator CreateRoomCoroutine(string name) {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix + "chat/create", form))
        {
            if (PlayerPrefs.HasKey("Token"))
            {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();

            StartCoroutine(GetChatList());
        }

    }

    IEnumerator InviteCoroutine(string name) {
        WWWForm form = new WWWForm();
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix + "chat"+currentRoom.ToString()+"/invite/" + name, form)) {
            if (PlayerPrefs.HasKey("Token"))
            {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();

            if (!request.isNetworkError)
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }

    IEnumerator UpdateCurrentChat() {
        using (UnityWebRequest request = UnityWebRequest.Get(APIprefix + "chat/" + currentRoom.ToString())) {
            if (PlayerPrefs.HasKey("Token"))
            {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();

            if (!request.isNetworkError) {
                //Debug.Log(request.downloadHandler.text);
                if (!request.isHttpError) {
                    ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    string msg = "";
                    foreach (var item in response.data) {
                        msg += item.when;
                        msg += "\n";
                        msg += "<b>" + item.author.username + "</b>: ";
                        msg += item.text + "\n";
                    }
                    ChatText.text = msg;
                }
            }
        }
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
                //Debug.Log(request.downloadHandler.text);
                if (!request.isHttpError)
                {
                    
                    roomList = JsonUtility.FromJson<RoomList>(request.downloadHandler.text).data;
                    List<string> names = new List<string>();
                    foreach (var item in roomList) {
                        names.Add(item.room.name);
                    }
                    ChatSelect.ClearOptions();
                    ChatSelect.AddOptions(names);
                    if (names.Count > 0) {
                        try
                        {
                            currentRoom = roomList[currentIndex].room.id;
                            canManageRoom = roomList[currentIndex].room.owner == User.id;
                            
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            currentRoom = roomList[0].room.id;
                            canManageRoom = roomList[0].room.owner == User.id;
                            currentIndex = 0;
                        }
                    }
                    
                }

            }
        }

    }

    IEnumerator SendMessageCoroutine(string message) {
        WWWForm form = new WWWForm();
        form.AddField("text", message);
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix + "chat/" + currentRoom.ToString(), form)) {
            if (PlayerPrefs.HasKey("Token"))
            {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();

            if (!request.isNetworkError)
            {
                //Debug.Log(request.downloadHandler.text);
                if (!request.isHttpError)
                {
                    ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                    string msg = "";
                    foreach (var item in response.data)
                    {
                        msg += item.when;
                        msg += "\n";
                        msg += "<b>" + item.author.username + "</b>: ";
                        msg += item.text + "\n";
                    }
                    ChatText.text = msg;
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
        InviteField.interactable = canManageRoom;
        if (currentRoom > 0 && Time.time > nextUpdate) {
            nextUpdate = Time.time + 0.2f;
            StartCoroutine(UpdateCurrentChat());
        }
    }
}


[System.Serializable]
public class ChatResponse {
    public ChatMsg[] data;
}

[System.Serializable]
public class ChatMsg {
    public string text;
    public string when;
    public UserResponse2 author;
    public int id;
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
    public int owner;
}

[System.Serializable]
public class LoginResponse {
    public string token;
    public string status;
    public string error;
}


[System.Serializable]
public class UserResponse2
{
    public int id;
    public string username;
    public int max_games_played;
    public int max_games_ruled;
}
[System.Serializable]
public class UserResponse
{
    public int id;
    public bool is_authenticated;
    public string username;
    public int max_games_played;
    public int max_games_ruled;
}

