//This code create by igame, don't modify
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;

namespace GCGame_igame.Table_igame
{
    public delegate void SerializableTable(string[] valuesList, int skey, Dictionary<int, List<object>> _hash);
    [Serializable]
    public class TableManager
    {
        private static string GetLoadPath(string localName)
        {
            string localPath = Application.persistentDataPath + "/ResData/Tables/" + localName + ".txt";
            if (File.Exists(localPath))
            {
                return localPath;
            }
#if UNITY_ANDROID && !UNITY_EDITOR
 return localPath;
#elif UNITY_EDITOR
            return Application.dataPath + "/BundleAssets_igame/Tables/" + localName + ".txt";
#else
 return Application.streamingAssetsPath + "/Tables/" + localName + ".txt";
#endif
        }
        private static string[] MySplit(string str, string[] nTypeList, string regix)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            String[] content = new String[nTypeList.Length];
            int nIndex = 0;
            int nstartPos = 0;
            while (nstartPos <= str.Length)
            {
                int nsPos = str.IndexOf(regix, nstartPos);
                if (nsPos < 0)
                {
                    String lastdataString = str.Substring(nstartPos);
                    if (string.IsNullOrEmpty(lastdataString) && nTypeList[nIndex].ToLower() != "string")
                    {
                        content[nIndex++] = "--";
                    }
                    else
                    {
                        content[nIndex++] = lastdataString;
                    }
                    break;
                }
                else
                {
                    if (nstartPos == nsPos)
                    {
                        if (nTypeList[nIndex].ToLower() != "string")
                        {
                            content[nIndex++] = "--";
                        }
                        else
                        {
                            content[nIndex++] = "";
                        }
                    }
                    else
                    {
                        content[nIndex++] = str.Substring(nstartPos, nsPos - nstartPos);
                    }
                    nstartPos = nsPos + 1;
                }
            }
            return content;
        }
        private static string m_Key = "";
        private static string[] m_Value;
        public static bool ReaderPList(String xmlFile, SerializableTable _fun, Dictionary<int, List<object>> _hash)
        {
            Debug.Log(xmlFile);
            string[] list = xmlFile.Split('.');
            string relTablePath = list[0].Substring(7);
            string tableFilePath = GetLoadPath(relTablePath);
            string[] alldataRow;
            Debug.Log(tableFilePath);
            if (File.Exists(tableFilePath))
            {
                StreamReader sr = null;
                sr = File.OpenText(tableFilePath);
                string tableData = sr.ReadToEnd();
                sr.Close();
                alldataRow = tableData.Split('\n');
            }
            else
            {
                TextAsset testAsset = Resources.Load(list[0], typeof(TextAsset)) as TextAsset;
                //igame_csf 安卓平台下persistentDataPath和resource均无法读取就使用www读取streamingassets
                if (testAsset == null)
                {
                    string path = Application.streamingAssetsPath + "/Tables/" + relTablePath + ".txt";
                    WWW www = new WWW(path);
                    //Debug.Log("开始读表！------>" + relTablePath);
                    while (!www.isDone)
                    {
                        //igame_csf 用while来限制必须读完表才往下走，表格如果不读完整UI会出现错乱
                    }
                    alldataRow = www.text.Split('\n');
                    //Debug.Log("结束读表！------>" + relTablePath);
                }
                else
                {
                    alldataRow = testAsset.text.Split('\n');
                }
            }
            //skip fort three
            int skip = 0;
            string[] typeList = null;
            foreach (string line in alldataRow)
            {
                int nKey = -1;
                if (skip == 1)
                {
                    string sztemp = line;
                    if (sztemp.Length >= 1)
                    {
                        if (sztemp[sztemp.Length - 1] == '\r')
                        {
                            sztemp = sztemp.TrimEnd('\r');
                        }
                    }
                    typeList = line.Split('\t');
                    m_Value = new string[typeList.Length];
                    ++skip;
                    continue;
                }
                if (++skip < 4) continue;
                if (String.IsNullOrEmpty(line)) continue;
                if (line[0] == '#') continue;
                string szlinetemp = line;
                if (szlinetemp.Length >= 1)
                {
                    if (szlinetemp[szlinetemp.Length - 1] == '\r')
                    {
                        szlinetemp = szlinetemp.TrimEnd('\r');
                    }
                }
                string[] strCol = MySplit(szlinetemp, typeList, "\t");
                if (strCol.Length == 0) continue;
                string skey = strCol[0];
                string[] valuesList = new string[strCol.Length];

                if (string.IsNullOrEmpty(skey) || skey.Equals("--"))
                {
                    skey = m_Key;
                    nKey = Int32.Parse(skey);
                    valuesList[0] = skey;
                    for (int i = 1; i < strCol.Length; ++i)
                    {
                        if (String.IsNullOrEmpty(strCol[i]) || strCol[i] == "--")
                        {
                            valuesList[i] = m_Value[i];
                        }
                        else
                        {
                            valuesList[i] = strCol[i];
                            m_Value[i] = strCol[i];
                        }
                    }

                }
                else
                {
                    m_Key = skey;
                    nKey = Int32.Parse(skey);

                    for (int i = 0; i < strCol.Length; ++i)
                    {
                        if (strCol[i] == "--")
                        {
                            valuesList[i] = "0";
                            m_Value[i] = "0";
                        }
                        else
                        {
                            valuesList[i] = strCol[i];
                            m_Value[i] = strCol[i];
                        }
                    }
                }
                _fun(valuesList, nKey, _hash);
            }
            return true;
        }
        private static Dictionary<int, List<Tab_AchieveNotice>> g_AchieveNotice = new Dictionary<int, List<Tab_AchieveNotice>>();
		public static bool InitTable_AchieveNotice()
		{
			g_AchieveNotice.Clear();
			Dictionary<int, List<object>> tmps = new Dictionary<int, List<object>>();
			if (!Tab_AchieveNotice.LoadTable(tmps)) return false;
			foreach (KeyValuePair<int, List<object>> kv in tmps)
			{
				List<Tab_AchieveNotice> values = new List<Tab_AchieveNotice>();
				foreach (object subit in kv.Value)
				{
					values.Add((Tab_AchieveNotice)subit);
				}
				g_AchieveNotice.Add(kv.Key, values);
			}
			return true;
		}private static Dictionary<int, List<Tab_BelleAddition>> g_BelleAddition = new Dictionary<int, List<Tab_BelleAddition>>();
		public static bool InitTable_BelleAddition()
		{
			g_BelleAddition.Clear();
			Dictionary<int, List<object>> tmps = new Dictionary<int, List<object>>();
			if (!Tab_BelleAddition.LoadTable(tmps)) return false;
			foreach (KeyValuePair<int, List<object>> kv in tmps)
			{
				List<Tab_BelleAddition> values = new List<Tab_BelleAddition>();
				foreach (object subit in kv.Value)
				{
					values.Add((Tab_BelleAddition)subit);
				}
				g_BelleAddition.Add(kv.Key, values);
			}
			return true;
		}private static Dictionary<int, List<Tab_BelleMatrixAddition>> g_BelleMatrixAddition = new Dictionary<int, List<Tab_BelleMatrixAddition>>();
		public static bool InitTable_BelleMatrixAddition()
		{
			g_BelleMatrixAddition.Clear();
			Dictionary<int, List<object>> tmps = new Dictionary<int, List<object>>();
			if (!Tab_BelleMatrixAddition.LoadTable(tmps)) return false;
			foreach (KeyValuePair<int, List<object>> kv in tmps)
			{
				List<Tab_BelleMatrixAddition> values = new List<Tab_BelleMatrixAddition>();
				foreach (object subit in kv.Value)
				{
					values.Add((Tab_BelleMatrixAddition)subit);
				}
				g_BelleMatrixAddition.Add(kv.Key, values);
			}
			return true;
		}
        public bool InitTable()
        {
            bool bRet = true;
            			bRet &= InitTable_AchieveNotice();
			bRet &= InitTable_BelleAddition();
			bRet &= InitTable_BelleMatrixAddition();

            return bRet;
        }

