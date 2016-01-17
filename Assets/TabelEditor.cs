using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

public class TabelEditor : MonoBehaviour {

	public static string tablePath;
	public static string targetPath;

//	[MenuItem("Tools/Tabel Tools/对当前选中的表生成代码()")]
//	public static void AddSelectTabelCode()
//	{
//		var select = Selection.activeObject;
//		var _path = AssetDatabase.GetAssetPath(select);
//		FileInfo myp = new FileInfo(_path);
//		if(myp.Exists)
//		{
//			GenerateCode(_path);
//		}else
//		{
//			Debug.Log("this is dir, do retry~");
//		}
//
//		AssetDatabase.Refresh();
//	}

//	[MenuItem("Tools/Tabel Tools/选中客户端配表文件夹生成代码")]
	public static void AddSelectAllTabelCode()
	{
//		var select = Selection.activeObject;
//		var _path = AssetDatabase.GetAssetPath(select);
		string _path = tablePath;
		DirectoryInfo myp = new DirectoryInfo(_path);
		if(myp.Exists)
		{
			try{
				FileInfo[] allFile = myp.GetFiles();

				//tableigame

				string codeFileTableIgameTemplateString = GetTableIgameTemplate();
				string[]  codeFileTableIgameTemplateArr = codeFileTableIgameTemplateString.Split(new string[]{"{#}"}, StringSplitOptions.RemoveEmptyEntries);
				string allTableIgameCodeText = "";

				string cfitstr = codeFileTableIgameTemplateArr[0];
				allTableIgameCodeText += cfitstr;

				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFile[i].Name.Replace(".txt","");
	//					Debug.Log(name);
	//					Debug.Log(curPath);
						GenerateCode(curPath);
						allTableIgameCodeText += AddToTableIgame1(name);
					}
				}
				allTableIgameCodeText += codeFileTableIgameTemplateArr[1];
				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFile[i].Name.Replace(".txt","");
						allTableIgameCodeText += AddToTableIgame2(name);
					}
				}
				allTableIgameCodeText += codeFileTableIgameTemplateArr[2];
				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFile[i].Name.Replace(".txt","");
						allTableIgameCodeText += AddToTableIgame3(name);
					}
				}
				allTableIgameCodeText += codeFileTableIgameTemplateArr[3];

				string tableIgamePath = CreateTableCodeFile("igame");

				StreamWriter sw = new StreamWriter(tableIgamePath);
				sw.Write(allTableIgameCodeText);
				sw.Flush();
				sw.Close();
				GameObject successful = GameObject.Find("Main").GetComponent<Test>().successfulWindow;
				successful.SetActive(true);
			}catch
			{
				GameObject failure = GameObject.Find("Main").GetComponent<Test>().failureWindow;
				failure.SetActive(true);
			}

		}else
		{
			Debug.Log("this is file, do retry~");
		}
