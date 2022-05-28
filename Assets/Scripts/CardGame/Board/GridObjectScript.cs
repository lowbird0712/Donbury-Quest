using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GridObjectScript : MonoBehaviour {
    [SerializeField] int[]          position = new int[2];
    [SerializeField] Animator       effectAnimator;
    [SerializeField] Animator       objectAnimator;
    [SerializeField] SpriteRenderer objectRenderer;
    [SerializeField] SpriteRenderer countDownRenderer;

    string                          objectName;
    CurrentObjectItem               currentObjectItem;
    bool                            hasObject = false;
    bool                            isNewCooking = false;
    int                             countDown = -1;

    public int[]                    Position => position;
    public CurrentObjectItem        CurrentObjectItem => currentObjectItem;
    public string                   ObjectName {
        get => objectName;
        set { objectName = value; }
    }
    public bool                     HasObject {
        get => hasObject;
        set { hasObject = value; }
    }
    public bool                     IsNewCooking {
        get => isNewCooking;
        set { isNewCooking = value; }
    }
    public int                      CountDown {
        get => countDown;
        set {
            countDown = value;
            if (countDown > 0)
                countDownRenderer.sprite = GridObjectMngScript.GridObjectSO.GetCountDown(countDown - 1);
        }
    }

    private void OnMouseDown() {
        if (hasObject && !CardGameMngScript.Inst.isLoading) {
            ObjectItem item = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName);
            string text = "";
            text += objectName + "\n";
            if (item.tool != null) {
                if (item.tool.neededSpiceNames != null) {
                    text += "필요한 조미료 :\n";
                    foreach (var spiceName in item.tool.neededSpiceNames) {
                        if (currentObjectItem.currentSpiceNames.Contains(spiceName))
                            text += spiceName + "(O)" + "\n";
                        else
                            text += spiceName + "\n";
                    }
                }
                if (currentObjectItem.currentObjectNums.Count != 0) {
                    text += "필요한 재료 :\n";
                    for (int i = 0; i < currentObjectItem.currentObjectNums.Count; i++)
                        text += $"{item.tool.neededObjectNames[i]}({currentObjectItem.currentObjectNums[i]}/{item.tool.neededObjectNums[i]})\n";
                }
                if (currentObjectItem.currentSpellNames.Count != 0) {
                    text += "사용 가능한 스펠 카드 :\n";
                    foreach (var spellName in currentObjectItem.currentSpellNames)
                        text += spellName + "\n";
                }
            }
            else if (countDown != -1)
                text += "남은 카운트다운 : " + countDown + "\n";
            text += "\n\n" + item.explain;
            CardGameMngScript.CardExplainPanel.Show(text);
        }
    }

    public void UseSpell(string _spellName) => effectAnimator.SetTrigger(CardMngScript.CardItemSO.GetSpellAnimationKey(_spellName));

    public void StartCooking() {
        string nextName = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName).tool.nextObjectName;
        ObjectItem nextItem = GridObjectMngScript.GridObjectSO.GetObjectItem(nextName);

        objectAnimator.SetTrigger(nextItem.animationKey);
        countDownRenderer.enabled = true;
        objectName = nextName;
        currentObjectItem.currentSpellNames = null;
        isNewCooking = true;
        CountDown = nextItem.cooking.originCountDown;

        GridObjectMngScript.GridObjectSO.ExecuteObjectFunc(objectName, SO_Timing.Start, position[0], position[1]);
    }

    public void EndCooking() {
        string nextName = GridObjectMngScript.GridObjectSO.GetObjectItem(objectName).cooking.nextObjectName;
        ObjectItem nextItem = GridObjectMngScript.GridObjectSO.GetObjectItem(nextName);
        if (nextName == null)
            StartCoroutine(RemoveObject());
        else {
            objectAnimator.SetTrigger("Stop");
            objectRenderer.sprite = nextItem.sprite;
            countDownRenderer.enabled = false;
            objectName = nextName;
            countDown = -1;
            currentObjectItem = new CurrentObjectItem(objectName);
            GridObjectMngScript.GridObjectSO.ExecuteObjectFunc(objectName, SO_Timing.Start, position[0], position[1]);
        }
    }

    public void InputObject(string _objectName) {
        if (_objectName == "조미료")
            return;
        if (GridObjectMngScript.GridObjectSO.GetObjectItem(_objectName).isSpice)
            currentObjectItem.currentSpiceNames.Add(_objectName);
        else
            currentObjectItem.currentObjectNums[GridObjectMngScript.GridObjectSO.GetObjectItem(objectName).tool.neededObjectNames.IndexOf(_objectName)]++;
        currentObjectItem.CurrentSpellNameUpdate();
    }

    public bool AdjacentObjectCheck(string _objectName) {
        foreach (var gridObject in GridObjectMngScript.GetAdjacentGridObjects(position)) {
            if (_objectName == gridObject.ObjectName)
                return true;
        }
        return false;
    }

    public IEnumerator SetObject(string _objectName) {
        ObjectItem item = GridObjectMngScript.GridObjectSO.GetObjectItem(_objectName);

        objectRenderer.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.down, Quaternion.identity, Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
        yield return new WaitForSeconds(Utils.cardExecDotweenTime);

        if (item.sprite != null) {
            objectRenderer.sprite = item.sprite;
            objectName = _objectName;
            if (item.cooking != null) {
                countDownRenderer.enabled = true;
                CountDown = item.cooking.originCountDown;
            }
        }
        else {
            objectRenderer.sprite = CardMngScript.CardItemSO.GetCardItem(_objectName).sprite;
            objectName = _objectName;
        }

        hasObject = true;
        if (currentObjectItem == null)
            currentObjectItem = new CurrentObjectItem(objectName);
        GridObjectMngScript.NextGridObjectUpdate();

        objectRenderer.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.up, Quaternion.identity, Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
        yield return new WaitForSeconds(Utils.cardExecDotweenTime);

        GridObjectMngScript.GridObjectSO.ExecuteObjectFunc(objectName, SO_Timing.Start, position[0], position[1]);
    }

    public IEnumerator RemoveObject() {
        objectRenderer.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.down, Quaternion.identity, Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
        if (countDown != -1) {
            countDownRenderer.enabled = false;
            countDown = -1;
        }
        yield return new WaitForSeconds(Utils.cardExecDotweenTime);

        objectRenderer.sprite = GridObjectMngScript.GridObjectDefaultSprite;
        objectName = null;
        hasObject = false;
        currentObjectItem = null;
        GridObjectMngScript.NextGridObjectUpdate();

        objectRenderer.GetComponent<DotweenMovingScript>().MoveTransform(new PRS(Vector3.up, Quaternion.identity, Vector3.one),
                                                                                Utils.cardExecDotweenTime, DG.Tweening.Ease.Linear, true);
        yield return new WaitForSeconds(Utils.cardExecDotweenTime);
    }
}
