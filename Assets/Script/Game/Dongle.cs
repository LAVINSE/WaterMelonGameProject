using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    #region ����
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
    #endregion // ����

    #region ������Ƽ
    public GameManager oGameManager { get; set; }
    #endregion // ������Ƽ

    #region �Լ�
    /** �ʱ�ȭ */
    private void Awake()
    {
        Rigid = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        Circle = GetComponent<CircleCollider2D>();
        Sprite = GetComponent<SpriteRenderer>();
    }

    /** �ʱ�ȭ => Ȱ��ȭ �ɶ� */
    private void OnEnable()
    {
        Anim.SetInteger("Level", Level);
    }

    /** �ʱ�ȭ => ��Ȱ��ȭ �ɶ� */
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

    /** �ʱ�ȭ => ���¸� �����Ѵ� */
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

    /** �ʱ�ȭ => �������� ��� (Ʈ����) */
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            DeadTime += Time.deltaTime;

            if(DeadTime > 2)
            {
                Sprite.color = new Color(0.9f, 0.2f, 0.2f);
            }

            // 5�ʵ��� ���ο� ���� ��� �������� 
            if(DeadTime > 5)
            {
                oGameManager.GameOver();
            }
        }
    }

    /** �ʱ�ȭ => ������ ������ ��� (Ʈ����) */
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            DeadTime = 0;
            Sprite.color = Color.white;
        }
    }

    /** �ʱ�ȭ => �������� ��� */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(TouchSound()); 
    }

    /** �ʱ�ȭ => �������� ��� */
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

                // ���� �Ʒ����� ���, ������ ������ ���, ���� �����ʿ� ���� ���
                if(Y < OtherY || (Y == OtherY && X > OtherX))
                {
                    Debug.Log("����");
                    OtherDongle.Hide(this.transform.position, false);
                    LevelUp();
                }
            }
        }
    }

    /** ���� ������ */
    private void LevelUp()
    {
        IsMerge = true;
        
        Rigid.velocity = Vector2.zero;
        Rigid.angularVelocity = 0;

        StartCoroutine(DongleLevelUp());
    }

    /** ���� ����Ʈ ���� */
    private void EffectPlay()
    {
        Effect.transform.position = this.transform.position;
        Effect.transform.localScale = this.transform.localScale;

        Effect.Play();
    }

    /** ������ ����� */
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

    /** ���� �巡�� */
    public void Drag()
    {
        IsDrag = true;
    }

    /** ���� ��� */
    public void Drop()
    {
        IsDrag = false;
        Rigid.simulated = true;
    }
    #endregion // �Լ�

    #region �ڷ�ƾ
    /** ������ ����� */
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

        // ���� �߰�
        oGameManager.Score += (int)Mathf.Pow(2, Level);
        oGameManager.ScoreTextUpdate(oGameManager.Score);

        IsMerge = false;
        this.gameObject.SetActive(false);
    }

    /** ���� ������ */
    private IEnumerator DongleLevelUp()
    {
        yield return new WaitForSeconds(0.2f);

        Anim.SetInteger("Level", Level + 1);
        EffectPlay();
        //AudioManager.Instance.PlaySFX(SFXEnum);

        yield return new WaitForSeconds(0.3f);
        Level++;

        // ���� ��ȯ���� ����
        oGameManager.MaxLevel = Mathf.Max(Level, oGameManager.MaxLevel);

        IsMerge = false;
    }

    /** �Ҹ� ���� */
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
    #endregion // �ڷ�ƾ
}
