using BeliefEngine.HealthKit;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "HealthActivityPair", menuName = "MindPlus/HealthActivityPair", order = 0)]

public class HealthActivityPair : ScriptableObject
{
    [System.Serializable]
    public class Pair
    {
        [Title("HealthType")]
        public HKDataType type;
        [Title("Description")]
        public string title="";
        [TextArea]
        [MultiLineProperty(10)]
        public string description="";
    }

    public List<Pair> Pairs = new List<Pair>();
   

    public Pair GetPair(HKDataType type)
    {
        foreach (var pair in Pairs)
        {
            if(pair.type == type)
            {
                return pair;
            }
        }
        Debug.LogError("Not Found HealthType Pair : " + type.ToString());
        return null;
    }
}
