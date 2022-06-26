using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class CardGameMngScript : MonoBehaviour {
    public static CardGameMngScript                                                         Inst { get; private set; }
    void Awake() => Inst = this;
    
    [SerializeField] [Tooltip("게임의 속도가 매우 빨라집니다")] bool                        fastMode;
    [SerializeField] [Tooltip("시작 카드 개수를 정합니다")] int                             startCardCount;
    [SerializeField] [Tooltip("패에 들고 있을 수 있는 최대 카드 개수를 정합니다")] int      maxCardCount;
    [SerializeField] [Tooltip("첫 턴에 놓을 수 있는 카드의 개수를 정합니다")] int           startPutCardCount;
    [SerializeField] [Tooltip("놓을 수 있는 카드의 최대 개수를 정합니다")] int              maxPutCardCount;

    [SerializeField] TurnStartPanelScript                                                   turnStartPanel;
    [SerializeField] TMP_Text                                                               turnNumText;
    [SerializeField] TMP_Text                                                               currentStageInfoText;
    [SerializeField] PanelScript                                                            cardExplainPanel;


    [Header("게임 시스템 변수")]
    public bool                                                                             isLoading;
    public List<bool>                                                                       isCoroutine = new List<bool>();

    bool                                                                                    myTurn;
    Dictionary<string, string>                                                              menuInfo = new Dictionary<string, string>();
    Dictionary<string, int>                                                                 stageInfo = new Dictionary<string, int>();
    Dictionary<string, int>                                                                 currentStageInfo = new Dictionary<string, int>();
    string                                                                                  recipeString;
    int                                                                                     stageNum = -1;
    int                                                                                     maxTurnNum;
    int                                                                                     turnNum = 0;

    static public int                                                                       MaxCardCount => Inst.maxCardCount;
    static public int                                                                       StartPutCardCount => Inst.startPutCardCount;
    static public int                                                                       MaxPutCardCount => Inst.maxPutCardCount;
    static public bool                                                                      MyTurn => Inst.myTurn;
    static public Dictionary<string, int>                                                   StageInfo => Inst.stageInfo;
    static public Dictionary<string, int>                                                   CurrentStageInfo => Inst.currentStageInfo;
    static public PanelScript                                                               CardExplainPanel => Inst.cardExplainPanel;
    static public int StageNum {
        get => Inst.stageNum;
        set { Inst.stageNum = value; }
    }

    public static List<bool> IsCoroutine => Inst.isCoroutine;

    void Start() {
        MainGameMngScript.SendStageNum();
        StartGame();
    }

    void Update() {
#if UNITY_EDITOR
        CheatKey();
#endif
    }

    private void OnDestroy() {
        if (fastMode) {
            Utils.cardDrawDotweenTime /= Utils.fastModeFloat;
            Utils.cardDrawExtraTime /= Utils.fastModeFloat;
        }
    }

    static public void CurrentStageInfoTextSet() {
        string text = "";
        foreach (var keyValue in Inst.stageInfo)
            text += Inst.menuInfo[keyValue.Key] + " : " + Inst.currentStageInfo[keyValue.Key] + "/" + Inst.stageInfo[keyValue.Key] + "\n";
        Inst.currentStageInfoText.text = text;
    }

    void                StartGame() => StartCoroutine(StartGameCo());
    void                StageClear() => turnStartPanel.Show();
    public void         ShowRecipeString() => CardExplainPanel.Show(recipeString);

    void CheatKey() {
        if (Input.GetKeyDown(KeyCode.O))
            CardMngScript.AddCardItem();
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(EndTurnCo());
    }

    void GameSetup(int _stageNum) {
        if (fastMode) {
            Utils.cardDrawDotweenTime *= Utils.fastModeFloat;
            Utils.cardDrawExtraTime *= Utils.fastModeFloat;
        }
        Init(_stageNum);
        CardMngScript.Init(_stageNum);
    }

    void Init(int _stageNum) {
        switch (_stageNum) {
            case 0:
                // 밥 1개 만들기, 제한 50턴
                menuInfo.Add("", "밥");
                stageInfo.Add("", 1);
                currentStageInfo.Add("", 0);
                maxTurnNum = 50;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "밥 짓기의 기본!\n" +
                    "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                    "주재료 : 쌀\n\n" +
                    "재료와 도구를 배치하는 법!.\n" +
                    "1. 카드를 상단의 공간에 드래그 해 놓는다.\n" +
                    "2. 턴 종료 시 왼쪽 위부터 차례대로 재료와 도구가 배치된다.";
                break;
            case 1:
                // 밥 2개 만들기, 제한 50턴
                menuInfo.Add("", "밥");
                stageInfo.Add("", 2);
                currentStageInfo.Add("", 0);
                maxTurnNum = 50;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "밥 짓기의 기본!\n" +
                    "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                    "주재료 : 쌀\n\n" +
                    "재료와 도구를 배치하는 법!.\n" +
                    "1. 카드를 상단의 공간에 드래그 해 놓는다.\n" +
                    "2. 턴 종료 시 왼쪽 위부터 차례대로 재료와 도구가 배치된다.";
                break;
            case 2:
                // 밥 2개 만들기, 제한 30턴
                menuInfo.Add("", "밥");
                stageInfo.Add("", 2);
                currentStageInfo.Add("", 0);
                maxTurnNum = 30;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "밥 짓기의 기본!\n" +
                    "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                    "주재료 : 쌀\n\n" +
                    "재료와 도구를 배치하는 법!.\n" +
                    "1. 카드를 상단의 공간에 드래그 해 놓는다.\n" +
                    "2. 턴 종료 시 왼쪽 위부터 차례대로 재료와 도구가 배치된다.";
                break;
            case 3:
                // 맛있는밥 1개 만들기, 제한 30턴
                menuInfo.Add("", "맛있는 밥");
                stageInfo.Add("", 1);
                currentStageInfo.Add("", 0);
                maxTurnNum = 30;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "맛있는 밥을 짓는 법!\n" +
                    "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                    "주재료 : 쌀\n" +
                    "조미료 : 도토리주\n\n" +
                    "조미로를 만드는 법!.\n" +
                    "1. 밥솥, 쌀, 불지피기를 뒤짚어 배치해 도토리솥, 도토리, 도토리 굽기를 얻는다.\n" +
                    "2. 도토리솥에 도토리를 넣고 불을 올린다.";
                break;
            case 4:
                // 맛있는밥 1개 만들기, 제한 50턴
                menuInfo.Add("", "맛있는 밥");
                stageInfo.Add("", 1);
                currentStageInfo.Add("", 0);
                maxTurnNum = 50;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "맛있는 밥을 짓는 법!\n" +
                    "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                    "주재료 : 쌀\n" +
                    "조미료 : 도토리주\n\n" +
                    "조미로를 만드는 법!.\n" +
                    "1. 밥솥, 쌀, 불지피기를 뒤짚어 배치해 도토리솥, 도토리, 도토리 굽기를 얻는다.\n" +
                    "2. 도토리솥에 도토리를 넣고 불을 올린다.";
                break;
            case 5:
                // 맛있는밥 2개 만들기, 제한 30턴
                menuInfo.Add("", "맛있는 밥");
                stageInfo.Add("", 2);
                currentStageInfo.Add("", 0);
                maxTurnNum = 30;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "맛있는 밥을 짓는 법!\n" +
                    "1. 밥솥에 아래의 재료를 넣고 불을 올린다.\n" +
                    "주재료 : 쌀\n" +
                    "조미료 : 도토리주\n\n" +
                    "조미로를 만드는 법!.\n" +
                    "1. 밥솥, 쌀, 불지피기를 뒤짚어 배치해 도토리솥, 도토리, 도토리 굽기를 얻는다.\n" +
                    "2. 도토리솥에 도토리를 넣고 불을 올린다.";
                break;
            case 6:
                // 규동 기본 1개 만들기, 제한 50턴
                menuInfo.Add("규동이 든 냄비(완료)", "규동 기본");
                stageInfo.Add("규동이 든 냄비(완료)", 1);
                currentStageInfo.Add("규동이 든 냄비(완료)", 0);
                maxTurnNum = 50;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "규동 기본 레시피\n" +
                    "1. 냄비에 아래의 재료를 넣고 끓인다.\n" +
                    "주재료 : 양파\n" +
                    "조미료 : 쇼유, 생강, 노랑도토리주, 해초도토리\n" +
                    "2. 끓인 후의 냄비에 아래의 재료를 넣고 다시 끓인다.\n" +
                    "주재료 : 우삼겹";
                break;
            case 7:
                // 규동 기본 1개 만들기, 제한 30턴
                menuInfo.Add("규동이 든 냄비(완료)", "규동 기본");
                stageInfo.Add("규동이 든 냄비(완료)", 1);
                currentStageInfo.Add("규동이 든 냄비(완료)", 0);
                maxTurnNum = 30;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "규동 기본 레시피\n" +
                    "1. 냄비에 아래의 재료를 넣고 끓인다.\n" +
                    "주재료 : 양파\n" +
                    "조미료 : 쇼유, 생강, 노랑도토리주, 해초도토리\n" +
                    "2. 끓인 후의 냄비에 아래의 재료를 넣고 다시 끓인다.\n" +
                    "주재료 : 우삼겹";
                break;
            case 8:
                // 규동 기본 2개 만들기, 제한 50턴
                menuInfo.Add("규동이 든 냄비(완료)", "규동 기본");
                stageInfo.Add("규동이 든 냄비(완료)", 2);
                currentStageInfo.Add("규동이 든 냄비(완료)", 0);
                maxTurnNum = 50;
                turnNumText.text = "남은 턴 : " + maxTurnNum.ToString();
                recipeString = "규동 기본 레시피\n" +
                    "1. 냄비에 아래의 재료를 넣고 끓인다.\n" +
                    "주재료 : 양파\n" +
                    "조미료 : 쇼유, 생강, 노랑도토리주, 해초도토리\n" +
                    "2. 끓인 후의 냄비에 아래의 재료를 넣고 다시 끓인다.\n" +
                    "주재료 : 우삼겹";
                break;
        }
        CurrentStageInfoTextSet();
    }

    bool TurnStart() {
        CardMngScript.IncreaseCardPutCount();
        turnNum++;
        turnNumText.text = "남은 턴 : " + (maxTurnNum - turnNum + 1).ToString();
        if (turnNum > maxTurnNum)
            return false;
        else
            return true;
    }

    bool IsStageClear() {
        foreach (var menu in stageInfo.Keys) {
            if (stageInfo[menu] > currentStageInfo[menu])
                return false;
        }
        return true;
    }

    IEnumerator StartGameCo() {
        GameSetup(stageNum);

        isLoading = true;

        for (int i = 0; i < startCardCount; i++) {
            CardMngScript.AddCardItem();
            yield return new WaitForSeconds(Utils.cardDrawDotweenTime + Utils.cardDrawExtraTime);
        }

        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo() {
        isLoading = true;

        myTurn = true;
        if (TurnStart()) {
            turnStartPanel.Show();
            yield return new WaitForSeconds(Utils.turnStartPanelDotweenTime);
            CardMngScript.AddCardItem();
            yield return new WaitForSeconds(Utils.cardDrawDotweenTime);
        }
        else
            StartCoroutine(GameOverCo());

        isLoading = false;
    }

    IEnumerator EndTurnCo() {
        isLoading = true;

        myTurn = false;
        isCoroutine.Add(true);
        isCoroutine.Add(true);
        StartCoroutine(CardMngScript.ExecuteCardsCo());
        while (isCoroutine[0]) yield return null;
        StartCoroutine(GridObjectMngScript.ExecuteGridObjectCo());
        while (isCoroutine[1]) yield return null;
        if (IsStageClear())
            StageClear();
        else
            StartCoroutine(StartTurnCo());
        isCoroutine.Clear();
    }

    IEnumerator GameOverCo() {
        ////
        yield break;
    }
}
