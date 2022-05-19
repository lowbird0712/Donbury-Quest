using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SO_CardType {
    CannotUse = 0,
    Normal = 1,
    Placable = 2,
    Spell = 3,
}

public enum SO_Dotori {
    Default = 0,
    DotoriSide = 1,
    DotoriBackSide = 2
}

public class CardItem {
    public SO_CardType  type = SO_CardType.CannotUse;
    public Sprite       sprite = null;
    public string       explain = null;
    public string       relatedObjectName = null;
    public bool         isDotori = false;
}

[CreateAssetMenu(fileName = "SO_CardItem", menuName = "Scriptable Object/SO_CardItem")]
public class SO_CardItemScript : ScriptableObject {
    [SerializeField] Sprite[]   cardSprites = null;

    string[]                    cardNames = null;
    CardItem[]                  cardItems = null;

    CardItem SetUpItem(string _cardName) {
        CardItem item = new CardItem();
        //item.type = SO_CardType.CannotUse;
        //item.sprite = cardSprites[0];
        //item.explain = "";
        //item.relatedObjectName = "";
        //item.haveDest = false;
        //item.isDotori = false;

        // 기본
        if (_cardName == "밥솥") {
            item.sprite = cardSprites[0];
            item.explain = "지정한 위치에 밥솥(준비)을 놓는다.";
            item.relatedObjectName = "밥솥(준비)";
            item.type = SO_CardType.Placable;
        }
        else if (_cardName == "불지피기") {
            item.sprite = cardSprites[1];
            item.explain = "지정한 위치에 불을 지핀다.";
            item.type = SO_CardType.Spell;
        }
        else if (_cardName == "쌀") {
            item.sprite = cardSprites[2];
            item.explain = "지정한 위치에 쌀을 놓는다.";
            item.relatedObjectName = "쌀";
            item.type = SO_CardType.Placable;
        }
        else if (_cardName == "조미료") {
            item.sprite = cardSprites[3];
            item.explain = "지정한 위치에 상좌우하 중에서 가장 앞 순서에 있는 타일에 필요한 조미료 중 사용되지 않은 조미료를 랜덤으로 놓는다.";
            item.type = SO_CardType.Placable;
        }

        // 기본 도토리
        else if (_cardName == "도토리 솥") {
            item.sprite = cardSprites[4];
            item.explain = "도토리를 넣을 수 있는 솥이다.";
            item.relatedObjectName = "도토리 솥(준비)";
            item.type = SO_CardType.Placable;
            item.isDotori = true;
        }
        else if (_cardName == "도토리") {
            item.sprite = cardSprites[5];
            item.explain = "탐스러운 도토리이다. 섬세하게 조절된 불로 구우면 쓸만한 조미료가 된다.";
            item.relatedObjectName = "도토리";
            item.type = SO_CardType.Placable;
            item.isDotori = true;
        }
        else if (_cardName == "도토리 굽기") {
            item.sprite = cardSprites[6];
            item.explain = "섬세하게 조절된 불로 도토리를 굽는다.";
            item.type = SO_CardType.Spell;
            item.isDotori = true;
        }
        else if (_cardName == "향기로운 도토리") {
            item.sprite = cardSprites[7];
            item.explain = "도토리가 강렬한 향기를 내뿜고 있다. 식욕을 돋우는 향이다.";
            item.relatedObjectName = "도토리";
            item.type = SO_CardType.Placable;
            item.isDotori = true;
        }

        // 규동 기본
        else if (_cardName == "냄비") {
            item.sprite = cardSprites[8];
            item.explain = "지정한 위치에 냄비(준비)을 놓는다.";
            item.relatedObjectName = "냄비(준비)";
            item.type = SO_CardType.Placable;
        }
        else if (_cardName == "우삼겹") {
            item.sprite = cardSprites[9];
            item.explain = "지정한 위치에 우삼겹을 놓는다.";
            item.relatedObjectName = "우삼겹";
            item.type = SO_CardType.Placable;
        }
        else if (_cardName == "양파") {
            item.sprite = cardSprites[10];
            item.explain = "지정한 위치에 양파를 놓는다.";
            item.relatedObjectName = "양파";
            item.type = SO_CardType.Placable;
        }

        // 규동 기본 도토리
        else if (_cardName == "도토리가 들어 있는 냄비") {
            item = new CardItem();
            item.sprite = cardSprites[11];
            item.explain = "도토리가 들어 있는 냄비이다.";
            item.isDotori = true;
        }
        else if (_cardName == "우삼겹에 말려 있는 도토리") {
            item = new CardItem();
            item.sprite = cardSprites[12];
            item.explain = "도토리가 두툼한 우삼겹에 말려 있다. 이대로 구우면 맛있을 것 같다.";
            item.isDotori = true;
        }
        else if (_cardName == "양파와 도토리") {
            item = new CardItem();
            item.sprite = cardSprites[13];
            item.explain = "도토리가 양파 옆에 놓여 있다.";
            item.isDotori = true;
        }

        return item;
	}

