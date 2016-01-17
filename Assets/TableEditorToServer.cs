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

//	[MenuItem("Tools/Tabel Tools/选中服务器配表文件夹生成代码")]
	public static void AddSelectAllServerTabelCode()
	{

//		var select = Selection.activeObject;
//		var _path = AssetDatabase.GetAssetPath(select);
		string _path = tablePath;
		DirectoryInfo myp = new DirectoryInfo(_path);
		if(myp.Exists)
		{
			try{
				FileInfo[] allFile = myp.GetFiles();
				
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
				
				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
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
						string name = allFile[i].Name.Replace(".txt","");
						allTableManagerCodeText += CreateCodeToTableManager1(name, count);
					}
				}
				allTableManagerCodeText += codeFileTableManagerTemplateArr[1];
				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFile[i].Name.Replace(".txt","");
						allTableManagerCodeText += CreateCodeToTableManager2(name);
					}
				}
				allTableManagerCodeText += codeFileTableManagerTemplateArr[2];

				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
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
						string name = allFile[i].Name.Replace(".txt","");
						allTableManagerCodeText += CreateCodeToTableManager3(name, id);
					}
				}
				allTableManagerCodeText += codeFileTableManagerTemplateArr[3];

				string tableIgamePath = CreateTableCodeFile("TableManager", ".h");
				
				StreamWriter sw = new StreamWriter(tableIgamePath);
				sw.Write(allTableManagerCodeText);
				sw.Flush();
				sw.Close();

				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFile[i].Name.Replace(".txt","");
						allTableDefineInfoCodeText += CreateCodeToTableDefineInfo1(name, curPath);
					}
				}
				allTableDefineInfoCodeText += codeFileTableDefineInfoTemplateArr[1];
				
				string tableDefineInfoPath = CreateTableCodeFile("TableDefineInfo", ".h");
				
				StreamWriter sw2 = new StreamWriter(tableDefineInfoPath);
				sw2.Write(allTableDefineInfoCodeText);
				sw2.Flush();
				sw2.Close();

				//TableManager cpp
				string TableManagerCppPath = CreateTableCodeFile("TableManager", ".cpp");
				StreamWriter tmcpp = new StreamWriter(TableManagerCppPath);
				tmcpp.Write("#include \"TableManager.h\"");
				tmcpp.Flush();
				tmcpp.Close();

				//TableDefineInfo cpp
				string TableDefineInfoCppPath = CreateTableCodeFile("TableDefineInfo", ".cpp");
				StreamWriter tdicpp = new StreamWriter(TableDefineInfoCppPath);
				string tdiWriteStr = "#include \"TableDefineInfo.h\"\r\n";
				for(int i = 0; i < allFile.Length; i++)
				{
					string curPath = allFile[i].FullName;
					string s = Path.GetExtension(curPath);
					if(s == ".txt")
					{
						string name = allFile[i].Name.Replace(".txt","");
						tdiWriteStr += "string "+ name +"::file = \""+ name +".txt\";\r\n";
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
		}
//		AssetDatabase.Refresh();
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
        myFile.open(path + "+ name +@"::file);
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
		return @"bl &= Init"+ name +"();";
	}

	static string CreateCodeToTableManager3(string name, string id)
	{
		return name + @"& Get"+ name +@"ById(int id)
    {
        int count = "+ name +@"ArrCount;
        int low = 0;  
        int high = count - 1;  
        int mid = 0;  
        while ( low <= high )  
        {  
            mid = (low + high )/2;  
            if (g_"+ name +@"[mid].Get"+ id +@"() < id)  
                low = mid + 1;  
            else if (g_"+ name +@"[mid].Get"+ id +@"() > id )     
                high = mid - 1;  
            else  
                break;  
        }
        return g_"+ name +@"[mid];
    }
    "+ name + @"& Get"+ name +@"ByIndex(int index)
    {
        return g_"+ name +@"[index];
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
		allLineStr[1] = allLineStr[1].ToLower();
		string[] fieldStrArrLine1 = allLineStr[0].Split();
		string[] fieldStrArrLine2 = allLineStr[1].Split();

		string[] typeArr = null;
		string[] fieldArr = null;
		if(fieldStrArrLine2[0] == "int" || fieldStrArrLine1[0] == "int")
		{
			if(fieldStrArrLine2[0] == "int")
			{
				typeArr = fieldStrArrLine2;
				fieldArr = fieldStrArrLine1;
			}else
			{
				typeArr = fieldStrArrLine1;
				fieldArr = fieldStrArrLine2;
			}
		}else
		{
			Debug.LogError("请检查字段信息，index必须为int类型");
			return "";
		}

		for(int i = 0; i < typeArr.Length; i++)
		{
			returnString += "\t" + typeArr[i] + " " + fieldArr[i] + ";\r\n";
		}
		returnString += @"
enum "+ name +@"Enum
{";
		for(int i = 0; i < typeArr.Length; i++)
		{
			returnString += "\t" + fieldArr[i].ToUpper() + " = "+ i +",\r\n";
		}
		returnString += @"};

public:
    "+ name +@"(){
    }
    ~"+ name +@"(){}
   
    
    static string file;
    ";
		for(int i = 0; i < typeArr.Length; i++)
		{
			returnString += "\t" + typeArr[i] + " Get" + fieldArr[i].ToLower() +"(){return " + fieldArr[i] + ";}\r\n";
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
		for(int i = 0; i < typeArr.Length; i++)
		{
			if(typeArr[i] == "int")
			{
				returnString += "\t\tvalues." + fieldArr[i] + " = atoi(str_list[(int)"+ name +"Enum::" + fieldArr[i].ToUpper() +"].c_str());\r\n";
			}else if(typeArr[i] == "string")
			{
				returnString += "\t\tvalues." + fieldArr[i] + " = str_list[(int)" + fieldArr[i].ToUpper() +"];\r\n";
			}else
			{
				Debug.LogError("error:类型仅支持int和string");
			}
		}
		returnString += @"

		};
		
	};
	";
		return returnString;
	}

}
