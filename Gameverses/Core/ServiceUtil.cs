using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameverses {

    public class ServiceUtil : MonoBehaviour {
        public bool processing = false;

        public delegate void HandleResponseTextCallback(string data);

        public delegate void HandleResponseObjectCallback(ResponseObject responseObject);

        // TODO other types...
        //public delegate void HandleResponseBytesCallback(byte[] bytes);
        //public delegate void HandleResponseWWWCallback(WWW www);
        //public delegate void HandleResponseObjectCallback(ResponseObject responseObject);
        //public delegate void HandleResponseAssetBundleCallback(AssetBundle assetBundle);
        //public delegate void HandleResponseAudioClipCallback(AudioClip assetBundle);
        //public delegate void HandleResponseTexture2DCallback(Texture2D texture2d);
        //public delegate void HandleResponseMovieTextureCallback(MovieTexture movieTexture);
        //public delegate void HandleResponseAudioClipOggVorbisCallback(AudioClip oggVorbisClip);

        //public HandleResponseTextCallback callbackText;
        //public HandleResponseBytesCallback callbackBytes;
        //public HandleResponseWWWCallback callbackWWW;
        //public HandleResponseAssetBundleCallback callbackAssetBundle;
        //public HandleResponseTexture2DCallback callbackTexture2D;
        //public HandleResponseAudioClipCallback callbackAudioClip;
        //public HandleResponseMovieTextureCallback callbackMovieTexture;
        //public HandleResponseAudioClipOggVorbisCallback callbackAudioClipOggVorbis;

        public enum RequestType {
            HTTP_POST,
            HTTP_GET,
            HTTP_PUT,
            HTTP_DELETE
        }

        public class ResponseObject {
            public bool validResponse;
            public string type;
            public WWW www;
            public int error;
            public string code;
            public string message;
            public string data;
            public JsonData dataValue;

            public ResponseObject() {
                validResponse = false;
                type = "";
                www = null;
                error = 0;
                code = "";
                message = "";
                data = "";
                dataValue = new JsonData();
            }
        }

        // -------------------------------------------------------------------
        // Singleton access

        private static ServiceUtil _instance = null;

        public static ServiceUtil instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType(typeof(ServiceUtil)) as ServiceUtil;
                    if (_instance == null)
                        LogUtil.Log("Could not locate an ServiceUtil object. You have to have exactly one ServiceUtil in the scene.");
                }

                return _instance;
            }
        }

        // -------------------------------------------------------------------
        // Initializers

        public void Init() {
            LogUtil.Log("ServiceUtil Init");
        }

        // -------------------------------------------------------------------
        // Unity auto called methods

        private void Awake() {
        }

        private void Start() {
            Init();
            LogUtil.Log("ServiceUtil Start");
        }

        private void Update() {
        }

        private void FixedUpdate() {
        }

        private void OnApplicationQuit() {
            _instance = null;
        }

        // -------------------------------------------------------------------
        // CUSTOM Methods

        public void ServiceResponseObjectCallback(ServiceUtil.ResponseObject responseObject) {
            try {
                if (!string.IsNullOrEmpty(responseObject.www.text))
                    LogUtil.Log("ServiceResponseTextCallback:" + responseObject.www.text);
            }
            catch (System.Exception e) {
                Debug.Log(e);
            }
        }

        //public void RequestText(string url, HandleResponseTextCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(url, callbackInternal);
        //    callbackText = callback;
        //}

        //public void RequestTextWithParams(RequestType requestType, string url, Dictionary<string, string> paramHash, HandleResponseWWWCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(requestType, url, paramHash, callbackInternal);
        //    callbackWWW = callback;
        //}

        //public void RequestTexture(string url, HandleResponseTexture2DCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(url, callbackInternal);
        //    callbackTexture2D = callback;
        //}

        //public void RequestBytes(string url, HandleResponseBytesCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(url, callbackInternal);
        //    callbackBytes = callback;
        //}

        //public void RequestAudioClip(string url, HandleResponseAudioClipCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(url, callbackInternal);
        //    callbackAudioClip = callback;
        //}

        //public void RequestAssetBundle(string url, HandleResponseAssetBundleCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(url, callbackInternal);
        //    callbackAssetBundle = callback;
        //}

        //public void RequestMovieTexture(string url, HandleResponseMovieTextureCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(url, callbackInternal);
        //    callbackMovieTexture = callback;
        //}

        //public void RequestAudioClipOggVorbis(string url, HandleResponseAudioClipOggVorbisCallback callback) {
        //    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
        //    Request(url, callbackInternal);
        //    callbackAudioClipOggVorbis = callback;
        //}

        // Core Request Helpers

        public string EnsureUrlUniqueByTime(string url) {
            if (url.IndexOf('?') > -1) {
                url = url + "&timestamp=" + System.DateTime.Now.Ticks.ToString();
            }
            else {
                url = url + "?timestamp=" + System.DateTime.Now.Ticks.ToString();
            }
            return url;
        }

        // DELEGATE BASED

        public void Request(string url, HandleResponseObjectCallback callback) {
            Request(RequestType.HTTP_GET, url, null, callback);
        }

        public void Request(RequestType requestType, string url, HandleResponseObjectCallback callback) {
            Request(requestType, url, null, callback);
        }

        public void Request(RequestType requestType, string url, Dictionary<string, string> paramHash, HandleResponseObjectCallback callback) {
            if (requestType == RequestType.HTTP_POST) {
                RequestPost(url, paramHash, callback);
            }
            else {
                RequestGet(url, paramHash, callback);
            }
        }

        public void RequestGet(string url, Dictionary<string, string> paramHash, HandleResponseObjectCallback callback) {
            if (paramHash != null) {
                if (paramHash.Count > 0) {

                    // append param types
                }
            }
            RequestStartCoroutineEnumerator(url, callback);
        }

        public void RequestPost(string url, Dictionary<string, string> paramHash, HandleResponseObjectCallback callback) {
            WWWForm form = new WWWForm();
            if (paramHash != null) {
                if (paramHash.Count > 0) {
                    foreach (KeyValuePair<string, string> pair in paramHash) {
                        form.AddField(pair.Key, pair.Value);
                    }
                }
            }
            RequestStartCoroutineEnumerator(url, callback, form);
        }

        public void RequestStartCoroutineEnumerator(string url, HandleResponseObjectCallback callback) {
            RequestStartCoroutineEnumerator(url, callback, null);
        }

        public void RequestStartCoroutineEnumerator(string url, HandleResponseObjectCallback callback, WWWForm form) {
            WWW www;

            if (form != null) {
                www = new WWW(url, form);
            }
            else {
                www = new WWW(url);
            }
            StartCoroutine(WaitForResponse(www, callback));
        }

        public IEnumerator WaitForResponse(WWW www, HandleResponseObjectCallback callback) {
            ResponseObject responseObject = new ResponseObject();
            processing = true;
            responseObject.validResponse = false;

            yield return www;

            try {
                responseObject.www = www;
                responseObject.type = "";

                if (www.error != null) {
                    LogUtil.Log("WWW Error:" + www.error);
                    responseObject.validResponse = false;
                }
                else {
                    responseObject.validResponse = true;
                }

                if (responseObject.www != null) {
                    if (!string.IsNullOrEmpty(responseObject.www.text)) {
                        if (responseObject.www.text.Length > 500) {
                            LogUtil.Log("WWW Output" + www.text.Substring(0, 499));
                        }
                    }
                }
            }
            catch (System.Exception e) {
                LogUtil.Log("Gameverses Error:" + e.Message + e.StackTrace);
                responseObject.validResponse = false;
            }
            finally {

                // Call callback after yeild
                callback(responseObject);

                //callback = null;
                processing = false;
            }
        }
    }
}