using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;



public class NativeShare : MonoBehaviour
{

	#if UNITY_IOS
	[DllImport ("__Internal")]
	private static extern void Unity_Share (string filePathJson, string subject, string msg, string emailsJson, string shareApp);
		#endif


	static AndroidJavaClass share;

	static AndroidJavaClass JavaShareClass {

		get {
			if (share == null) {

				share = new AndroidJavaClass ("com.draft.unityplugins.NativeShare");

			}

			return share;
		}


	}

	public delegate void DownloadingHanderler (float progress);

	public static event DownloadingHanderler OnDownloading;

	public delegate void DownloadCompletedHanderler ();

	public static event DownloadCompletedHanderler OnDownloadCompleted;


	public delegate void ShareCompletedHanderler (SharingResult result);

	public static event ShareCompletedHanderler OnShareCompleted;



	static NativeShare instnace;

	static NativeShare Instance {

		get {
			if (instnace == null) {
				shareObject = new GameObject ("NativeShare");
				instnace = shareObject.AddComponent<NativeShare> ();
			}
			return instnace;
		}

	}

	static GameObject shareObject;

	static GameObject listenner;


	public enum SharingResult
	{
		Unknown = 0,
		Success = 1,
		Cancel = 2,
		Fail = 3


	}

	public void NativeShare_OnShareCompleted(string result){

		SharingResult r = (SharingResult)System.Int32.Parse (result);


		if (OnShareCompleted != null)
			OnShareCompleted (r);

		if (listenner != null)
			DestroyObject (gameObject);


	}




	static void Bridge_Share( string[] filesPath, string subject, string msg,string[] emails,string shareApp=""){



		bool detectURL = false;

		if(filesPath != null)	
			detectURL = DetectURL (filesPath);



		if (detectURL)
			Instance.StartCoroutine (Instance.LoadFiles (filesPath, subject, msg, null, shareApp));
		else {
			if (Application.isEditor) {

				Debug.LogWarning ("[Native Share] : You can share on your device");
				return;
			}


			listenner = new GameObject ("NativeShareListener", typeof(NativeShare));

			#if UNITY_ANDROID

			string shareType = "*/*";


			if(shareApp == ShareApp.Instagram)
				shareType = "image/*";
			

				if(DetectEmailApp(shareApp))	shareType = "message/rfc822";

				JavaShareClass.CallStatic ("Native_Share", filesPath, subject, msg, emails, shareType, shareApp);


			#elif UNITY_IOS

			string filesJsonString =  "";

			if(filesPath != null || filesPath.Length > 0)
				filesJsonString = JsonListToString (filesPath);

			//Debug.Log(filesJsonString);

					string emailJsonString = JsonListToString (emails);

					Unity_Share (filesJsonString, subject, msg, emailJsonString, shareApp);

			#endif
		}

	}

	static void Bridge_ShareImageData (Texture2D[] textures, string subject, string msg,string[] emails,string shareApp="")
	{

		Instance.StartCoroutine(Instance.EncodeImageData(textures,subject,msg,emails,shareApp));
	}








