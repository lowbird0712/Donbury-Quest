# **돈부리 퀘스트 기술 문서**
[![jp](https://img.shields.io/badge/lang-jp-red.svg)](https://github.com/lowbird0712/Donbury-Quest/blob/develop/README.jp.md)

## **돈부리 퀘스트는 어떤 게임?**

**돈부리 퀘스트는 “요리하는 카드게임”를 테마로 하고 있습니다. 기존의 요리 게임들의 시스템이 실제 조리법과의 연관성이 부족하고 그 결과 요리의 테마가 다름에도 비슷한 방식의 게임 플레이를 하게 되는 부분에 초점을 맞춰 고안했습니다.**

**(현재, Coroutine로직을 UniRX로 바꾸는 작업을 진행 중에 있습니다)**

[**프로토타입 플레이 영상**](https://github.com/lowbird0712/Projects-before-Donbury-Quest/tree/master/Prototype%20Videos)

[**전체 코드**](https://github.com/lowbird0712/Donbury-Quest/tree/master/Assets/Scripts)

## **간략한 룰과 시스템 설명**

**- 플레이어는 요리를 완성하기 위해 “조리 기구”, “재료”, 조리 동작을 의미하는 “조리”, 그리고 요리의 특성을 나타낸 “유틸” 카드를 사용해 게임을 진행하게 됩니다. 플레이어는 사용할 카드를 “카드보드”에 배치할 수 있고, 요구하는 요리들을 주어진 턴 수 안에 완성해야 합니다. (A)**

**- 플레이어가 조리 기구, 재료, 그리고 일부 유틸 카드를 사용하면 4x4 필드의 네모난 칸 하나(이후 “그리드”로 칭함)에 왼쪽 위부터 차례대로 배치됩니다. 그리드에 배치된 물체들을 “오브젝트”라고 합니다. 그리드에 오브젝트를 배치하지 않는 카드를 “스펠” 카드라고 합니다. (B)**

**- 모든 카드는 앞면, 혹은 뒷면으로 사용할 수 있으며 앞면으로 사용할 시 각 카드의 효과가 발동되고, 뒷면으로 사용할 시 각 카드와 연결된 “도토리 카드”로 바뀌어 패에 더해지게 됩니다. 도토리 카드는 스펠 카드이거나 그렇지 않을 수 있습니다. 도토리 카드는 일반적으로 앞면이면 그리드를 메워 해당 그리드에 조리 기구와 재료가 배치되지 않게 하기 위해, 뒷면이면 패를 순환시키기 위해 사용됩니다. 이 방식으로 그리드에 배치된 물체들을 “도토리 오브젝트”라고 합니다. (B)**

**- 요리 과정은 크게 “밥 짓기”와 “요리 만들기”로 나누어집니다. 밥을 짓기 위해서는 “밥솥”을 그리드에 배치한 후 “쌀”을 넣고 “불지피기”를 사용해 조리해야 합니다. 모든 요리의 조리 과정에는 “조미료”가 여러 종류 필요하며 이는 밥을 짓기 위해 필요한 카드들과 연관된 도토리 카드들인 “도토리 솥”, “도토리”, “도토리 굽기”를 사용해 조리해 얻을 수 있습니다. (C)**

**- 특정 도토리 카드들은 일정 개수의 도토리 오브젝트를 소모해야만 앞면으로 사용할 수 있습니다. 이 경우, 필요 개수 만큼의 도토리 오브젝트가 사라지고 카드의 효과가 발동됩니다. 비워진 그리드에는 다시 오브젝트를 배치할 수 있습니다. 프로토타입 영상에서는 이 기능이 아직 나타나고 있지 않습니다.**

**- 조미료 카드를 앞면으로 사용할 경우 다른 오브젝트들처럼 다음에 배치해야 할 그리드의 위치에 배치됩니다. 이 때 인접한 그리드에 조미료를 필요로 하는 오브젝트(대부분 조리 도구)가 있으면 그 중 해당 오브젝트에 아직 들어있지 않은 하나를 랜덤으로 배치합니다. 없을 경우에는 “조미료” 오브젝트를 배치합니다. 조미료 오브젝트는 도토리 오브젝트처럼 기본적으로는 그리드를 메우는 역할을 하지만 인접한 그리드에 조리 도구가 있을 경우 해당 조리 도구 안으로 사라지고 그리드가 비워집니다. (D)**

## **전체적인 프로젝트의 구조**

```cs
IEnumerator StartGameCo() {
        GameSetup(0);

        isLoading = true;

        for (int i = 0; i < startCardCount; i++) {
            CardMngScript.AddCardItem();
            yield return new WaitForSeconds(Utils.cardDrawDotweenTime + Utils.cardDrawExtraTime);
        }

        StartCoroutine(StartTurnCo());
    }
```

```cs
IEnumerator StartTurnCo() {
        isLoading = true;

        myTurn = true;
        TurnStart();
        yield return new WaitForSeconds(Utils.turnStartPanelDotweenTime);
        CardMngScript.AddCardItem();
        yield return new WaitForSeconds(Utils.cardDrawDotweenTime);

        isLoading = false;
    }
```

```cs
IEnumerator EndTurnCo() {
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
```

**스테이지 시작 -> (턴 시작 -> 플레이어의 카드 조작 및 행동 결정 -> 턴 종료) -> 스테이지 종료**

**1. 스테이지의 초기값 설정 : 스테이지에서 사용할 덱과 해당 스테이지의 클리어 목표를 설정합니다. 기본적인 것들을 세팅한 뒤 덱에서 초기 손패를 몇 장 뽑습니다.**

**2. 턴 시작 : 턴 시작 연출을 재생시킵니다. 사용할 수 있는 카드의 개수를 1장 증가시킨 후, 카드를 한 장 뽑습니다.**

**3. 플레이어의 카드 조작 : 이 페이즈에서 플레이어는 아래와 같은 행동을 할 수 있습니다.**

`	`**- 손패의 카드 순서 정리**

`	`**- 사용할 카드를 손패 또는 카드보드에서 드래그하여 이동**

`	`**- 사용할 카드의 앞면, 뒷면 설정**

**4. 턴 종료 : 플레이어가 사용하기로 한 카드들의 효과를 순차적으로 적용합니다. 그 후 그리드에 배치된 오브젝트들의 효과를 순차적으로 적용합니다. 그 뒤에는 스테이지의 성공 여부를 판단한 뒤 판단이 가능할 경우 스테이지를 종료합니다. 그렇지 않을 경우 2로 돌아가게 됩니다.**

## **A. 플레이어의 카드 조작**

` `**플레이어는 마우스를 통해 사용할 카드를 뒤집거나 카드보드에 배치할 수 있습니다. 플레이어의 마우스 조작은 크게 “화면 클릭”과 “카드 클릭”으로 나눌 수 있습니다.**

```cs
private void Update() {
        DetectCardArea();
        SetCardState();

        if (Input.GetMouseButtonDown(0)) {
            if (cardState == ECardState.CanMouseDrag && !onCardArea && !onCardPutArea)
                cardExplainPanel.ScaleZero();
            else if (cardState == ECardState.CardPutUp) {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
                int layer = LayerMask.NameToLayer("Card");
                if (!Array.Exists(hits, x => x.collider.gameObject.layer == layer))
                    StartCoroutine(PutDownCard());
            }
        }

        if (cardDragging)
            DragCard();
    }
```

**화면 클릭**

**마우스 커서 위치 판단 -> 페이즈에 따른 카드 조작 가능 여부 판단 -> 자동 내려놓기 or 드래그**

```cs
public static void CardMouseUp() {
        Inst.cardDragging = false;

        if (Inst.cardState == ECardState.Nothing)
            return;
        else if (Inst.cardState == ECardState.CanMouseDrag && (!Inst.onCardArea && !Inst.onCardPutArea))
            Inst.StartCoroutine(Inst.PutUpCard());
        else if (Inst.cardState == ECardState.CardPutUp && (!Inst.onCardArea && !Inst.onCardPutArea)) {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
            if (!Array.Exists(hits, x => x.collider.gameObject.layer == LayerMask.NameToLayer("Grid")))
                Inst.StartCoroutine(Inst.SwapCard());
        }
        else {
            Inst.cardExplainPanel.ScaleZero();
            Inst.StartCoroutine(Inst.PutDownCard());
        }
    }
```

**카드 클릭(ex.CardMouseUp)**

**카드 충돌체 마우스 이벤트 발생 -> 카드 띄우기(앞뒷면을 설정하기로 결정) or 카드 뒤집기 or 카드 자동 내려놓기**

## **B. 카드의 사용과 그리드 배치**

` `**사용하고 싶은 카드들의 앞, 뒷면을 설정한 후에는 카드보드에 옮겨다 놓을 수 있습니다. 그 후 턴 종료를 하게 되면 카드보드에 있는 카드들(putCards)이 순차적으로 사용됩니다. 각각의 카드가 Default:앞면인가, DotoriSide:뒷면인가, DotoriBackSide:도토리 카드이고 뒷면인가, 에 따라 카드의 효과 발동이 달라지게 됩니다.**

```cs
public static IEnumerator ExecuteCardsCo() {
        List<CardScript> putCards = PutCards;
        CardScript card;
        
        for (int i = 0; i < putCards.Count; i++) {
            card = putCards[i];
            card.gameObject.SetActive(false);
            if (card.IsDotori && !card.IsFront)
                CardItemSO.ExecuteCardFunc(card.CardName, SO_Dotori.DotoriBackSide);
            else if (!card.IsFront)
                CardItemSO.ExecuteCardFunc(card.CardName, SO_Dotori.DotoriSide);
            else
                CardItemSO.ExecuteCardFunc(card.CardName, SO_Dotori.Default);
            Destroy(card.gameObject);
            yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
        }

        putCards.Clear();
        CardGameMngScript.IsCoroutine[0] = false;
    }
```

```cs
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
```

**기본적으로 각각의 카드 효과에 대한 명시는 하드코딩으로 하고 있습니다. 엑셀 등의 외부 문서 프로그램을 연결해서 하는 방법도 고려하였으나 카드와 특성이 각각 가로, 세로 한 줄씩을 차지하게 되는 엑셀의 특성 상 나중에 종류가 많아지면 공백이 차지하는 부분이 많아져(모든 카드가 대부분의 특성을 가지고 있는 것이 아님) 관리가 어려울 것이라 판단하였습니다. 또한, 조건문 활용이나 코드 내 함수 호출 등의 유연한 활용이 외부 프로그램으로는 어려울 것이라 생각했습니다. 작은 함수를 많이 만들어 프로그래밍을 모르는 기획자가 작업할 때 어렵지 않도록 하는 것에 초점을 맞추었습니다.**

```cs
public static void NextGridObjectUpdate() {
        for (int i = 0; i < Inst.gridObjects.Count; i++) {
            if (!Inst.gridObjects[i].HasObject) {
                Inst.nextGridObjectIndex = i;
                Inst.nextGridObject = Inst.gridObjects[i];
                return;
            }
        }
        Inst.nextGridObjectIndex = -1;
        Inst.nextGridObject = null;
    }
```

` `**그리드에 배치하는 효과를 가진 카드의 효과가 처리될 때 미리 저장해놓은 다음에 배치되어야 할 그리드의 위치 정보를 참조합니다. 정보의 갱신은 그리드에 무언가가 배치되거나 사라질 때마다 이루어집니다.**

## **C. 일반적인 요리 시스템과 그리드의 처리**

```cs
public static IEnumerator ExecuteGridObjectCo() {
        GridObjectScript    grid = null;
        GridObjectScript    donburyGrid = null;
        string              menu = null;

        for (int i = 0; i < 16; i++) {
            grid = Inst.gridObjects[i];

            if (grid.CountDown != -1) { // 요리 중인 오브젝트
                if (grid.IsNewCooking)
                    grid.IsNewCooking = false;
                else {
                    grid.CountDown -= 1;
                    if (grid.CountDown == 0) {
                        GridObjectSO.ExecuteObjectFunc(grid.ObjectName, SO_Timing.End, grid.Position[0], grid.Position[1]);
                        grid.EndCooking();
                    }
                    else
                        GridObjectSO.ExecuteObjectFunc(grid.ObjectName, SO_Timing.Effect, grid.Position[0], grid.Position[1]);
                    yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
                }
            }
            else if (grid.ObjectName == "밥솥(완료)" && (menu = Inst.DonburyCheck(ref donburyGrid)) != null) { // 밥솥(완료)
                Inst.MakeDonbury(grid, donburyGrid, menu);
                yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
            }
            else if (Inst.SO_GridObject.GetObjectItem(grid.ObjectName).tool == null) { // 도구가 아닌 오브젝트
                List<GridObjectScript> objects = GetAdjacentGridObjects(grid.Position[0], grid.Position[1]);
                foreach (var obj in objects) {
                    if (!obj.HasObject)
                        continue;

                    ObjectSubItemTool tool = Inst.SO_GridObject.GetObjectItem(obj.ObjectName).tool;
                    if (tool == null)
                        continue;
                    if (!tool.IsNeeded(grid.ObjectName, true, obj) && !tool.IsNeeded(grid.ObjectName, false, obj))
                        continue;

                    Inst.InputObject(grid.Position, obj.Position);
                    yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
                }
            }
        }

        CardGameMngScript.IsCoroutine[1] = false;
    }
```

` `**그리드에 배치되어 있는 오브젝트의 특성에 맞게 효과를 처리합니다.**

**요리 중인 오브젝트 : 카운트다운을 1 진행시키고 남은 카운트다운에 따라 효과를 처리합니다.**

**밥솥(완료) 오브젝트 : 밥을 짓는 것을 완료하게 되면 생성되는 오브젝트입니다. 이 오브젝트가 있을 시 스테이지 목표에 맞는 돈부리를 만들 수 있는 오브젝트(밥 위에 올라가는 요리 부분)이 있는지를 체크하고 있으면 돈부리를 하나 만들어 목표에 반영합니다.**

**조리 도구가 아닌 오브젝트(대부분 재료) : 주변에 오브젝트를 필요로 하는 조리 기구가 있으면 재료를 집어 넣습니다.**

` `**조리가 완료되는 경우에 125번 라인이 실행되는데 오브젝트가 조리 후 오브젝트로 넘어가는 부분은 모두 이 라인을 통해 구현됩니다. 현재 프로토타입에서는 129번 라인이 실행되지 않습니다. 그러나 추후 확장을 통해 매 턴 게임에 영향을 끼치는 오브젝트의 등장 가능성을 열어놓기 위해, 그리고 해당 기능을 추가하는 시간이 얼마 걸리지 않아 보였기 때문에 추가하게 되었습니다.**

## **D. 조미료 시스템**

```cs
public static string GetRandomNeededSpice() {
        foreach (var gridObject in GetAdjacentGridObjects()) {
            if (!gridObject.HasObject)
                continue;
            ObjectItem objectItem = GridObjectSO.GetObjectItem(gridObject.ObjectName);
            if (objectItem.tool == null)
                continue;
            if (objectItem.tool.neededSpiceNames == null)
                continue;
            if (objectItem.tool.neededSpiceNames.Count != gridObject.CurrentObjectItem.currentSpiceNames.Count) {
                int index = -1;
                string spice = null;
                do {
                    index = Random.Range(0, objectItem.tool.neededSpiceNames.Count);
                    spice = objectItem.tool.neededSpiceNames[index];
                    if (gridObject.AdjacentObjectCheck(spice)) {
                        spice = null;
                        if (objectItem.tool.neededSpiceNames.Count == gridObject.CurrentObjectItem.currentSpiceNames.Count + 1)
                            return "조미료";
                    }
                } while (gridObject.CurrentObjectItem.currentSpiceNames.Contains(spice) || spice == null);
                return spice;
            }
        }
        return "조미료";
    }
```

**조미료 카드가 사용될 때 실행되는 함수입니다. 함수는 결과적으로 배치될 오브젝트의 이름을 리턴합니다. 한 턴에 여러 장의 조미료 카드가 사용되고 한 개의 조리 도구의 인접한 그리드들에 두 개 이상의 조미료 카드로부터 나온 오브젝트가 배치될 예정인 경우 같은 이름의 조미료 오브젝트가 생성되지 않도록 해 주고 있습니다.**
