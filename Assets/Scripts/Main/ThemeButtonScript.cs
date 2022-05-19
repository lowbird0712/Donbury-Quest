using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeButtonScript : MonoBehaviour {
    [SerializeField]
    int     mThemeNum = 0;
    [SerializeField]
    int     mInitGold = 0;

    bool    mFirstTime = true;

    public void ThemeButton() {
        MainGameMngScript.StorySelectingIndex = mThemeNum;

        if (mFirstTime)
            MainGameMngScript.InitGold = mInitGold;

        SceneManager.LoadScene("ManagerScene");
    }
}
