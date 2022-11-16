using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CustomHorizontalScrollInfinity : MonoBehaviour
{
    [Tooltip("크기와 위치를 지정할 Rect를 설정해주면 됩니다. 사라지는 위치도 추가로 지정해 주어야 합니다.")]
    public RectTransform[] scrollAnchorPoints;
    [Tooltip("변경할 이미지의 오브젝트")]
    public UIMapBox[] scrollTarget;
    public Transform roomNamePanel;
    public Text roomNameText;
    public Button rightArrow;
    public Button leftArrow;
    public float animTime = 1f;
    public AnimationCurve ease;

    private List<ThumbnailData.RoomThumbnail> roomThumbnails = new List<ThumbnailData.RoomThumbnail>();
    private int currentIdx;
    private ThumbnailData thumbnailData;
    private Sequence sequence;
    private UIMapBox[] seat;

    public bool TryGetSelectRoom(out ThumbnailData.RoomThumbnail data)
    {
        if (roomThumbnails.Count > 0)
        {
            data = roomThumbnails[currentIdx];
            return true;
        }

        data = null;
        return false;
    }
    public void Initalize(ThumbnailData thumbnail)
    {
        this.thumbnailData = thumbnail;
        seat = scrollTarget;
    }

    public void Set()
    {
        roomThumbnails = thumbnailData.GetThumbnails(ContentTypes.Event);
        currentIdx = 0;

        if (roomThumbnails.Count <= 0 || roomThumbnails == null)
        {
            leftArrow.interactable = false;
            rightArrow.interactable = false;
            roomNamePanel.gameObject.SetActive(true);
            roomNameText.text = "Not Found Map";
            for (int i = 0; i < seat.Length; i++)
            {
                seat[i].DisableImage();
            }
            return;
        }

        if (roomThumbnails.Count == 1)
        {
            leftArrow.interactable = false;
            rightArrow.interactable = false;

            for (int i = 0; i < seat.Length; i++)
            {
                if (i == 2)
                {
                    roomNamePanel.gameObject.SetActive(true);
                    roomNameText.text = roomThumbnails[currentIdx].sceneName;
                    seat[i].EnableImage(roomThumbnails[currentIdx].thumbnail);
                    continue;
                }

                seat[i].DisableImage();
            }
            return;
        }

        if (roomThumbnails.Count == 2)
        {
            leftArrow.interactable = false;
            rightArrow.interactable = true;
            for (int i = 0; i < seat.Length; i++)
            {
                if (i == 2)
                {
                    roomNamePanel.gameObject.SetActive(true);
                    roomNameText.text = roomThumbnails[currentIdx].sceneName;
                    seat[i].EnableImage(roomThumbnails[currentIdx].thumbnail);
                    continue;
                }

                if (i == 3)
                {
                    seat[i].EnableImage(roomThumbnails[currentIdx+1].thumbnail);
                    continue;
                }

                seat[i].DisableImage();
            }
            return;
        }


        currentIdx = 1;

        leftArrow.interactable = true;
        rightArrow.interactable = true;

        roomNamePanel.gameObject.SetActive(true);
        roomNameText.text = roomThumbnails[currentIdx].sceneName;

        for (int i = 0; i < seat.Length; i++)
        {
            if (i == 0)
            {
                continue;
            }
            seat[i].EnableImage(roomThumbnails[i - 1].thumbnail);
        }

    }
    public void ResetData()
    {
        currentIdx = 0;
        roomThumbnails.Clear();
    }
    public void OnAnimation(bool isRight = true)
    {
        if (sequence.IsActive())
        {
            sequence.Complete(true);
        }

        if (roomThumbnails.Count == 2)
        {
            MakeTwoRoomSelect(isRight);
            return;
        }

        MakeThreeRoomSelect(isRight);
    }

    private void MakeTwoRoomSelect(bool isRight)
    {
        seat[0].rect.anchoredPosition = isRight ? scrollAnchorPoints[0].anchoredPosition : scrollAnchorPoints[4].anchoredPosition;

        sequence = DOTween.Sequence();
        sequence.SetEase(ease);
        leftArrow.interactable = false;
        rightArrow.interactable = false;
        roomNamePanel.gameObject.SetActive(false);

        if (isRight)
        {
            SetAnchorPosSequence(seat[3], scrollAnchorPoints[2]);
            SetSizeDelteSequence(seat[3], scrollAnchorPoints[2]);
            SetAnchorPosSequence(seat[2], scrollAnchorPoints[1]);
            SetSizeDelteSequence(seat[2], scrollAnchorPoints[1]);
            seat[3].transform.SetAsLastSibling();
        }
        else
        {
            SetAnchorPosSequence(seat[1], scrollAnchorPoints[2]);
            SetSizeDelteSequence(seat[1], scrollAnchorPoints[2]);
            SetAnchorPosSequence(seat[2], scrollAnchorPoints[3]);
            SetSizeDelteSequence(seat[2], scrollAnchorPoints[3]);
            seat[1].transform.SetAsLastSibling();
        }

        sequence.OnComplete(() =>
        {
            UIMapBox[] changeSeat;
            if (isRight)
            {
                changeSeat = new UIMapBox[] { seat[1], seat[2], seat[3], seat[0] };
                currentIdx--;
                if (currentIdx < 0)
                {
                    currentIdx = roomThumbnails.Count - 1;
                }
                leftArrow.interactable = true;
                rightArrow.interactable = false;

            }
            else
            {
                changeSeat = new UIMapBox[] { seat[3], seat[0], seat[1], seat[2] };
                currentIdx++;
                if (currentIdx > roomThumbnails.Count - 1)
                {
                    currentIdx = 0;
                }
                leftArrow.interactable = false;
                rightArrow.interactable = true;
            }

            seat = changeSeat;

            roomNamePanel.gameObject.SetActive(true);
            roomNameText.text = roomThumbnails[currentIdx].sceneName;
        });
    }
    private void MakeThreeRoomSelect(bool isRight)
    {
        seat[0].rect.anchoredPosition = isRight ? scrollAnchorPoints[0].anchoredPosition : scrollAnchorPoints[4].anchoredPosition;

        sequence = DOTween.Sequence();
        sequence.SetEase(ease);
        leftArrow.interactable = false;
        rightArrow.interactable = false;
        roomNamePanel.gameObject.SetActive(false);

        int nextIdx;

        if (isRight)
        {
            if (currentIdx - 2 < 0)
            {
                if (currentIdx == 1)
                {
                    nextIdx = roomThumbnails.Count - 1;
                }
                else
                {
                    nextIdx = roomThumbnails.Count - 2;
                }
            }
            else
            {
                nextIdx = currentIdx - 2;
            }

            SetAnchorPosSequence(seat[0], scrollAnchorPoints[1]);
            SetAnchorPosSequence(seat[1], scrollAnchorPoints[2]);
            SetSizeDelteSequence(seat[1], scrollAnchorPoints[2]);
            SetAnchorPosSequence(seat[2], scrollAnchorPoints[3]);
            SetSizeDelteSequence(seat[2], scrollAnchorPoints[3]);
            SetAnchorPosSequence(seat[3], scrollAnchorPoints[4]);
            seat[1].transform.SetAsLastSibling();
        }
        else
        {
            if (currentIdx + 2 > roomThumbnails.Count - 1)
            {
                nextIdx = 0;
            }
            else
            {
                nextIdx = currentIdx + 2;
            }
            SetAnchorPosSequence(seat[0], scrollAnchorPoints[3]);
            SetAnchorPosSequence(seat[3], scrollAnchorPoints[2]);
            SetSizeDelteSequence(seat[3], scrollAnchorPoints[2]);
            SetAnchorPosSequence(seat[2], scrollAnchorPoints[1]);
            SetSizeDelteSequence(seat[2], scrollAnchorPoints[1]);
            SetAnchorPosSequence(seat[1], scrollAnchorPoints[0]);
            seat[3].transform.SetAsLastSibling();
        }
        seat[0].EnableImage(roomThumbnails[nextIdx].thumbnail);

        sequence.OnComplete(() =>
        {
            UIMapBox[] changeSeat;
            if (isRight)
            {
                changeSeat = new UIMapBox[] { seat[3], seat[0], seat[1], seat[2] };
                currentIdx--;
                if (currentIdx < 0)
                {
                    currentIdx = roomThumbnails.Count - 1;
                }
                leftArrow.interactable = true;
                rightArrow.interactable = true;
            }
            else
            {
                changeSeat = new UIMapBox[] { seat[1], seat[2], seat[3], seat[0] };
                currentIdx++;
                if (currentIdx > roomThumbnails.Count - 1)
                {
                    currentIdx = 0;
                }
                leftArrow.interactable = true;
                rightArrow.interactable = true;
            }

            seat = changeSeat;

            roomNamePanel.gameObject.SetActive(true);
            roomNameText.text = roomThumbnails[currentIdx].sceneName;
        });

    }

    private void SetAnchorPosSequence(UIMapBox mapbox, RectTransform targetRect)
    {
        sequence.Insert(0, mapbox.rect.DOAnchorPos(targetRect.anchoredPosition, animTime));
    }
    private void SetSizeDelteSequence(UIMapBox mapbox, RectTransform targetRect)
    {
        sequence.Insert(0, mapbox.rect.DOSizeDelta(targetRect.sizeDelta, animTime));
    }
}
