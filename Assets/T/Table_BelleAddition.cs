//This code create by igame ,don't modify
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace GCGame_igame.Table_igame
{

    [Serializable]
    public class Tab_BelleAddition
    {
        private const string TAB_FILE_DATA = "Tables/BelleAddition.txt";
        enum _ID
        {
            INVLAID_INDEX = -1,
             	ID_ID,
 	ID_DESC = 1,
 	ID_BELLECOUNT,
 	ID_BELLEID,
 	ID_ADDITION_VALUE_0,
 	ID_ADDITION_VALUE_1,
 	ID_ADDITION_VALUEV_1,
 	ID_ADDITION_VALUEV_3,
 	ID_ADDITION_VALUEV_4,

            MAX_RECORD
        }
        public static string GetInstanceFile(){return TAB_FILE_DATA; }
         	private int m_Id;
 	public int Id{ get { return m_Id; } }
 	private string m_Desc;
 	public string Desc{ get { return m_Desc; } }
 	private int m_BelleCount;
 	public int BelleCount{ get { return m_BelleCount; } }
 	private int m_BelleID;
 	public int BelleID{ get { return m_BelleID; } }
	public int getAdditionValueCount() { return 2; }
	private int[] m_AdditionValue = new int[2];
	public int GetAdditionValuebyIndex(int idx)
				{
					if (idx >= 0 && idx < 2) return m_AdditionValue[idx];
					return -1;
				}
	public int getAdditionValuevCount() { return 5; }
	private int[] m_AdditionValuev = new int[5];
	public int GetAdditionValuevbyIndex(int idx)
				{
					if (idx >= 0 && idx < 5) return m_AdditionValuev[idx];
					return -1;
				}


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
            Tab_BelleAddition _values = new Tab_BelleAddition();
 	_values.m_Id = Convert.ToInt32(valuesList[(int)_ID.ID_ID] as string); 
 	_values.m_Desc = valuesList[(int)_ID.ID_DESC] as string; 
 	_values.m_BelleCount = Convert.ToInt32(valuesList[(int)_ID.ID_BELLECOUNT] as string); 
 	_values.m_BelleID = Convert.ToInt32(valuesList[(int)_ID.ID_BELLEID] as string); 
	_values.m_AdditionValue[0] = Convert.ToInt32(valuesList[(int)_ID.ID_ADDITION_VALUE_0] as string); 
	_values.m_AdditionValue[1] = Convert.ToInt32(valuesList[(int)_ID.ID_ADDITION_VALUE_1] as string); 
	_values.m_AdditionValuev[1] = Convert.ToInt32(valuesList[(int)_ID.ID_ADDITION_VALUEV_1] as string); 
	_values.m_AdditionValuev[3] = Convert.ToInt32(valuesList[(int)_ID.ID_ADDITION_VALUEV_3] as string); 
	_values.m_AdditionValuev[4] = Convert.ToInt32(valuesList[(int)_ID.ID_ADDITION_VALUEV_4] as string); 


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

