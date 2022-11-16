using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActivityDescriptions", menuName = "MindPlus/ActivityDescriptions", order = 0)]
public class ChallengeDescriptions : ScriptableObject
{
    [System.Serializable]
    public class Description
    {
        [Title("Description")]
        public string challengeId;
        [TableColumnWidth(80, Resizable = false)]
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        public Sprite image;
        public string title;
        [TextArea]
        [MultiLineProperty(10)]
        public string description;
    }
    public Description[] descriptions;


    public Description Get(string challengeId)
    {
        foreach (var description in descriptions)
        {
            if(description.challengeId == challengeId)
            {
                return description;
            }
        }
        Debug.LogError("Not Found Description Set To " + challengeId);
        return null;
    }

}
