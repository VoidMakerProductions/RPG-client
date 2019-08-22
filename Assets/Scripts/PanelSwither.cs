using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSwither : MonoBehaviour
{
    public GameObject[] panels;
    public Button[] buttons;
    public int Selected { get; private set; }
    public bool[] master_restricted;
    public Talker talker;

    public void SwitchTo(int panelIndex) {
        Selected = panelIndex;
        for (int i = 0; i < panels.Length; i++) {
            panels[i].SetActive(panelIndex == i);
            buttons[i].interactable = panelIndex != i && (!master_restricted[i] || talker.User.is_master );
        }
    }
}
