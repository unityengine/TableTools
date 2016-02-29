using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

public class TableEditorToServer : MonoBehaviour {

	public static string tablePath;
	public static string targetPath;

	static List<FileInfo> allFileInfo = new List<FileInfo>();

//	[MenuItem("Tools/Tabel Tools/选中服务器配表文件夹生成代码")]
	public static void AddSelectAllServerTabelCode()
	{
		allFileInfo.Clear();
//		var select = Selection.activeObject;
//		var _path = AssetDatabase.GetAssetPath(select);
		string _path = tablePath;
		DirectoryInfo myp = new DirectoryInfo(_path);

		if(myp.Exists)
		{
			try{
				DirectoryInfo dirInfo = new DirectoryInfo(_path);
				PrintAllFile(dirInfo);
//				FileInfo[] allFile = myp.GetFiles();

				
				string codeFileTableManagerTemplateString = GetServerTableManagerTemplate();
				string[]  codeFileTableManagerTemplateArr = codeFileTableManagerTemplateString.Split(new string[]{"{#}"}, StringSplitOptions.RemoveEmptyEntries);
				string allTableManagerCodeText = "";
				
				string cfitstr = codeFileTableManagerTemplateArr[0];
				allTableManagerCodeText += cfitstr;


				string codeFileTableDefineInfoTemplateString = GetServerTableDefineInfoTemplate();
				string[]  codeFileTableDefineInfoTemplateArr = codeFileTableDefineInfoTemplateString.Split(new string[]{"{#}"}, StringSplitOptions.RemoveEmptyEntries);
				string allTableDefineInfoCodeText = "";
				
				string fbdiastr = codeFileTableDefineInfoTemplateArr[0];
				allTableDefineInfoCodeText += fbdiastr;
				
				for(int i = 0; i < allFileInfo.Count; i++)
				{
					string curPath = allFileInfo[i].FullName;
					string s = Path.GetExtension(curPath);
					int count = 0;
					if(s == ".txt")
					{
						string[] allLine = File.ReadAllLines(curPath);
						for(int m = 0; m < allLine.Length; m++)
						{
							int b = -1;
							if(int.TryParse(allLine[m].Split('\t')[0].ToLower(), out b))
							{
								if(b > -1)
									count++;
							}
						}
					}
					string sm = Path.GetExtension(curPath);
					if(sm == ".txt")
					{
						string name = allFileInfo[i].Name.Replace(".txt","");
						allTableManagerCodeText += CreateCodeToTableManager1(name, count);
					}
				}
				allTableManagerCodeText += codeFileTableManagerTemplateArr[1];
				for(int i = 0; i < allFileInfo.Count; i++)
				{
					string curPath = allFileInfo[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFileInfo[i].Name.Replace(".txt","");
						allTableManagerCodeText += CreateCodeToTableManager2(name);
					}
				}
				allTableManagerCodeText += codeFileTableManagerTemplateArr[2];

				for(int i = 0; i < allFileInfo.Count; i++)
				{
					string curPath = allFileInfo[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string[] allLine = File.ReadAllLines(curPath);
						string id = "";
						if(allLine[0].Split('\t')[0].ToLower() == "int")
						{
							id = allLine[1].Split('\t')[0].ToLower();
						}else
						{
							id = allLine[0].Split('\t')[0].ToLower();
						}
						string name = allFileInfo[i].Name.Replace(".txt","");
						allTableManagerCodeText += CreateCodeToTableManager3(name, id);
					}
				}
				allTableManagerCodeText += codeFileTableManagerTemplateArr[3];

				string tableIgamePath = CreateTableCodeFile("TableManager", ".h");
				
				StreamWriter sw = new StreamWriter(tableIgamePath, false, new System.Text.UTF8Encoding(true));
				sw.Write(allTableManagerCodeText);
				sw.Flush();
				sw.Close();

				for(int i = 0; i < allFileInfo.Count; i++)
				{
					string curPath = allFileInfo[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFileInfo[i].Name.Replace(".txt","");
						string addStr = CreateCodeToTableDefineInfo1(name, curPath);
						if(addStr != "")
						{
							allTableDefineInfoCodeText += addStr;
						}else
						{
							return;
						}

					}
				}
				allTableDefineInfoCodeText += codeFileTableDefineInfoTemplateArr[1];
				
				string tableDefineInfoPath = CreateTableCodeFile("TableDefineInfo", ".h");
				
				StreamWriter sw2 = new StreamWriter(tableDefineInfoPath, false, new System.Text.UTF8Encoding(true));
				sw2.Write(allTableDefineInfoCodeText);
				sw2.Flush();
				sw2.Close();

				//TableManager cpp
				string TableManagerCppPath = CreateTableCodeFile("TableManager", ".cpp");
				StreamWriter tmcpp = new StreamWriter(TableManagerCppPath, false, new System.Text.UTF8Encoding(true));
				tmcpp.Write("#include \"TableManager.h\"");
				tmcpp.Flush();
				tmcpp.Close();

				//TableDefineInfo cpp
				string TableDefineInfoCppPath = CreateTableCodeFile("TableDefineInfo", ".cpp");
				StreamWriter tdicpp = new StreamWriter(TableDefineInfoCppPath, false, new System.Text.UTF8Encoding(true));
				string tdiWriteStr = "#include \"TableDefineInfo.h\"\r\n";
				for(int i = 0; i < allFileInfo.Count; i++)
				{
					string curPath = allFileInfo[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string fullName = allFileInfo[i].FullName.Replace(".txt","");
						string nm =  allFileInfo[i].Name.Replace(".txt","");
						fullName = fullName.Replace(_path+@"/","");
						fullName = fullName.Replace(_path+@"\","");
						fullName = fullName.Replace(@"\","/");
						tdiWriteStr += "string "+ nm +"::file = \""+ fullName +".txt\";\r\n";
					}
				}
				tdicpp.Write(tdiWriteStr);
				tdicpp.Flush();
				tdicpp.Close();
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
			Test.instance.errorT.text = "error: this is file, do retry~";
			Test.instance.failureWindow.SetActive(true);
		}
//		AssetDatabase.Refresh();
	}

	public static void PrintAllFile(DirectoryInfo di)
	{
		FileInfo[] fiArray = di.GetFiles("*.txt");
		DirectoryInfo[] diArray = di.GetDirectories();
		foreach (FileInfo inst in fiArray)
		{
			allFileInfo.Add(inst);
		}
		foreach (DirectoryInfo inst in diArray)
		{
			PrintAllFile(inst);
		}
	}


	static string GetServerTableManagerTemplate()
	{
		TextAsset ta = (TextAsset)Resources.Load("TableManagerTemplateToServer");
		return ta.text;//File.ReadAllText(Application.dataPath + "/Editor/TabelEditor/TableManagerTemplateToServer.txt");
	}
	
	static string GetServerTableDefineInfoTemplate()
	{
		TextAsset ta = (TextAsset)Resources.Load("TableDefineInfoTemplateToServer");
		return ta.text;//File.ReadAllText(Application.dataPath + "/Editor/TabelEditor/TableDefineInfoTemplateToServer.txt");
	}

	public static string CreateTableCodeFile(string name, string suffix)
	{
//		string[] dirs = Directory.GetDirectories(Application.dataPath, "Server", SearchOption.AllDirectories);
//		foreach(string dir in dirs)
//		{
			string fileP = targetPath + "/" + name + suffix;
			
//			if(TabelEditor.IsFileInUse(fileP))
//			{
//				Debug.LogError("文件被其他程序占用，请重新生成");
//				return "";
//			}
			
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


	static string CreateCodeToTableManager1(string name, int count)
	{

		return @"private:
    "+ "#define "+ name +"ArrCount "+ count +@"
    "+ name +" g_"+ name + @"["+ name +@"ArrCount];
    bool Init"+ name +@"()
    {
        ifstream myFile;
		"+@"#if defined(__LINUX__)
		myFile.open(string(path + "+ name +@"::file).c_str(), ios_base::in);
		"+
		@"#elif defined(__WINDOWS__)
		myFile.open(path + "+ name +@"::file);
		#else
		myFile.open(path + "+ name +@"::file);
		" + @"#endif
		int i=1;
        if (myFile.is_open())
        {
           string temp;
		   int j = 0;
           while(getline(myFile,temp))
           {
               if(i > 2 && temp.substr(0,1) != ""#"")
               {
					"+ name +@" loLine;
                    "+ name +@"::SetLineInfo(loLine, temp);
                    g_"+ name +@"[j] = loLine;
					j++;
               }
               i++;
           }
        }
        myFile.close();
        return true;
    };" + "\r\n";
	}

	static string CreateCodeToTableManager2(string name)
	{
		return @"bl &= Init"+ name +"();\r\n";
	}

	static string CreateCodeToTableManager3(string name, string id)
	{
		return name + @"* Get"+ name +@"ById(int id)
    {
        int count = "+ name +@"ArrCount;
        int low = 0;  
        int high = count - 1;  
        int mid = 0;  
		bool bFind = false;
        while ( low <= high )  
        {  
            mid = (low + high )/2;  
            if (g_"+ name +@"[mid].Get"+ id +@"() < id)  
                low = mid + 1;  
            else if (g_"+ name +@"[mid].Get"+ id +@"() > id )     
                high = mid - 1;  
            else  
            {
				bFind = true;
			    break;  
			}
        }
		if(!bFind)
			return NULL;
        return &g_"+ name +@"[mid];
    }
    "+ name + @"* Get"+ name +@"ByIndex(int index)
    {
		int count = "+ name +@"ArrCount;
		if(index < 0 || index >= count)
			return NULL;
        return &g_"+ name +@"[index];
    }
    int Get"+ name +@"Count()
    {
        return " + name + @"ArrCount;
    }" + "\r\n";
	}

	static string CreateCodeToTableDefineInfo1(string name, string _path)
	{
		string returnString;
		returnString = @"class "+ name +@"
{
private:
    ";

		string[] allLineStr = File.ReadAllLines(_path);

//		allLineStr[1] = allLineStr[1].ToLower();
		string[] fieldStrArrLine1 = allLineStr[0].ToLower().Split('\t');
		string[] fieldStrArrLine2 = allLineStr[1].ToLower().Split('\t');

		string[] typeArr = null;
		string[] fieldArr = null;
		if(fieldStrArrLine2[0] == "int" || fieldStrArrLine1[0] == "int")
		{
			if(fieldStrArrLine2[0] == "int")
			{
				typeArr = fieldStrArrLine2;
				fieldArr = allLineStr[0].Split('\t');
			}else
			{
				typeArr = fieldStrArrLine1;
				fieldArr = allLineStr[1].Split('\t');
			}
		}else
		{
			Debug.LogError("请检查字段信息，index必须为int类型");
			Test.instance.errorT.text = "error:" + name + "请检查字段信息，index必须为int类型 path is :" + name;
			Test.instance.failureWindow.SetActive(true);
			return "";
		}

		//表数据合法性检测

		List<string[]> infoList = new List<string[]>();
		for(int infoi = 0; infoi < allLineStr.Length; infoi++)
		{
			string[] hInfo = allLineStr[infoi].Split('\t');
			infoList.Add(hInfo);
		}
		if(!CheckTableInfo(typeArr, infoList))
		{
//			Test.instance.errorT.text = "error:" + name + "表数据合法性检测失败，有部分表数据存在异常,table is:" + name;
//			Test.instance.failureWindow.SetActive(true);
			return "";
		}


		//类型检测
		for(int tyi = 0; tyi < typeArr.Length; tyi++)
		{
			if(typeArr[tyi] != "string" && typeArr[tyi] != "int" && typeArr[tyi] != "float" && typeArr[tyi] != "double" && typeArr[tyi] != "short" && typeArr[tyi] != "bool")
			{
				Debug.LogError(name + "配表包含未知类型 path is :" + _path);
				Test.instance.errorT.text = "error:" + name + "配表包含未知类型 path is :" + _path;
				Test.instance.failureWindow.SetActive(true);
				return "";
			}
		}


		//被检测过后的正确数据类型字段信息
		//根据当前的字段信息获取所有的字段，包括数组检测
		Dictionary<string, List<int>> AllFieldInfo = CheckFieldInformation(typeArr, fieldArr);
		if(AllFieldInfo == null)
		{
			Debug.LogError("请检查字段信息");
			Test.instance.errorT.text = "error:" + name + "请检查字段信息 path is :" + _path;
			Test.instance.failureWindow.SetActive(true);
			return "";
		}
//		for(int i = 0; i < typeArr.Length; i++)
//		{
//			returnString += "\t" + typeArr[i] + " " + fieldArr[i] + ";\r\n";
//		}

		foreach(string strChild in AllFieldInfo.Keys)
		{
			string typeStr = CheckTypeInfomation(strChild, typeArr, fieldArr);
			//判定是否为数组
			if(AllFieldInfo[strChild] != null)
			{
				//获取数组长度
				//这个是根据当前策划所填写的数字决定的，以策划最大的数字＋1为数组的长度，＋1是因为数组从0下标开始
				int arrMaxNum = ComputeListIntMaxNum(AllFieldInfo[strChild]) + 1;
				returnString += "\t" + typeStr + " " + strChild + "[" + arrMaxNum.ToString() + "];\r\n";
			}else
			{
				returnString += "\t" + typeStr + " " + strChild + ";\r\n";
			}
		}
		returnString += @"
enum " +@"
{";
		for(int i = 0; i < typeArr.Length; i++)
		{
			returnString += "\t"+ name +"_"+ fieldArr[i].ToUpper() + " = "+ i +",\r\n";
		}
//		int enumJ = 0;
//		foreach(string strChild in AllFieldInfo.Keys)
//		{
//			returnString += "\t"+ name +"_"+ strChild.ToUpper() + " = "+ enumJ +",\r\n";
//			enumJ++;
//		}
		returnString += @"};

public:
    "+ name +@"(){
    }
    ~"+ name +@"(){}
   
    
    static string file;
    ";
//		for(int i = 0; i < typeArr.Length; i++)
//		{
//			returnString += "\t" + typeArr[i] + " Get" + fieldArr[i].ToLower() +"(){return " + fieldArr[i] + ";}\r\n";
//		}

		foreach(string strChild in AllFieldInfo.Keys)
		{
			string typeStr = CheckTypeInfomation(strChild, typeArr, fieldArr);
			if(AllFieldInfo[strChild] != null)
			{
				returnString += "\t" + typeStr + "* Get" + strChild.ToLower() +"(){return &" + strChild + "[0];}\r\n";
			}else
			{
				returnString += "\t" + typeStr + " Get" + strChild.ToLower() +"(){return " + strChild + ";}\r\n";
			}

		}

		returnString += @"
	static void SetLineInfo("+ name +@" &values, string oneLine)
    {
        string o_str = oneLine;
        vector<string> str_list; // 存放分割后的字符串
        int comma_n = 0;
        do
        {
        std::string tmp_s = " + "\"\"" +@";
        comma_n = o_str.find( ""\t"" );
        if( -1 == comma_n )
        {
        tmp_s = o_str.substr( 0, o_str.length() );
        str_list.push_back( tmp_s );
        break;
        }
        tmp_s = o_str.substr( 0, comma_n );
        o_str.erase( 0, comma_n+1 );
        str_list.push_back( tmp_s );
        }
        while(true);" + "\r\n";
//		for(int i = 0; i < typeArr.Length; i++)
//		{
//			if(typeArr[i] == "int")
//			{
//				returnString += "\t\tvalues." + fieldArr[i] + " = atoi(str_list[(int)"+ name +"_" + fieldArr[i].ToUpper() +"].c_str());\r\n";
//			}else if(typeArr[i] == "string")
//			{
//				returnString += "\t\tvalues." + fieldArr[i] + " = str_list[(int)"+ name +"_" + fieldArr[i].ToUpper() +"];\r\n";
//			}else
//			{
//				Debug.LogError("error:类型仅支持int和string");
//			}
//		}

		foreach(string strChild in AllFieldInfo.Keys)
		{
			string typeStr = CheckTypeInfomation(strChild, typeArr, fieldArr);
			if(typeStr == "")
			{
				Debug.LogError("CheckTypeInfomation Error");
				Test.instance.errorT.text = "error:" + name + "error:GetParentStrInfomation Error path is :" + _path;
				Test.instance.failureWindow.SetActive(true);
				return "";
			}
			if(AllFieldInfo[strChild] != null)
			{
				for(int i = 0; i < AllFieldInfo[strChild].Count; i++)
				{
					int currArrIndex = AllFieldInfo[strChild][i];
					string currFieldStr = GetParentStrInfomation(strChild, fieldArr);
					if(currFieldStr == "")
					{
						Debug.LogError("GetParentStrInfomation Error");
						Test.instance.errorT.text = "error:" + name + "error:GetParentStrInfomation Error path is :" + _path;
						Test.instance.failureWindow.SetActive(true);
						return "";
					}
					string s = System.Text.RegularExpressions.Regex.Replace(currFieldStr, @"\d", "");
					string enumStr = name +"_" + s.ToUpper() + currArrIndex.ToString();
					if(typeStr == "int")
					{
						returnString += "\t\tvalues." + strChild + "["+ currArrIndex +"] = atoi(str_list[(int)"+ enumStr +"].c_str());\r\n";
					}else if(typeStr == "string")
					{
						returnString += "\t\tvalues." + strChild + "["+ currArrIndex +"] = str_list[(int)"+ enumStr +"];\r\n";
					}else if(typeStr == "float")
					{
						returnString += "\t\tvalues." + strChild + "["+ currArrIndex +"] = (float)atof(str_list[(int)"+ enumStr +"].c_str());\r\n";
					}else if(typeStr == "double")
					{
						returnString += "\t\tvalues." + strChild + "["+ currArrIndex +"] = atof(str_list[(int)"+ enumStr +"].c_str());\r\n";
					}else if(typeStr == "bool")
					{
						returnString += "\t\tvalues." + strChild + "["+ currArrIndex +"] = str_list[(int)"+ enumStr +"] == \"0\" ? false : true;\r\n";
					}else if(typeStr == "short")
					{
						returnString += "\t\tvalues." + strChild + "["+ currArrIndex +"] = (short)atoi(str_list[(int)"+ enumStr +"].c_str());\r\n";
					}else
					{
						Debug.LogError("error:类型仅支持int string float");
						Test.instance.errorT.text = "error:" + name + "error:类型仅支持int string float path is :" + _path;
						Test.instance.failureWindow.SetActive(true);
					}
				}

			}else
			{
				if(typeStr == "int")
				{
					returnString += "\t\tvalues." + strChild + " = atoi(str_list[(int)"+ name +"_" + strChild.ToUpper() +"].c_str());\r\n";
				}else if(typeStr == "string")
				{
					returnString += "\t\tvalues." + strChild + " = str_list[(int)"+ name +"_" + strChild.ToUpper() +"];\r\n";
				}else if(typeStr == "float")
				{
					returnString += "\t\tvalues." + strChild + " = (float)atof(str_list[(int)"+ name +"_" + strChild.ToUpper() +"].c_str());\r\n";
				}else if(typeStr == "double")
				{
					returnString += "\t\tvalues." + strChild + " = atof(str_list[(int)"+ name +"_" + strChild.ToUpper() +"].c_str());\r\n";
				}else if(typeStr == "bool")
				{
					returnString += "\t\tvalues." + strChild + " = str_list[(int)"+ name +"_" + strChild.ToUpper() +"] == \"0\" ? false : true;\r\n";
				}else if(typeStr == "short")
				{
					returnString += "\t\tvalues." + strChild + " = (short)atoi(str_list[(int)"+ name +"_" + strChild.ToUpper() +"].c_str());\r\n";
				}else
				{
					Debug.LogError("error:类型仅支持int string float");
					Test.instance.errorT.text = "error:" + name + "error:类型仅支持int string float path is :" + _path;
					Test.instance.failureWindow.SetActive(true);
				}
			}
		}

		returnString += @"

		};
		
	};
	";
		return returnString;
	}

	//检测字段信息
	static Dictionary<string, List<int>> CheckFieldInformation(string[] typeArr, string[] fieldArr)
	{
		if(typeArr[0] != "int" || Regex.Matches(fieldArr[0], @"(\d+)").Count != 0)
		{
			return null;
		}

		Dictionary<string, List<int>> arrDic = new Dictionary<string, List<int>>();
		for(int i = 0; i < fieldArr.Length; i++)
		{
			MatchCollection vMatchs = Regex.Matches(fieldArr[i], @"(\d+)");
			if(vMatchs.Count != 0)
			{
				string s = System.Text.RegularExpressions.Regex.Replace(fieldArr[i], @"[_0-9]*", "");
				int num = Convert.ToInt32(System.Text.RegularExpressions.Regex.Replace(fieldArr[i], @"[^\d]*", ""));
				if(arrDic.ContainsKey(s))
				{
					arrDic[s].Add(num);
				}else
				{
					List<int> loList = new List<int>();
					loList.Add(num);
					arrDic.Add(s, loList);
				}
			}else
			{
				arrDic.Add(fieldArr[i], null);
			}
		}
		return arrDic;
	}

	//根据字段信息获取类型信息
	public static string CheckTypeInfomation(string str, string[] typeArr, string[] fieldArr)
	{
		for(int i = 0; i < fieldArr.Length; i++)
		{
			string s = System.Text.RegularExpressions.Regex.Replace(fieldArr[i], @"[_0-9]*", "");
			str = System.Text.RegularExpressions.Regex.Replace(str, @"[_0-9]*", "");
			if(s.ToLower() == str.ToLower())
			{
				return typeArr[i];
			}
		}
		Test.instance.errorT.text = "error: CheckTypeInfomation";
		Test.instance.failureWindow.SetActive(true);
		return "";
	}

	//根据子字符串获取包含字符串信息
	static string GetParentStrInfomation(string str, string[] fieldArr)
	{
		for(int i = 0; i < fieldArr.Length; i++)
		{
			string s = System.Text.RegularExpressions.Regex.Replace(fieldArr[i], @"[_0-9]*", "");
			str = System.Text.RegularExpressions.Regex.Replace(str, @"[_0-9]*", "");
			if(s.ToLower() == str.ToLower())
			{
				return fieldArr[i];
			}
		}
		Test.instance.errorT.text = "error: CheckTypeInfomation";
		Test.instance.failureWindow.SetActive(true);
		return "";
	}

	//获取list<int>里面最大的数字
	public static int ComputeListIntMaxNum(List<int> info)
	{
		int temp = info[0];
		for (int n = 1; n < info.Count; n++)
		{
			if (temp < info[n])
			{
				temp = info[n];
			}
		}
		return temp;
	}

	//配置表数据合法性检测
	public static bool CheckTableInfo(string[] typyArr, List<string[]> tableInfoArr)
	{
		for(int i = 4; i < tableInfoArr.Count; i++)
		{
			for(int j = 0; j < typyArr.Length; j++)
			{
				switch (typyArr[j])
				{
				case "int":
					if(!IsNumeric(tableInfoArr[i][j]))
					{
						Test.instance.errorT.text = "error: 类型检测错误，目前不支持类型: 第" + i.ToString() + "行" + tableInfoArr[i][j] + " is not int" ;
						Test.instance.failureWindow.SetActive(true);
						return false;
					}
					break;
				case "float":
					if(!IsFloat(tableInfoArr[i][j]))
					{
						Test.instance.errorT.text = "error: 类型检测错误，目前不支持类型: 第" + i.ToString() + "行"  + tableInfoArr[i][j] + " is not float" ;
						Test.instance.failureWindow.SetActive(true);
						return false;
					}
					break;
				case "double":
					if(!IsFloat(tableInfoArr[i][j]))
					{
						Test.instance.errorT.text = "error: 类型检测错误，目前不支持类型: 第" + i.ToString() + "行"  + tableInfoArr[i][j] + " is not double" ;
						Test.instance.failureWindow.SetActive(true);
						return false;
					}
					break;
				case "string":

					break;
				case "short":
					if(!IsNumeric(tableInfoArr[i][j]))
					{
						Test.instance.errorT.text = "error: 类型检测错误，目前不支持类型: 第" + i.ToString() + "行"  + tableInfoArr[i][j] + " is not short" ;
						Test.instance.failureWindow.SetActive(true);
						return false;
					}
					break;
				case "bool":
					if(!(tableInfoArr[i][j] == "0") && !(tableInfoArr[i][j] == "1"))
					{
						Test.instance.errorT.text = "error: 类型检测错误，目前不支持类型: 第" + i.ToString() + "行"  + tableInfoArr[i][j] + " is not bool" ;
						Test.instance.failureWindow.SetActive(true);
						return false;
					}
					break;
				default:
					Test.instance.errorT.text = "error: 类型检测错误，目前不支持类型:" + tableInfoArr[i][j] + " is not number" ;
					Test.instance.failureWindow.SetActive(true);
					return false;
					break;
				}
			}
		}
		return true;
	}

	//判定字符串是否为整数
	public static bool IsNumeric(string str) 
	{ 
		System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^(-?[0-9])\d*$"); 
		return reg1.IsMatch(str); 
	}

	//判断字符串是否为浮点数
	public static bool IsFloat(string str)
	{
		string regextext = @"^(-?\d+)(\.\d+)?$";	
		Regex regex = new Regex (regextext,RegexOptions.None );
		return regex.IsMatch (str.Trim ());
	}
}

