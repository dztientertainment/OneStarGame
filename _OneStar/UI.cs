using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public OneStarGameBase oneStarGameBase;

	public InputField user;
	public InputField id;
	public InputField score;
	public InputField leaders;
	public InputField status;

	public void setLeaders(string text){
		leaders.text = text;
	}
}
