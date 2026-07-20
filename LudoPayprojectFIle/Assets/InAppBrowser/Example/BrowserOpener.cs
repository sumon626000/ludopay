using UnityEngine;
using System.Collections;

public class BrowserOpener : MonoBehaviour {

	public string pageToOpen = "http://www.google.com";

	// check readme file to find out how to change title, colors etc.
	public void OnButtonClicked(string url) {
		InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
		options.displayURLAsPageTitle = false;		

		InAppBrowser.OpenURL(url, options);
	}

	public void OnClearCacheClicked() {
		InAppBrowser.ClearCache();
	}
}
