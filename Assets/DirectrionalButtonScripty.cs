using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;
using System;

public class DirectrionalButtonScripty : MonoBehaviour {
	
	public KMAudio Audio;
	public KMBombInfo bomb;
		static int moduleIdCounter = 1;
	int moduleID;
	private int Presses = 0;
	private bool up = false;
	public KMSelectable Bottun;
	public KMSelectable Up;
	public KMSelectable Down;
	public Renderer Buttoncolor;
	public TextMesh BottunText;
	private int a = 0;
	private int b = 0;
	private int CorrectPres;
	private bool ModuleSolved = false;
	private int Stage = 1;
	private static string[] Color = {"Red", "Blue", "White"};
	private static string[] Texts = {"Abort", "Detonate", "GG M8"};
	public Color[] Colors;
	void Awake () {
		moduleID = moduleIdCounter++;
		Bottun.OnInteract += delegate () { BottunPres(); return false; };
		Down.OnInteract += delegate () { Downa(); return false; };
		Up.OnInteract += delegate () { Upa(); return false; };

	}
	// Use this for initialization
	void Start () {
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void Generate() {
		a = UnityEngine.Random.Range(0,3);
		b = UnityEngine.Random.Range(0,2);
		BottunText.text = Texts[b];
		Buttoncolor.material.SetColor("_Color", Colors[a]);	
		Presses = 0;
	Debug.LogFormat("[Directional Button #{0}] The phrase on the button is '{1},' and the color is {2}", moduleID, Texts[b], Color[a]);
		Ruling();
	}
	void BottunPres() {
	Bottun.AddInteractionPunch();
	GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);

	Presses = Presses+1;
	}
	void Downa(){
	GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);	Down.AddInteractionPunch();

		if (ModuleSolved==true){
			return;
		}
		if(up==false && Presses == CorrectPres){
			Stage = Stage+1;
			if(Stage==6){
				GetComponent<KMBombModule>().HandlePass();
				Solve();
				ModuleSolved = true;
			}
			else {
				Generate();
			}
		}
		else {
		Generate();
		GetComponent<KMBombModule>().HandleStrike();
		}
	}
	void Upa(){
	GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);
	Up.AddInteractionPunch();

		if (ModuleSolved==true){
			return;
		}
		if(up==true && Presses == CorrectPres){
			Stage = Stage+1;
			if(Stage==6){
				GetComponent<KMBombModule>().HandlePass();
				Solve();
				ModuleSolved = true;
			}
			else {
				Generate();
			}
		}
		else {
		Generate();
		GetComponent<KMBombModule>().HandleStrike();
		}
	}
	void Ruling() {
		if (a==1&&b==1){
			CorrectPres = 1;
			up = false;
			Debug.LogFormat("[Directional Button #{0}] You need to press the button once and then go down.", moduleID);
		}
		else if (a==0){
			CorrectPres = 2;
			up = false;
			Debug.LogFormat("[Directional Button #{0}] You need to press the button twice and then go down.", moduleID);

		}
		else if (b==0){
			CorrectPres = 3;
			up = true;
			Debug.LogFormat("[Directional Button #{0}] You need to press the button three times and then go up.", moduleID);

		}
		else if (a==2){
			CorrectPres = 4;
			up = true;
			Debug.LogFormat("[Directional Button #{0}] You need to press the button four times and then go up.", moduleID);

		}
	}
	void Solve() {
	BottunText.text = Texts[2];
	}
}
