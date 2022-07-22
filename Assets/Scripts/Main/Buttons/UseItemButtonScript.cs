using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemButtonScript : MonoBehaviour {
    int itemIndex;

    public int ItemIndex { get => itemIndex; set => itemIndex = value; }

    public void UseItem() { CampMngScript.UseItem(LinBoxMngScript.GetItem(itemIndex)); }
}
