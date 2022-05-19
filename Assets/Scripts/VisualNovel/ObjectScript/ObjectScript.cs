using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour {
    [SerializeField]
    protected string                            mPanelName = "";
    [SerializeField]
    protected Sprite                            mNPC_sprite = null;

    protected bool                              mInteracting = false;

    protected delegate void InteractPointer();
    protected InteractPointer[]                 mInteractArray;
    [SerializeField]
    protected int                               mInteractIndex = 0;

    public bool Interacting { get { return mInteracting; } }
    public int  InteractIndex {
        get { return mInteractIndex; }
        set { mInteractIndex = value; }
    }

    public void Interact() {
        mInteracting = true;
        mInteractArray[mInteractIndex]();
    }
}
