using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Talker : MonoBehaviour
{
    public static string APIprefix = "http://172.22.10.10/";

    public UserResponse User;
    public InputField username;
    public InputField password;
    public Text Greeting;
    public GameObject LoginPanel;
    public GameObject GamePanel;
    public PanelSwither swither;
    public CharManager charManager;
    public RoomListItem[] roomList;
    public Dropdown ChatSelect;
    public TextMeshProUGUI ChatText;
    public TextMeshProUGUI ICChatText;
    int selectedCharacter;
    public Character[] myChars;
    bool canManageRoom;
    [SerializeField]
    int currentRoom = 0;
    int currentCharIndex;
    float nextUpdate;
    public InputField InviteField;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(GetAuth());
        
    }

    IEnumerator sendNewChar()
    {
        WWWForm form = charManager.formFromCharacter();
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix + "char/new/", form))
        {
            if (PlayerPrefs.HasKey("Token"))
            {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();

            if (!request.isNetworkError) {
                if (!request.isHttpError) {
                    StartCoroutine(GetMyChars());
                }
            }
        }
    }

    public void SendNewChar() {
        charManager.CanChangeStats = false;
        StartCoroutine(sendNewChar());
    }

    public void NextChar() {
        StartCoroutine(nextChar());
    }

    public void PrevChar() {
        StartCoroutine(prevChar());
    }
    IEnumerator nextChar() {
        yield return GetMyChars();
        charManager.CanChangeStats = false;
        currentCharIndex++;
        currentCharIndex = currentCharIndex >= myChars.Length ? 0 : currentCharIndex;
        try
        {
            charManager.loadCharacter(myChars[currentCharIndex]);
            selectedCharacter = myChars[currentCharIndex].id;
        }
        catch (System.IndexOutOfRangeException)
        {
            charManager.newChar();
        }
    }
    IEnumerator prevChar()
    {
        yield return GetMyChars();
        charManager.CanChangeStats = false;
        currentCharIndex--;
        currentCharIndex = currentCharIndex < 0  ? myChars.Length-1 : currentCharIndex;
        try
        {
            charManager.loadCharacter(myChars[currentCharIndex]);
            selectedCharacter = myChars[currentCharIndex].id;
        }
        catch (System.IndexOutOfRangeException)
        {
            charManager.newChar();
        }
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
                    Debug.Log(request.downloadHandler.text); 
                    Greeting.text = User.is_authenticated ? "Greetings, " + User.username + " !" : "";

                    LoginPanel.SetActive(!User.is_authenticated);
                    GamePanel.SetActive(User.is_authenticated);
                    if (User.is_authenticated) {
                        StartCoroutine(UpdateCurrentChat());
                    }
                }
                else {
                    
                    LoginPanel.SetActive(true);
                    GamePanel.SetActive(false);
                }
                
            }
            else
            {
                Debug.Log(request.error);
                LoginPanel.SetActive(true);
                GamePanel.SetActive(false);
            }

        }
    }

    public void SendChatMessage(string message) {
        switch (swither.Selected) {
            case 0:
                StartCoroutine(SendMessageCoroutine(message));
                break;
            case 1:
                StartCoroutine(SendICMessageCoroutine(message));
                break;
        }
        
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

    bool ValidateCharName() {
        return (!string.IsNullOrWhiteSpace(charManager.name_if.text));
    }



    IEnumerator GetMyChars() {
        using (UnityWebRequest request = UnityWebRequest.Get(APIprefix + "char/mylist/")) {
            if (PlayerPrefs.HasKey("Token"))
            {
                request.SetRequestHeader("Authorization", "Token " + PlayerPrefs.GetString("Token"));
            }
            yield return request.SendWebRequest();

            if (!request.isNetworkError) {
                if (!request.isHttpError) {
                    myChars = JsonUtility.FromJson<CharResponse>(request.downloadHandler.text).data;
                }
            }
        }

    }


    IEnumerator UpdateICChat()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(APIprefix + "ictalk/"))
        {
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
                    ICChatResponse response = JsonUtility.FromJson<ICChatResponse>(request.downloadHandler.text);
                    string msg = "";
                    foreach (var item in response.data)
                    {
                        msg += item.timestamp;
                        msg += "\n";
                        msg += "<b>" + item.author.name + "</b>: ";
                        msg += item.text + "\n";
                    }
                    ICChatText.text = msg;
                }
            }
        }

    }
    IEnumerator UpdateCurrentChat() {
        using (UnityWebRequest request = UnityWebRequest.Get(APIprefix + "chat/" )) {
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
                        msg += item.timestamp;
                        msg += "\n";
                        msg += "<b>" + item.author.username + "</b>: ";
                        msg += item.text + "\n";
                    }
                    ChatText.text = msg;
                }
            }
        }
    }

    IEnumerator SendMessageCoroutine(string message) {
        WWWForm form = new WWWForm();
        form.AddField("text", message);
        
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix + "chat/", form)) {
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
                    ICChatResponse response = JsonUtility.FromJson<ICChatResponse>(request.downloadHandler.text);
                    string msg = "";
                    foreach (var item in response.data)
                    {
                        msg += item.timestamp;
                        msg += "\n";
                        msg += "<b>" + item.author.name + "</b>: ";
                        msg += item.text + "\n";
                    }
                    ICChatText.text = msg;
                }
            }

        }

    }


    IEnumerator SendICMessageCoroutine(string message)
    {
        WWWForm form = new WWWForm();
        form.AddField("text", message);
        form.AddField("char", selectedCharacter);
        using (UnityWebRequest request = UnityWebRequest.Post(APIprefix + "ictalk/", form))
        {
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
                        msg += item.timestamp;
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
                if (!request.isHttpError)
                {
                    LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
                    Debug.Log(response.token);
                    PlayerPrefs.SetString("Token", response.token);
                    StartCoroutine(GetAuth());
                }
                else {
                    Debug.Log(request.error);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        if (Time.time > nextUpdate) {
            nextUpdate = Time.time + 0.2f;
            switch (swither.Selected) {
                case 0:
                    StartCoroutine(UpdateCurrentChat());
                    break;
                case 1:
                    StartCoroutine(UpdateICChat());
                    break;
            }
            
        }
    }
}



[System.Serializable]
public class ICChatResponse
{
    public ICChatMsg[] data;
}

[System.Serializable]
public class ICChatMsg
{
    public string text;
    public string timestamp;
    public CharResponse2 author;
    public int id;
}

[System.Serializable]
public class CharResponse2
{
    public int id;
    public string name;
}

[System.Serializable]
public class CharResponse {
    public Character[] data;
}


[System.Serializable]
public class Character {
    public int id;
    public string name;
    public int player;
    public int intel;
    public int will;
    public int attr;
    public int dex;
    public int str;
    public int end;
    public int nat_blade_res;
    public int nat_pierce_res;
    public int nat_blunt_res;
    public int nat_fire_res;
    public int nat_cold_res;
    public int nat_elec_res;
    public int nat_acid_res;
    public int ap;
    public Artifact[] equip;

    public int stat_sum {
        get {
            return intel + will + attr + dex + str + end;
        }
    }


    public int MaxHP { get {
            int res = 30 + end * 5;
            foreach (Artifact artifact in equip) {
                res += artifact.hp;
            }
            return res;
        } }

    public int blade_res
    {
        get
        {
            int res = nat_blade_res;
            foreach (Artifact artifact in equip)
            {
                res += artifact.blade_res;
            }
            return res;
        }
    }
    public int pierce_res
    {
        get
        {
            int res = nat_pierce_res;
            foreach (Artifact artifact in equip)
            {
                res += artifact.pierce_res;
            }
            return res;
        }
    }
    public int blunt_res
    {
        get
        {
            int res = nat_blunt_res;
            foreach (Artifact artifact in equip)
            {
                res += artifact.blunt_res;
            }
            return res;
        }
    }
    public int fire_res
    {
        get
        {
            int res = nat_fire_res;
            foreach (Artifact artifact in equip)
            {
                res += artifact.fire_res;
            }
            return res;
        }
    }
    public int cold_res
    {
        get
        {
            int res = nat_cold_res;
            foreach (Artifact artifact in equip)
            {
                res += artifact.cold_res;
            }
            return res;
        }
    }
    public int acid_res
    {
        get
        {
            int res = nat_acid_res;
            foreach (Artifact artifact in equip)
            {
                res += artifact.acid_res;
            }
            return res;
        }
    }
    public int elec_res
    {
        get
        {
            int res = nat_elec_res;
            foreach (Artifact artifact in equip)
            {
                res += artifact.elec_res;
            }
            return res;
        }
    }
}


[System.Serializable]
public class Artifact {
    public int blade_res;
    public int pierce_res;
    public int blunt_res;
    public int fire_res;
    public int cold_res;
    public int elec_res;
    public int acid_res;
    public int hp;
    public bool weapon;
    public string dmg_type;
    public int base_dmg;
    public int ap_cost;
    public int base_prec;
}

[System.Serializable]
public class ChatResponse {
    public ChatMsg[] data;
}

[System.Serializable]
public class ChatMsg {
    public string text;
    public string timestamp;
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
}
[System.Serializable]
public class UserResponse
{
    public int id;
    public bool is_authenticated;
    public string username;
}

[System.Serializable]
public class IPResponse {
    public string ip;
}