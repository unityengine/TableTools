//This code create by igame ,don't modify
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace GCGame_igame.Table_igame
{

    [Serializable]
    public class Tab_DropNotify
    {
        private const string TAB_FILE_DATA = "Tables/DropNotify.txt";
        enum _ID
        {
            INVLAID_INDEX = -1,
             	ID_ID,
 	ID_DESC = 1,
 	ID_CONTENTTYPE,
 	ID_CONTENT1,
 	ID_CONTENT2,

            MAX_RECORD
        }
        public static string GetInstanceFile(){return TAB_FILE_DATA; }
         	private int m_Id;
 	public int Id{ get { return m_Id; } }
 	private string m_Desc;
 	public string Desc{ get { return m_Desc; } }
 	private int m_ContentType;
 	public int ContentType{ get { return m_ContentType; } }
	public int getContentCount() { return 3; }
	private string[] m_Content = new string[3];
	public string GetContentbyIndex(int idx)
				{
					if (idx >= 0 && idx < 3) return m_Content[idx];return "";}


        public static bool LoadTable(Dictionary<int, List<object>> _tab)
        {
            if (!TableManager.ReaderPList(GetInstanceFile(), SerializableTable, _tab))
            {
                string er = string.Format("Load File{0} Fail!!!", GetInstanceFile());
				Debug.LogError(er);
            }
            return true;
        }
        public static void SerializableTable(string[] valuesList, int skey, Dictionary<int, List<object>> _hash)
        {
            if ((int)_ID.MAX_RECORD != valuesList.Length)
            {
                string er = string.Format("Load {0} error as CodeSize:{1} not Equal DataSize:{2}", GetInstanceFile(), _ID.MAX_RECORD, valuesList.Length);
				Debug.LogError(er);
            }
            Tab_DropNotify _values = new Tab_DropNotify();
 	_values.m_Id = Convert.ToInt32(valuesList[(int)_ID.ID_ID] as string); 
 	_values.m_Desc = valuesList[(int)_ID.ID_DESC] as string; 
 	_values.m_ContentType = Convert.ToInt32(valuesList[(int)_ID.ID_CONTENTTYPE] as string); 
	_values.m_Content[1] = valuesList[(int)_ID.ID_CONTENT1] as string; 
	_values.m_Content[2] = valuesList[(int)_ID.ID_CONTENT2] as string; 


            if (_hash.ContainsKey(skey))
            {
                List<object> tList = _hash[skey];
                tList.Add(_values);
            }
            else
            {
                List<object> tList = new List<object>();
                tList.Add(_values);
                _hash.Add(skey, (List<object>)tList);
            }
        }


    }
}

