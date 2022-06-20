using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SO_Timing {
    Start = 0,
    Effect = 1,
    End = 2
}

public class CurrentObjectItem {
    public string       objectName;
    public List<string> currentSpiceNames = new List<string>();
    public List<int>    currentObjectNums = new List<int>();
    public List<string> currentSpellNames = new List<string>();

    public CurrentObjectItem(string _objectName) {
        ObjectItem objectItem = GridObjectMngScript.GridObjectSO.GetObjectItem(_objectName);
        objectName = _objectName;
        if (objectItem.tool != null) {
            for (int i = 0; i < objectItem.tool.neededObjectNames.Count; i++)
                currentObjectNums.Add(0);
        }
        CurrentSpellNameSet();
    }

    void CurrentSpellNameSet() {

    }

    public void CurrentSpellNameUpdate() {
        ObjectItem          objectItem = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName);
        ObjectSubItemTool   tool = objectItem.tool;
        bool                neededObjectFlag = false;
        foreach (var spellName in objectItem.usableSpellNames) {
            if (spellName == "불지피기" || spellName == "도토리 굽기") {
                if (tool.neededSpiceNames != null && tool.neededSpiceNames.Count != currentSpiceNames.Count)
                    continue;
                if (tool.neededObjectNames != null) {
                    for (int i = 0; i < tool.neededObjectNames.Count; i++) {
                        if (tool.neededObjectNums[i] != currentObjectNums[i]) {
                            neededObjectFlag = true;
                            break;
                        }
                    }
                    if (neededObjectFlag) {
                        neededObjectFlag = false;
                        continue;
                    }
                }
            }
            currentSpellNames.Add(spellName);
        }
    }
}

public class ObjectItem {
    public ObjectSubItemTool    tool;
    public ObjectSubItemCooking cooking;
    public string               explain;
    public bool                 isSpice;

    public List<string>         usableSpellNames;
    public Sprite               sprite;
    public string               animationKey;
}

public class ObjectSubItemTool {
    public string       nextObjectName;
    public List<string> neededSpiceNames;
    public List<string> neededObjectNames;
    public List<int>    neededObjectNums;

    public bool IsNeeded(string _name, bool _isSpice, GridObjectScript _gridObject) {
        if (_isSpice) {
            if (_name == "조미료")
                return true;
            if (neededSpiceNames == null || !neededSpiceNames.Contains(_name))
                return false;
            if (_gridObject.CurrentObjectItem.currentSpiceNames.Contains(_name))
                return false;
        }
        else {
            if (neededObjectNames == null || !neededObjectNames.Contains(_name))
                return false;
            int index = neededObjectNames.IndexOf(_name);
            if (neededObjectNums[index] == _gridObject.CurrentObjectItem.currentObjectNums[index])
                return false;
        }

        return true;
    }
}

public class ObjectSubItemCooking {
    public string               nextObjectName;
    public int                  originCountDown = -1;
}

[CreateAssetMenu(fileName = "SO_GridObject", menuName = "Scriptable Object/SO_GridObject")]
public class SO_GridObjectScript : ScriptableObject {
    [SerializeField] Sprite[]   objectSprites = null;
    [SerializeField] Sprite[]   countDownSprites = null;

    string[]                    objectNames;
    ObjectItem[]                objectItems;

    public Sprite GetCountDown(int _index) => countDownSprites[_index];

