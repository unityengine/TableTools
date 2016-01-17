using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
//using UnityEditor;
using UnityEngine.UI;

public class Test : MonoBehaviour {

	string s;
	string b = "";

	public GameObject successfulWindow;
	public GameObject failureWindow;

	public InputField inputPath;

	public InputField OutputPath;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClientBtClick()
	{
		TabelEditor.tablePath = inputPath.text;
		TabelEditor.targetPath = OutputPath.text;
		TabelEditor.AddSelectAllTabelCode();
//		s = Application.dataPath;
		//			string[] sQlitArrStr = s.Split('/');
		//			sQlitArrStr[sQlitArrStr.Length - 1] = null;
		//			sQlitArrStr[sQlitArrStr.Length - 2 ] = null;
		//			s = string.Join("/",sQlitArrStr);
//		s = OutputPath.text;
//		Directory.CreateDirectory(s+"/生成内容");
	}

	public void OnServerBtClick()
	{

		TableEditorToServer.tablePath = inputPath.text;
		TableEditorToServer.targetPath = OutputPath.text;
		TableEditorToServer.AddSelectAllServerTabelCode();
//		s = Application.dataPath;
		//			string[] sQlitArrStr = s.Split('/');
		//			sQlitArrStr[sQlitArrStr.Length - 1] = null;
		//			sQlitArrStr[sQlitArrStr.Length - 2 ] = null;
		//			s = string.Join("/",sQlitArrStr);
//		s = OutputPath.text;
//		Directory.CreateDirectory(s+"/生成内容");
	}

	public void OnSuccessfulBtClick()
	{
		successfulWindow.SetActive (false);
	}

	public void OnfailureBtClick()
	{
		failureWindow.SetActive (false);
	}
}
