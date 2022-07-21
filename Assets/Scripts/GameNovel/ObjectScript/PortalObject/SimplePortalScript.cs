using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimplePortalScript : ObjectScript {
    [SerializeField]
    string              mClosedString = null;
    [SerializeField]
    Sprite              mClosedSprite = null;

    [SerializeField]
    protected string    mNextMapName = null;

    public string ClosedString { set { mClosedString = value; } }

    private void Start() {
        mInteractArray = new InteractPointer[3];

        mInteractArray[0] = Closed;
        mInteractArray[1] = Enter;
        mInteractArray[2] = DefaultClosed;
    }

    private void Update() {
        if (!mInteracting)
            return;

        // 이 경우는 항상 Closed나 DefaultClosed가 실행된 후이다.
        if (Input.GetButtonDown("Jump")) {
            mInteracting = false;
            PlayerScript.Interact = false;
            StoryBlockMngScript.Active = false;
        }
    }

    protected void Closed() {
        if (mClosedString == null) {
            Debug.LogError("mClosedString이 null이에요!");
            return;
        }

        StoryBlockMngScript.Text.text = mClosedString;
        StoryBlockMngScript.NPC_Name.text = mPanelName;
        StoryBlockMngScript.NPC_Panel.sprite = mClosedSprite;
        StoryBlockMngScript.ActiveOnlyPanels();
    }

    protected void Enter() {
        GameMngScript.PreMapName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(mNextMapName);
    }
    
    protected void DefaultClosed() {
        StoryBlockMngScript.Text.text = "뭔가 할 일이 있는 것 같다.. 그것을 처리하는 게 먼저야..!";
        StoryBlockMngScript.NPC_Name.text = "타유";
        StoryBlockMngScript.NPC_Panel.sprite = StoryBlockMngScript.NPC_Sprites[0];
        StoryBlockMngScript.ActiveOnlyPanels();
    }
}
