using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region 변수
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
    #endregion // 변수

    #region 프로퍼티
    public Dongle oDongleObject { get; set; }
    #endregion // 프로퍼티

    #region 함수
    /** 초기화 */
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

    /** 초기화 */
    private void Start()
    {
        NextDongle();
    }

    /** 게임 종료 */
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

    /** 동글 생성 */
    private Dongle CreateDongle()
    {
        // 이펙트 생성
        GameObject EffectObj = Instantiate(DongleEffectPrefab, DongleEffectGroupRoot);
        EffectObj.name = "Effect" + EffectPool.Count;
        ParticleSystem EffectComponent = EffectObj.GetComponent<ParticleSystem>();
        EffectPool.Add(EffectComponent);

        // 동글 생성
        GameObject DongleObj = Instantiate(DonglePrefab, DongleGroupRoot);
        DongleObj.name = "Dongle" + DonglePool.Count;
        Dongle DongleComponent = DongleObj.GetComponent<Dongle>();

        DongleComponent.oGameManager = this;
        DongleComponent.Effect = EffectComponent;

        DonglePool.Add(DongleComponent);

        return DongleComponent;
    }

    /** 동글을 생성한다 */
    private Dongle GetDongle()
    {
        // 풀링
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

    /** 다음 동글가지고 온다 */
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

    /** 클릭할 경우 동글 드래그 가능 */
    public void TouchDown()
    {
        if(oDongleObject == null) { return; }
        oDongleObject.Drag();
    }

    /** 클릭을 뗄 경우 동글 드랍 */
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
    #endregion // 함수

    #region 코루틴
    /** 동글이 떨어질때까지 기다린다 */
    private IEnumerator WaitNextDongle()
    {
        while(oDongleObject != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

        NextDongle();
    }

    /** 게임을 종료한다 */
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
    #endregion // 코루틴
}
