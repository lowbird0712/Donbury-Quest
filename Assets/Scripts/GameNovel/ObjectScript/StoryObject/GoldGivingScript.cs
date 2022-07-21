using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldGivingScript : StoryObjectScript {
    [SerializeField]
    int         mGivingGold = 0;

    bool        mGiving = false;

    public int  GivingGold { set { mGivingGold = value; } }
    public bool Giving { set { mGiving = value; } }

    protected override void SetOtherThings(string _mapName) {
        base.SetOtherThings(_mapName);

        if (mGiving)
            GameMngScript.Gold += mGivingGold;
    }
}