    // 도토리 관련 함수
    void ChangeToDotori(string _cardName) {
        // 기본
        if (_cardName == "불지피기")
            CreateCardToHand("도토리 굽기");
        else if (_cardName == "밥솥")
            CreateCardToHand("도토리 솥");
        else if (_cardName == "쌀")
            CreateCardToHand("도토리");
        else if (_cardName == "조미료")
            CreateCardToHand("향기로운 도토리");

        // 규동 기본
        else if (_cardName == "냄비")
            CreateCardToHand("도토리가 들어 있는 냄비");
        else if (_cardName == "우삼겹")
            CreateCardToHand("우삼겹에 말려 있는 도토리");
        else if (_cardName == "양파")
            CreateCardToHand("양파와 도토리");
    }

    void DotoriReturn(string _cardName) {
        // 기본 도토리
        if (_cardName == "도토리 솥")
            AddCardToDeck("밥솥");
        else if (_cardName == "도토리 굽기")
            AddCardToDeck("불지피기");
        else if (_cardName == "도토리")
            AddCardToDeck("쌀");
        else if (_cardName == "향기로운 도토리")
            AddCardToDeck("조미료");

        // 규동 기본 도토리
        else if (_cardName == "냄비 안의 도토리")
            AddCardToDeck("냄비");
        else if (_cardName == "우삼겹에 말려 있는 도토리")
            AddCardToDeck("우삼겹");
        else if (_cardName == "양파와 도토리")
            AddCardToDeck("양파");

        CardMngScript.AddCardItem();
    }

    GridObjectScript GetSpellNextGridObject(string _cardName) {
        if (GetCardItem(_cardName).type != SO_CardType.Spell)
            Debug.LogError("_cardName의 카드는 스펠 카드가 아닙니다!");

        CurrentObjectItem current;
        foreach (var gridObject in GridObjectMngScript.GridObjects) {
            current = gridObject.CurrentObjectItem;
            if (current == null)
                continue;
            if (current.currentSpellNames == null)
                continue;
            if (!current.currentSpellNames.Contains(_cardName))
                continue;
            return gridObject;
        }
        return null;
    }

    bool IsSpiceToolExist() {
        foreach (var gridObject in GridObjectMngScript.GetAdjacentGridObjects()) {
            if (!gridObject.HasObject)
                continue;
            ObjectItem objectItem = GridObjectMngScript.GridObjectSO.GetObjectItem(gridObject.ObjectName);
            if (objectItem.tool == null)
                continue;
            if (objectItem.tool.neededSpiceNames == null)
                continue;
            if (objectItem.tool.neededSpiceNames.Count != gridObject.CurrentObjectItem.currentSpiceNames.Count)
                return true;
        }
        return false;
    }

    // 카드 효과 함수
    public void CreateCardToHand(string _cardName, int _num = 1) {
        for (int i = 0; i < _num; i++) {
            CardMngScript.CardBuffer.Insert(0, _cardName);
            CardMngScript.AddCardItem();
        }
    }

    public void AddCardToDeck(string _cardName, int _num = 1) {
        int index;
        for (int i = 0; i < _num; i++) {
            index = Random.Range(0, CardMngScript.CardBuffer.Count + 1);
            if (index == CardMngScript.CardBuffer.Count + 1)
                CardMngScript.CardBuffer.Add(_cardName);
            else
                CardMngScript.CardBuffer.Insert(index, _cardName);
        }
    }

    public void SetUp(string _deckName) {
        if (_deckName == "규동 기본") {
            cardNames = new string[14];
            cardItems = new CardItem[14];

            cardNames[0] = "밥솥";
            cardItems[0] = SetUpItem("밥솥");
            cardNames[1] = "냄비";
            cardItems[1] = SetUpItem("냄비");
            cardNames[2] = "불지피기";
            cardItems[2] = SetUpItem("불지피기");
            cardNames[3] = "쌀";
            cardItems[3] = SetUpItem("쌀");
            cardNames[4] = "우삼겹";
            cardItems[4] = SetUpItem("우삼겹");
            cardNames[5] = "양파";
            cardItems[5] = SetUpItem("양파");
            cardNames[6] = "조미료";
            cardItems[6] = SetUpItem("조미료");

            cardNames[7] = "도토리 솥";
            cardItems[7] = SetUpItem("도토리 솥");
            cardNames[8] = "도토리가 들어 있는 냄비";
            cardItems[8] = SetUpItem("도토리가 들어 있는 냄비");
            cardNames[9] = "도토리 굽기";
            cardItems[9] = SetUpItem("도토리 굽기");
            cardNames[10] = "도토리";
            cardItems[10] = SetUpItem("도토리");
            cardNames[11] = "우삼겹에 말려 있는 도토리";
            cardItems[11] = SetUpItem("우삼겹에 말려 있는 도토리");
            cardNames[12] = "양파와 도토리";
            cardItems[12] = SetUpItem("양파와 도토리");
            cardNames[13] = "향기로운 도토리";
            cardItems[13] = SetUpItem("향기로운 도토리");
        }
    }