//		AssetDatabase.Refresh();
	}



	public static string CreateTableCodeFile(string name)
	{
//		string[] dirs = Directory.GetDirectories(Application.dataPath, "GameTables", SearchOption.AllDirectories);
//		foreach(string dir in dirs)
//		{
			string fileP = targetPath + "/Table_" + name + ".cs";


			FileInfo inf = new FileInfo(fileP);
			if(inf.Exists)
			{
//				Debug.Log("is have");
				File.Delete(fileP);

			}
			FileStream sf = File.Create(fileP);
			sf.Close();
			return fileP;
//		}
//		return null;
	}


	static string GetTableTemplate()
	{
		TextAsset ta = (TextAsset)Resources.Load("TableTemplate");
		return ta.text;//File.ReadAllText(Application.dataPath + "/Editor/TabelEditor/TableTemplate.txt");
	}

	static string GetTableIgameTemplate()
	{
		TextAsset ta = (TextAsset)Resources.Load("TableIgameTemplate");
		return ta.text;//File.ReadAllText(Application.dataPath + "/Editor/TabelEditor/TableIgameTemplate.txt");
	}

	/// <summary>
	/// Generates the code.
	/// </summary>
	/// <param name="_path">_path.表路径</param>
	static void GenerateCode(string _path)
	{
		//			Debug.Log("this is file");
		string[] allLineStr = File.ReadAllLines(_path);
		/*for(int i = 0; i < allLineStr.Length; i++)
			{
				if(allLineStr[i][0] == '#')
				{
					return;
				}
				if(i == 1)
				{
					allLineStr[i] = allLineStr[i].ToLower();
				}
				string[] splitStrArr = allLineStr[i].Split();
//				for(int j = 0; j < splitStrArr.Length; j++)
//				{
//					Debug.Log(splitStrArr[j]);
//				}
//				Debug.Log(allLineStr[i]);

			}*/
		allLineStr[1] = allLineStr[1].ToLower();
		string[] fieldStrArr = allLineStr[0].Split();
		string[] fieldTypeStrArr = allLineStr[1].Split();
		string tableName = Path.GetFileNameWithoutExtension(_path);
		//写入字符串代码到文件
		string codeFilePath = CreateTableCodeFile(tableName);
		if(codeFilePath == "")
		{
			return;
		}
		string codeFileTemplateString = GetTableTemplate();
		string[]  codeFileTemplateArr = codeFileTemplateString.Split(new string[]{"{#}"}, StringSplitOptions.RemoveEmptyEntries);
		string allCodeText = "";
		allCodeText += codeFileTemplateArr[0];
		allCodeText += tableName;
		allCodeText += codeFileTemplateArr[1];
		allCodeText += tableName;
		allCodeText += codeFileTemplateArr[2];
		
		for(int i = 0; i < fieldStrArr.Length; i++)
		{
			if(i == 1)
			{
				allCodeText += " \tID_"+fieldStrArr[i].ToUpper()+" = 1,\r\n";
			}else
			{
				allCodeText += " \tID_"+fieldStrArr[i].ToUpper()+",\r\n";
			}
		}
		allCodeText += codeFileTemplateArr[3];
		
		Dictionary<string, List<int>> arrDic = new Dictionary<string, List<int>>();

		int flagInt = 0;
		string field = "";
		for(int i = 0; i < fieldStrArr.Length;)
		{
			MatchCollection vMatchs = Regex.Matches(fieldStrArr[i], @"(\d+)");
			if(vMatchs.Count != 0)
			{
				
				string s = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i], @"[_0-9]+", "");
				if(arrDic.ContainsKey(s))
				{
					arrDic[s].Add(i);
				}else
				{
					List<int> loList = new List<int>();
					loList.Add(i);
					arrDic.Add(s, loList);
				}
				
				//					if(field != System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i], @"[_0-9]+", "") && field != "" && flagInt > 0)
				//					{
				//						string loField = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i-1], @"[_0-9]+", "");
				//						allCodeText += "\tpublic int get" + loField + "Count() { return " + flagInt +"; }\r\n";
				//						allCodeText += "\tprivate int[] m_" + loField + " = new int[" + flagInt + "];\r\n";
				//						allCodeText += "\tpublic int Get" + loField + @"byIndex(int idx)
				//						{
				//							if (idx >= 0 && idx < " + flagInt + ") return m_" + loField + @"[idx];
				//							return -1;
				//						}";
				//						allCodeText += "\r\n";
				//						flagInt = 1;
				//						field = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i], @"[_0-9]+", "");
				//					}else
				//					{
				//						field = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i], @"[_0-9]+", "");
				//						int vmInt = System.Convert.ToInt32(vMatchs[0].Value);
				//						flagInt++;
				//					}
				//					if(i == fieldStrArr.Length -1)
				//					{
				//						string loField = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i-1], @"[_0-9]+", "");
				//						
				//						allCodeText += "\tpublic int get" + loField + "Count() { return " + flagInt +"; }\r\n";
				//						allCodeText += "\tprivate int[] m_" + loField + " = new int[" + flagInt + "];\r\n";
				//						allCodeText += "\tpublic int Get" + loField + @"byIndex(int idx)
				//						{
				//							if (idx >= 0 && idx < " + flagInt + ") return m_" + loField + @"[idx];
				//							return -1;
				//						}";
				//						allCodeText += "\r\n";
				//						flagInt = 0;
				//					}
			}else
			{
				//					if(flagInt == 0)
				//					{
				allCodeText += " \tprivate " + fieldTypeStrArr[i] + " " + "m_" + fieldStrArr[i] + ";\r\n";
				allCodeText += " \tpublic "+fieldTypeStrArr[i]+" "+ fieldStrArr[i] + "{ get { return " + "m_" + fieldStrArr[i] + "; } }\r\n";
				//					}else
				//					{
				//						string loField = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i-1], @"[_0-9]+", "");
				//
				//						allCodeText += "\tpublic int get" + loField + "Count() { return " + flagInt +"; }\r\n";
				//						allCodeText += "\tprivate int[] m_" + loField + " = new int[" + flagInt + "];\r\n";
				//						allCodeText += "\tpublic int Get" + loField + @"byIndex(int idx)
				//						{
				//							if (idx >= 0 && idx < " + flagInt + ") return m_" + loField + @"[idx];
				//							return -1;
				//						}";
				//						allCodeText += "\r\n";
				//						flagInt = 0;
				//						field = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i], @"[_0-9]+", "");
				//						continue;
				//					}
			}
			i++;
		}
		
		
		foreach(string k in arrDic.Keys)
		{
			allCodeText += "\tpublic int get" + k + "Count() { return " + arrDic[k].Count +"; }\r\n";
			if(fieldTypeStrArr[arrDic[k][0]] == "string")
			{
				allCodeText += "\tprivate string[] m_" + k + " = new string[" + arrDic[k].Count + "];\r\n";
				allCodeText += "\tpublic string Get" + k + @"byIndex(int idx)
				{
					if (idx >= 0 && idx < " + arrDic[k].Count + ") return m_" + k + "[idx];return \"\";}";
			}else if(fieldTypeStrArr[arrDic[k][0]] == "bool")
			{
				allCodeText += "\tprivate bool[] m_" + k + " = new bool[" + arrDic[k].Count + "];\r\n";
				allCodeText += "\tpublic bool Get" + k + @"byIndex(int idx)
				{
					if (idx >= 0 && idx < " + arrDic[k].Count + ") return m_" + k + @"[idx];
					return false;
				}";
			}else if(fieldTypeStrArr[arrDic[k][0]] == "double")
			{
				allCodeText += "\tprivate double[] m_" + k + " = new double[" + arrDic[k].Count + "];\r\n";
				allCodeText += "\tpublic double Get" + k + @"byIndex(int idx)
				{
					if (idx >= 0 && idx < " + arrDic[k].Count + ") return m_" + k + @"[idx];
					return -1d;
				}";
			}else if(fieldTypeStrArr[arrDic[k][0]] == "float")
			{
				allCodeText += "\tprivate float[] m_" + k + " = new float[" + arrDic[k].Count + "];\r\n";
				allCodeText += "\tpublic float Get" + k + @"byIndex(int idx)
				{
					if (idx >= 0 && idx < " + arrDic[k].Count + ") return m_" + k + @"[idx];
					return -1f;
				}";
			}else if(fieldTypeStrArr[arrDic[k][0]] == "int")
			{
				allCodeText += "\tprivate int[] m_" + k + " = new int[" + arrDic[k].Count + "];\r\n";
				allCodeText += "\tpublic int Get" + k + @"byIndex(int idx)
				{
					if (idx >= 0 && idx < " + arrDic[k].Count + ") return m_" + k + @"[idx];
					return -1;
				}";
			}else if(fieldTypeStrArr[arrDic[k][0]] == "short")
			{
				allCodeText += "\tprivate short[] m_" + k + " = new short[" + arrDic[k].Count + "];\r\n";
				allCodeText += "\tpublic short Get" + k + @"byIndex(int idx)
				{
					if (idx >= 0 && idx < " + arrDic[k].Count + ") return m_" + k + @"[idx];
					return -1;
				}";
			}else if(fieldTypeStrArr[arrDic[k][0]] == "byte")
			{
				allCodeText += "\tprivate byte[] m_" + k + " = new byte[" + arrDic[k].Count + "];\r\n";
				allCodeText += "\tpublic byte Get" + k + @"byIndex(int idx)
				{
					if (idx >= 0 && idx < " + arrDic[k].Count + ") return m_" + k + @"[idx];
					return -1;
				}";
			}
			
			allCodeText += "\r\n";
		}
		
		allCodeText += codeFileTemplateArr[4];
		allCodeText += "Tab_"+tableName+" _values = new Tab_" + tableName + "();\r\n";
		for(int i = 0; i < fieldStrArr.Length; i++)
		{
			
			MatchCollection vMatchs = Regex.Matches(fieldStrArr[i], @"(\d+)");
			if(vMatchs.Count != 0)
			{
				int vmInt = System.Convert.ToInt32(vMatchs[0].Value);
				string loField = System.Text.RegularExpressions.Regex.Replace(fieldStrArr[i], @"[_0-9]+", "");
				if(fieldTypeStrArr[i] == "string")
				{
					allCodeText +=  "\t_values.m_" + loField + "[" + (vmInt-1).ToString() + "] = valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string; \r\n";
				}else if(fieldTypeStrArr[i] == "bool")
				{
					allCodeText +=  "\t_values.m_" + loField + "[" + (vmInt-1).ToString() + "] = Convert.ToInt16(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string) != 0; \r\n";
				}else if(fieldTypeStrArr[i] == "double")
				{
					allCodeText +=  "\t_values.m_" + loField + "[" + (vmInt-1).ToString() + "] = Convert.ToDouble(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "float")
				{
					allCodeText +=  "\t_values.m_" + loField + "[" + (vmInt-1).ToString() + "] = Convert.ToSingle(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "int")
				{
					allCodeText +=  "\t_values.m_" + loField + "[" + (vmInt-1).ToString() + "] = Convert.ToInt32(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "short")
				{
					allCodeText +=  "\t_values.m_" + loField + "[" + (vmInt-1).ToString() + "] = Convert.ToInt16(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "byte")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = Convert.ToByte(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string, 10); \r\n";
				}
			}else
			{
				if(fieldTypeStrArr[i] == "string")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string; \r\n";
				}else if(fieldTypeStrArr[i] == "bool")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = Convert.ToInt16(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string) != 0; \r\n";
				}else if(fieldTypeStrArr[i] == "double")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = Convert.ToDouble(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "float")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = Convert.ToSingle(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "int")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = Convert.ToInt32(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "short")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = Convert.ToInt16(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string); \r\n";
				}else if(fieldTypeStrArr[i] == "byte")
				{
					allCodeText += " \t_values.m_" + fieldStrArr[i] + " = Convert.ToByte(valuesList[(int)_ID.ID_" + fieldStrArr[i].ToUpper() + "] as string, 10); \r\n";
				}
			}

		}
		allCodeText += codeFileTemplateArr[5];
		//			Debug.Log(allCodeText);
		//			File.WriteAllText(codeFilePath, allCodeText);
		StreamWriter sw = new StreamWriter(codeFilePath);
		sw.Write(allCodeText);
		sw.Flush();
		sw.Close();
	}



	static string AddToTableIgame1(string tableName)
	{
		return @"private static Dictionary<int, List<Tab_" + tableName + ">> g_" + tableName + " = new Dictionary<int, List<Tab_" + tableName + @">>();
		public static bool InitTable_" + tableName + @"()
		{
			g_" + tableName + @".Clear();
			Dictionary<int, List<object>> tmps = new Dictionary<int, List<object>>();
			if (!Tab_" + tableName + @".LoadTable(tmps)) return false;
			foreach (KeyValuePair<int, List<object>> kv in tmps)
			{
				List<Tab_" + tableName + "> values = new List<Tab_" + tableName + @">();
				foreach (object subit in kv.Value)
				{
					values.Add((Tab_" + tableName + @")subit);
				}
				g_" + tableName + @".Add(kv.Key, values);
			}
			return true;
		}";
	}

	static string AddToTableIgame2(string tableName)
	{
		return "\t\t\tbRet &= InitTable_" + tableName + "();\r\n";
	}


	static string AddToTableIgame3(string tableName)
	{
		return @"public static List<Tab_" + tableName + @"> Get" + tableName + @"ByID(int nKey)
		{
			if (g_" + tableName + @".Count == 0)
			{
				InitTable_" + tableName + @"();
			}
			if (g_" + tableName + @".ContainsKey(nKey))
			{
				return g_" + tableName + @"[nKey];
			}
			return null;
		}
		public static Tab_" + tableName + @" Get" + tableName + @"ByID(int nKey, int nIndex)
		{
			if (g_" + tableName + @".Count == 0)
			{
				InitTable_" + tableName + @"();
			}
			if (g_" + tableName + @".ContainsKey(nKey))
			{
				if (nIndex >= 0 && nIndex < g_" + tableName + @"[nKey].Count)
					return g_" + tableName + @"[nKey][nIndex];
			}
			return null;
		}
		public static Dictionary<int, List<Tab_" + tableName + @">> Get" + tableName + @"()
		{
			if (g_" + tableName + @".Count == 0)
			{
				InitTable_" + tableName + @"();
			}
			return g_" + tableName + @";
		}";
	}

//	[MenuItem("Tools/Tabel Tools/wj")]
//	public static void AddSeelCode()
//	{
//		string name = "asdf";
//		string[] dirs = Directory.GetDirectories(Application.dataPath, "GameTables", SearchOption.AllDirectories);
//		foreach(string dir in dirs)
//		{
//			string fileP = dir + "/" + name + ".cs";
//			FileInfo inf = new FileInfo(fileP);
//			FileStream fs = null;
//			File.Delete(fileP);
//			fs = File.Create(fileP);
//
////			File.WriteAllText(fileP, "1a35f1ewaw13f51ew5awef53eaw15f1waf25aew");
//			fs.Close();
//			StreamWriter sw = new StreamWriter(fileP);  //创建写入流
//			sw.Write("你好");
//			sw.Flush();
//			sw.Close();
//			//			return dir + name;
//		}	
//		
//		AssetDatabase.Refresh();
//	}

}
