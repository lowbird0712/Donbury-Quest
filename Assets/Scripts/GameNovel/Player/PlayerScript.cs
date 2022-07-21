using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    static public PlayerScript  Inst { get; set; } = null;

    Rigidbody2D                 mRigidbody2D = null;
    Animator                    mAnimator = null;

    bool                        mMovingHorizontal = false;
    Vector2                     mDir = Vector2.zero;

    GameObject                  mObject = null;
    bool                        mInteract = false;

    bool                        mRightAfterBlockFree = false;

    [SerializeField]
    float                       mSpeed = 5.0f;

    static public bool Interact {
        get { return Inst.mInteract; }
        set { Inst.mInteract = value; }
    }

    static public bool RightAfterBlockFree {
        get { return Inst.mRightAfterBlockFree; }
        set { Inst.mRightAfterBlockFree = value; }
    }

    static public void SetEnteringPosition(string _preMapName, string _nowMapName) {
        if (_nowMapName == "Minimori1stRoad") {
            if (_preMapName == "KokoroRestaurant")
                Inst.transform.localPosition = ObjectMngScript.GetObjects(0).transform.localPosition + Vector3.left;
            else if (_preMapName == "DotoriMinimoriPlane")
                Inst.transform.localPosition = ObjectMngScript.GetObjects(3).transform.localPosition + Vector3.down;
        }
        if (_nowMapName == "KokoroRestaurant") {
            if (_preMapName == "KokoroKitchen")
                Inst.transform.localPosition = ObjectMngScript.GetObjects(0).transform.localPosition + Vector3.down;
        }
    }

    void Awake() {
        if (!Inst)
            Inst = this;
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();

        if (!mRigidbody2D)
            Debug.Log("This Object doesn't have a rigidbody2D!");
        if (!mAnimator)
            Debug.Log("This Object doesn't have a Animator!");
    }

    void FixedUpdate() {
        if (ShouldBlocked())
            return;

        // 메뉴창, 퀘스트 창이 닫힌 직후에 애니메이션 실행 없이 캐릭터가 윰직이는 것을 방지하는 부분이다
        if (mRightAfterBlockFree)
            return;

        // 플레이어가 상하좌우로 움직인다
        if (!mInteract) {
            int horizontalAxis = (int)Input.GetAxisRaw("Horizontal");
            int verticalAxis = (int)Input.GetAxisRaw("Vertical");

            if (mMovingHorizontal)
                mRigidbody2D.velocity = new Vector2(horizontalAxis, 0.0f) * mSpeed;
            else
                mRigidbody2D.velocity = new Vector2(0.0f, verticalAxis) * mSpeed;
        }
    }

    void Update() {
        if (ShouldBlocked())
            return;

        if (!mInteract) {
            // 플레이어의 이동 키 상황을 변수에 저장한다
            int horizontalAxis = (int)Input.GetAxisRaw("Horizontal");
            int verticalAxis = (int)Input.GetAxisRaw("Vertical");

            if (mAnimator.GetBool("Animation Changing"))
                mAnimator.SetBool("Animation Changing", false);

            // 이동 관련 플래그와 애니메이션, 바라보고 있는 방향을 설정한다
            if (Input.GetButtonDown("Horizontal")) {
                mMovingHorizontal = true;
                SetAnimation(horizontalAxis, verticalAxis);
            }
            if (Input.GetButtonUp("Vertical")) {
                mMovingHorizontal = Input.GetButton("Vertical") ? false : true;
                SetAnimation(horizontalAxis, verticalAxis);
            }
            if (Input.GetButtonDown("Vertical")) {
                mMovingHorizontal = false;
                SetAnimation(horizontalAxis, verticalAxis);
            }
            if (Input.GetButtonUp("Horizontal")) {
                mMovingHorizontal = Input.GetButton("Horizontal") ? true : false;
                SetAnimation(horizontalAxis, verticalAxis);
            }

            // 플레이어가 오브젝트와 상호작용할 수 있는지를 체크한다
            if (mDir != Vector2.zero) {
                RaycastHit2D cast = Physics2D.Raycast((Vector2)transform.position, mDir, 0.7f, LayerMask.GetMask("Objects"));

                Collider2D raycastColl = cast.collider;
                if (raycastColl != null)
                    mObject = raycastColl.gameObject;
                else
                    mObject = null;
            }
        }
    }

    private void LateUpdate() {
        // 플레이어가 상호작용할 수 있는 오브젝트가 있는 상태에서 상호작용한다
        if (Input.GetKeyDown(KeyCode.C) && mObject != null) {
            ObjectScript objectScript = mObject.GetComponent<ObjectScript>();

            if (!mInteract) {
                FreezeAnimation();
                mInteract = true;
                objectScript.Interact();
            }
        }
    }

    public static void FreezeAnimation() {
        Inst.mAnimator.SetInteger("Vertical Axis", 0);
        Inst.mAnimator.SetInteger("Horizontal Axis", 0);
        Inst.mAnimator.SetBool("Animation Changing", true);
        Inst.mDir = Vector2.zero;

        Inst.mRightAfterBlockFree = true;
    }

    void SetAnimation(int _horizontalAxis, int _verticalAxis) {
        // 플래그에 따라 애니메이션 트리거를 설정한다
        if (mMovingHorizontal) {
            mAnimator.SetInteger("Horizontal Axis", _horizontalAxis);
            mAnimator.SetInteger("Vertical Axis", 0);
            mDir = Vector2.right * _horizontalAxis;
        }
        else {
            mAnimator.SetInteger("Horizontal Axis", 0);
            mAnimator.SetInteger("Vertical Axis", _verticalAxis);
            mDir = Vector2.up * _verticalAxis;
        }

        mAnimator.SetBool("Animation Changing", true);

        if (mRightAfterBlockFree)
            mRightAfterBlockFree = false;
    }

    // 메뉴, 상점, 퀘스트 창 등이 켜져 있을 때에는 플레이어가 이동하거나 상호작용 하지 못한다.
    // 이를 판단하기 위한 함수이다
    bool ShouldBlocked() {
        bool shouldBlocked = false;

        if (GameMngScript.IsMenuActive)
            shouldBlocked = true;
        if (QuestMngScript.QuestPanel.IsActive())
            shouldBlocked = true;

        if (shouldBlocked) {
            mRigidbody2D.velocity = Vector2.zero;
            mAnimator.SetInteger("Horizontal Axis", 0);
            mAnimator.SetInteger("Vertical Axis", 0);
            mDir = Vector2.zero;
        }

        return shouldBlocked;
    }
}