    ObjectItem SetUpItem(string _objectName) {
        ObjectItem item = new ObjectItem();
        //// ObjectItem
        //item = new ObjectItem();
        //item.explain = "";
        //item.isSpice = false;
        //item.usableSpellNames = new string[1];
        //item.sprite = objectSprites[0];
        //item.animationKey = "";
        //// ObjectSubItemTool
        //item.tool = new ObjectSubItemTool();
        //item.nextObjectName = "";
        //item.neededObjectNames = new string[1];
        //item.insideObjectNames = new string[1];
        //item.neenedObjectNums = new int[1];
        //item.insideObjectNums = new int[1];
        //// ObjectSubItemCooking
        //item.cooking = new ObjectSubItemCooking();
        //item.nextObjectName = "";
        //item.originCountDown = 0;

        // 기본
        if (_objectName == "밥솥(준비)") {
            item.usableSpellNames = new List<string>(1) { "불지피기" };
            item.sprite = objectSprites[0];
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "밥솥(조리중)";
            item.tool.neededObjectNames = new List<string>(1) { "쌀" };
            item.tool.neededObjectNums = new List<int>(1) { 1 };
            item.tool.neededSpiceNames = new List<string>(1) { "도토리주" };
        }
        else if (_objectName == "밥솥(조리중)") {
            item.explain = "다음 오브젝트 : 밥솥(완료)";
            item.animationKey = "Rice Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.nextObjectName = "밥솥(완료)";
            item.cooking.originCountDown = 3;
        }
        else if (_objectName == "밥솥(완료)")
            item.sprite = objectSprites[0];
        else if (_objectName == "쌀")
            item.sprite = objectSprites[1];
        else if (_objectName == "조미료")
            item.sprite = objectSprites[11];
        else if (_objectName == "도토리주") {
            item.isSpice = true;
            item.sprite = objectSprites[12];
        }

        // 기본 도토리
        else if (_objectName == "도토리 솥(준비)") {
            item.usableSpellNames = new List<string>(1) { "도토리 굽기" };
            item.sprite = objectSprites[2];
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "도토리 솥(조리중)";
            item.tool.neededObjectNames = new List<string>(1) { "도토리" };
            item.tool.neededObjectNums = new List<int>(1) { 1 };
        }
        else if (_objectName == "도토리 솥(조리중)") {
            item.explain = "조리가 완료될 때 조미료 3장을 패에 추가합니다.";
            item.animationKey = "Rice Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.originCountDown = 3;
        }
        else if (_objectName == "도토리")
            item.sprite = objectSprites[3];

        // 규동 기본
        else if (_objectName == "냄비(준비)") {
            item.usableSpellNames = new List<string>(1) { "불지피기" };
            item.sprite = objectSprites[4];
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "냄비(조리중)";
            item.tool.neededSpiceNames = new List<string>(4) { "생강", "쇼유", "노랑도토리주", "해초 도토리" };
            item.tool.neededObjectNames = new List<string>(1) { "양파" };
            item.tool.neededObjectNums = new List<int>(1) { 1 };
        }
        else if (_objectName == "냄비(조리중)") {
            item.explain = "다음 오브젝트 : 규동이 든 냄비(준비)";
            item.animationKey = "Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.nextObjectName = "규동이 든 냄비(준비)";
            item.cooking.originCountDown = 3;
        }
        else if (_objectName == "규동이 든 냄비(준비)") {
            item.usableSpellNames = new List<string>(1) { "불지피기" };
            item.sprite = objectSprites[4];
            item.tool = new ObjectSubItemTool();
            item.tool.nextObjectName = "규동이 든 냄비(조리중)";
            item.tool.neededObjectNames = new List<string>(1) { "우삼겹" };
            item.tool.neededObjectNums = new List<int>(1) { 1 };
        }
        else if (_objectName == "규동이 든 냄비(조리중)") {
            item.explain = "다음 오브젝트 : 규동이 든 냄비(완료)";
            item.animationKey = "Pot Cooking";
            item.cooking = new ObjectSubItemCooking();
            item.cooking.nextObjectName = "규동이 든 냄비(완료)";
            item.cooking.originCountDown = 1;
        }
        else if (_objectName == "규동이 든 냄비(완료)")
            item.sprite = objectSprites[4];
        else if (_objectName == "우삼겹")
            item.sprite = objectSprites[5];
        else if (_objectName == "양파")
            item.sprite = objectSprites[6];
        else if (_objectName == "생강") {
            item.isSpice = true;
            item.sprite = objectSprites[7];
        }
        else if (_objectName == "쇼유") {
            item.isSpice = true;
            item.sprite = objectSprites[8];
        }
        else if (_objectName == "노랑도토리주") {
            item.isSpice = true;
            item.sprite = objectSprites[9];
        }
        else if (_objectName == "해초 도토리") {
            item.isSpice = true;
            item.sprite = objectSprites[10];
        }

        return item;
    }