    public CardItem GetCardItem(string _cardName) {
        CardItem result = new CardItem();

        for (int i = 0; i < cardNames.Length; i++) {
            if (cardNames[i] == _cardName) {
                result = cardItems[i];
                return result;
            }
        }

        Debug.LogError("찾으려는 CardItem이 Scriptable Object에 없습니다!");
        return result;
    }

    public string GetSpellAnimationKey(string _cardName) {
        if (GetCardItem(_cardName).type != SO_CardType.Spell)
            Debug.LogError("_cardName의 카드는 스펠 카드가 아닙니다!");

        if (_cardName == "불지피기")
            return "Fire";
        else if (_cardName == "도토리 굽기")
            return "Fire";
        else {
            Debug.LogError("아직 해당 스펠의 애니메이션 키가 설정되지 않았습니다!");
            return null;
        }
	}

    public void ExecuteCardFunc(string _cardName, SO_Dotori _dotoriFlag) {
        GridObjectScript nextGridObject;
        int x;
        int y;

        if (_dotoriFlag == SO_Dotori.Default) {
            if (GetCardItem(_cardName).type != SO_CardType.Spell) {
                nextGridObject = GridObjectMngScript.NextGridObject;
                x = nextGridObject.Position[0];
                y = nextGridObject.Position[1];
            }
            else {
                nextGridObject = GetSpellNextGridObject(_cardName);
                x = nextGridObject.Position[0];
                y = nextGridObject.Position[1];
            }

            // 기본
            if (_cardName == "밥솥")
                GridObjectMngScript.PlaceObject("밥솥(준비)", x, y);
            else if (_cardName == "불지피기") {
                GridObjectMngScript.Spell("불지피기", x, y);
                GridObjectMngScript.StartCooking(x, y);
            }
            else if (_cardName == "쌀")
                GridObjectMngScript.PlaceObject("쌀", x, y);
            else if (_cardName == "조미료") {
                if (IsSpiceToolExist())
                    GridObjectMngScript.PlaceObject(GridObjectMngScript.GetRandomNeededSpice(), x, y);
                else
                    GridObjectMngScript.PlaceObject("조미료", x, y); ;
            }

            // 기본 도토리
            else if (_cardName == "도토리 솥")
                GridObjectMngScript.PlaceObject("도토리 솥(준비)", x, y);
            else if (_cardName == "도토리")
                GridObjectMngScript.PlaceObject("도토리", x, y);
            else if (_cardName == "도토리 굽기") {
                GridObjectMngScript.Spell("도토리 굽기", x, y);
                GridObjectMngScript.StartCooking(x, y);
            }
            else if (_cardName == "향기로운 도토리")
                GridObjectMngScript.PlaceObject("도토리", x, y);

            // 규동 기본
            else if (_cardName == "냄비")
                GridObjectMngScript.PlaceObject("냄비(준비)", x, y);
            else if (_cardName == "양파")
                GridObjectMngScript.PlaceObject("양파", x, y);
            else if (_cardName == "우삼겹")
                GridObjectMngScript.PlaceObject("우삼겹", x, y);

            // 도토리 카드
            else
                GridObjectMngScript.PlaceObject(_cardName, x, y);
        }
        else if (_dotoriFlag == SO_Dotori.DotoriSide)
            ChangeToDotori(_cardName);
        else if (_dotoriFlag == SO_Dotori.DotoriBackSide)
            DotoriReturn(_cardName);
    }

    public bool GetUsable(string _cardName) {
        SO_CardType type = GetCardItem(_cardName).type;
        if (type == SO_CardType.Placable && GridObjectMngScript.NextGridObject != null &&
            GridObjectMngScript.NextGridObjectIndex + CardMngScript.PutCards.Count > GridObjectMngScript.GridObjects.Count)
            return false;
        else if (type == SO_CardType.Spell && GetSpellNextGridObject(_cardName) == null)
            return false;
        return true;
    }
}
