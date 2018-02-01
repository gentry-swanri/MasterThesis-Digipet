using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour {

	public GameObject targetObj;
	private SaveManager otherScritpToAccess;
	public Text valueText;
	public int score;
	// Use this for initialization

	void Start () {
		//otherScritpToAccess = targetObj.GetComponent<StatusManager>();
		//valueText = otherScritpToAccess.BonesText;
		//UpdateScore ();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateScore ();
	}

	public void UpdateScore(){
		//valueText.text = "EXP: " + score.ToString ();
	}
}