        public static List<Tab_AchieveNotice> GetAchieveNoticeByID(int nKey)
		{
			if (g_AchieveNotice.Count == 0)
			{
				InitTable_AchieveNotice();
			}
			if (g_AchieveNotice.ContainsKey(nKey))
			{
				return g_AchieveNotice[nKey];
			}
			return null;
		}
		public static Tab_AchieveNotice GetAchieveNoticeByID(int nKey, int nIndex)
		{
			if (g_AchieveNotice.Count == 0)
			{
				InitTable_AchieveNotice();
			}
			if (g_AchieveNotice.ContainsKey(nKey))
			{
				if (nIndex >= 0 && nIndex < g_AchieveNotice[nKey].Count)
					return g_AchieveNotice[nKey][nIndex];
			}
			return null;
		}
		public static Dictionary<int, List<Tab_AchieveNotice>> GetAchieveNotice()
		{
			if (g_AchieveNotice.Count == 0)
			{
				InitTable_AchieveNotice();
			}
			return g_AchieveNotice;
		}public static List<Tab_BelleAddition> GetBelleAdditionByID(int nKey)
		{
			if (g_BelleAddition.Count == 0)
			{
				InitTable_BelleAddition();
			}
			if (g_BelleAddition.ContainsKey(nKey))
			{
				return g_BelleAddition[nKey];
			}
			return null;
		}
		public static Tab_BelleAddition GetBelleAdditionByID(int nKey, int nIndex)
		{
			if (g_BelleAddition.Count == 0)
			{
				InitTable_BelleAddition();
			}
			if (g_BelleAddition.ContainsKey(nKey))
			{
				if (nIndex >= 0 && nIndex < g_BelleAddition[nKey].Count)
					return g_BelleAddition[nKey][nIndex];
			}
			return null;
		}
		public static Dictionary<int, List<Tab_BelleAddition>> GetBelleAddition()
		{
			if (g_BelleAddition.Count == 0)
			{
				InitTable_BelleAddition();
			}
			return g_BelleAddition;
		}public static List<Tab_BelleMatrixAddition> GetBelleMatrixAdditionByID(int nKey)
		{
			if (g_BelleMatrixAddition.Count == 0)
			{
				InitTable_BelleMatrixAddition();
			}
			if (g_BelleMatrixAddition.ContainsKey(nKey))
			{
				return g_BelleMatrixAddition[nKey];
			}
			return null;
		}
		public static Tab_BelleMatrixAddition GetBelleMatrixAdditionByID(int nKey, int nIndex)
		{
			if (g_BelleMatrixAddition.Count == 0)
			{
				InitTable_BelleMatrixAddition();
			}
			if (g_BelleMatrixAddition.ContainsKey(nKey))
			{
				if (nIndex >= 0 && nIndex < g_BelleMatrixAddition[nKey].Count)
					return g_BelleMatrixAddition[nKey][nIndex];
			}
			return null;
		}
		public static Dictionary<int, List<Tab_BelleMatrixAddition>> GetBelleMatrixAddition()
		{
			if (g_BelleMatrixAddition.Count == 0)
			{
				InitTable_BelleMatrixAddition();
			}
			return g_BelleMatrixAddition;
		}

        }
}
