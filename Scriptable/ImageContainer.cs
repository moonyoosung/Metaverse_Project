using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ImageContainer", menuName = "MindPlus/ImageContainer", order = 0)]
public class ImageContainer : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public string ID;
        [TableColumnWidth(80,Resizable = false)]
        [PreviewField(Alignment =ObjectFieldAlignment.Center)]
        public Sprite image;
    }
    [SerializeField]
    private List<Data> images = new List<Data>();

    public Sprite Get(string ID)
    {
        foreach (var image in images)
        {
            if(image.ID == ID)
            {
                return image.image;
            }
        }
        Debug.LogError("Not Set " + ID + "ImageContainer");
        return null;
    }

}
