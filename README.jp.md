# **丼ものクェスト技術文書**
[![kr](https://img.shields.io/badge/lang-kr-red.svg)](https://github.com/lowbird0712/Donbury-Quest)

## **丼ものクェストはどんなケーム?**

**丼ものクェストは”料理するカードゲーム”をテーマとしています. 既存の料理ゲームたちのシステムが実際の調理法との連関性が足りず、その結果料理のテーマが違うにもかかわらず似た方式のゲームプレイをすることになるところに焦点を合わせて考案いたしました.**

**(現在、一部ロジックのUniRX/UniTask化を行っています)**

[**プロトタイプのプレイ動画**](https://github.com/lowbird0712/Projects-before-Donbury-Quest/tree/master/Prototype%20Videos)

[**全体コード**](https://github.com/lowbird0712/Donbury-Quest/tree/master/Assets/Scripts)

## **略したルールとシステムへの説明**

**- プレイヤーは料理を完成するために”調理道具”、“食材”、調理工程を示す“調理”、そして料理の特性を示した“ユーティル”カードを使用しゲームを進行することになります.プレイヤーは使用するカードを“カードボード”に配置でき、要求する料理たちを与えられたターン数内に完成しなければなりません. (A)**

**- プレイヤーが調理道具、食材、そして一部ユーティルカードを使用すると4x4フィールドの資格のマース一つ(後、“グリッド”とする)に左上から順番に配置されます. グリッドに配置された物体たちを“オブジェクト”と言います. グリッドにオブジェクトを配置しないカードを“スペル”カードだと言います. (B)**

**- 全てのカードは前、もしくは後で使え、前で使う場合各カードの効果が発動、後で使う場合各カードとつながっている“どんぐりカード”に変え、手札にくあわることになります. どんぐりカードはスペルカードやスペルじゃないカードになります. どんぐりカードは一般的に前だとグリッドを埋め、該当グリッドに調理道具と食材が配置されないようにするために、後ろだと手札を回させるために使われます. この方式でグリッドに配置されたオブジェクトを“どんぐりオブジェクト”だと言います. (B)**

**- 料理の過程は“米たて”と“料理作り”に分けられます. 米をたてるためには“がま”をグリッドに配置してから“米”を入れ“火づけ”を使って調理しなければなりません. 全ての料理の調理工程には“スパイス”が何種類必要でこれは米をたてるために必要なカードたちとつながっているどんぐりカード、“どんぐりがま”、“どんぐり”、“どんぐり焼き”を使って調理して得ることができます. (C)**

**- 特定のどんぐりカードたちは何個かのどんぐりオブジェクトを消耗してこそ前で使うことができます. この場合、必要数分のどんぐりオブジェクトが消えカードの効果が発動します. 開けられたグリッドにはまたオブジェクトを配置することができます. プロトタイプ映像ではこの機能がまだ現れていません.**

**- スパイスカードを前で使う場合、他のオブジェクトたちみたいに次に配置さるべきのグリッドの位置に配置されます. この時、隣接したグリッドにスパイスを必要とするオブジェクト(ほとんど調理道具)があればその中から該当オブジェクトにまだ入っていない一つをランダムで配置します. ない場合には“スパイス”オブジェクトを配置します. スパイスオブジェクトはどんぐりオブジェクトみたいに基本的にはグリッドを埋める役目をするが、隣接したグリッドに調理道具がある場合、該当調理道具の中に消え、グリッドが開けられることになります. (D)**

## **全体的なプロジェクトの構造**

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
            StageClear()
        else
            StartCoroutine(StartTurnCo());
        isCoroutine.Clear();
    }
```

**ステージ開始 -> (ターン開始 -> プレイヤーのカード配置や行動の決定 -> ターン終了) -> ステージ終了**

**1. ステージの初期値設定 : ステージで使うデッキと該当ステージのクリア目標を設定します. 基本的なことをセッティングしてからデッキから初手を何枚引きます.**

**2. ターン開始 : ターン開始の演出を再生します. 使えるカードの枚数を1枚増加して、カードを1枚引きます.**

**3. プレイヤーのカード配置 : このフェーズでプレイヤーは下の行動ができます.**

`	`**- 手札のカードの順番の整理**

`	`**- 使うカードを手札もしくはカードボードからドラッグして移動**

`	`**- 使うカードの前、後ろ設定**

**4. ターン終了 : プレイヤーが使おうと決めたカードたちの効果を順番に適用します. その後グリッドに配置されたオブジェクトの効果を順番に適用します. それから、ステージの成功失敗を判断して判断ができる場合、ステージを終了します. そうじゃない場合、2に戻ります.**

## **A. プレイヤーのカード配置**

` `**プレイヤーはマウスで使うカードをひっくり返したりカードボードに配置することができます. プレイヤーのマウス操作は大きく“画面クリック”と“カードクリック”に分けることができます.**

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

**画面クリック**

**マウスカーサーの位置の判断 -> フェーズによるカード操作の可否を判断 -> 自動的なカード戻り or カードのドラッグ**

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

**カードのクリック(ex.CardMouseUp)**

**カード衝突体のマウスイベント発生 -> カードが浮かせる(前後ろを設定することを決定) or カードをひっくり返す or 自動的なカード戻り**

## **B. カードの使用とグリッド配置**

` `**使いたいカードの前、後ろを設定してからはカードボードに動き運ぶことができます. その後、ターン終了をするとカードボードにあるカードたち(putCards)が順番に使われます. 各自のカードがDefault:前か、DotoriSide:後ろか、DotoriBackSide:どんぐりカードで後ろか、によってカードの効果発動が違くなります.**

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

            // 基本
            if (_cardName == "がま")
                GridObjectMngScript.PlaceObject("がま(準備)", x, y);
            else if (_cardName == "火づけ") {
                GridObjectMngScript.Spell("火づけ", x, y);
                GridObjectMngScript.StartCooking(x, y);
            }
            else if (_cardName == "米")
                GridObjectMngScript.PlaceObject("米", x, y);
            else if (_cardName == "スパイス") {
                if (IsSpiceToolExist())
                    GridObjectMngScript.PlaceObject(GridObjectMngScript.GetRandomNeededSpice(), x, y);
                else
                    GridObjectMngScript.PlaceObject("スパイス", x, y); ;
            }

            // 基本どんぐり
            else if (_cardName == "どんぐりがま")
                GridObjectMngScript.PlaceObject("どんぐりがま(準備)", x, y);
            else if (_cardName == "どんぐり")
                GridObjectMngScript.PlaceObject("どんぐり", x, y);
            else if (_cardName == "どんぐり焼き") {
                GridObjectMngScript.Spell("どんぐり焼き", x, y);
                GridObjectMngScript.StartCooking(x, y);
            }
            else if (_cardName == "芳しいどんぐり")
                GridObjectMngScript.PlaceObject("どんぐり", x, y);

            // 牛丼基本
            else if (_cardName == "鍋")
                GridObjectMngScript.PlaceObject("鍋(準備)", x, y);
            else if (_cardName == "玉ねぎ")
                GridObjectMngScript.PlaceObject("玉ねぎ", x, y);
            else if (_cardName == "牛バラ")
                GridObjectMngScript.PlaceObject("牛バラ", x, y);

            // どんぐりカード
            else
                GridObjectMngScript.PlaceObject(_cardName, x, y);
        }
        else if (_dotoriFlag == SO_Dotori.DotoriSide)
            ChangeToDotori(_cardName);
        else if (_dotoriFlag == SO_Dotori.DotoriBackSide)
            DotoriReturn(_cardName);
    }
```

**基本的に各自のカード効果に関する明示はカードコーディングにしています. エクセルなどの外部文章プログラムを連動してやる方法も考慮したが、ガードとカード効果の特性が各自横縦一本づつ占めることになるエクセルの特性上、後で種類が多くなれば空白が占める部分が多くなり(全てのカードがほとんどの特性を持っているわけではない)、管理が難しくなると思いました. また、条件文活用やコード内関数呼びたしだどの柔軟な活用が外部プログラムでは難しいと思いました. 小さい関数を多く作ってプログリミングを知らないゲームデザイナーが作業する時に難しくないことに重点を当てました.**

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

` `**グリッドに配置する効果を持っているカードの効果が処理される時、すでにセーブした次に配置されるべきのグリッドの位置情報をリファレンスします. 情報の更新はグリッドに何かが配置されたり消えるたびに行われます.**

## **C. 一般的な料理システムとグリッドの処理**

```cs
public static IEnumerator ExecuteGridObjectCo() {
        GridObjectScript    grid = null;
        GridObjectScript    donburyGrid = null;
        string              menu = null;

        for (int i = 0; i < 16; i++) {
            grid = Inst.gridObjects[i];

            if (grid.CountDown != -1) { // 料理中のオブジェクト
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
            else if (grid.ObjectName == "がま(完了)" && (menu = Inst.DonburyCheck(ref donburyGrid)) != null) { // がま(完了)
                Inst.MakeDonbury(grid, donburyGrid, menu);
                yield return new WaitForSeconds(Utils.cardExecDotweenTime * 2);
            }
            else if (Inst.SO_GridObject.GetObjectItem(grid.ObjectName).tool == null) { // 道具じゃないオブジェクト
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

` `**グリッドに配置されているオブジェクトの特性に当てて効果を処理します.**

**料理中のオブジェクト : カウントダウンを1進め、残りのカウントダウンによって効果を処理します.**

**がま(完了)オブジェクト : 米をたてることを完了すると生成されるオブジェクトです. このオブジェクトがある時、ステージの目情に合う丼を作れるオブジェクト(丼の頭部分)があるかをチェックし、あれば丼を一つ作って目標に反映します.**

**調理道具じゃないオブジェクト(おとんど食材) : 周りに該当のオブジェクトを必要とする調理道具があればオブジェクトを入れます.**

` `**調理が完了する時に125番ラインが実行されます. オブジェクトが調理後オブジェクトに渡る部分は全て個のラインを通して具現されます. 現在のプロトタイプでは129番ラインが実行されません. しかし、今後の拡張で毎ターンゲームに影響を与えるオブジェクトの登場可能性を開けておくために、そして該当の機能を追加する時間があまりかからなさそうに見えたので追加することになりました.**

## **D. スパイスシステム**

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
                            return "スパイス";
                    }
                } while (gridObject.CurrentObjectItem.currentSpiceNames.Contains(spice) || spice == null);
                return spice;
            }
        }
        return "スパイス";
    }
```

**スパイスカードが使われた時に実行される関数です. 関数は結果的に配置されるオブジェクトの名前をリターンします. 一ターンに複数枚のスパイスカードが使われ、一つの調理度具の隣接したグリッドたちに複数個以上のスパイスカードから出たオブジェクトが配置される予定の場合、同名のスパイスオブジェクトが生成されないようにしています.**
