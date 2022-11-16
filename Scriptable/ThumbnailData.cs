using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomThumbnail", menuName = "MindPlus/RoomThumbnail", order = 0)]
public class ThumbnailData : ScriptableObject
{
    public Sprite errorRoom;
    [System.Serializable]
    public class RoomThumbnail
    {
        public string sceneName;
        public string type;
        public Sprite thumbnail;

        public RoomThumbnail(string type, string sceneName, Sprite sprite)
        {
            this.sceneName = sceneName;
            this.type = type;
            this.thumbnail = sprite;
        }
    }

    public List<RoomThumbnail> thumbnails = new List<RoomThumbnail>();
    public RoomThumbnail Get(string sceneName)
    {
        foreach (var thumbnail in thumbnails)
        {
            if (thumbnail.sceneName == sceneName)
            {
                return thumbnail;
            }
        }

        Debug.LogWarning("Not Set " + sceneName + " ThumbNail Image...");
        return new RoomThumbnail("None", "Title", errorRoom);
    }
    public List<RoomThumbnail> GetThumbnails(params string[] roomTypes)
    {
        List<RoomThumbnail> result = new List<RoomThumbnail>();
        foreach (var thumbnail in thumbnails)
        {
            foreach (var roomType in roomTypes)
            {
                if (thumbnail.type == roomType)
                {
                    result.Add(thumbnail);
                }
            }
        }
        return result;
    }
}
