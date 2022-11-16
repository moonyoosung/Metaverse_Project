using MindPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleRoomManager : MindPlus.RoomManager, AccountManager.IEventHandler
{
    public interface IEventHandler
    {
        void OnCreatePlayer(MindPlusPlayer player);
    }
    [Header("Others")]
    public bool isTestVR;
    public Transform playerAnchor;
    public TitleUIManager titleUI;

    private PhotonInstantiation instantiation;
    private List<IEventHandler> eventHandlers = new List<IEventHandler>();


    private void OnDestroy()
    {
        accountManager?.UnResisterEvent(this);
    }
    public override IEnumerator Initialize(GameManager gameManager)
    {
        yield return null;
        this.instantiation = gameManager.Persistent.PhotonInstantiation;
        this.roomDataManager = gameManager.Persistent.RoomDataBaseManager;

        yield return titleUI.Initialize(gameManager.Persistent);
        this.accountManager = gameManager.Persistent.AccountManager;
        accountManager.ResisterEvent(this);

        List<UIView> firstShow = new List<UIView>();

        firstShow.Add(UIView.Get("TitleBGView"));
        firstShow.Add(UIView.Get<TitleMainPanelView>());

        StartCoroutine(WaitForInitalized(firstShow));
    }

    //타이틀 스플래쉬 화면이 종료 후 UI 활성화
    private IEnumerator WaitForInitalized(List<UIView> firstShow)
    {
        if (GameManager.Instance.splashView)
        {
            yield return new WaitUntil(() => GameManager.Instance.splashView.isSplashEnd);
        }
       
        yield return new WaitForSeconds(0.6f);

        titleUI.Push(StringTable.Init, false, false, null, firstShow.ToArray());

        yield return null;
        GameManager.Instance.splashView.gameObject.SetActive(false);
    }

    public void Resist(IEventHandler eventHandler)
    {
        eventHandlers.Add(eventHandler);
    }
    public void UnResist(IEventHandler eventHandler)
    {
        eventHandlers.Remove(eventHandler);
    }
    private void CreateCharacter()
    {
        MindPlusPlayer player = instantiation.SpawnPlayer(new BuilderSetting(false, true, false, false));

        player.Initialize(GameManager.Instance, false, true, false);
        player.transform.SetParent(playerAnchor);
        player.transform.localPosition = new Vector3(0, 1f, 0);
        player.transform.localRotation = Quaternion.Euler(Vector3.zero);
        player.GetPart<LookAtCanvas>().setPlayerNick.transform.localRotation = new Quaternion(0, 180f, 0, 0);


        UIView.Get<SelectCharacterView>().OnCreatePlayer(player);
    }
    public void OnFailLogin(string error)
    {

    }

    public void OnSuccessLogin(LocalPlayerData playerData)
    {
        if (playerData.userName == null || playerData.userName == string.Empty)
        {
            CreateCharacter();
        }
        else
        {
        }
    }
    public void OnSuccessSignUp()
    {
        CreateCharacter();
    }

    public void OnFailSignUp(string error)
    {

    }

    public void OnSuccessGetUser(LocalPlayerData playerData)
    {
       
    }

    public void OnFailGetUser(string error)
    {
    }
}
