using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region ����
    public Text ScoreText;
    public Text MaxScoreText;
    public int MaxLevel;
    public int Score;
    public bool IsGameOver;

    [Range(1, 30)]public int PoolSize;
    public int PoolCount;
    public List<Dongle> DonglePool;
    public List<ParticleSystem> EffectPool;

    public GameObject DonglePrefab;
    public GameObject DongleEffectPrefab;
    public Transform DongleGroupRoot;
    public Transform DongleEffectGroupRoot;
    #endregion // ����

    #region ������Ƽ
    public Dongle oDongleObject { get; set; }
    #endregion // ������Ƽ

    #region �Լ�
    /** �ʱ�ȭ */
    private void Awake()
    {
        DonglePool = new List<Dongle>();
        EffectPool = new List<ParticleSystem>();

        for(int i =0; i<PoolSize; i++)
        {
            CreateDongle();
        }

        MaxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();
    }

    /** �ʱ�ȭ */
    private void Start()
    {
        NextDongle();
    }

    /** ���� ���� */
    public void GameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        PlayerPrefs.SetInt("MaxScore", Score);
        StartCoroutine(DongleGameOver());
    }

    /** ���� ���� */
    private Dongle CreateDongle()
    {
        // ����Ʈ ����
        GameObject EffectObj = Instantiate(DongleEffectPrefab, DongleEffectGroupRoot);
        EffectObj.name = "Effect" + EffectPool.Count;
        ParticleSystem EffectComponent = EffectObj.GetComponent<ParticleSystem>();
        EffectPool.Add(EffectComponent);

        // ���� ����
        GameObject DongleObj = Instantiate(DonglePrefab, DongleGroupRoot);
        DongleObj.name = "Dongle" + DonglePool.Count;
        Dongle DongleComponent = DongleObj.GetComponent<Dongle>();

        DongleComponent.oGameManager = this;
        DongleComponent.Effect = EffectComponent;

        DonglePool.Add(DongleComponent);

        return DongleComponent;
    }

    /** ������ �����Ѵ� */
    private Dongle GetDongle()
    {
        // Ǯ��
        for(int i = 0; i< DonglePool.Count; i++)
        {
            PoolCount = (PoolCount + 1) % DonglePool.Count;

            if (!DonglePool[PoolCount].gameObject.activeSelf)
            {
                return DonglePool[PoolCount];
            }
        }

        return CreateDongle();
    }

    /** ���� ���۰����� �´� */
    private void NextDongle()
    {
        if (IsGameOver)
        {
            return;
        }

        oDongleObject = GetDongle();
        oDongleObject.Level = Random.Range(0, MaxLevel);
        oDongleObject.gameObject.SetActive(true);

        //AudioManager.Instance.PlaySFX(SFXEnum);
        StartCoroutine(WaitNextDongle());
    }

    /** Ŭ���� ��� ���� �巡�� ���� */
    public void TouchDown()
    {
        if(oDongleObject == null) { return; }
        oDongleObject.Drag();
    }

    /** Ŭ���� �� ��� ���� ��� */
    public void TouchUp()
    {
        if (oDongleObject == null) { return; }
        oDongleObject.Drop();
        oDongleObject = null;
    }

    public void ScoreTextUpdate(int Score)
    {
        ScoreText.text = Score.ToString();
    }
    #endregion // �Լ�

    #region �ڷ�ƾ
    /** ������ ������������ ��ٸ��� */
    private IEnumerator WaitNextDongle()
    {
        while(oDongleObject != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

        NextDongle();
    }

    /** ������ �����Ѵ� */
    private IEnumerator DongleGameOver()
    {
        Dongle[] DongleArray = GameObject.FindObjectsOfType<Dongle>();

        for (int i = 0; i < DongleArray.Length; i++)
        {
            DongleArray[i].Rigid.simulated = false;
        }

        for (int i = 0; i < DongleArray.Length; i++)
        {
            DongleArray[i].Hide(Vector3.up * 100, true);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);
        //AudioManager.Instance.PlaySFX();
    }
    #endregion // �ڷ�ƾ
}
