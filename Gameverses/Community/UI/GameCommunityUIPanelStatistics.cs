using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Networking;

public class GameCommunityUIPanelStatistics : MonoBehaviour {
	
	public GameObject listGridRoot;
    public GameObject listItemPrefab;
	
	public List<GameStatistic> currentStatistics = null;
			
	public static GameCommunityUIPanelStatistics Instance;
	
	void Awake () {		
		
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(gameObject);
            return;
        }
		
        Instance = this;
	}
	
	void OnEnable() {
		Messenger.AddListener(GameCommunityPlatformMessages.gameCommunityReady, OnGameCommunityReady);
	}
	
	void OnDisable() {
		Messenger.RemoveListener(GameCommunityPlatformMessages.gameCommunityReady, OnGameCommunityReady);
	}
	
	void OnGameCommunityReady() {
		Init();	
	}
	
	public void Start() {
		
	}
	
	public void Init() {
		currentStatistics = new List<GameStatistic>();
		LoadData();
	}
	
	public void LoadData() {
				
		currentStatistics = new List<GameStatistic>();
		
		StartCoroutine(LoadDataCo());
	}
	
	public List<GameStatistic> GetStatistics() {
		
		if(currentStatistics == null) {
			currentStatistics = new List<GameStatistic>();
		}
		else { 
			currentStatistics.Clear();
		}
		
		currentStatistics = GameStatistics.Instance.GetAll();
				
		return currentStatistics;
	}
	
	IEnumerator LoadDataCo() {
						
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.transform.parent.gameObject.GetComponent<UIDraggablePanel>().ResetPosition();
	        yield return new WaitForEndOfFrame();
		
			GetStatistics();
			
			int i = 0;
			
	        foreach(GameStatistic statistic in currentStatistics) {
								
	            GameObject item = NGUITools.AddChild(listGridRoot, listItemPrefab);
	            item.name = "AStatisticItem" + i;				
				
	            item.transform.FindChild("LabelName").GetComponent<UILabel>().text = statistic.display_name;
	            item.transform.FindChild("LabelDescription").GetComponent<UILabel>().text = statistic.description;
				
				double statValue = GameProfileStatistics.Current.GetStatisticValue(statistic.code);
				string displayValue = GameStatistics.Instance.GetStatisticDisplayValue(statistic, statValue);
				
				item.transform.FindChild("LabelValue").GetComponent<UILabel>().text = displayValue;				
								
				i++;
	        }
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();	
	        listGridRoot.transform.parent.gameObject.GetComponent<UIDraggablePanel>().ResetPosition();
	        yield return new WaitForEndOfFrame();	
			
        }
	}
	
}

