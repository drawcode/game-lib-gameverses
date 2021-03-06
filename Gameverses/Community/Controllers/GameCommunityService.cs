using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;

public class SearchFilter {
    public int page = 1;
    public int pageSize = 25;
    public string sortBy = "";
    public string sortDirection = "";
    public string filter = "";
}

public class SearchFilterLeaderboard : SearchFilter {
    public DateTime dateStart;
    public DateTime dateEnd;
    public string url = "";
    public string statCode = "";
    public string rangeType = "all";
    public string username = "";
}

public class GameCommunityService {
    
    public static string apiUrl = AppConfigs.apiUrlWeb;
    public static string apiPathGames = "games";
    public static string apiPathGame = "game";
    public static string apiPathGameLeaderboards = "leaderboards";
    public static string apiPathGameLeaderboard = "leaderboard";
    public static string apiPathGameSync = "sync";
    private static volatile GameCommunityService instance;
    private static System.Object syncRoot = new System.Object();
        
    public static GameCommunityService Instance {
        get {
            if (instance == null) { 
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new GameCommunityService();
                }
            }   
            return instance;
        }
    }
    
    public static string GetRootApiRoute() {
        return apiUrl;
    }
    
    public static string GetGameApiRoute(string action) {
        string apiPath = Path.Combine(apiUrl, apiPathGame);
        apiPath = Path.Combine(apiPath, AppConfigs.socialGameCommunityAppCode);
        apiPath = Path.Combine(apiPath, action);
        if (!apiPath.EndsWith("/")) {
            apiPath += "/";
        }
        return apiPath;
    }
    
    // ------------------------------------------------------------------------
    // STATISTIC LEADERBOARD
    
    
    public static void GetLeaderboardFull(
        string statCode, int page, int pageSize, string rangeType) {
        SearchFilterLeaderboard filter = new SearchFilterLeaderboard();
        filter.statCode = statCode;
        filter.page = page;
        filter.pageSize = pageSize;
        filter.rangeType = rangeType;
        GetLeaderboard(filter);
    }
    
    public static void GetLeaderboardUser(
        string statCode, int page, int pageSize, string rangeType, string username) {
        SearchFilterLeaderboard filter = new SearchFilterLeaderboard();
        filter.statCode = statCode;
        filter.page = page;
        filter.pageSize = pageSize;
        filter.rangeType = rangeType;
        filter.username = username;
        GetLeaderboardUser(filter);
    }
    
    public static void GetLeaderboardFull(string statCode) {
    
        SearchFilterLeaderboard filter = new SearchFilterLeaderboard();
        filter.statCode = statCode;
        GetLeaderboard(filter);
    }
    
    public static void GetLeaderboard(SearchFilterLeaderboard filter) {
        if (Instance != null) {
            Instance.getLeaderboard(filter);
        }
    }
    
    private void getLeaderboard(SearchFilterLeaderboard filter) {
        
        Dictionary<string, object> data = new Dictionary<string, object>();
                
        string profileId = GameProfiles.Current.uuid;
        string auth = AppConfigs.socialGameCommunityAppAuth;
        
        data.Add("profileId", profileId);
        data.Add("auth", auth);
        data.Add("page", filter.page);
        data.Add("page-size", filter.pageSize);
        data.Add("pageSize", filter.pageSize);
        data.Add("username", filter.username);
                
        string url = GetGameApiRoute(apiPathGameLeaderboard + "/" + filter.statCode);
        LogUtil.Log("getLeaderboard profileId:" + profileId);
        LogUtil.Log("getLeaderboard auth:" + auth);
                
        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_GET, 
            url, 
            data, 
            HandleGetLeaderboardCallback);
    }
    
    void HandleGetLeaderboardCallback(Engine.Networking.WebRequests.ResponseObject response) {
        string responseText = response.dataValueText;
        //LogUtil.Log("HandleGetLeaderboardCallback responseText:" + responseText);
        
        if (string.IsNullOrEmpty(responseText)) {
            return;
        }
        
        GameCommunityLeaderboardData leaderboardData = parseLeaderboard(responseText);
        
        if (leaderboardData != null) {
            Messenger<GameCommunityLeaderboardData>.Broadcast(
                GameCommunityMessages.gameCommunityLeaderboardData, leaderboardData);
        }
    }
    
    public static void GetLeaderboardUser(SearchFilterLeaderboard filter) {
        if (Instance != null) {
            Instance.getLeaderboardUser(filter);
        }
    }
    
    private void getLeaderboardUser(SearchFilterLeaderboard filter) {
        
        Dictionary<string, object> data = new Dictionary<string, object>();
                
        string profileId = GameProfiles.Current.uuid;
        string auth = AppConfigs.socialGameCommunityAppAuth;
        
        data.Add("profileId", profileId);
        data.Add("auth", auth);
        data.Add("page", filter.page);
        data.Add("page-size", filter.pageSize);
        data.Add("username", filter.username);
                
        string url = GetGameApiRoute(apiPathGameLeaderboard + "/" + filter.statCode);
        LogUtil.Log("getLeaderboard profileId:" + profileId);
        LogUtil.Log("getLeaderboard auth:" + auth);

        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_GET, 
            url, 
            data, 
            HandleGetLeaderboardUserCallback);
    }
    
    void HandleGetLeaderboardUserCallback(Engine.Networking.WebRequests.ResponseObject responseObject) {
        string responseText = responseObject.dataValueText;
        //LogUtil.Log("HandleGetLeaderboardCallback responseText:" + responseText);
        
        if (string.IsNullOrEmpty(responseText)) {
            return;
        }
        
        GameCommunityLeaderboardData leaderboardData = parseLeaderboard(responseText);
        
        if (leaderboardData != null) {
            Messenger<GameCommunityLeaderboardData>.Broadcast(
                GameCommunityMessages.gameCommunityLeaderboardUserData, leaderboardData);
        }
    }
    
    public static string testLeaderboardResult = "{\"data\":[{\"user\":{\"name\":\"Test\",\"id\":\"1351861467\"},\"score\":240,\"application\":{\"name\":\"Game Community\",\"namespace\":\"community\",\"id\":\"135612503258930\"}},{\"user\":{\"name\":\"Test Labs\",\"id\":\"1494687700\"},\"score\":23,\"application\":{\"name\":\"Game Community\",\"namespace\":\"community\",\"id\":\"135612503258930\"}}]}";
    
    public static void ParseTestLeaderboard(string responseText) {
        GameCommunityLeaderboardData leaderboardData = ParseLeaderboard(responseText);
        
        Messenger<GameCommunityLeaderboardData>.Broadcast(
            GameCommunityMessages.gameCommunityLeaderboardData, leaderboardData);
    }
    
    public static GameCommunityLeaderboardData ParseLeaderboard(string responseText) {
        if (Instance != null) {
            return Instance.parseLeaderboard(responseText);
        }
        return null;
    }
    
    public GameCommunityLeaderboardData parseLeaderboard(string responseText) {
        
        GameCommunityLeaderboardData leaderboardData = new GameCommunityLeaderboardData();
                
        
        if (string.IsNullOrEmpty(responseText)) {
            return leaderboardData;
        }
        
        List<GameCommunityLeaderboardItem> leaderboardItems = new List<GameCommunityLeaderboardItem>();

        string json = responseText.Replace("\\\"", "\"");
        
        Dictionary<string, object> jsonData = json.FromJson<Dictionary<string, object>>();

        if(jsonData != null && jsonData.Count > 0) {

            Dictionary<string, object> dataNode = jsonData.Get<Dictionary<string, object>>("data");

            if(dataNode != null && dataNode.Count > 0) {

                double totalRows = dataNode.Get<double>("total_rows");

                leaderboardData.totalCount = int.Parse(totalRows.ToString());

                List<Dictionary<string, object>> dataItems = dataNode.Get<List<Dictionary<string, object>>>("data");

                if(dataItems != null && dataItems.Count > 0) {

                    for(int i = 0; i < dataItems.Count; i++) {

                        Dictionary<string, object> data = dataItems[i];

                        GameCommunityLeaderboardItem leaderboardItem = new GameCommunityLeaderboardItem();

                        string username = data.Get<string>("username");
                        leaderboardItem.username = username;

                        string profile_id = data.Get<string>("profile_id");
                        leaderboardItem.userId = profile_id;

                        double score = data.Get<double>("absolute_value");
                        leaderboardItem.value = float.Parse(score.ToString());
                        leaderboardItem.valueFormatted = leaderboardItem.value.ToString("N0");

                        leaderboardItem.network = "facebook";
                        leaderboardItem.name = leaderboardItem.username;
                        leaderboardItem.type = "int";
                        leaderboardItem.urlImage = String.Format("http://graph.facebook.com/{0}/picture", leaderboardItem.username);
                        
                        leaderboardItems.Add(leaderboardItem);
                    }
                }
            }
        }
        
        //JsonData jsonData = responseText.Replace("\\\"", "\"").FromJson();
        
        //if (jsonData != null && jsonData.IsObject) {
        
        //    JsonData dataNode = jsonData["data"];
            
        //    if (dataNode != null && dataNode.IsObject) {
                
        //        JsonData totalRows = dataNode["total_rows"];
        //        if (totalRows != null) {                            
        //            if (totalRows.IsDouble || totalRows.IsInt || totalRows.IsLong) {
        //                leaderboardData.totalCount = int.Parse(totalRows.ToString());                       
        //            }
        //        }
                
        //        JsonData dataItems = dataNode["data"];                  
                
        //        if (dataItems != null && dataItems.IsArray) {
                    
        //            for (int i = 0; i < dataItems.Count; i++) {
                    
        //                var data = dataItems[i];                
                    
        //                GameCommunityLeaderboardItem leaderboardItem = new GameCommunityLeaderboardItem();
                                                
        //                JsonData username = data["username"];
        //                if (username != null) {                         
        //                    if (username.IsString) {
        //                        leaderboardItem.username = username.ToString();                     
        //                    }
        //                }
                        
        //                JsonData profile_id = data["profile_id"];
        //                if (profile_id != null) {                           
        //                    if (profile_id.IsString) {
        //                        leaderboardItem.userId = profile_id.ToString();                     
        //                    }
        //                }
                        
        //                JsonData score = data["absolute_value"];
        //                if (score != null) {                            
        //                    if (score.IsDouble) {
        //                        leaderboardItem.value = float.Parse(score.ToString());
        //                        leaderboardItem.valueFormatted = leaderboardItem.value.ToString("N0");                      
        //                    }
        //                }   
                        
        //                leaderboardItem.network = "facebook";
        //                leaderboardItem.name = leaderboardItem.username;
        //                leaderboardItem.type = "int";
        //                leaderboardItem.urlImage = String.Format("http://graph.facebook.com/{0}/picture", leaderboardItem.username);
                            
                    
        //                leaderboardItems.Add(leaderboardItem);
        //            }
        //        }
        //    }
        //}
        
        leaderboardData.leaderboards.Add("high-score", leaderboardItems);

        
        return leaderboardData;
    }
    
    
    // ------------------------------------------------------------------------
    // STATISTIC SEND
    
    public static void SyncData() {
    
        //SyncData(filter);
    }
    
    public static void SyncData(GameCommunitySyncData syncData) {
        if (Instance != null) {
            Instance.setSyncData(syncData);
        }
    }
    
    private void setSyncData(GameCommunitySyncData syncData) {
        
        Dictionary<string, object> data = new Dictionary<string, object>();
                
        string profileId = GameProfiles.Current.uuid;
        string auth = AppConfigs.socialGameCommunityAppAuth;
        
        data.Add("profileId", profileId);
        data.Add("auth", auth);
        
        data.Add("check", "gamecommunity01");
        data.Add("api_key", "gamecommunity01");

        string syncDataJson = syncData.ToJson();
        data.Add("data", syncDataJson);
                
        string url = GetGameApiRoute(apiPathGameSync);
        LogUtil.Log("setSyncData profileId:" + profileId);
        LogUtil.Log("setSyncData auth:" + auth);
        LogUtil.Log("setSyncData syncDataJson:" + syncDataJson);
        
        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_POST, 
            url, 
            data, 
            HandleSetSyncCallback);
    }
    
    void HandleSetSyncCallback(Engine.Networking.WebRequests.ResponseObject response) {
        string responseText = response.dataValueText;
        //LogUtil.Log("HandleSetSyncCallback responseText:" + responseText);
        
        if (string.IsNullOrEmpty(responseText)) {
            return;
        }
        else {
            LogUtil.Log(responseText.Replace("\\\"", "\""));
            
        }
        
        GameCommunitySyncDataResponse syncDataResponse = parseSyncData(responseText);
        
        Messenger<GameCommunitySyncDataResponse>.Broadcast(
            GameCommunityMessages.gameCommunitySyncComplete, syncDataResponse);

    }
    
    public static string testSyncDataResult = "{\"data\":[{\"user\":{\"name\":\"Test\",\"id\":\"1351861467\"},\"score\":240,\"application\":{\"name\":\"Game Community\",\"namespace\":\"community\",\"id\":\"135612503258930\"}},{\"user\":{\"name\":\"Draw Labs\",\"id\":\"1494687700\"},\"score\":23,\"application\":{\"name\":\"Game Community\",\"namespace\":\"community\",\"id\":\"135612503258930\"}}]}";
    
    public static void ParseTestSync(string responseText) {
        
        GameCommunitySyncDataResponse syncDataResponse = ParseSyncData(responseText);
        
        Messenger<GameCommunitySyncDataResponse>.Broadcast(
            GameCommunityMessages.gameCommunitySyncComplete, syncDataResponse);
    }
    
    public static GameCommunitySyncDataResponse ParseSyncData(string responseText) {
        if (Instance != null) {
            return Instance.parseSyncData(responseText);
        }
        return null;
    }
    
    public GameCommunitySyncDataResponse parseSyncData(string responseText) {
        
        GameCommunitySyncDataResponse syncDataResponse = new GameCommunitySyncDataResponse();
                
        
        if (string.IsNullOrEmpty(responseText)) {
            return syncDataResponse;
        }

        Dictionary<string, object> jsonData = responseText.FromJson<Dictionary<string, object>>();
        
        if (jsonData != null && jsonData.Count > 0) {

            Dictionary<string, object> dataNode = jsonData.Get<Dictionary<string, object>>("data");
            
            if (dataNode != null && dataNode.Count > 0) {

                string message = dataNode.Get<string>("message");
                syncDataResponse.message = message;
                
                string data = dataNode.Get<string>("data");
                syncDataResponse.data = data.Replace("\\\"", "\"");
            }
        }
        
        return syncDataResponse;
    }
    
    /* 
     * 
     * FACBEOOK
     * 
     * 
    public GameCommunityLeaderboardData parseLeaderboard(string responseText) {
        
        GameCommunityLeaderboardData leaderboardData = new GameCommunityLeaderboardData();
                
        
        if(string.IsNullOrEmpty(responseText)) {
            return leaderboardData;
        }
        
        List<GameCommunityLeaderboardItem> leaderboardItems = new List<GameCommunityLeaderboardItem>();
        
        JsonData jsonData = JsonMapper.ToObject(responseText.Replace("\\\"", "\""));
        
        if(jsonData != null) {
        
            JsonData dataNode = jsonData["data"];
            
            if(dataNode != null && dataNode.IsArray) {
            
                for(int i = 0; i < dataNode.Count; i++) {
                    
                    GameCommunityLeaderboardItem leaderboardItem = new GameCommunityLeaderboardItem();
                    
                    var data = dataNode[i];
                
                    JsonData user = data["user"];
                    JsonData application = data["application"];
                    JsonData score = data["score"];
                    
                    if(score != null) {
                        if(score.IsInt) {
                            leaderboardItem.value = float.Parse(score.ToString());
                            leaderboardItem.valueFormatted = leaderboardItem.value.ToString("N0");                      
                        }
                    }
                    
                    if(user != null) {
                        if(user.IsObject) {
                            
                            JsonData name = user["name"];
                            if(name != null) {
                                if(name.IsString) {
                                    string nameValue = name.ToString();
                                    if(!string.IsNullOrEmpty(nameValue)) {
                                        leaderboardItem.username = nameValue;
                                    }
                                }
                            }
                            
                            JsonData id = user["id"];
                            if(id != null) {
                                if(id.IsString) {
                                    string idValue = id.ToString();
                                    if(!string.IsNullOrEmpty(idValue)) {
                                        leaderboardItem.userId = idValue;
                                    }
                                }
                            }
                            
                            leaderboardItem.network = "facebook";
                            leaderboardItem.name = leaderboardItem.username;
                            leaderboardItem.type = "int";
                            leaderboardItem.urlImage = String.Format("http://graph.facebook.com/{0}/picture", leaderboardItem.username);;
                        }
                    }
                    
                    if(application != null) {
                        if(application.IsObject) {
                            JsonData name = application["name"];
                            if(name != null) {
                                if(name.IsString) {
                                    string nameValue = name.ToString();
                                    if(!string.IsNullOrEmpty(nameValue)) {
                                        leaderboardData.appName = nameValue;
                                    }
                                }
                            }
                            
                            JsonData namespaceNode = application["name"];
                            if(namespaceNode != null) {
                                if(namespaceNode.IsString) {
                                    string namespaceValue = namespaceNode.ToString();
                                    if(!string.IsNullOrEmpty(namespaceValue)) {
                                        leaderboardData.appNamespace = namespaceValue;
                                    }
                                }
                            }
                            
                            JsonData appId = application["id"];
                            if(appId != null) {
                                if(appId.IsString) {
                                    string appIdValue = appId.ToString();
                                    if(!string.IsNullOrEmpty(appIdValue)) {
                                        leaderboardData.appId = appIdValue;
                                    }
                                }
                            }
                        }
                    }
                    
                    leaderboardItems.Add(leaderboardItem);
                }
            }
        }
    
        
        leaderboardData.leaderboards.Add("high-score", leaderboardItems);

        
        return leaderboardData;
    }
    */
    
}

