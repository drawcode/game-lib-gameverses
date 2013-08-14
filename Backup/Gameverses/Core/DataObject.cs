using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Gameverses {

	public class DataObject {	
		
		public Dictionary<string, DataAttribute> attributes;	
		
		public DataObject() {
			
		}	
				
		public string GetDefaultPlayer() {
			return "Player";
		}
		
		public virtual void Clone(DataObject toCopy) {
			attributes = toCopy.attributes;
		}
		
		public string LoadDataFromResources(string resourcesFilePath) {
			string fileData = "";
			
			TextAsset textData = Resources.Load(resourcesFilePath, typeof(TextAsset)) as TextAsset;			
			if(textData != null) {
				fileData = textData.text;
			}
			
			return fileData;
		}	
			
		public string LoadDataFromPrefs(string key) {
			string data = "";
			
			if(!SystemPrefUtil.HasLocalSetting(key)) {
				data = SystemPrefUtil.GetLocalSettingString(key);
			}
			return data;
		}
		
		public string LoadData(string fileFullPath) {
			string fileData = "";
			
			if(File.Exists(fileFullPath)) {
	#if !UNITY_WEBPLAYER			
				fileData = FileSystemUtil.ReadString(fileFullPath);
	#endif
			}		
			
			return fileData;
		}
		
		public void SaveData(string fileFullPath, string data){
	#if !UNITY_WEBPLAYER	
			FileSystemUtil.WriteString(fileFullPath, data);
	#endif
		}
		
		public virtual void Reset() {
			attributes = new Dictionary<string, DataAttribute>();
		}
		
		
		// ATTRIBUTES
		
		public void SetAttribute(DataAttribute attribute) {
			// UPSERT		
			if(CheckIfAttributeExists(attribute.code)) {
				// UPDATE
				attributes[attribute.code] = attribute;
			}
			else {
				// INSERT
				attributes.Add(attribute.code, attribute);
			}
		}
		
		public bool CheckIfAttributeExists(string code) {
			if(attributes.ContainsKey(code)) {
				return true;
			}
			return false;
		}
		
		public DataAttribute GetAttribute(string code) {
			DataAttribute attribute = new DataAttribute();
			
			if(CheckIfAttributeExists(code)) {
				attribute = attributes[code];
			}
			
			return attribute;
		}
		
		
		// ATTRIBUTES
		
		public List<DataAttribute> GetAttributesList() {
			List<DataAttribute> attributesList = new List<DataAttribute>();
			foreach(DataAttribute attribute in attributes.Values) {
				attributesList.Add(attribute);
			}
			return attributesList;
		}
		
		public List<DataAttribute> GetAttributesList(string objectType) {
			List<DataAttribute> attributesFiltered = new List<DataAttribute>();
			foreach(DataAttribute attribute in attributes.Values) {
				if(attribute.otype == objectType) {
					attributesFiltered.Add(attribute);
				}
			}
			return attributesFiltered;
		}	
		
		public Dictionary<string, DataAttribute> GetAttributesDictionary() {
			return attributes;	
		}
		
		public Dictionary<string, DataAttribute> GetAttributesDictionary(string objectType) {
			Dictionary<string, DataAttribute> attributesFiltered = new Dictionary<string, DataAttribute>();
			foreach(KeyValuePair<string, DataAttribute> pair in attributes) {
				if(pair.Value.otype == objectType) {
					attributesFiltered.Add(pair.Value.code, pair.Value);
				}
			}
			return attributesFiltered;
		}	
		
		public void SetAttributeBoolValue(string code, bool val) {
			DataAttribute att = new DataAttribute();
			att.val = val;
			att.code = code;
			att.name = code;
			att.type = "bool";
			att.otype = "attribute";
			SetAttribute(att);
		}
		
		public bool GetAttributeBoolValue(string code) {
			bool currentValue = false;
			object objectValue = GetAttribute(code).val;
			if(objectValue != null) {
				currentValue = Convert.ToBoolean(objectValue);
			}
			
			return currentValue;
		}
		
		public void SetAttributeStringValue(string code, string val) {
			DataAttribute att = new DataAttribute();
			att.val = val;
			att.code = code;
			att.name = code;
			att.type = "string";
			att.otype = "attribute";
			SetAttribute(att);
		}
		
		public string GetAttributeStringValue(string code) {
			string currentValue = "";
			object objectValue = GetAttribute(code).val;
			if(objectValue != null) {
				currentValue = Convert.ToString(objectValue);
			}
			
			return currentValue;
		}
		
		public void SetAttributeDoubleValue(string code, double val) {
			DataAttribute att = new DataAttribute();
			att.val = val;
			att.code = code;
			att.name = code;
			att.type = "double";
			att.otype = "attribute";
			SetAttribute(att);
		}
		
		public double GetAttributeDoubleValue(string code) {
			double currentValue = 0.0;
			object objectValue = GetAttribute(code).val;
			if(objectValue != null) {
				currentValue = Convert.ToDouble(objectValue);
			}
			
			return currentValue;
		}
		
		public void SetAttributeIntValue(string code, int val) {
			DataAttribute att = new DataAttribute();
			att.val = val;
			att.code = code;
			att.name = code;
			att.type = "int";
			att.otype = "attribute";
			SetAttribute(att);
		}
		
		public int GetAttributeIntValue(string code) {
			int currentValue = 0;
			object objectValue = GetAttribute(code).val;
			if(objectValue != null) {
				currentValue = Convert.ToInt32(objectValue);
			}
			
			return currentValue;
		}
		
		public void SetAttributeObjectValue(string code, object val) {
			DataAttribute att = new DataAttribute();
			att.val = val;
			att.code = code;
			att.name = code;
			att.type = "int";
			att.otype = "attribute";
			SetAttribute(att);
		}
		
		public object GetAttributeObjectValue(string code) {
			object objectValue = GetAttribute(code).val;
			return objectValue;
		}
		
	}
}
