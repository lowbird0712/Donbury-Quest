using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct ProductButtonInfo {
    public Image   productPanel;
    public Text    productName;
    public Text    productCost;
    public Button  buyButton;
    public Image   productImage;
    public Image   unbuyableImage;
}

public class ShopMngScript : MonoBehaviour {
    static public ShopMngScript     Inst { get; set; } = null;

    [SerializeField]
    Image                           mShopPanel = null;
    [SerializeField]
    Image[]                         mProductPanels = null;
    [SerializeField]
    Text[]                          mProductNames = null;
    [SerializeField]
    Text[]                          mProductCosts = null;
    [SerializeField]
    Button[]                        mBuyButtons = null;
    [SerializeField]
    Image[]                         mProductImages = null;
    [SerializeField]
    Image[]                         mUnbuyableImages = null;

    static public Image             ShopPanel { get { return Inst.mShopPanel; } }

    private void Awake() {
        if (!Inst)
            Inst = this;
    }

    private void Update() {
        if (!mShopPanel.IsActive())
            return;

        // ESC키를 누르면 상점 이용을 종료한다
        if (Input.GetButtonDown("Cancel")) {
            for (int i = 0; i < mProductPanels.Length; i++) {
                mProductPanels[i].gameObject.SetActive(true);
                mBuyButtons[i].interactable = false;
                mUnbuyableImages[i].gameObject.SetActive(true);
            }

            mShopPanel.gameObject.SetActive(false);
        }
    }

    public static ProductButtonInfo ProductButtonInfo(int _index) {
        ProductButtonInfo info;
        info.productPanel   = Inst.mProductPanels[_index];
        info.productName    = Inst.mProductNames[_index];
        info.productCost    = Inst.mProductCosts[_index];
        info.buyButton      = Inst.mBuyButtons[_index];
        info.productImage   = Inst.mProductImages[_index];
        info.unbuyableImage = Inst.mUnbuyableImages[_index];

        return info;
    }
}
