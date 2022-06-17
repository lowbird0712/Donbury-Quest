using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using IEnumerator = System.Collections.IEnumerator;

public class CardMngScript : MonoBehaviour {
    public static CardMngScript         Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] SO_CardItemScript  SO_cardItem;
    [SerializeField] GameObject         cardPrefab;
    
    [SerializeField] Transform          cardLeftLine;
    [SerializeField] Transform          cardRightLine;
    [SerializeField] Transform          cardSpawnPoint;
    [SerializeField] Transform          cardPutUpPoint;
    [SerializeField] GameObject         cardPutCover;
    [SerializeField] Transform          cardPutLeftLine;
    [SerializeField] Transform          cardPutRightLine;
    [SerializeField] CardScript         emptyCard;

    enum ECardState                     { Nothing, OnlyMouseClick, CanMouseDrag, CardPutUp }
    enum ECardMode                      { DEFAULT, MYCARDS, PUTCARDS }
    ECardState                          cardState;

    string                              deckName;
    List<string>                        cardBuffer = new List<string>();
    List<CardScript>                    myCards = new List<CardScript>();
    List<CardScript>                    putCards = new List<CardScript>();
    CardScript                          draggingCard;
    bool                                cardDragging;
    bool                                onCardArea;
    bool                                onCardPutArea;
    bool                                onPutUpCardArea;
    bool                                onGridObjectArea;
    bool                                onUIArea;
    bool                                cardPuttingUp;
    float                               oneCardPutWidth;
    float                               oneCardPutX;
    int                                 cardPutCount;

    public static SO_CardItemScript     CardItemSO => Inst.SO_cardItem;
    public static string                DeckName { set => Inst.deckName = value; }
    public static List<string>          CardBuffer => Inst.cardBuffer;
    public static List<CardScript>      PutCards => Inst.putCards;
    public static bool                  OnCardArea { set => Inst.onCardArea = value; }
    public static float                 OneCardPutWidth => Inst.oneCardPutWidth;
    public static float                 OneCardPutX => Inst.oneCardPutX;

    private void Start() => cardPutCount = CardGameMngScript.StartPutCardCount;

    private void Update() {
        DetectCardArea();
        SetCardState();

        if (Input.GetMouseButtonDown(0)) {
            if (cardState == ECardState.CanMouseDrag && !onCardArea && !onCardPutArea && !onGridObjectArea)
                CardGameMngScript.CardExplainPanel.ScaleZero();
            else if (cardState == ECardState.CardPutUp && !onUIArea) {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
                int layer = LayerMask.NameToLayer("Card");
                if (!Array.Exists(hits, x => x.collider.gameObject.layer == layer))
                    StartCoroutine(PutDownCard());
            }
        }

        if (cardDragging)
            DragCard();
    }

    public static void Init(int _stageNum) {
        switch (_stageNum) {
            case 0:
                Inst.deckName = "규동 기본";
                break;
        }
        Inst.SetupCardItemSO();
        Inst.SetupCardBuffer();
        InitCardPutArea(CardGameMngScript.StartPutCardCount);
    }

    public static CardItem GetCardItem(string _cardName) {
        return Inst.SO_cardItem.GetCardItem(_cardName);
    }

    public static void AddCardItem() {
        if (Inst.myCards.Count == CardGameMngScript.MaxCardCount)
            return;

        var cardClone = Instantiate(Inst.cardPrefab, Inst.cardSpawnPoint.position, Quaternion.identity);
        var cardItem = cardClone.GetComponent<CardScript>();
        cardItem.Setup(Inst.PopCard());
        Inst.myCards.Add(cardItem);

        Inst.SetCardOriginOrders(ECardMode.MYCARDS);
        Inst.AlignCards(Utils.cardDrawDotweenTime, ECardMode.MYCARDS);
    }

    public static void InsertCardItem(string _cardName, int _index) {
        if (_index >= Inst.cardBuffer.Count)
            return;

        Inst.cardBuffer.Insert(_index, _cardName);
	}

    #region Card & Mouse
    public static void CardMouseDown(CardScript _card) {
        if (Inst.cardState == ECardState.Nothing)
            return;
        else if (Inst.cardState == ECardState.CardPutUp && (Inst.onCardArea || Inst.onCardPutArea))
            Inst.StartCoroutine(Inst.PutDownCard());
        else if (Inst.cardState == ECardState.CanMouseDrag) {
            Inst.draggingCard = _card;
            Inst.cardDragging = true;
            string explainText = GetCardItem(_card.CardName).explain;
            CardGameMngScript.CardExplainPanel.Show(explainText);
        }
        else if (Inst.cardState == ECardState.CardPutUp) {
            Inst.draggingCard = _card;
            Inst.cardDragging = true;
        }
    }

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
            CardGameMngScript.CardExplainPanel.ScaleZero();
            Inst.StartCoroutine(Inst.PutDownCard());
        }
    }
    #endregion

    public static void InitCardPutArea(int _startCount) {
        Inst.oneCardPutWidth = Inst.cardPutCover.transform.localScale.x / CardGameMngScript.MaxPutCardCount;
        Inst.oneCardPutX = (Inst.cardPutRightLine.position - Inst.cardPutLeftLine.position).x / CardGameMngScript.MaxPutCardCount / 2;
        Inst.cardPutCover.transform.localScale -= Vector3.right * Inst.oneCardPutWidth * _startCount;
        Inst.cardPutCover.transform.localPosition += Vector3.right * Inst.oneCardPutX * _startCount;
        Inst.cardPutRightLine.position -= Vector3.right * Inst.oneCardPutX * (CardGameMngScript.MaxPutCardCount - _startCount) * 2;
    }

    public static void IncreaseCardPutCount() {
        if (Inst.cardPutCount == CardGameMngScript.MaxPutCardCount)
            return;

        Inst.cardPutCount += 1;
        Inst.cardPutCover.GetComponent<CardPutCoverScript>().RightSwipe();
        Inst.cardPutRightLine.position += Vector3.right * Inst.oneCardPutX * 2;
    }

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

    void SetupCardItemSO() {
        SO_cardItem.SetUp(deckName);
        GridObjectMngScript.GridObjectSO.SetUp(deckName);
    }

    void SetupCardBuffer() {
        if (deckName == "규동 기본") {
            cardBuffer = new List<string>(15); ////15
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("밥솥");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("쌀");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("불지피기");
            cardBuffer.Add("냄비");
            cardBuffer.Add("냄비");
            cardBuffer.Add("양파");
            cardBuffer.Add("양파");
            cardBuffer.Add("우삼겹");
            cardBuffer.Add("우삼겹");

            for (int i = 0; i < cardBuffer.Count; i++) {
                int rand = Random.Range(i, cardBuffer.Count);
                string tmp = cardBuffer[i];
                cardBuffer[i] = cardBuffer[rand];
                cardBuffer[rand] = tmp;
            }
        }
    }

    string PopCard() {
        if (cardBuffer.Count == 0)
            SetupCardBuffer();

        string cardName = cardBuffer[0];
        cardBuffer.RemoveAt(0);

        return cardName;
    }

    void SetCardOriginOrders(ECardMode _mode) {
        List<CardScript> target = new List<CardScript>();
        switch (_mode) {
            case ECardMode.MYCARDS:
                target = myCards;
                break;
            case ECardMode.PUTCARDS:
                target = putCards;
                break;
        }

        for (int i = 0; i < target.Count; i++) {
            var targetCard = target[i];
            targetCard?.GetComponent<CardOrderScript>().SetOriginOrder(i);
        }
    }

    void AlignCards(float _dotween, ECardMode _mode, bool _move = true) {
        List<PRS>           originPRSs = new List<PRS>();
        List<CardScript>    targetCards = new List<CardScript>();

        switch(_mode) {
            case ECardMode.MYCARDS:
                targetCards = myCards;
                originPRSs = GetCardOriginPRSs(targetCards.Count, Utils.cardScale, ECardMode.MYCARDS);
                break;
            case ECardMode.PUTCARDS:
                targetCards = putCards;
                originPRSs = GetCardOriginPRSs(targetCards.Count, Utils.cardScale, ECardMode.PUTCARDS);
                break;
        }

        for (int i = 0; i < targetCards.Count; i++) {
            var targetCard = targetCards[i];
            targetCard.OriginPRS = originPRSs[i];

            if (!targetCard.IsFront && putCards.Contains(targetCard))
                targetCard.OriginPRS.rot = Quaternion.AngleAxis(180, Vector3.up);

            if (_move)
                targetCard.MoveTransform(targetCard.OriginPRS, _dotween, Ease.Linear);  
        }
    }

    List<PRS> GetCardOriginPRSs(int _objCount, Vector3 _scale, ECardMode _mode) {
        float[] objLerps = new float[_objCount];
        List<PRS> result = new List<PRS>(_objCount);
        Transform leftLine = null;
        Transform rightLine = null;

        switch (_mode) {
            case ECardMode.MYCARDS:
                leftLine = cardLeftLine;
                rightLine = cardRightLine;

                switch (_objCount) {
                    case 1:
                        objLerps[0] = 0.5f;
                        break;
                    case 2:
                        objLerps[0] = 0.27f;
                        objLerps[1] = 0.73f;
                        break;
                    case 3:
                        objLerps[0] = 0.1f;
                        objLerps[1] = 0.5f;
                        objLerps[2] = 0.9f;
                        break;
                    default:
                        float interval = 1.0f / (_objCount - 1);
                        for (int i = 0; i < _objCount; i++)
                            objLerps[i] = interval * i;
                        break;
                }
                break;
            case ECardMode.PUTCARDS:
                leftLine = cardPutLeftLine;
                rightLine = cardPutRightLine;

                float interval2 = 1.0f / (_objCount + 1);
                for (int i = 0; i < _objCount; i++) {
                    objLerps[i] = interval2 + interval2 * i;
                }
                break;
        }

        for (int i = 0; i < _objCount; i++) {
            var originPos = Vector3.Lerp(leftLine.position, rightLine.position, objLerps[i]);
            var originRot = Quaternion.identity;
            result.Add(new PRS(originPos, originRot, Utils.cardScale));
        }

        return result;
    }

    void DragCard() {
        bool flag = false;

        if (cardState == ECardState.CanMouseDrag)
            flag = true;
        else if (cardState == ECardState.CardPutUp && !onPutUpCardArea) {
            flag = true;
            cardPuttingUp = false;
            string explainText = GetCardItem(draggingCard.CardName).explain;
            CardGameMngScript.CardExplainPanel.Show(explainText);
        }

        if (flag) {
            if (draggingCard.IsPut && putCards.Contains(draggingCard))
                putCards.Remove(draggingCard);
            else if (!draggingCard.IsPut && myCards.Contains(draggingCard))
                myCards.Remove(draggingCard);

            draggingCard.GetComponent<CardOrderScript>().SetMostFrontOrder(true);
            if (draggingCard.IsFront)
                draggingCard.MoveTransform(new PRS(Utils.MousePos, Quaternion.identity, Utils.cardScale * Utils.cardDragFloat));
            else
                draggingCard.MoveTransform(new PRS(Utils.MousePos, Quaternion.AngleAxis(180, Vector3.up), Utils.cardScale * Utils.cardDragFloat));

            if (onCardArea)
                InsertEmptyCard(Utils.MousePos.x, ECardMode.MYCARDS);
            else if (onCardPutArea)
                InsertEmptyCard(Utils.MousePos.x, ECardMode.PUTCARDS);
            else if (myCards.Contains(emptyCard)) {
                myCards.Remove(emptyCard);
                AlignCards(Utils.cardAlignmentDotweenTime, ECardMode.MYCARDS);
            }
            else if (putCards.Contains(emptyCard)) {
                putCards.Remove(emptyCard);
                AlignCards(Utils.cardAlignmentDotweenTime, ECardMode.PUTCARDS);
            }
        }
    }

    void DetectCardArea() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);

        int layer = LayerMask.NameToLayer("Card Area");
        onCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);

        layer = LayerMask.NameToLayer("Card Put Area");
        onCardPutArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer &&
            (cardPutLeftLine.position.x <= Utils.MousePos.x && Utils.MousePos.x <= cardPutRightLine.position.x));

        if (cardState == ECardState.CardPutUp) {
            layer = LayerMask.NameToLayer("Card");
            onPutUpCardArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer && x.collider.GetComponent<CardScript>() == draggingCard);
        }

        layer = LayerMask.NameToLayer("Grid Object Area");
        onGridObjectArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);

        layer = LayerMask.NameToLayer("UI");
        onUIArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
        if (onUIArea)
            Debug.Log("Detected");
    }

    void SetCardState() {
        if (CardGameMngScript.Inst.isLoading)
            cardState = ECardState.Nothing;
        else {
            if (!CardGameMngScript.MyTurn)
                cardState = ECardState.OnlyMouseClick;
            if (CardGameMngScript.MyTurn)
                cardState = ECardState.CanMouseDrag;
            if (CardGameMngScript.MyTurn && cardPuttingUp)
                cardState = ECardState.CardPutUp;
        }
    }

    void InsertEmptyCard(float _x, ECardMode _mode) {
        List<CardScript> cards = null;
        int maxCardCount = -1;

        switch(_mode) {
            case ECardMode.MYCARDS:
                cards = myCards;
                maxCardCount = CardGameMngScript.MaxCardCount;
                break;
            case ECardMode.PUTCARDS:
                cards = putCards;
                maxCardCount = cardPutCount;
                break;
        }

        if (cards.Count == maxCardCount)
            return;

        if (!cards.Contains(emptyCard))
            cards.Add(emptyCard);

        Vector3 tmp = emptyCard.transform.position;
        tmp.x = _x;
        emptyCard.transform.position = tmp;

        int index = GetEmptyCardIndex(_mode);
        cards.Sort((card1, card2) => card1.transform.position.x.CompareTo(card2.transform.position.x));
        if (GetEmptyCardIndex(_mode) != index || GetEmptyCardIndex(_mode) == cards.Count - 1)
            AlignCards(Utils.cardAlignmentDotweenTime, _mode);
    }

    int GetEmptyCardIndex(ECardMode _mode) {
        switch (_mode) {
            case ECardMode.MYCARDS:
                return myCards.FindIndex(x => x == emptyCard);
            case ECardMode.PUTCARDS:
                return putCards.FindIndex(x => x == emptyCard);
            default:
                return -1;
        }
    }

    IEnumerator PutUpCard() {
        CardGameMngScript.Inst.isLoading = true;

        CardGameMngScript.CardExplainPanel.ScaleZero();

        PRS putUpPRS;
        if (draggingCard.IsFront)
            putUpPRS = new PRS(cardPutUpPoint.position, Quaternion.identity, Utils.cardScale * Utils.cardPutUpFloat);
        else
            putUpPRS = new PRS(cardPutUpPoint.position, Quaternion.AngleAxis(180, Vector3.up), Utils.cardScale * Utils.cardPutUpFloat);
        draggingCard.MoveTransform(putUpPRS, Utils.cardPutUpDownDotweenTime, Ease.OutQuad);

        yield return new WaitForSeconds(Utils.cardPutUpDownDotweenTime);

        cardPuttingUp = true;

        CardGameMngScript.Inst.isLoading = false;
    }

    IEnumerator PutDownCard() {
        CardGameMngScript.Inst.isLoading = true;

        ECardMode mode = ECardMode.DEFAULT;

        if (cardState == ECardState.CanMouseDrag && onCardArea) {
            mode = ECardMode.MYCARDS;
            if (!myCards.Contains(emptyCard)) {
                StartCoroutine(PutUpCard());
                yield break;
            }
            draggingCard.IsPut = false;
            myCards[GetEmptyCardIndex(mode)] = draggingCard;
        }
        else if (cardState == ECardState.CanMouseDrag && onCardPutArea) {
            mode = ECardMode.PUTCARDS;
            if (!putCards.Contains(emptyCard)) {
                StartCoroutine(PutUpCard());
                yield break;
            }
            else if (draggingCard.IsFront == true && !SO_cardItem.GetUsable(draggingCard.CardName)) {
                putCards.Remove(emptyCard);
                AlignCards(Utils.cardAlignmentDotweenTime, ECardMode.PUTCARDS);
                StartCoroutine(PutUpCard());
                yield break;
            }
            draggingCard.IsPut = true;
            putCards[GetEmptyCardIndex(mode)] = draggingCard;
        }
        else if (cardState == ECardState.CardPutUp) {
            if (draggingCard.IsPut) {
                mode = ECardMode.PUTCARDS;
                putCards.Add(draggingCard);
            }
            else {
                mode = ECardMode.MYCARDS;
                myCards.Add(draggingCard);
            }
		}

        SetCardOriginOrders(mode);
        AlignCards(Utils.cardPutUpDownDotweenTime, mode);

        yield return new WaitForSeconds(Utils.cardPutUpDownDotweenTime / 2);

        if (!draggingCard.IsFront && myCards.Contains(draggingCard))
			draggingCard.Swap();

        yield return new WaitForSeconds(Utils.cardPutUpDownDotweenTime / 2);

        draggingCard = null;
        cardPuttingUp = false;

        CardGameMngScript.Inst.isLoading = false;
    }

    IEnumerator SwapCard() {
        CardGameMngScript.Inst.isLoading = true;

        PRS swapPRS = new PRS(draggingCard.transform.position, draggingCard.transform.rotation * Quaternion.AngleAxis(90, Vector3.up), Utils.cardScale * Utils.cardPutUpFloat);
        draggingCard.MoveTransform(swapPRS, Utils.cardSwapDotweenTime, Ease.Linear);

        yield return new WaitForSeconds(Utils.cardSwapDotweenTime);

        draggingCard.Swap();
        swapPRS.rot = draggingCard.transform.rotation * Quaternion.AngleAxis(90, Vector3.up);
        draggingCard.MoveTransform(swapPRS, Utils.cardSwapDotweenTime, Ease.Linear);

        yield return new WaitForSeconds(Utils.cardSwapDotweenTime);

        CardGameMngScript.Inst.isLoading = false;
    }
}
