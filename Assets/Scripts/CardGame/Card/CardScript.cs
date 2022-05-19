using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CardScript : DotweenMovingScript {
    [SerializeField] SpriteRenderer cardRenderer;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TMP_Text       nameTMP;
    [SerializeField] Sprite         cardFrontSprite;
    [SerializeField] Sprite         cardBackSprite;

    string                          cardName;
    int                             selectedGridNum;
    bool                            isFront = true;
    bool                            isDotori = false;
    bool                            isPut;
    PRS                             originPRS;

    public string                   CardName => cardName;
    public int                      SelectedGridNum { get => selectedGridNum; set { selectedGridNum = value; } }
    public bool                     IsFront => isFront;
    public bool                     IsDotori => isDotori;
    public bool                     IsPut { get => isPut; set { isPut = value; } }
    public PRS                      OriginPRS { get => originPRS; set => originPRS = value; }

    #region Card & Mouse
    private void OnMouseDown() => CardMngScript.CardMouseDown(this);

    private void OnMouseUpAsButton() => CardMngScript.CardMouseUp();
    #endregion

    public void Setup(string _cardName) {
        cardName = _cardName;
        CardItem cardItem = CardMngScript.GetCardItem(cardName);
        cardRenderer.sprite = cardFrontSprite;
        spriteRenderer.sprite = cardItem.sprite;
        nameTMP.text = cardName;
        if (cardItem.isDotori)
            isDotori = true;
    }

    public void Swap() {
        if (isFront) {
            cardRenderer.sprite = cardBackSprite;
            spriteRenderer.sprite = null;
            nameTMP.text = "";
            isFront = false;
        }
        else {
            var cardItem = CardMngScript.GetCardItem(cardName);
            cardRenderer.sprite = cardFrontSprite;
            spriteRenderer.sprite = cardItem.sprite;
            nameTMP.text = cardName;
            isFront = true;
        }
    }
}
