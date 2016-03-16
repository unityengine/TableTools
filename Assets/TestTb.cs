using UnityEngine;
using System.Collections;
using GCGame_igame.Table_igame;
using System.Collections.Generic;

public class TestTb : MonoBehaviour {

 
	// Use this for initialization
	void Start () {
		TableManager a = new TableManager();
		a.InitTable();
		object obj = TableManager.GetTabelByName("DropNotify");
		var aa = (Dictionary<int, List<Tab_DropNotify>>)obj;
		Debug.LogError(aa[1][0].Desc);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