	IEnumerator EncodeImageData (Texture2D[] textures, string subject, string msg, string[] emails, string shareApp = "")
	{

	

		string[] filesList = new string[textures.Length];


		string baseFloder = Path.Combine (Application.persistentDataPath, "NativeShare");
		if (!Directory.Exists (baseFloder)) {
			Directory.CreateDirectory (baseFloder);
		}

		for (int i = 0; i < textures.Length; i++) {

			//string imageData = "";
			byte[] bytes;

			bytes = textures [i].EncodeToPNG ();
			yield return bytes;
	
			string pathSave = Path.Combine (baseFloder, "Image_" + System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss")+".png");


			try {
				File.WriteAllBytes (pathSave, bytes);
			} catch (System.Exception ex) {
				Debug.Log ("write file error : " + ex.Message);
			} finally {

				Debug.Log ("add path : " + pathSave);
				filesList [i] = pathSave;

			}



		}

		if (Application.isEditor) {
			Debug.LogWarning ("[Native Share] : You can share on your device");

			yield break;

		}

		listenner = new GameObject ("NativeShareListener", typeof(NativeShare));


		#if UNITY_ANDROID
		string shareType = "image/*";
		if (DetectEmailApp (shareApp))
		shareType = "message/rfc822";
		JavaShareClass.CallStatic ("Native_Share", filesList, subject, msg, emails,shareType, shareApp);

		//JavaShareClass.CallStatic ("Native_ShareImageData", stringDataList, subject, msg, emails,shareType, shareApp);

		#elif UNITY_IOS


		string filesJsonString = JsonListToString (filesList);
		string emailJsonString = JsonListToString (emails);

		Unity_Share (filesJsonString, subject, msg, emailJsonString, shareApp);


		#endif

		DestroyObject (gameObject);
	}

	IEnumerator LoadFiles (string[] filesPath, string subject, string msg, string[] emails, string shareApp = "")
	{
		if (filesPath == null)
			yield break;
		
		string[] filesList = new string[filesPath.Length];

		string baseFloder = Path.Combine (Application.persistentDataPath, "NativeShare");
		if (!Directory.Exists (baseFloder)) {
			Directory.CreateDirectory (baseFloder);
		}

		float progres = 0;


		bool loadFile = false;
		for (int i = 0; i < filesPath.Length; i++) {


			string fileURL = filesPath [i];
			FileInfo fileInfo = new FileInfo (filesPath [i]);


			if (!DetectURL (fileURL)) {

				Debug.Log ("add path : " + fileURL);
				filesList [i] = fileURL;
				continue;
			}


			loadFile = true;


			string pathSave = Path.Combine (baseFloder, fileInfo.Name);
			//Debug.Log ("download path : " + fileURL);

			WWW www = new WWW (fileURL);
			while (!www.isDone) {

				progres = (www.progress * 100f);

				if (OnDownloading != null) {

					OnDownloading (progres);
				}

				yield return null;
			}

			if (OnDownloading != null) {

				OnDownloading (100);
			}

			if (string.IsNullOrEmpty (www.error)) {

				try {
					File.WriteAllBytes (pathSave, www.bytes);
				} catch (System.Exception ex) {
					Debug.Log ("write file error : " + ex.Message);
				} finally {
					
					//Debug.Log ("add path : " + pathSave);
					filesList [i] = pathSave;
					www.Dispose ();
				}


			} else {
				Debug.Log ("Download error : " + www.error);
			}


		}


		if (loadFile) {
			//Debug.Log ("Download Complete");
			if (OnDownloadCompleted != null) {

				OnDownloadCompleted ();
			}
		}
			


		Bridge_Share (filesList, subject, msg, emails, shareApp);

		DestroyObject (gameObject);


	}




	static bool DetectEmailApp (string shareApp)
	{
		Match mailAppDetect = Regex.Match (shareApp, ShareApp.Mail + "|" + ShareApp.Gmail);

		return mailAppDetect.Success;
	}


	static bool DetectURL (string url)
	{
		bool detect = false;
		string pattern = @"\b(?:https?|ftp://www\.)\S+\b";
		Regex reg = new Regex (pattern);
		detect = reg.IsMatch (url);

		#if UNITY_ANDROID

		FileInfo fileInfo = new FileInfo(url);
		if(fileInfo.DirectoryName.Substring (11) == Application.streamingAssetsPath.Substring (12))
			detect = true;	
		#endif


		return detect;
	}

	static bool DetectURL (string[] urls)
	{
		bool detect = false;



		for (int i = 0; i < urls.Length; i++) {
			
			if (DetectURL (urls [i])) {
				detect = true;
				break;
			}
		}

		


		return detect;


	}

	static string JsonListToString (string[] data)
	{
		string jsonString = "";

		if (data == null)
			return "[]";

		jsonString = "[";
		if (data.Length > 0) {

			for (int i = 0; i < data.Length; i++) {

				if (i >= 1)
					jsonString += ",";

				jsonString += "\"" + data [i] + "\"";

			}


		}
		jsonString += "]";
		return jsonString;

	}




	//Public


	public static void ShareText (string msg, string shareApp = "")
	{
		Bridge_Share (null, "", msg, null, shareApp);
	}
	public static void ShareText (string subject,string msg, string shareApp = "")
	{
		Bridge_Share (null, subject, msg, null, shareApp);
	}


	//ShareFile
	public static void ShareFile (string filepath, string shareApp = "")
	{
		ShareFile (filepath, "", "", shareApp);
	}

	public static void ShareFile (string filepath, string subject, string msg, string shareApp = "")
	{

		Bridge_Share (new string[1]{ filepath }, subject, msg, null, shareApp);
	}


	//ShareTexture2D
	public static void ShareTexture (Texture2D texture, string shareApp = "")
	{

		ShareTexture (texture, "", "", shareApp);

	}

	public static void ShareTexture (Texture2D texture, string subject, string msg, string shareApp = "")
	{

	
		Bridge_ShareImageData (new Texture2D[1]{ texture }, subject, msg, null, shareApp);
	}

	//ShareMultiFiles

	public static void ShareFiles (string[] filepaths, string shareApp = "")
	{

		ShareFiles (filepaths, "", "", shareApp);
	}

	public static void ShareFiles (string[] filespath, string subject, string msg, string shareApp = "")
	{

		Bridge_Share (filespath, subject, msg, null, shareApp);
	}


	//ShareMultiTextures
	public static void ShareTextures (Texture2D[] texture, string shareApp = "")
	{

		ShareTextures (texture, "", "", shareApp);
	}

	public static void ShareTextures (Texture2D[] textures, string subject, string msg, string shareApp = "")
	{

		Bridge_ShareImageData (textures, subject, msg, null, shareApp);
	}


	//Send Email

	public static void SendEmail (string subject, string msg, string email, string shareApp = "")
	{

		if (string.IsNullOrEmpty (shareApp))
			shareApp = ShareApp.Mail;

		Bridge_Share (null, subject, msg, new string[1]{email}, shareApp);
	}

	public static void SendEmail (string path, string subject, string msg, string email, string shareApp = "")
	{

		if (string.IsNullOrEmpty (shareApp))
			shareApp = ShareApp.Mail;

		Bridge_Share (new string[1]{path}, subject, msg, new string[1]{email}, shareApp);
	}
	public static void SendEmail (string[] paths, string subject, string msg, string[] emails, string shareApp = "")
	{

		if (string.IsNullOrEmpty (shareApp))
			shareApp = ShareApp.Mail;

		Bridge_Share (paths, subject, msg, emails, shareApp);
	}


	public static void SendEmail (Texture2D texture, string subject, string msg, string email, string shareApp = "")
	{
		if (string.IsNullOrEmpty (shareApp))
			shareApp = ShareApp.Mail;

		Bridge_ShareImageData (new Texture2D[1]{texture}, subject, msg, new string[1]{email}, shareApp);
	}
	public static void SendEmail (Texture2D[] textures, string subject, string msg, string[] emails, string shareApp = "")
	{
		if (string.IsNullOrEmpty (shareApp))
			shareApp = ShareApp.Mail;

		Bridge_ShareImageData (textures, subject, msg, emails, shareApp);
	}





}


public  class ShareApp
{

