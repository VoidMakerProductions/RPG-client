using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharManager : MonoBehaviour
{
    Character character;

    public TextMeshProUGUI name_t,intel,will,attr,dex,str,end,sum;
    public TMP_InputField name_if;
    public Button[] statchangers;
    public Button sndbutton;

    public bool CanChangeStats { get {
            return statchangers[0].interactable;
        } set {
            foreach (Button b in statchangers) {
                b.interactable = value;
            }
            name_if.interactable = value;
        } }


    public WWWForm formFromCharacter() {
        WWWForm form = new WWWForm();
        form.AddField("name", character.name);
        form.AddField("intel", character.intel);
        form.AddField("will", character.will);
        form.AddField("attr", character.attr);
        form.AddField("dex", character.dex);
        form.AddField("str", character.str);
        form.AddField("end", character.end);
        form.AddField("nat_blade_res", character.nat_blade_res);
        form.AddField("nat_pierce_res", character.nat_pierce_res);
        form.AddField("nat_blunt_res", character.nat_blunt_res);
        form.AddField("nat_fire_res", character.nat_fire_res);
        form.AddField("nat_cold_res", character.nat_cold_res);
        form.AddField("nat_elec_res", character.nat_elec_res);
        form.AddField("nat_acid_res", character.nat_acid_res);

        return form;
    }


    public void loadCharacter(Character character) {
        this.character = character;
        Display();
    }

    void Display() {
        name_t.text = character.name;
        intel.text = character.intel.ToString();
        will.text = character.will.ToString();
        attr.text = character.attr.ToString();
        dex.text = character.dex.ToString();
        str.text = character.str.ToString();
        end.text = character.end.ToString();
        sum.text = character.stat_sum.ToString();
    }

    public void changeName(string name) {
        sndbutton.interactable = !string.IsNullOrWhiteSpace(name);
        character.name = name;
        Display();
    }

    public void newChar() {
        character = new Character();
        CanChangeStats = true;
        Display();
    }

    public void increaseStat(string stat) {
        switch (stat) {
            case "intel":
                character.intel++;
                break;
            case "will":
                character.will++;
                break;
            case "attr":
                character.attr++;
                break;
            case "dex":
                character.dex++;
                break;
            case "str":
                character.str++;
                break;
            case "end":
                character.end++;
                break;
            case "nat_blade_res":
                character.nat_blade_res++;
                break;
            case "nat_pierce_res":
                character.nat_pierce_res++;
                break;
            case "nat_blunt_res":
                character.nat_blunt_res++;
                break;
            case "nat_fire_res":
                character.nat_fire_res++;
                break;
            case "nat_cold_res":
                character.nat_cold_res++;
                break;
            case "nat_elec_res":
                character.nat_elec_res++;
                break;
            case "nat_acid_res":
                character.nat_acid_res++;
                break;
        }
        Display();
    }

    public void decreaseStat(string stat) {
        switch (stat) {
            case "intel":
                character.intel--;
                break;
            case "will":
                character.will--;
                break;
            case "attr":
                character.attr--;
                break;
            case "dex":
                character.dex--;
                break;
            case "str":
                character.str--;
                break;
            case "end":
                character.end--;
                break;
            case "nat_blade_res":
                character.nat_blade_res--;
                break;
            case "nat_pierce_res":
                character.nat_pierce_res--;
                break;
            case "nat_blunt_res":
                character.nat_blunt_res--;
                break;
            case "nat_fire_res":
                character.nat_fire_res--;
                break;
            case "nat_cold_res":
                character.nat_cold_res--;
                break;
            case "nat_elec_res":
                character.nat_elec_res--;
                break;
            case "nat_acid_res":
                character.nat_acid_res--;
                break;
        }
        Display();
    }
    
}
