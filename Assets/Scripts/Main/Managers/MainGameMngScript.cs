using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MainGameMngScript : MonoBehaviour {
    static public MainGameMngScript Inst { get; set; } = null;

    [SerializeField] PanelScript            messagePanel;
    [SerializeField] Text                   dotoriNumText; 
    [SerializeField] GameObject             calander;
    [SerializeField] GameObject             travelNote;
    [SerializeField] GameObject             recipeBook;
    [SerializeField] GameObject             box;

    bool                                    isUIActive;
    ReactiveProperty<int>                   dotoriNum = new ReactiveProperty<int>();
    int                                     stageNum = -1;

    static public PanelScript               MessagePanel => Inst.messagePanel;
    static public ReactiveProperty<int>     DotoriNum => Inst.dotoriNum;
    static public int                       StageNum { set { Inst.stageNum = value; } }

    private void Awake() => Inst = this;
    private void Start() {
        dotoriNum.Subscribe(_dotoriNum => dotoriNumText.text = _dotoriNum.ToString());
        dotoriNum.Value = 50; //// 테스트
    }

    static public void SendStageNum() {
        CardGameMngScript.StageNum = Inst.stageNum;
        Destroy(Inst.gameObject);
    }


    public void CalanderButton() {
        if (!isUIActive) {
            isUIActive = true;
            calander.SetActive(true);
        }
        else if (calander.activeSelf) {
            isUIActive = false;
            calander.SetActive(false);
        }
    }

    public void TravelNoteButton() {
        if (!isUIActive) {
            isUIActive = true;
            travelNote.SetActive(true);
        }
        else if (travelNote.activeSelf) {
            isUIActive = false;
            travelNote.SetActive(false);
        }
    }

    public void RecipeBookButton() {
        if (!isUIActive) {
            isUIActive = true;
            recipeBook.SetActive(true);
        }
        else if (recipeBook.activeSelf) {
            isUIActive = false;
            recipeBook.SetActive(false);
        }
    }

    public void BoxButton() {
        if (!isUIActive) {
            isUIActive = true;
            box.SetActive(true);
        }
        else if (box.activeSelf) {
            isUIActive = false;
            box.SetActive(false);
        }
    }
}
