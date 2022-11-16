using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScriptAddFrame : UIScriptFrame
{
    public HorizontalLayoutGroup imageHorizontalGroup;


    public override void Set(ScriptData scriptData)
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            if(scriptData.scripts == null || scriptData.scripts.Length <= 0)
            {
                scripts[i].gameObject.SetActive(false);
                continue;
            }
            if ( scriptData.scripts[i] == "")
            {
                scripts[i].gameObject.SetActive(false);
            }
            else
            {
                scripts[i].gameObject.SetActive(true);
                scripts[i].text = scriptData.scripts[i];
            }
        }
        bool isActive = false;
        for (int i = 0; i < images.Length; i++)
        {
            if (scriptData.sprites == null || scriptData.sprites.Length <= 0)
            {
                images[i].gameObject.SetActive(false);
                continue;
            }
            if ( scriptData.sprites[i] == null)
            {
                images[i].gameObject.SetActive(false);
            }
            else
            {
                if (!isActive) isActive = true;
                images[i].gameObject.SetActive(true);
                images[i].sprite = scriptData.sprites[i];
            }
        }
        imageHorizontalGroup.gameObject.SetActive(isActive);
    }

}
