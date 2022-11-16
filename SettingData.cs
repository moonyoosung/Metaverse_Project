using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingData", menuName = "ScriptableObjects/SettingData", order = 1)]
public class SettingData : ScriptableObject
{
    public enum MoveType
    {
        Stick = 0,
        Teleport = 1
    }
    [Range(0, 100)]
    public int masterVolume = 100;
    [Range(0, 100)]
    public int voiceVolume = 80;
    [Range(0, 100)]
    public int othersVoiceVolume = 80;
    [Range(0, 100)]
    public int effectVolume = 100;
    
    //locomotion 0 : stick move  1 : teleport
    public MoveType moveType = MoveType.Stick;

    [Header("camera")]
    public float cameraRotateSpeed = 300;
    //1인칭<->3인칭 전환 속도
    public float cameraBlendSpeed = 0.5f;

    public readonly float weight = 2f;  // 보이스 음량 가중치
}
