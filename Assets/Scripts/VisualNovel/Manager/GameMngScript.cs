using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMngScript : MonoBehaviour {
    static public GameMngScript             Inst { get; set; } = null;

    [SerializeField]
    Image                                   mMenuPanel = null;
    bool                                    mIsMenuActive = false;

    [SerializeField]
    Text                                    mGoldText = null;
    int                                     mGold;

    Dictionary<string, int>                 mVisitedMapName = new Dictionary<string, int>();
    string                                  mPreMapName = "";

    static public bool                      IsMenuActive { get { return Inst.mIsMenuActive; } }
    static public Text                      GoldText { get { return Inst.mGoldText; } }
    static public Dictionary<string, int>   VisitedMapName { get { return Inst.mVisitedMapName; } }
    static public string                    PreMapName { set { Inst.mPreMapName = value; } }

    static public int Gold {
        get {
            return Inst.mGold;
        }
        set {
            Inst.mGold = value;
            GoldText.text = Inst.mGold.ToString();
        }
    }

    private void Awake() {
        if (!Inst)
            Inst = this;

        //PlayerPrefs.DeleteAll();
    }

    private void Start() {
        TestInfo    info = TesterScript.GetInfo();

        if (info.Using)
            Gold = info.givenGold;
        else {
            Load();
            Gold = MainGameMngScript.InitGold;
        }
    }

    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if (ShouldMenuBlocked())
                return;

            if (mMenuPanel.IsActive())
                MenuContinueButton();
            else {
                mMenuPanel.gameObject.SetActive(true);
                mIsMenuActive = true;
                PlayerScript.FreezeAnimation();
            }
        }
    }

    public static void ReStart(string _mapName) {
        if (!TesterScript.GetInfo().Using) {
            if (Inst.mVisitedMapName.ContainsKey(_mapName))
                Inst.mVisitedMapName[_mapName] += 1;
            else
                Inst.mVisitedMapName.Add(_mapName, 1);
        }

        PlayerScript.SetEnteringPosition(Inst.mPreMapName, _mapName);
        QuestMngScript.LoadQuest(_mapName);
        StoryBlockMngScript.LoadStoryBlock(_mapName);
        SoundMngScript.LoadSound(_mapName);
        ObjectMngScript.LoadObjects(_mapName);
    }

    bool ShouldMenuBlocked() {
        if (PlayerScript.Inst == null)
            return true;

        // 스토리 패널, 상점, 퀘스트 창 등이 켜져 있을 때에는 메뉴 창을 열지 못한다.
        // 이를 판단하기 위한 함수이다
        if (PlayerScript.Interact)
            return true;
        if (QuestMngScript.QuestPanel.IsActive())
            return true;

        return false;
    }

    public void MenuContinueButton() {
        mMenuPanel.gameObject.SetActive(false);
        mIsMenuActive = false;
    }

    public void MenuExitButton() {
        Save();
        Application.Quit();
    }

    public void Save() {
        //// 플레이어 위치 정보 저장
        //Vector2 playerPos = PlayerScript.Inst.GetComponent<Transform>().position;
        //PlayerPrefs.SetFloat("PlayerX", playerPos.x);
        //PlayerPrefs.SetFloat("PlayerY", playerPos.y);

        //// 퀘스트 진행 현황 저장
        //Text[] questTexts = QuestMngScript.QuestTexts;
        //PlayerPrefs.SetInt("QuestEmptyIndex", QuestMngScript.EmptyIndex);
        //PlayerPrefs.SetString("QuestFirstText", questTexts[0].text);
        //PlayerPrefs.SetString("QuestSecondText", questTexts[1].text);
        //PlayerPrefs.SetString("QuestLastText", questTexts[2].text);

        //int questNum = 0;
        //DoingQuest[] doingQuestInfos = QuestMngScript.DoingQuestInfos;
        //if (doingQuestInfos != null) {
        //    for (int i = 0; i < doingQuestInfos.Length; i++) {
        //        if (!doingQuestInfos[i].isDoing)
        //            continue;

        //        PlayerPrefs.SetInt("Quest" + i + "TextIndex", doingQuestInfos[i].textIndex);

        //        int j;
        //        if (doingQuestInfos[i].intConditions != null) {
        //            for (j = 0; j < doingQuestInfos[i].intConditions.Length; j++)
        //                PlayerPrefs.SetInt("Quest" + i + "IntCondition" + j, doingQuestInfos[i].intConditions[j]);
        //            PlayerPrefs.SetInt("Quest" + i + "IntConditionLength", j);
        //        }
        //        if (doingQuestInfos[i].boolConditions != null) {
        //            for (j = 0; j < doingQuestInfos[i].boolConditions.Length; j++) {
        //                if (doingQuestInfos[i].boolConditions[j])
        //                    PlayerPrefs.SetInt("Quest" + i + "BoolCondition" + j, 1);
        //                else
        //                    PlayerPrefs.SetInt("Quest" + i + "BoolCondition" + j, 0);
        //            }
        //            PlayerPrefs.SetInt("Quest" + i + "BoolConditionLength", j);
        //        }

        //        if (questNum == 2)
        //            break;
        //    }
        //}

        //GameObject[] questObjects = QuestMngScript.QuestObjects;
        //if (questObjects != null) {
        //    // 퀘스트 오브젝트 Active 현황 저장
        //    for (int i = 0; i < questObjects.Length; i++) {
        //        if (questObjects[i].activeSelf)
        //            PlayerPrefs.SetInt("QuestObject" + i + "Active", 1);
        //        else
        //            PlayerPrefs.SetInt("QuestObject" + i + "Active", 0);
        //    }

        //    // 퀘스트 오브젝트 InteractIndex 저장
        //    for (int i = 0; i < questObjects.Length; i++)
        //        PlayerPrefs.SetInt("QuestObject" + i + "InteractIndex", questObjects[i].GetComponent<ObjectScript>().InteractIndex);

        //    PlayerPrefs.SetInt("QuestObjectLength", questObjects.Length);
        //}

        //PlayerPrefs.Save();
    }

    public void Load() {
        //// 게임 첫 실행인지 체크
        //if (!PlayerPrefs.HasKey("PlayerX"))
        //    return;

        //// 플레이어 위치 정보 불러오기
        //float playerX = PlayerPrefs.GetFloat("PlayerX");
        //float playerY = PlayerPrefs.GetFloat("PlayerY");
        //PlayerScript.Inst.gameObject.transform.position = new Vector2(playerX, playerY);

        //// 퀘스트 진행 현황 불러오기
        //Text[] questTexts = QuestMngScript.QuestTexts;
        //QuestMngScript.EmptyIndex = PlayerPrefs.GetInt("QuestEmptyIndex");
        //questTexts[0].text = PlayerPrefs.GetString("QuestFirstText");
        //questTexts[1].text = PlayerPrefs.GetString("QuestSecondText");
        //questTexts[2].text = PlayerPrefs.GetString("QuestLastText");

        //DoingQuest[] doingQuestInfos = QuestMngScript.DoingQuestInfos;
        //for (int i = 0; i < doingQuestInfos.Length; i++) {
        //    if (!PlayerPrefs.HasKey("Quest" + i + "TextIndex"))
        //        break;

        //    doingQuestInfos[i].isDoing = true;
        //    doingQuestInfos[i].textIndex = PlayerPrefs.GetInt("Quest" + i + "TextIndex");

        //    if (PlayerPrefs.HasKey("Quest" + i + "IntConditionLength")) {
        //        int intConditionLength = PlayerPrefs.GetInt("Quest" + i + "IntConditionLength");
        //        doingQuestInfos[i].intConditions = new int[intConditionLength];
        //        for (int j = 0; j < intConditionLength; j++)
        //            doingQuestInfos[i].intConditions[j] = PlayerPrefs.GetInt("Quest" + i + "IntCondition" + j);
        //    }
            
        //    if (PlayerPrefs.HasKey("Quest" + i + "BoolConditionLength")) {
        //        int boolConditionLength = PlayerPrefs.GetInt("Quest" + i + "BoolConditionLength");
        //        doingQuestInfos[i].boolConditions = new bool[boolConditionLength];
        //        for (int j = 0; j < boolConditionLength; j++) {
        //            switch (PlayerPrefs.GetInt("Quest" + i + "BoolCondition" + j)) {
        //                case 0:
        //                    doingQuestInfos[i].boolConditions[j] = false;
        //                    break;
        //                case 1:
        //                    doingQuestInfos[i].boolConditions[j] = true;
        //                    break;
        //            }
        //        }
        //    }
        //}

        //// 오브젝트 Active 현황 불러오기
        //GameObject[] questObjects = QuestMngScript.QuestObjects;
        //if (questObjects != null) {
        //    int questObjectLength = PlayerPrefs.GetInt("QuestObjectLength");
        //    for (int i = 0; i < questObjectLength; i++) {
        //        switch (PlayerPrefs.GetInt("QuestObject" + i + "Active")) {
        //            case 0:
        //                questObjects[i].SetActive(false);
        //                break;
        //            case 1:
        //                questObjects[i].SetActive(true);
        //                break;
        //        }
        //    }

        //    // 오브젝트 InteractIndex 불러오기
        //    for (int i = 0; i < questObjectLength; i++)
        //        questObjects[i].gameObject.GetComponent<ObjectScript>().InteractIndex = PlayerPrefs.GetInt("QuestObject" + i + "InteractIndex");
        //}
    }
}
