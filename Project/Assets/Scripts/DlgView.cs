using System;
using PuzzlesKingdom.Notifications;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public class DlgView : MonoBehaviour
{
    [Inject] private IGameNotificationsManager _gameNotificationsManager;
    
    public TMP_InputField TextTitle;
    public TMP_InputField TextBody;
    public TMP_InputField TextTime;
    public TMP_InputField ID;
    public Button ButtonSend;
    
    public Button ButtonAddMinute;
    public Button ButtonAdd1S;
    public Button ButtonAdd5S;

    public Toggle ToggleUseID;
    public Toggle ToggleRepeat;
    public Toggle ToggleGroup;
    public Toggle ToggleBage;

    private const int ID_60 = 60;
    private const int ID_1= 1;
    private const int ID_5 = 5;
    
    private void Awake()
    {
        int indexMin = 0;
        ButtonAddMinute.OnClickAsObservable().Subscribe(_ =>
        {
            var data = new OptionalNotification
            {
                Title = "+ 1 min",
                Body = "Index:" + indexMin++,
                DeliveryTime = System.DateTime.Now.AddSeconds(60),
            };

            UpdateNotification(ID_60, data);
        });
        
        int index1 = 0;
        ButtonAdd1S.OnClickAsObservable().Subscribe(_ =>
        {
            var data = new OptionalNotification
            {
                Title = "+ 1 s",
                Body = "Index:" + index1++,
                DeliveryTime = System.DateTime.Now.AddSeconds(1)
            };

            UpdateNotification(ID_1, data);
        });
        
        int index5 = 0;
        ButtonAdd5S.OnClickAsObservable().Subscribe(_ =>
        {
            var data = new OptionalNotification
            {
                Title = "+ 5 s",
                Body = "Index:" + index5++,
                DeliveryTime = System.DateTime.Now.AddSeconds(5)
            };

            UpdateNotification(ID_5, data);
        });
        
        ButtonSend.OnClickAsObservable().Subscribe(_ =>
        {
            var data = new OptionalNotification
            {
                Title = TextTitle.text,
                Body = TextBody.text,
                DeliveryTime = System.DateTime.Now.AddSeconds(int.Parse(TextTime.text))
            };

            UpdateNotification(int.Parse(ID.text), data);
        });
    }

    private void UpdateNotification(int id, OptionalNotification data)
    {
        data.Repeat = ToggleRepeat.isOn;

        if (ToggleGroup.isOn)
        {
            data.group = data.Body;
        }

        if (ToggleBage.isOn)
        {
            data.badgeNumber = id;
        }
        
        if (ToggleUseID.isOn)
        {
            _gameNotificationsManager.UpdateScheduledNotification(id, data);
        }
        else
        {
            _gameNotificationsManager.CreateNotification(data);
        }
    }
}