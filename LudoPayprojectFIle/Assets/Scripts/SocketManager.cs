using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class SocketManager : MonoBehaviour
{
    public static SocketManager Instance { get; private set; }

    /// <summary>Inspector override before build (e.g. ws://PUBLIC_IP:3000/socket.io/?EIO=3&amp;transport=websocket).</summary>
    [Tooltip("If set, used instead of SocketIOComponent.url in the scene.")]
    public string serverUrlOverride = "";

    private const string PrefsSocketUrl = "SocketServerUrl";
    private const string PrefsServerHost = "SocketServerHost";
    private const string PrefsServerPort = "SocketServerPort";
    private const string DefaultSocketPath = "/socket.io/?EIO=3&transport=websocket";

    private SocketIOComponent socket;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log("🔵 SocketManager Start()");

        socket = GetComponent<SocketIOComponent>();

        if (socket == null)
        {
            Debug.LogError("❌ SocketIOComponent component নাই! Inspector এ add করুন");
            return;
        }

        ApplyServerUrl();
        Debug.Log("🔵 Socket URL: " + socket.url);

        socket.On("open", OnOpen);
        socket.On("error", OnError);
        socket.On("close", OnClose);
        socket.On("guestLoginResponse", OnGuestResponse);

        Debug.Log("🔵 Connecting...");
        socket.Connect();
    }

    void OnOpen(SocketIOEvent e) { Debug.Log("✅ CONNECTED to server"); }
    void OnError(SocketIOEvent e) { Debug.LogError("❌ ERROR: " + e.data); }
    void OnClose(SocketIOEvent e) { Debug.LogWarning("⚠️ CLOSED: " + e.data); }
    void OnGuestResponse(SocketIOEvent e) { Debug.Log("📥 Guest response: " + e.data); }

    public void GuestLogin()
    {
        Debug.Log("🔵 Sending guestLogin event");
        socket.Emit("guestLogin");
    }

    public SocketIOComponent GetSocketIOComponent() => socket;

    /// <summary>Call from settings UI: full ws URL or host + port.</summary>
    public static void SetServerEndpoint(string host, int port = 3000)
    {
        PlayerPrefs.SetString(PrefsServerHost, host ?? "");
        PlayerPrefs.SetInt(PrefsServerPort, port);
        PlayerPrefs.DeleteKey(PrefsSocketUrl);
        PlayerPrefs.Save();
    }

    public static void SetServerUrl(string wsUrl)
    {
        PlayerPrefs.SetString(PrefsSocketUrl, wsUrl ?? "");
        PlayerPrefs.Save();
    }

    private void ApplyServerUrl()
    {
        if (!string.IsNullOrWhiteSpace(serverUrlOverride))
        {
            socket.url = serverUrlOverride.Trim();
            return;
        }

        string fullUrl = PlayerPrefs.GetString(PrefsSocketUrl, "");
        if (!string.IsNullOrWhiteSpace(fullUrl))
        {
            socket.url = fullUrl.Trim();
            return;
        }

        string host = PlayerPrefs.GetString(PrefsServerHost, "");
        if (!string.IsNullOrWhiteSpace(host))
        {
            int port = PlayerPrefs.HasKey(PrefsServerPort)
                ? PlayerPrefs.GetInt(PrefsServerPort, 3000)
                : 3000;
            host = host.Trim().Replace("ws://", "").Replace("wss://", "").Replace("http://", "").Replace("https://", "");
            int slash = host.IndexOf('/');
            if (slash >= 0) host = host.Substring(0, slash);
            var scheme = PlayStoreCompliance.AllowCleartextTraffic ? "ws://" : "wss://";
            socket.url = scheme + host + ":" + port + DefaultSocketPath;
        }
    }
}
