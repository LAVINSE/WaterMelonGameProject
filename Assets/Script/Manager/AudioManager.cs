using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

#region ����
/** ����� ���� */
public enum BGMEnum
{
    MainBGM,
}

/** ȿ���� ���� */
public enum SFXEnum
{
    
}
#endregion // ����

public class AudioManager : MonoBehaviour
{
    #region ����
    [Header("=====> BGM Setting <=====")]
    [Tooltip(" ����� ��� ")][SerializeField] private AudioClip[] BGMClips; // �����
    [Tooltip(" 0 ~ 1 ")][SerializeField] private float BGMVolume = 0.0f; // ����� ����

    [Header("=====> SFX Setting <=====")]
    [Tooltip(" ȿ���� ��� ")][SerializeField] private AudioClip[] SFXClips; // ȿ����
    [Tooltip(" 0 ~ 1 ")][SerializeField] private float SFXVolume = 0.0f; // ȿ���� ����
    [Tooltip(" ȿ���� ä�� ���� ")][SerializeField] private int SFXChannel = 0;

    private AudioSource[] BGMPlayers;
    private AudioSource[] SFXPlayers;
    private int ChannelIndex;
    #endregion // ����

    #region ������Ƽ
    public static AudioManager Instance; 
    public float oBGMVolume
    {
        get => BGMVolume;
        set => BGMVolume = value;
    }
    public float oSFXVolume
    {
        get => SFXVolume;
        set => SFXVolume = value;
    }
    #endregion // ������Ƽ

    #region �Լ�
    /** �ʱ�ȭ */
    private void Awake()
    {
        Instance = this;
        BGMInit();
        SFXInit();
    }

    /** �ʱ�ȭ >> ���¸� �����Ѵ� */
    private void Update()
    {
        
    }

    /** ������� �����Ѵ� */
    private void BGMInit()
    {
        GameObject BGMObject = new GameObject("BGMPlayer");
        BGMObject.transform.parent = this.transform;
        BGMPlayers = new AudioSource[BGMClips.Length];

        for (int i = 0; i < BGMPlayers.Length; i++)
        {
            BGMPlayers[i] = BGMObject.AddComponent<AudioSource>();
            BGMPlayers[i].playOnAwake = false;
            BGMPlayers[i].loop = true;
            BGMPlayers[i].volume = BGMVolume;
            BGMPlayers[i].clip = BGMClips[i];
        }
    }

    /** ȿ������ �����Ѵ� */
    private void SFXInit()
    {
        GameObject SFXObject = new GameObject("SFXPlayer");
        SFXObject.transform.parent = this.transform;
        SFXPlayers = new AudioSource[SFXChannel];

        for (int i = 0; i < SFXPlayers.Length; i++)
        {
            SFXPlayers[i] = SFXObject.AddComponent<AudioSource>();
            SFXPlayers[i].playOnAwake = false;
            SFXPlayers[i].volume = SFXVolume;
        }
    }

    /** ȿ������ ����Ѵ� */
    public void PlaySFX(SFXEnum SFXType)
    {
        for (int i = 0; i < SFXPlayers.Length; i++)
        {
            int LoopIndex = (i + ChannelIndex) % SFXPlayers.Length;

            // ȿ������ ������� ���
            if (SFXPlayers[LoopIndex].isPlaying)
            {
                continue;
            }

            ChannelIndex = LoopIndex;
            SFXPlayers[LoopIndex].clip = SFXClips[(int)SFXType];
            SFXPlayers[LoopIndex].Play();
            break;
        }
    }

    /** ������� ����Ѵ� */
    public void PlayBGM(BGMEnum BGMType)
    {
        for (int i = 0; i < BGMPlayers.Length; i++)
        {

            // ȿ������ ������� ���
            if (BGMPlayers[i].isPlaying)
            {
                BGMPlayers[i].Stop();
                continue;
            }

            BGMPlayers[i].clip = BGMClips[(int)BGMType];
            BGMPlayers[i].Play();
            break;
        }
    }

    /** ������� ����� */
    public void StopBGM()
    {
        for (int i = 0; i < BGMPlayers.Length; i++)
        {
            // ������� ������� ���
            if (BGMPlayers[i].isPlaying)
            {
                BGMPlayers[i].Stop();
            }
        }
    }

    /** ȿ���� ������ �����Ѵ� */
    public void SFXSettingVolume(float SFXVolume)
    {
        for (int i = 0; i < SFXPlayers.Length; i++)
        {
            SFXPlayers[i].volume = SFXVolume;
        }
    }

    /** ����� ������ �����Ѵ� */
    public void BGMSettingVolume(float BGMVolume)
    {
        for (int i = 0; i < BGMPlayers.Length; i++)
        {
            BGMPlayers[i].volume = BGMVolume;
        }
    }
    #endregion // �Լ�
}
