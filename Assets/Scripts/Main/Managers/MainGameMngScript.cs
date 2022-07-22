using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MainGameMngScript : MonoBehaviour {
    static public MainGameMngScript Inst { get; set; } = null;

    [SerializeField] GameObject             mainSceneCanvas;
    [SerializeField] PanelScript            messagePanel;
    [SerializeField] Text                   dotoriNumText; 
    [SerializeField] GameObject             calander;
    [SerializeField] GameObject             travelNote;
    [SerializeField] GameObject             recipeBook;
    [SerializeField] GameObject             tayuBox;
    [SerializeField] GameObject             linBox;
    [SerializeField] GameObject             campingShop;

    bool                                    isUIActive;
    ReactiveProperty<int>                   dotoriNum = new ReactiveProperty<int>();
    int                                     stageNum = -1;

    static public GameObject                MainSceneCanvas => Inst.mainSceneCanvas;
    static public PanelScript               MessagePanel => Inst.messagePanel;
    static public ReactiveProperty<int>     DotoriNum => Inst.dotoriNum;
    static public int                       StageNum { set { Inst.stageNum = value; } }

    private void Awake() {
        Inst = this;
        dotoriNum.Value = 50; //// 테스트
    }
    private void Start() => dotoriNum.Subscribe(_dotoriNum => dotoriNumText.text = _dotoriNum.ToString());

    static public void SendStageNum() {
        CardGameMngScript.StageNum = Inst.stageNum;
        Inst.CloseEveryUIs();
        MainSceneCanvas.SetActive(false);
    }

    public void CloseEveryUIs() {
        isUIActive = false;
        calander.SetActive(false);
        travelNote.SetActive(false);
        recipeBook.SetActive(false);
        tayuBox.SetActive(false);
        linBox.SetActive(false);
        campingShop.SetActive(false);
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

    public void TayuBoxButton() {
        if (!isUIActive) {
            isUIActive = true;
            tayuBox.SetActive(true);
        }
        else if (tayuBox.activeSelf) {
            isUIActive = false;
            tayuBox.SetActive(false);
        }
    }

    public void LinBoxButton() {
        if (!isUIActive) {
            isUIActive = true;
            linBox.GetComponent<LinBoxMngScript>().Init();
            linBox.SetActive(true);
        }
        else if (linBox.activeSelf) {
            isUIActive = false;
            linBox.SetActive(false);
        }
    }

    public void CampingShopButton() {
        if (!isUIActive) {
            isUIActive = true;
            campingShop.SetActive(true);
        }
        else if (campingShop.activeSelf) {
            isUIActive = false;
            campingShop.SetActive(false);
        }
    }
}