	#if UNITY_IOS
	public static string Facebook { get { return "com.apple.share.Facebook.post"; } }
	public static string Messenger {get{ return "com.facebook.Messenger.ShareExtension"; }} 
	public static string Twitter { get { return "com.apple.share.Twitter.post"; } }
	public static string Line { get { return "jp.naver.line.Share"; } }
	public static string Instagram { get { return "com.burbn.instagram.shareextension"; } }


	public static string Weibo { get { return "com.apple.UIKit.activity.PostToWeibo"; } }
	public static string Gmail { get { return "com.google.Gmail.ShareExtension"; } }


	public static string Whatsapp { get { return "net.whatsapp.WhatsApp.ShareExtension"; } }
	public static string Mail { get { return "com.apple.mobilemail"; } }

	#elif UNITY_ANDROID
	public static string Facebook {get{ return "com.facebook.katana"; }} 
	public static string Messenger {get{ return "com.facebook.orca"; }} 
	public static string Twitter {get{ return "com.twitter.android"; }}
	public static string Line {get{ return "jp.naver.line.android"; }}
	public static string Instagram {get{ return "com.instagram.android"; }}
	public static string Weibo { get { return "com.weico.international"; } }

	public static string Gmail {get{ return "com.google.android.gm"; }}
	public static string Mail {get{ return "com.google.android.gm"; }}

	public static string Whatsapp { get { return "com.whatsapp"; } }


	#else

	public static string Facebook {get{ return ""; }} 
	public static string Messenger {get{ return ""; }} 
	public static string Twitter {get{ return ""; }}
	public static string Line {get{ return ""; }}
	public static string Instagram {get{ return ""; }}
	public static string Weibo { get { return ""; } }
	public static string Gmail {get{ return ""; }}
	public static string Mail {get{ return ""; }}
	public static string Whatsapp { get { return ""; } }


	#endif


}
