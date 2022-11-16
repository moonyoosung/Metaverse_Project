using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "UIResources", menuName = "MindPlus/UIResources", order = 0)]
public class UIResources : ScriptableObject
{
    public Platform type;
    public StreamQueue StreamQueue;
    public Dial dial;
    [SerializeField]
    public Gradient[] levelGradients;
    [ValueDropdown("GetAllSpriteAssets", IsUniqueList = true)]
    public Sprite[] levelIcons;
    [TitleGroup("[Challenge Categorys]")]
    [GUIColor(0.94f, 0.95f, 0.36f, 1f)]
    public Color inappColor;
    [TitleGroup("[Challenge Categorys]")]
    [GUIColor(0.94f, 0.95f, 0.36f, 1f)]
    public Color outdoorColor;
#if UNITY_EDITOR
    private static IEnumerable GetAllSpriteAssets()
    {
        var root = "Assets/A_MindPlus/0_Common/Images/UI/Icons";

        return UnityEditor.AssetDatabase.FindAssets("t:Sprite", new[] { root })
          .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
          .Select(x => new ValueDropdownItem(x, UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(x)));
    }
#endif
}
