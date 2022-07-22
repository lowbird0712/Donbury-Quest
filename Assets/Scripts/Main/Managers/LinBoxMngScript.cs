using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item {
    public Sprite sprite;
    public string name;
    public string explain;
}

public class LinBoxMngScript : MonoBehaviour {
    static public LinBoxMngScript Inst { get; set; } = null;

    [SerializeField] Sprite                 itemEmptySprite;
    [SerializeField] UseItemButtonScript[]  useItemButtons;
    [SerializeField] Image[]                itemImages;
    [SerializeField] Text[]                 itemNameTexts;
    [SerializeField] Text[]                 itemExplainTexts;

    List<Item>                              itemList = new List<Item>();
    int                                     topItemIndex;

    private void Awake() => Inst = this;
    void Start() => gameObject.SetActive(false);

    public int TopItemIndex {
        get => topItemIndex;
        set {
            topItemIndex = value;
            for (int i = topItemIndex; i < topItemIndex + itemImages.Length; i++) {
                if (i >= itemList.Count) {
                    useItemButtons[i - topItemIndex].GetComponent<Button>().interactable = false;
                    itemImages[i - topItemIndex].sprite = itemEmptySprite;
                    itemNameTexts[i - topItemIndex].text = "";
                    itemExplainTexts[i - topItemIndex].text = "";
                    continue;
                }
                useItemButtons[i - topItemIndex].GetComponent<Button>().interactable = true;
                useItemButtons[i - topItemIndex].ItemIndex = i;
                itemImages[i - topItemIndex].sprite = Inst.itemList[i].sprite;
                itemNameTexts[i - topItemIndex].text = Inst.itemList[i].name;
                itemExplainTexts[i - topItemIndex].text = Inst.itemList[i].explain;
            }
        }
    }

    static public Item GetItem(int _index) => Inst.itemList[_index];

    public void Init() {
        List<Product> pruducts = CampingShopMngScript.ProductList;
        itemList.Clear();
        foreach (var product in pruducts) {
            if (product.state != ProductState.PS_BOUGHT)
                continue;
            Item item = new Item();
            item.sprite = product.sprite;
            item.name = product.name;
            item.explain = product.explain;
            itemList.Add(item);
        }
        TopItemIndex = 0;
    }

    public void GoToLeft() {
        if (topItemIndex >= 3)
            TopItemIndex -= 3;
    }

    public void GoToRight() {
        if (topItemIndex + 3 < itemList.Count)
            TopItemIndex += 3;
    }
}
