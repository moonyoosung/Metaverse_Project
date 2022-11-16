using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo Data", menuName = "ScriptableObjects/GameInfo Data", order = int.MaxValue)]
public class GameInfoList : ScriptableObject
{
    public List<GameInfo> list;
}