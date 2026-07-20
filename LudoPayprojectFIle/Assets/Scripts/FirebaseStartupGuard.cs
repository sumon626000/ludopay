using UnityEngine;

#if !GUEST_BUILD
/// <summary>
/// Disables Firebase-related MonoBehaviours before their Awake/Start run on first scene load.
/// Guest APK uses socket login only; Facebook/Firebase init happens on explicit user action.
/// </summary>
public static class FirebaseStartupGuard
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void DisableFirebaseComponentsAtStartup()
    {
        foreach (var manager in Object.FindObjectsOfType<FirebaseAuthManager>(true))
            manager.enabled = false;

        foreach (var manager in Object.FindObjectsOfType<FirebaseFacebookManager>(true))
            manager.enabled = false;
    }
}
#endif
