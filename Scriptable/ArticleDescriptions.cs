using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ArticleDescriptions", menuName = "MindPlus/ArticleDescriptions", order = 0)]

public class ArticleDescriptions : ScriptableObject
{
    [System.Serializable]
    public class Article
    {
        [Title("Article")]
        public string title;
        [TableColumnWidth(200, Resizable = false)]
        [PreviewField(200, Alignment = ObjectFieldAlignment.Center)]
        public Sprite image;
        [Title("DescriptionData")]
        [ValueDropdown("ScripDataList", ExpandAllMenuItems = true)]
        public List<ScriptData> scriptDatas;
        public Article(List<ScriptData> scriptDatas)
        {
            this.scriptDatas = new List<ScriptData>(scriptDatas);
        }

        private IEnumerable ScripDataList = new ValueDropdownList<ScriptData>()
        {
            new SubScriptData(),
            new PanelScriptData(),
            new AddScriptData(),
        };

    }
    public List<Article> articles = new List<Article>()
    {
        new Article(new List<ScriptData>(){
            new SubScriptData(),
            new AddScriptData(),
            new PanelScriptData(),
            new AddScriptData(),
            new AddScriptData(),
            new AddScriptData(),}),
        new Article(new List<ScriptData>(){  
            new SubScriptData(),
            new AddScriptData(),
            new AddScriptData(),
            new AddScriptData(),
            new AddScriptData(),
            new AddScriptData(),})
    };
}

[System.Serializable]
public class ScriptData
{
    [TextArea]
    [MultiLineProperty(3)]
    public string[] scripts;
    [TableColumnWidth(200, Resizable = false)]
    [PreviewField(200, Alignment = ObjectFieldAlignment.Left)]
    public Sprite[] sprites;


}

[System.Serializable]
public class SubScriptData : ScriptData { }
[System.Serializable]
public class PanelScriptData : ScriptData { }
[System.Serializable]
public class AddScriptData : ScriptData { }
