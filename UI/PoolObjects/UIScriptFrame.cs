using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScriptFrame : UIPoolObject
{
    public Text[] scripts;
    public Image[] images;
    public override void InActivePool()
    {
        base.InActivePool();
        foreach (var script in scripts)
        {
            script.text = "";
        }
        foreach (var image in images)
        {
            image.sprite = null;
        }
    }
    public virtual void Set(ScriptData scriptData)
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            if (scriptData.scripts == null || scriptData.scripts.Length <= 0)
            {
                scripts[i].gameObject.SetActive(false);
                continue;
            }
            if (scriptData.scripts[i] == "")
            {
                scripts[i].gameObject.SetActive(false);
            }
            else
            {
                scripts[i].gameObject.SetActive(true);
                scripts[i].text = scriptData.scripts[i];
            }
        }
        for (int i = 0; i < images.Length; i++)
        {
            if (scriptData.sprites == null || scriptData.sprites.Length <= 0)
            {
                images[i].gameObject.SetActive(false);
                continue;
            }
            if (scriptData.sprites[i] == null)
            {
                images[i].gameObject.SetActive(false);
            }
            else
            {
                images[i].gameObject.SetActive(true);
                images[i].sprite = scriptData.sprites[i];
            }
        }
    }
}
