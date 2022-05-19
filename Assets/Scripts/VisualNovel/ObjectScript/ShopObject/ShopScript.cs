using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ShopItem {
	public string	name;
	public Sprite	sprite;
	public int		cost;
	public bool		buyable;
}

public class ShopScript : ObjectScript {
	[SerializeField]
	ShopItem[]	mShopItems = null;

	private void Start() {
		mInteractArray = new InteractPointer[1];
		mInteractArray[0] = ShopOpenInteract;
	}

	private void Update() {
		if (!mInteracting)
			return;

		if (!ShopMngScript.ShopPanel.IsActive()) {
			mInteracting = false;
			PlayerScript.Interact = false;
		}
	}

	// 상점을 연다
	void ShopOpenInteract() {
		// 상점 패널을 활성화 시킨다
		ShopMngScript.ShopPanel.gameObject.SetActive(true);

		// 상점의 판매 목록을 초기화한다
		ProductButtonInfo info;
		for (int i = 0; i < mShopItems.Length; i++) {
			info = ShopMngScript.ProductButtonInfo(i);
			info.productPanel.gameObject.SetActive(true);
			info.productName.text		= mShopItems[i].name;
			info.productCost.text		= mShopItems[i].cost.ToString();
			info.productImage.sprite	= mShopItems[i].sprite;
			if (mShopItems[i].buyable) {
				info.buyButton.interactable = true;
				info.unbuyableImage.gameObject.SetActive(false);
			}
		}
	}

	public ShopItem[] GetShopItems() {
		return mShopItems;
	}
}
