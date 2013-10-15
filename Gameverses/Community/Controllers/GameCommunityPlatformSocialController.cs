using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;


public class GameCommunityPlatformSocialController : MonoBehaviour {
	
	public static GameCommunityPlatformSocialController Instance;
	
	public float currentTimeBlock = 0.0f;
	public float actionInterval = 1.0f;
	
	public Material photoMaterial;

	string fileName;
	string filePath;

    public void Awake() {
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            //Destroy(gameObject);
            return;
        }
		
        Instance = this;
		
		Init();
	}
	
	void OnEnable() {
		Messenger<GameCommunityMessageResult>.AddListener(
			GameCommunityPlatformMessages.gameCommunityResultMessage, OnGameCommunityResultMessage);
	}
	
	void OnDisable() {
		Messenger<GameCommunityMessageResult>.RemoveListener(
			GameCommunityPlatformMessages.gameCommunityResultMessage, OnGameCommunityResultMessage);		
	}
	
	void OnGameCommunityResultMessage(GameCommunityMessageResult result) {
		
		// show message from title, message in alert
		GameCommunityUIPanelLoading.ShowGameCommunityLoading(
			result.title, 
			result.message
		);		
	}
	
	void Init() {
		
	}	
	
	void Start () {	
		initSocial();
	}
	
	public void initSocial() {		
		//Invoke("initFacebook", 2);	
		//Invoke("initTwitter", 4);
	}
	
	public static void LikeUrl(string url) {
		if(Instance != null) {
			Instance.likeUrl(url);
		}
	}
	
	public void likeUrl(string url) {	
		
		// like facebook
		SocialNetworks.LikeUrlFacebook(url);
				
		// like custom
		
		// like other
	}
	
	public static void TakePhoto() {
		if(Instance != null) {
			Instance.takePhoto();
		}
	}	

	public void takePhoto() {
		StartCoroutine(takePhotoCo());
	}
	
	/*
	public void FindSocialNetworkFacebookObject() {		
		if(socialNetworkFacebookObject == null) {
			socialNetworkFacebookObject = GameObject.Find("SocialNetworkFacebook");
			if(socialNetworkFacebookObject == null) {
				socialNetworkFacebookObject = new GameObject("SocialNetworkFacebook");
				DontDestroyOnLoad(socialNetworkFacebookObject);
			}
		}
	}
	
	public void FindSocialNetworkTwitterObject() {		
		if(socialNetworkTwitterObject == null) {
			socialNetworkTwitterObject = GameObject.Find("SocialNetworkTwitter");
			if(socialNetworkTwitterObject == null) {
				socialNetworkTwitterObject = new GameObject("SocialNetworkTwitter");
				DontDestroyOnLoad(socialNetworkTwitterObject);
			}
		}
	}
	
	public void FindFacebookManagerObject() {		
		FindSocialNetworkFacebookObject();		
		if(facebookManager == null) {
			facebookManager = socialNetworkFacebookObject.GetComponent<FacebookManager>();
			facebookEventListener = socialNetworkFacebookObject.GetComponent<FacebookEventListener>();
			
			if(facebookManager == null || facebookEventListener == null) {
				
				//Debug.Log("initFacebook:adding:" + facebookManager);
				
				facebookManager = socialNetworkFacebookObject.AddComponent<FacebookManager>();
				facebookEventListener = socialNetworkFacebookObject.AddComponent<FacebookEventListener>();
			}
		}
	}
	
	public void FindTwitterManagerObject() {
		
		FindSocialNetworkTwitterObject();
		
		if(twitterManager == null) {
			
			twitterManager = socialNetworkTwitterObject.GetComponent<TwitterManager>();
			
			#if UNITY_IPHONE
			twitterEventListener = socialNetworkTwitterObject.GetComponent<TwitterEventListener>();
			#elif UNITY_ANDROID
			twitterEventListener = socialNetworkTwitterObject.GetComponent<TwitterAndroidEventListener>();
			#endif
			
			if(twitterManager == null) {
				
				//Debug.Log("initTwitter:adding:" + twitterManager);
				
				twitterManager = socialNetworkTwitterObject.AddComponent<TwitterManager>();
				#if UNITY_IPHONE
				twitterEventListener = socialNetworkTwitterObject.AddComponent<TwitterEventListener>();
				#elif UNITY_ANDROID
				twitterEventListener = socialNetworkTwitterObject.AddComponent<TwitterAndroidEventListener>();
				#endif
			}
		}
	}
	
	
	public void initFacebook() {
		
		//Debug.Log("initFacebook:" + facebookManager);
	
		FindFacebookManagerObject();
						
	#if UNITY_IPHONE
		FacebookBinding.init(FACEBOOK_APP_ID);
	#elif UNITY_ANDROID
		FacebookAndroid.init(FACEBOOK_APP_ID);
	#endif
		
		initFacebookEvents();
	}
	public void initFacebookEvents() {
		// [CH] Uncomment this to test initial user workflow.
		//FacebookBinding.logout();
		if(photoMaterial != null) {
			photoMaterial = new Material(photoMaterial); // [CH] Copy the material so we don't destroy the original material.
		}
		FacebookManager.loginSucceededEvent += () =>
		{
			//Debug.Log("[CH] Login succeeded! uploading photo....");
			uploadPhotoToFacebook();
		};
	}
	
	public void initTwitter() {
		
		FindTwitterManagerObject();
				
	#if UNITY_IPHONE
		TwitterBinding.init(TWITTER_KEY, TWITTER_SECRET);
	#elif UNITY_ANDROID
		TwitterAndroid.init(TWITTER_KEY, TWITTER_SECRET);
	#endif
		
		initTwitterEvents();
	}
	
	public void initTwitterEvents() {

		TwitterManager.twitterPost += () =>
		{
			//Debug.Log("[CH] Post uploaded!");
			GameCommunityPlatformController.SendResultMessage(
				twitterPostSuccessTitle, twitterPostSuccessMessage);
		};

		TwitterManager.twitterPostFailed += errorMessage =>
		{
			//Debug.Log("[CH] Post error!");
			// [CH] Display the ingame gui. At this point in time no gui is displayed.
			GUIInGame.SetActive(true);
			alwaysOnController.refreshInterface();
		};
	}
*/
	IEnumerator takePhotoCo() {
		yield return new WaitForSeconds(0.5f);
		
		fileName = "screenshot.png";
		filePath = Application.persistentDataPath+"/" +fileName;
		
		File.Delete(filePath);

#if UNITY_EDITOR
		Application.CaptureScreenshot(filePath);
#else
		/**
		 * For iOS you have to pass in just the name, while it seems like
		 * an absolute path is necessary/ or it puts it in a location that is
		 * different than persistentDataPath.
		**/
		Application.CaptureScreenshot(fileName);
#endif

		/**
		 * We wait for a half second so that the photo can be taken, otherwise
		 * we'll get the 'saving screenshot' screen on it. Hopefully this is long
		 * enough for all devices. I think we can assume so. In the editor this
		 * means that you won't see the 'saving screenshot' screen.
		**/
		yield return new WaitForSeconds(0.5f);
		displayPendingSaveAnimation();

		while(File.Exists(filePath) == false){
			yield return null;
		}

		//Debug.Log("screen capture complete!");

		var tex = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
		WWW www = new WWW("file://"+filePath);
		//Debug.Log("[CH] looking for:"+www.url);
		yield return www;

		if(www.error != null){
			//Debug.Log("Cannot load file"+www.error+" Path:"+www.url);
		}
		www.LoadImageIntoTexture(tex);
		
		if(photoMaterial != null) {
			photoMaterial.mainTexture = tex;
		
			//AppViewerUIController.Instance.HidePhotoCompleteActionUI();
			//AppViewerUIController.Instance.ShowPhotoShareActionUI();
		
			updatePhotoPreview();
		}
		else {
			Debug.Log("Set photoMaterial property with read/write material/texture before taking photo");
		}
	}

	public void updatePhotoPreview() {
		// load this into the preview image texture.
		GameObject photoPreview = GameObject.Find("PhotoPreview");
		if(photoPreview != null) {
			UITexture uiTexture = photoPreview.GetComponent<UITexture>();
			if(uiTexture != null) {
				uiTexture.material = photoMaterial;
			}
		}
	}

	public void startFacebookPhotoUploadProcess() {
		if(!GameCommunity.IsLoggedIn()) {
			GameCommunity.Login();
		}
		else {		
			//Debug.Log("We have a valid session.");
			uploadCurrentPhotoToFacebook();
		}
	}

	public void startTwitterPhotoUploadProcess() {
		if(SocialNetworks.IsTwitterAvailable()) {
#if UNITY_EDITOR
			displayPendingUploadAnimation();
#endif
			uploadCurrentPhotoToTwitter();
		}
		else {
			
			GameCommunityPlatformController.SendResultMessage(
				GameCommunityConfig.stringTwitterDisabledTitle , 
				GameCommunityConfig.stringTwitterDisabledMessage);
		}
	}
	
	void uploadCurrentPhotoToFacebook() {
		uploadPhotoToFacebook(photoMaterial);
	}
	
	void uploadPhotoToFacebook(Material photoMaterialPlaceholder) {
		
		Texture2D tex = (Texture2D)photoMaterialPlaceholder.mainTexture;
		uploadPhotoToFacebook(tex);
	}

	void uploadPhotoToFacebook(Texture2D tex) {

		// normally, we would just encode the Texture to a PNG but Facebook does not like Unity created PNG's since 3.4.0 came out
		//var bytes = tex.EncodeToPNG();		
		//var tex = new Texture2D( Screen.width, Screen.height, TextureFormat.RGB24, false );
		//tex.ReadPixels( new Rect( 0, 0, Screen.width, Screen.height ), 0, 0, false );
		//var bytes = tex.EncodeToPNG();
		//Destroy( tex );
		
		// We will instead make a JPG and upload that
		var encoder = new ImageJPGEncoder( tex, 75.0f );
		encoder.doEncoding();
		var bytes = encoder.GetBytes();		
		
		displayPendingUploadAnimation();
		
		Facebook.instance.postImage( 
			bytes, GameCommunityConfig.stringFacebookPostMessage, onFacebookUploadComplete );		
	}

	void onFacebookUploadComplete( string error, object result ) {		

		if(error != null) {
			//Debug.LogError( error );
						
			GameCommunityPlatformController.SendResultMessage(
				GameCommunityConfig.stringFacebookUploadErrorTitle, 
				GameCommunityConfig.stringFacebookUploadErrorMessage + error);
		}
		else {			
			GameCommunityPlatformController.SendResultMessage(
				GameCommunityConfig.stringFacebookUploadSuccessTitle , 
				GameCommunityConfig.stringFacebookUploadSuccessMessage);
		}
	}
	
	void uploadCurrentPhotoToTwitter() {
		uploadPhotoToTwitter(filePath);
	}
	
	void uploadPhotoToTwitter(string filePathToUpload) {
		SocialNetworks.ShowComposerTwitter(
			GameCommunityConfig.stringTwitterPostMessage, filePathToUpload );
	}

	void displayPendingSaveAnimation() {
		GameCommunityPlatformController.SendResultMessage(
			"SAVING", GameCommunityConfig.stringPendingCreatingScreenshot );
	}

	void displayPendingUploadAnimation() {
		GameCommunityPlatformController.SendResultMessage(
			"UPLOADING", GameCommunityConfig.stringPendingUploadingPost );

#if UNITY_EDITOR
		StartCoroutine(waitAndDisplayAlert());
#endif

	}

	IEnumerator waitAndDisplayAlert() {
		yield return new WaitForSeconds(2.0f);
		GameCommunityPlatformController.SendResultMessage(
			"UPLOAD", "Facebook/Twitter upload doesn't work on desktop currently." );
	}
	
	public static void SaveImageToLibraryDefault() {
		if(Instance != null) {
			Instance.saveImageToLibraryDefault();	
		}
	}
	
	public void saveImageToLibraryDefault() {
		string name = GameCommunityConfig.stringSavedPhotoTitleDefault;	
		string filePath = Path.Combine(Application.persistentDataPath, "screenshot.png");
		saveImageToLibrary(name, filePath);
	}
	
	public static void SaveImageToLibrary(string name, string fileToSave) {
		if(Instance != null) {
			Instance.saveImageToLibrary(name, fileToSave);	
		}
	}

	public void saveImageToLibrary(string name, string fileToSave) {
#if UNITY_IPHONE
		EtceteraBinding.saveImageToPhotoAlbum(fileToSave);
#elif UNITY_ANDROID
		EtceteraAndroid.saveImageToGallery(fileToSave, name);
#endif	
		
		GameCommunityPlatformController.SendResultMessage(
			GameCommunityConfig.stringLibraryPhotoSavedTitle, 
			GameCommunityConfig.stringLibraryPhotoSavedMessage );
	}
}