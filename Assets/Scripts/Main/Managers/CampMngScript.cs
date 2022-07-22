using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampMngScript : MonoBehaviour {
    static public CampMngScript     Inst { get; set; } = null;

    [SerializeField] GameObject[]   objectSpots;
    [SerializeField] Image          backGround;
    [SerializeField] Sprite         defaultBackGroundSprite;

    string                          objectSetName;
    string                          backGroundName;
    string                          BGM_Name;
    string                          BGM_defaultName;

    private void Awake() => Inst = this;

    private void Start() { BGM_defaultName = "Default BGM"; } //// 추후에 제목 고칠 것

    static void ResetObjectSpot() {
        foreach(var spot in Inst.objectSpots) {
            spot.SetActive(false);
            spot.GetComponent<SpriteRenderer>().sprite = null;
        }
        Inst.objectSetName = "";
    }

    static public void UseItem(Item _item) {
        if (_item.name == Inst.objectSetName) {
            ResetObjectSpot();
            return;
        }
        else if (_item.name == Inst.backGroundName) {
            Inst.backGround.sprite = Inst.defaultBackGroundSprite;
            return;
        }
        else if (_item.name == Inst.BGM_Name) {
            ////
            return;
        }

        if (_item.name == "테스트용 상자") {
            ResetObjectSpot();
            Inst.objectSetName = _item.name;
            Inst.objectSpots[0].SetActive(true);
            Inst.objectSpots[0].GetComponent<SpriteRenderer>().sprite = _item.sprite;
        }
    }
}
