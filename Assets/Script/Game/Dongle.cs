using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    #region 변수
    public int Level;
    public bool IsDrag;
    public bool IsMerge;
    public bool isTouchSound;
    public ParticleSystem Effect;

    private float DeadTime;
    public Rigidbody2D Rigid;
    private Animator Anim;
    private CircleCollider2D Circle;
    private SpriteRenderer Sprite;
    #endregion // 변수

    #region 프로퍼티
    public GameManager oGameManager { get; set; }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        Circle = GetComponent<CircleCollider2D>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    /** 초기화 => 활성화 될때 */
    private void OnEnable()
    {
        Anim.SetInteger("Level", Level);
    }

    /** 초기화 => 비활성화 될때 */
    private void OnDisable()
    {
        Level = 0;
        IsDrag = false;
        IsMerge = false;
        isTouchSound = false;

        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.zero;

        Rigid.simulated = false;
        Rigid.velocity = Vector2.zero;
        Rigid.angularVelocity = 0f;
        Circle.enabled = true;
    }

    /** 초기화 => 상태를 갱신한다 */
    private void Update()
    {
        if(IsDrag)
        {
            Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float WallLeft = -8.5f + this.transform.localScale.x / 2f;
            float WallRight = 8.5f - this.transform.localScale.x / 2f;

            if (MousePos.x < WallLeft)
            {
                MousePos.x = WallLeft;
            }
            else if (MousePos.x > WallRight)
            {
                MousePos.x = WallRight;
            }

            MousePos.y = 12f;
            MousePos.z = 0;
            this.transform.position = Vector3.Lerp(this.transform.position, MousePos, 0.2f);
        }
    }

    /** 초기화 => 접촉중일 경우 (트리거) */
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            DeadTime += Time.deltaTime;

            if(DeadTime > 2)
            {
                Sprite.color = new Color(0.9f, 0.2f, 0.2f);
            }

            // 5초동안 라인에 있을 경우 게임종료 
            if(DeadTime > 5)
            {
                oGameManager.GameOver();
            }
        }
    }

    /** 초기화 => 접촉이 끝났을 경우 (트리거) */
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            DeadTime = 0;
            Sprite.color = Color.white;
        }
    }

    /** 초기화 => 접촉했을 경우 */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(TouchSound()); 
    }

    /** 초기화 => 접촉중일 경우 */
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Dongle"))
        {
            Dongle OtherDongle = collision.gameObject.GetComponent<Dongle>();

            if(Level == OtherDongle.Level && !IsMerge && !OtherDongle.IsMerge && Level < 7)
            {
                float X = this.transform.position.x;
                float Y = this.transform.position.y;

                float OtherX = OtherDongle.transform.position.x;
                float OtherY = OtherDongle.transform.position.y;

                // 내가 아래있을 경우, 동일한 높이일 경우, 내가 오른쪽에 있을 경우
                if(Y < OtherY || (Y == OtherY && X > OtherX))
                {
                    Debug.Log("합쳐");
                    OtherDongle.Hide(this.transform.position, false);
                    LevelUp();
                }
            }
        }
    }

    /** 동글 레벨업 */
    private void LevelUp()
    {
        IsMerge = true;
        
        Rigid.velocity = Vector2.zero;
        Rigid.angularVelocity = 0;

        StartCoroutine(DongleLevelUp());
    }

    /** 동글 이펙트 실행 */
    private void EffectPlay()
    {
        Effect.transform.position = this.transform.position;
        Effect.transform.localScale = this.transform.localScale;

        Effect.Play();
    }

    /** 동글을 숨긴다 */
    public void Hide(Vector3 TargetPos, bool IsOver)
    {
        IsMerge = true;

        Rigid.simulated = false;
        Circle.enabled = false;

        if(IsOver == true)
        {
            EffectPlay();
        }

        StartCoroutine(HideDongle(TargetPos, IsOver));
    }

    /** 동글 드래그 */
    public void Drag()
    {
        IsDrag = true;
    }

    /** 동글 드랍 */
    public void Drop()
    {
        IsDrag = false;
        Rigid.simulated = true;
    }
    #endregion // 함수

    #region 코루틴
    /** 동글을 숨긴다 */
    private IEnumerator HideDongle(Vector3 TargetPos, bool IsOver)
    {
        int FrameCount = 0;

        while(FrameCount < 20)
        {
            FrameCount++;
            if(IsOver == false)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, TargetPos, 0.5f);
            }
            else if(IsOver == true)
            {
                this.transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }
            
            yield return null;
        }

        // 점수 추가
        oGameManager.Score += (int)Mathf.Pow(2, Level);
        oGameManager.ScoreTextUpdate(oGameManager.Score);

        IsMerge = false;
        this.gameObject.SetActive(false);
    }

    /** 동글 레벨업 */
    private IEnumerator DongleLevelUp()
    {
        yield return new WaitForSeconds(0.2f);

        Anim.SetInteger("Level", Level + 1);
        EffectPlay();
        //AudioManager.Instance.PlaySFX(SFXEnum);

        yield return new WaitForSeconds(0.3f);
        Level++;

        // 동글 소환레벨 증가
        oGameManager.MaxLevel = Mathf.Max(Level, oGameManager.MaxLevel);

        IsMerge = false;
    }

    /** 소리 제어 */
    private IEnumerator TouchSound()
    {
        if (isTouchSound)
        {
            yield break;
        }

        isTouchSound = true;
        //AudioManager.Instance.PlaySFX();
        yield return new WaitForSeconds(0.2f);
        isTouchSound = false;
    }
    #endregion // 코루틴
}
