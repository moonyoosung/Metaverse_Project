using MindPlus;
using MindPlus.Contexts.Pool;
using Slash.Unity.DataBind.Core.Presentation;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UIGesture : UIPoolObject
{
    public ContextHolder holder;
    private UIGestureContext context;

    private string beforeGesture = "";

    public override void Initialize(Transform parent)
    {
        base.Initialize(parent);
        this.context = new UIGestureContext();
        holder.Context = context;

    }

    public void Set(MindPlus.AnimationController.Gesture gesture, bool isFront = false, System.Action callback = null)
    {
        if (string.Compare(gesture.ToString(), beforeGesture) == 0)
            return;
        context.SetValue("IsActiveBackLight", isFront);
        context.SetValue("IsActiveFavorite", false);
        //context.SetValue("IsActiveFavorite", !isFront);
        context.SetValue("GestureNameText", gesture.ToString());
        beforeGesture = gesture.ToString();
        Image icon = transform.Find("ImageIcon").GetComponent<Image>();
        icon.sprite = Resources.Load<Sprite>("UI/Gesture/" + gesture.ToString());

        switch (gesture)
        {
            case AnimationController.Gesture.BREATHING:
                context.onClickGesture = () =>
                {
                    callback?.Invoke();
                    NetworkManager.Instance.currentRoomManager.player.GetPart<PlayerRig>().animationController.PlayGestureAnimation(AnimationController.Gesture.MEDITATION);
                    AddMeditationScene();
                };
                break;
            case AnimationController.Gesture.HIGHFIVE:
                context.onClickGesture = () =>
                {
                    callback?.Invoke();
                    NetworkManager.Instance.currentRoomManager.player.SendInteractionType(InteractionTarget.IconKinds.Highfive);
                };
                break;
            default:
                context.onClickGesture = () =>
                {
                    callback?.Invoke();
                    NetworkManager.Instance.currentRoomManager.player.GetPart<PlayerRig>().animationController.PlayGestureAnimation(gesture);
                };
                break;
        }

    }

    private void AddMeditationScene()
    {
        AudioManager.Instance.SetActiveSceneSoundMute(true);
        GameManager.Instance.Persistent.UIManager.PopToRoot(null, 0);
        StartCoroutine(GameManager.Instance.LoadSceneManager.AdditiveSceneAsync("Meditation", true));
    }
}