    public void SetUp(string _deckName) {
        if (_deckName == "규동 기본") {
            objectNames = new string[21];
            objectItems = new ObjectItem[21];

            // 기본
            objectNames[0] = "밥솥(준비)";
            objectItems[0] = SetUpItem("밥솥(준비)"); 
            objectNames[1] = "밥솥(조리중)";
			objectItems[1] = SetUpItem("밥솥(조리중)");
            objectNames[2] = "밥솥(완료)";
            objectItems[2] = SetUpItem("밥솥(완료)");
            objectNames[3] = "쌀";
            objectItems[3] = SetUpItem("쌀");
            objectNames[19] = "조미료";
            objectItems[19] = SetUpItem("조미료");
            objectNames[20] = "도토리주";
            objectItems[20] = SetUpItem("도토리주");

            // 기본 도토리
            objectNames[4] = "도토리 솥(준비)";
            objectItems[4] = SetUpItem("도토리 솥(준비)");
            objectNames[5] = "도토리 솥(조리중)";
            objectItems[5] = SetUpItem("도토리 솥(조리중)");
            objectNames[6] = "도토리 솥(완료)";
            objectItems[6] = SetUpItem("도토리 솥(완료)");
            objectNames[7] = "도토리";
            objectItems[7] = SetUpItem("도토리");

            // 규동 기본
            objectNames[8] = "냄비(준비)";
            objectItems[8] = SetUpItem("냄비(준비)");
            objectNames[9] = "냄비(조리중)";
            objectItems[9] = SetUpItem("냄비(조리중)");
            objectNames[10] = "규동이 든 냄비(준비)";
            objectItems[10] = SetUpItem("규동이 든 냄비(준비)");
            objectNames[11] = "규동이 든 냄비(조리중)";
            objectItems[11] = SetUpItem("규동이 든 냄비(조리중)");
            objectNames[12] = "규동이 든 냄비(완료)";
            objectItems[12] = SetUpItem("규동이 든 냄비(완료)");
            objectNames[13] = "우삼겹";
            objectItems[13] = SetUpItem("우삼겹");
            objectNames[14] = "양파";
            objectItems[14] = SetUpItem("양파");
            objectNames[15] = "생강";
            objectItems[15] = SetUpItem("생강");
            objectNames[16] = "쇼유";
            objectItems[16] = SetUpItem("쇼유");
            objectNames[17] = "노랑도토리주";
            objectItems[17] = SetUpItem("노랑도토리주");
            //objectNames[18] = "설탕";
            //objectItems[18] = SetUpItem("설탕");
            objectNames[18] = "해초 도토리";
            objectItems[18] = SetUpItem("해초 도토리");
            //// 19번 인덱스에는 "조미료" 오브젝트를 설정했음
        }
    }

    public ObjectItem GetObjectItem(string _objectName) {
        ObjectItem result = new ObjectItem();

        for (int i = 0; i < objectNames.Length; i++) {
            if (objectNames[i] == _objectName) {
                result = objectItems[i];
                return result;
            }
        }

        return result;
    }

    public void ExecuteObjectFunc(string _objectName, SO_Timing _timing, int _x = -1, int _y = -1) {
        // 오브젝트 효과 발동
        switch (_timing) {
            case SO_Timing.Start:

                break;
            case SO_Timing.Effect:

                break;
            case SO_Timing.End:
                if (_objectName == "도토리 솥(조리중)") {
                    CardMngScript.CardItemSO.CreateCardToHand("조미료", 3);
                }
                break;
        }
    }
}
