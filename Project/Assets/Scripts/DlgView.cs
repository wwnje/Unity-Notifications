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
    
//    public TMP_Text TextTimeLeft;
    public TMP_InputField TextInput;
    public Button ButtonAdd;
    public int Index = 0;
    private void Awake()
    {
        ButtonAdd.OnClickAsObservable().Subscribe(_ =>
        {
            if (int.TryParse(TextInput.text, out var seconds))
            {
                _gameNotificationsManager.CreateNotification(new OptionalNotification
                {
                    Title = "Hello",
                    Body = "Index:" + Index++,
                    DeliveryTime = System.DateTime.Now.AddSeconds(seconds)
                });
            }
        });
    }
}