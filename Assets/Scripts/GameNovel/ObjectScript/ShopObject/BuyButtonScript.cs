using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButtonScript : MonoBehaviour {
    [SerializeField]
    int     mIndex = -1;

    public void Buy() {
        ProductButtonInfo   info = ShopMngScript.ProductButtonInfo(mIndex);
        for (int i = 0; i < QuestMngScript.OriginalQuest.intConditionNames.Length; i++) {
            if (QuestMngScript.OriginalQuest.intConditionNames[i] == info.productName.text) {
                if (GameMngScript.Gold < int.Parse(info.productCost.text)) {
                    Debug.Log("돈이 부족해요!");
                    break;
                }
                else if (QuestMngScript.OriginalQuest.intConditions[i] == QuestMngScript.DoingQuestConditions.intConditions[i]) {
                    Debug.Log("더 이상 살 필요가 없어요!");
                    break;
                }

                QuestMngScript.DoingQuestConditions.intConditions[i]++;
                QuestMngScript.RefreshIntConditionText(i);
                GameMngScript.Gold -= int.Parse(info.productCost.text);
                break;
            }
        }
    }
}
