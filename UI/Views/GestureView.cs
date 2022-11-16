using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MindPlus.Contexts.Master;
using System;
using UnityEngine.UI;
using static MindPlus.AnimationController;

public class GestureView : UIView
{
    private UIPool gesturePool;
    public ScrollRect scroll;
    private List<UIGesture> uIGestures = new List<UIGesture>();

    public override void Initialize(Persistent persistent, BaseUIManager uIManager)
    {
        base.Initialize(persistent, uIManager);
        this.gesturePool = (uIManager as UIManager).GetPool(StringTable.UIGesturePool);

    }
    public override void OnStartShow()
    {
        base.OnStartShow();
        GetGestureList();
    }
    public void GetGestureList()
    {
        var list = GetGestureOrderList();

        int length = list.Count;

        for (int i = 0; i < list.Count; i++)
        {
            UIGesture uiGesture = gesturePool.Get<UIGesture>(scroll.content.transform);
            uiGesture.Set(list[i], i < 2, ()=>UIManager.Pop());
            uIGestures.Add(uiGesture);
        }
        GridLayoutGroup layout = scroll.content.GetComponent<GridLayoutGroup>();
        float y = layout.cellSize.y + layout.padding.top + layout.padding.bottom + layout.spacing.y;
        if (length % 2 != 0)
            length += 1;
        scroll.content.sizeDelta = new Vector2(scroll.content.sizeDelta.x, y * (int)(length / 2) );
    }

    List<MindPlus.AnimationController.Gesture> GetGestureOrderList()
    {
        List<MindPlus.AnimationController.Gesture> list = new List<MindPlus.AnimationController.Gesture>();

        MindPlus.AnimationController.Gesture gesture = (MindPlus.AnimationController.Gesture)0;
        int length = LooxidExtensions.EnumLength<MindPlus.AnimationController.Gesture>(gesture);
        list.Add(Gesture.BREATHING);
        list.Add(Gesture.HIGHFIVE);

        for (int i = 0; i < length; i++)
        {
            var newGesture = LooxidExtensions.GetEnumAt<MindPlus.AnimationController.Gesture>(gesture, i);
            if (!list.Contains(newGesture))
                list.Add(newGesture);
        }

        return list;
    }
}
