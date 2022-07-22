using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ProductState {
    PS_UNBOUGHT,
    PS_BOUGHT
}

public class Product {
    public Sprite           sprite;
    public string           name;
    public string           explain;
    public int              price;
    public ProductState     state;
}

public class CampingShopMngScript : MonoBehaviour {
    static public CampingShopMngScript  Inst { get; set; } = null;

    [SerializeField] Sprite             productEmptySprite;
    [SerializeField] Sprite             productBoughtSprite;
    [SerializeField] Sprite[]           objectSprites;
    [SerializeField] Sprite[]           backGroundSprites;
    [SerializeField] Sprite[]           CD_Sprites;
    [SerializeField] BuyButtonScript[]  buyButtons;
    [SerializeField] Image[]            productImages;
    [SerializeField] Text[]             productNameTexts;
    [SerializeField] Text[]             productExplainTexts;
    [SerializeField] Text[]             productPriceTexts;

    List<Product>                       productList = new List<Product>();
    int                                 topProductIndex;

    private void Awake() => Inst = this;
    void Start() {
        Init();
        gameObject.SetActive(false);
    }

    static public List<Product>         ProductList => Inst.productList;

    public int TopProductIndex {
        get => Inst.topProductIndex;
        set {
            Inst.topProductIndex = value;
            for (int i = TopProductIndex; i < TopProductIndex + Inst.productImages.Length; i++) {
                if (i >= Inst.productList.Count) {
                    Inst.buyButtons[i - TopProductIndex].GetComponent<Button>().interactable = false;
                    Inst.productImages[i - TopProductIndex].sprite = Inst.productEmptySprite;
                    Inst.productNameTexts[i - TopProductIndex].text = "";
                    Inst.productExplainTexts[i - TopProductIndex].text = "";
                    Inst.productPriceTexts[i - TopProductIndex].text = "";
                    continue;
                }
                switch (Inst.productList[i].state) {
                    case ProductState.PS_UNBOUGHT:
                        Inst.buyButtons[i - TopProductIndex].GetComponent<Button>().interactable = true;
                        Inst.buyButtons[i - TopProductIndex].Price = Inst.productList[i].price;
                        Inst.buyButtons[i - TopProductIndex].ProductIndex = i;
                        Inst.productImages[i - TopProductIndex].sprite = Inst.productList[i].sprite;
                        Inst.productNameTexts[i - TopProductIndex].text = Inst.productList[i].name;
                        Inst.productExplainTexts[i - TopProductIndex].text = Inst.productList[i].explain;
                        Inst.productPriceTexts[i - TopProductIndex].text = "도토리 : " + Inst.productList[i].price.ToString();
                        break;
                    case ProductState.PS_BOUGHT:
                        Inst.buyButtons[i - TopProductIndex].GetComponent<Button>().interactable = false;
                        Inst.productImages[i - TopProductIndex].sprite = Inst.productBoughtSprite;
                        Inst.productNameTexts[i - TopProductIndex].text = Inst.productList[i].name;
                        Inst.productExplainTexts[i - TopProductIndex].text = Inst.productList[i].explain;
                        Inst.productPriceTexts[i - TopProductIndex].text = "도토리 : " + Inst.productList[i].price.ToString();
                        break;
                }
            }
        }
    }

    static public void Buy(int _index) {
        Inst.productList[_index].state = ProductState.PS_BOUGHT;
        Inst.TopProductIndex = Inst.TopProductIndex;
    }

    void Init() {
        Product product = new Product();
        product.sprite = objectSprites[0];
        product.name = "테스트용 상자";
        product.explain = "상품 적용 테스트를 위한 상자이다.";
        product.price = 2;
        productList.Add(product);
        TopProductIndex = 0;
    }

    public void GoToLeft() {
        if (TopProductIndex >= 3)
            TopProductIndex -= 3;
    }

    public void GoToRight() {
        if (TopProductIndex + 3 < productList.Count)
            TopProductIndex += 3;
    }
}
