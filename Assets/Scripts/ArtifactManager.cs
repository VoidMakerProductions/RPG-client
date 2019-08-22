using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TMP_InputField inputField;
    public TMP_Dropdown dmgtype;
    public TMP_Dropdown dropdown;
    public TMP_Dropdown artSelect;
    public Toggle toggle;
    public CharManager @char;
    [SerializeField]
    public bool is_new;
    public string[] damts;
    [SerializeField]
    Artifact artifact;
    static string[] excluded = {"approved" };
    public Artifact Artifact {
        get {
            return artifact;
        }
        set {
            is_new = false;
            artifact = value;
            Display();
        }
    }
    public void NewArtifact() {
        Artifact = new Artifact();
        is_new = true;
        Artifact.name = "";
        Artifact.dmg_type = "bld";
    }
    void Display() {
        text.text = artifact.repr();
    }

    public void refreshEquip(ICollection<Artifact> artSet) {
        artSelect.options.Clear();
        artSelect.options.Add(new TMP_Dropdown.OptionData("Новый артефакт"));
        artSelect.options.AddRange(from art in artSet select new TMP_Dropdown.OptionData(art.name));
        if (artSet.Count == 0) {
            NewArtifact();
        }
        SetParamInput();
    }
   
    public WWWForm formFromArt() {
        WWWForm form = new WWWForm();
        
        foreach (var f in artifact.GetType().GetFields()) {
            if (!excluded.Contains(f.Name)) {
                form.AddField(f.Name, f.GetValue(artifact).ToString());
            }
        }

        return form;
    }

    public void selectArt(int select) {
        if (select == 0)
        {
            NewArtifact();
        }
        else {
            Artifact = @char.Character.equip[--select];
            is_new = false;
        }

    }


    public void SetParamInput() {
        switch (dropdown.value) {
            case 1:
                toggle.gameObject.SetActive(true);
                dmgtype.gameObject.SetActive(false);
                inputField.gameObject.SetActive(false);
                break;
            case 5:
                toggle.gameObject.SetActive(false);
                dmgtype.gameObject.SetActive(true);
                inputField.gameObject.SetActive(false);
                break;
            default:
                toggle.gameObject.SetActive(false);
                dmgtype.gameObject.SetActive(false);
                inputField.gameObject.SetActive(true);
                break;
        }
    }


    public void SetParam() {

        switch (dropdown.value) {
            case 0:
                artifact.name = inputField.text;
                break;
            case 1:
                artifact.weapon = toggle.isOn;
                break;
            case 2:
                int.TryParse(inputField.text, out artifact.base_dmg);
                break;
            case 3:
                int.TryParse(inputField.text, out artifact.base_prec);
                break;
            case 4:
                int.TryParse(inputField.text, out artifact.range);
                break;
            case 5:
                artifact.dmg_type = damts[dmgtype.value];
                break;
            case 6:
                int.TryParse(inputField.text, out artifact.hp);
                break;
            case 7:
                int.TryParse(inputField.text, out artifact.blade_res);
                break;
            case 8:
                int.TryParse(inputField.text, out artifact.pierce_res);
                break;
            case 9:
                int.TryParse(inputField.text, out artifact.blunt_res);
                break;
            case 10:
                int.TryParse(inputField.text, out artifact.fire_res);
                break;
            case 11:
                int.TryParse(inputField.text, out artifact.cold_res);
                break;
            case 12:
                int.TryParse(inputField.text, out artifact.elec_res);
                break;
            case 13:
                int.TryParse(inputField.text, out artifact.acid_res);
                break;
        }
        Display();
    }

}
