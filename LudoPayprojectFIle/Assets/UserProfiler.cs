using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfiler : MonoBehaviour
{
    public UIInput Username;
    public UILabel ID, Points, Level;
    public UILabel Online_played, Online_won, Friend_played, Friend_won, Tokenscaptured_mine, Tokenscaptured_opponents, Winstreaks_current, Winstreaks_best;
    public ImageDownload Photo;

    public void SetUI(UserDetail user, Texture userphoto)
    {
        Username.value = user.username;
        ID.text = user.userid;
        Points.text = user.points.ToString("N0");

        Level.text = user.referral_count.ToString();
        
        Photo.GetComponent<UIMaskedTexture>().mainTexture = userphoto;
        Online_played.text = user.online_multiplayer.played.ToString();
        Online_won.text = user.online_multiplayer.won.ToString();
        Friend_played.text = user.friend_multiplayer.played.ToString();
        Friend_won.text = user.friend_multiplayer.won.ToString();
        Tokenscaptured_mine.text = user.tokens_captured.mine.ToString();
        Tokenscaptured_opponents.text = user.tokens_captured.opponents.ToString();
        Winstreaks_current.text = user.won_streaks.current.ToString();
        Winstreaks_best.text = user.won_streaks.best.ToString();

        
    }
    public void SetUI_BOT()
    {

    }
    public void OnExit()
    {
        GetComponent<UIPopup>().Close();
    }
}
