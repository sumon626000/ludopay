using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class ChatPanelController : MonoBehaviour
{
    public Transform MessageGrid;
    public Transform EmojiGrid;
    public LudoRoundController RoundController;
    private SocketIOComponent socket;

    private void OnEnable()
    {
        for (int i = 0; i < MessageGrid.childCount; i++)
        {
            Global.AddOnClickTriggerEvent(this, MessageGrid.GetChild(i).GetComponent<UIEventTrigger>(), "SendMsg", i, typeof(int));
        }
        for (int i = 0; i < EmojiGrid.childCount; i++)
        {
            Global.AddOnClickTriggerEvent(this, EmojiGrid.GetChild(i).GetComponent<UIEventTrigger>(), "SendEmoji", i, typeof(int));
        }
    }
    void Start()
    {
        socket = SocketManager.Instance.GetSocketIOComponent();
        socket.On("REQ_CHAT_RESULT", OnGetChatResult);
    }

    public void SendMsg(int i)
    {
        if (isSend)
        {
            string message = MessageGrid.GetChild(i).GetChild(0).GetComponent<UILabel>().text;
            Chat(message);
            cor = StartCoroutine(CheckDoubleClick());
        }
    }
    Coroutine cor;
    public void SendEmoji(int i)
    {
        if (isSend)
        {
            Chat("", i);
            cor = StartCoroutine(CheckDoubleClick());
        }
    }
    bool isSend = true;
    IEnumerator CheckDoubleClick()
    {
        isSend = false;
        yield return new WaitForSeconds(2.5f);
        isSend = true;
    }
    private void Chat(string message = "", int emoji = -1)
    {
        if (emoji == -1)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("roomid", GameManager.Instance.RoomID.ToString());
            data.Add("username", GameManager.Instance.UserName);
            data.Add("message", message + ".");
            JSONObject jdata = new JSONObject(data);
            socket.Emit("REQ_CHAT", jdata);
        }
        else if (string.IsNullOrEmpty(message))
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("roomid", GameManager.Instance.RoomID.ToString());
            data.Add("username", GameManager.Instance.UserName);
            data.Add("message", emoji.ToString());
            JSONObject jdata = new JSONObject(data);
            socket.Emit("REQ_CHAT", jdata);
        }
    }
    int a = 0;
    private void OnGetChatResult(SocketIOEvent evt)
    {
        string result = Global.JsonToString(evt.data.GetField("result").ToString(), "\"");
        a++;
        if (a > 1)
            return;
        if (result.Equals("success"))
        {
            string username = Global.JsonToString(evt.data.GetField("username").ToString(), "\"");
            string message = Global.JsonToString(evt.data.GetField("message").ToString(), "\"");

            for (int i = 0; i < RoundController.UserDetails.Count; i++)
            {
                if (RoundController.UserDetails[i].username == username)
                {
                    LudoPlayerController obj = RoundController.PlayerControllers[i];
                    if (message.Contains("."))
                    {
                        obj.Chat.GetChild(0).gameObject.SetActive(true);
                        obj.Chat.GetChild(1).gameObject.SetActive(false);
                        obj.Chat.GetChild(0).GetComponent<UILabel>().text = message;
                        StartCoroutine(DisableMessage(obj.Chat.GetChild(0).gameObject));
                    }
                    else
                    {
                        obj.Chat.GetChild(0).gameObject.SetActive(false);
                        obj.Chat.GetChild(1).gameObject.SetActive(true);
                        int index = int.Parse(message);
                        string sprite_name = EmojiGrid.GetChild(index).GetComponent<UISprite>().spriteName;
                        obj.Chat.GetChild(1).GetComponent<UISprite>().spriteName = sprite_name;
                        StartCoroutine(DisableMessage(obj.Chat.GetChild(1).gameObject));
                    }
                    
                    break;
                }
            }
            a = 0;
        }
    }
    IEnumerator DisableMessage(GameObject obj)
    {
        yield return new WaitForSeconds(2.0f);
        obj.SetActive(false);
    }
}
