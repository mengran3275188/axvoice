﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

using com.axia;

public class MainCamera : MonoBehaviour {
	private uint currentVoiceID;
	private string recordButtonText = "BeginRecord";
	private bool recordStatus = false;
	private Rect windowRect;
	private string m_dataPath;
	
	private string m_serverURL = ""; 
	private string m_voiceID = "";
	private int m_lineHeight, m_windowWidth;
	
	// Use this for initialization
	void Start () {
	#if UNITY_IPHONE 
	#elif UNITY_ANDROID
		m_dataPath=Application.persistentDataPath;
	#else
		m_dataPath=Path.GetTempPath();
	#endif
			Debug.Log("datapath=" + m_dataPath);
  		CallAxVoice.Init(m_dataPath, "http://111.206.74.78/upload_voice");
	}
	
	// Update is called once per frame
	void Update () {
		CallAxVoice.DispatchMessage("Main Camera");
	}
	
	void OnGUI() 
	{
	#if UNITY_IPHONE || UNITY_ANDROID
		
		int fontSize = 40;
		GUI.skin.button.fontSize = fontSize;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.textField.fontSize = fontSize;

		m_lineHeight = 80;
	#else
		m_lineHeight = 30;
	#endif
			
		int screenWidth = Screen.width;
		int screenHeight = Screen.height;
		m_windowWidth = screenWidth-100;
		int windowHeight = 5*m_lineHeight;
		int windowX = (screenWidth-m_windowWidth)/2;
		int windowY = (screenHeight-windowHeight)/2;
		windowRect = new Rect(windowX, windowY, m_windowWidth, windowHeight);
		
		GUILayout.Window(0, windowRect, UserForm, "");
	}
	
	void UserForm(int id)
	{
		GUILayout.BeginVertical(); 
		
		GUILayout.BeginHorizontal();  
    m_serverURL = GUILayout.TextField(m_serverURL, GUILayout.MaxWidth(m_windowWidth-200), GUILayout.Height(m_lineHeight));  
    if(GUILayout.Button("GET", GUILayout.Width(200), GUILayout.Height(m_lineHeight)))
    {
    	Debug.Log("Read file=" + m_dataPath+"/url.txt");
    	StreamReader sr = new StreamReader(m_dataPath+"/url.txt");
    	m_serverURL = sr.ReadLine();
    }
    GUILayout.EndHorizontal();  
    
		GUILayout.BeginHorizontal();  
    if(GUILayout.Button("Create", GUILayout.Height(m_lineHeight)))
    {
    	currentVoiceID = CallAxVoice.CreatVoice(m_serverURL);
    	m_voiceID = ""+currentVoiceID;
			Debug.Log("Create Voice, voiceid=" + currentVoiceID);
    }
    GUILayout.EndHorizontal();  

		GUILayout.Space(m_lineHeight);
    
		GUILayout.BeginHorizontal();  
    m_voiceID = GUILayout.TextField(m_voiceID, GUILayout.Height(m_lineHeight));  
    if(GUILayout.Button("GET", GUILayout.Width(200), GUILayout.Height(m_lineHeight)))
    {
    	StreamReader sr = new StreamReader(m_dataPath+"/id.txt");
    	m_voiceID = sr.ReadLine();
    }
    GUILayout.EndHorizontal();  


		GUILayout.BeginHorizontal();  
    if(GUILayout.Button("Download", GUILayout.Width(m_windowWidth/3), GUILayout.Height(m_lineHeight)))
    {
    	CallAxVoice.DownloadVoice(System.UInt32.Parse(m_voiceID));
			Debug.Log("Begin Download Voice, voiceid=" + currentVoiceID);
    }
    if(GUILayout.Button("Play", GUILayout.Width(m_windowWidth/3), GUILayout.Height(m_lineHeight)))
    {
    	CallAxVoice.PlayVoice(System.UInt32.Parse(m_voiceID));
			Debug.Log("Begin Play Voice, voiceid=" + currentVoiceID);
    }
    if(GUILayout.Button("Stop", GUILayout.Width(m_windowWidth/3), GUILayout.Height(m_lineHeight)))
    {
    	CallAxVoice.StopVoice();
			Debug.Log("Stop Current Voice");
    }
    GUILayout.EndHorizontal();  

		GUILayout.Space(m_lineHeight);

		if(GUILayout.RepeatButton(recordButtonText, GUILayout.Height(m_lineHeight*2))) 
		{
			if(!recordStatus)
			{
				currentVoiceID = CallAxVoice.BeginRecord();
				Debug.Log("Begin Record, voiceid=" + currentVoiceID);
				recordButtonText = "Recording...";
			}
			recordStatus = true;
		}
		else if(Event.current.type == EventType.Repaint)
		{
			if(recordStatus)
			{
				CallAxVoice.CompleteRecord(currentVoiceID);
				recordButtonText = "BeginRecord";
				Debug.Log("Stop Record...");
			}
			recordStatus = false;
		}
		
		GUILayout.EndVertical();  
	}
	
	void _OnAxVoiceRecordMessage(string args)	{
		OnAxVoiceRecordMessage(args.Split('|'));
	}	
	void _OnAxVoiceUploadMessage(string args)	{
		OnAxVoiceUploadMessage(args.Split('|'));
	}	
	void _OnAxVoiceDownloadMessage(string args)	{
		OnAxVoiceDownloadMessage(args.Split('|'));
	}	
	void _OnAxVoicePlayMessage(string args)	{
		OnAxVoicePlayMessage(args.Split('|'));
	}	
	
	void OnAxVoiceRecordMessage(string[] args)
	{
		uint voiceID = System.UInt32.Parse(args[0]);
		string type = args[1];
		string success = args[2];
		string result = args[3];
		if(type == "complete")
		{
			Debug.Log("Record complete, voiceid=" + voiceID + ", success=" + success + ", result=" + result);
			//begin upload
			CallAxVoice.UploadVoice(currentVoiceID);
			
			m_voiceID = args[0];
		}
	}

	void OnAxVoiceUploadMessage(string[] args)
	{
		uint voiceID = System.UInt32.Parse(args[0]);
		string type = args[1];
		string success = args[2];
		string result = args[3];
		if(type == "complete")
		{
			Debug.Log("Upload complete, voiceid=" + voiceID + ", success=" + success + ", result=" + result);
		}
	}
	
	void OnAxVoiceDownloadMessage(string[] args)
	{
		uint voiceID = System.UInt32.Parse(args[0]);
		string type = args[1];
		string success = args[2];
		string result = args[3];
		if(type == "complete")
		{
			Debug.Log("Download complete, voiceid=" + voiceID + ", success=" + success + ", result=" + result);
		}
	}	

	void OnAxVoicePlayMessage(string[] args)
	{
		uint voiceID = System.UInt32.Parse(args[0]);
		string type = args[1];
		if(type == "complete")
		{
			Debug.Log("Play complete, voiceid=" + voiceID);
		}
	}
}