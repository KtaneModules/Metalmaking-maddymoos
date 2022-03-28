using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;

public class MetalmakingScript : MonoBehaviour {

	public MeshRenderer Stupidface;
	public Transform GayRotator;
	public MeshRenderer[] Meteos;
	public Material[] Materials;
	public KMAudio Audio;
	public KMBombModule Module;
	public KMBombInfo Bomb;
	public KMSelectable[] Buttons;
	static private int _moduleIdCounter = 1;
	private int _moduleId;

	private int[] MeteoSets = new int[8], MeteoIndex = new int[8];
	private int RareMetal = 3;

	private float[][] MeteoVals = new float[][]
	{
		new float[3],
		new float[3],
		new float[3],
		new float[3],
		new float[3],
		new float[3],
		new float[3],
		new float[3]
    };
	static readonly private float[][] StatsMeteo = new float[][]
	{
		new float[] {3, 1, 1.5f},
		new float[] {1, 0.5f, 3},
		new float[] {.5f, 1.5f, 3},
		new float[] {.5f, 3, 2},
		new float[] {1.5f, 2, 2},
		new float[] {2, .5f, 2},
		new float[] {1.5f, 1, 3},
		new float[] {1, 3, .5f},
		new float[] {2.5f, 2, 1},
		new float[] {1, 3, 0.5f},
	};
	static readonly private float[][] StatsPlanetEven = new float[][]
	{
		new float[] {1.5f,3.5f,3.5f },
		new float[] {5,2,1.5f },
		new float[] {1f,4,3.5f},
		new float[] {3.5f,2.5f,2.5f },
		new float[] {2,2,4.5f },
		new float[] {4.5f,1.5f,2.5f },
		new float[] {1f,0.5f,1.5f },
		new float[] {3,1.5f,4 },
		new float[] {0.5f,4,4 },
		new float[] {3,3,2.5f },
		new float[] {5,1.5f,2 },
		new float[] {2.5f,2.5f,3.5f },
		new float[] {2.5f,4,2 },
		new float[] {4,2,2.5f },
		new float[] {3.5f,2,3 },
		new float[] {2,2.5f,4 },
		new float[] {3.5f,1.5f,3.5f },
		new float[] {4.5f,2,2 }
	};
	static readonly private float[][] StatsPlanetOdd = new float[][]
	{
		new float[] {2,1.5f,5},
		new float[] {1,3.5f,4 },
		new float[] {2,5,1.5f },
		new float[] {3.5f,2.5f,2.5f },
		new float[] {5,1,2.5f },
		new float[] {1,0.5f,3.5f },
		new float[] {4,0.5f,4 },
		new float[] {3,2,3.5f },
		new float[] {0.5f,4,4 },
		new float[] {3.5f,0.5f,4.5f },
		new float[] {1.5f,2.5f,4.5f },
		new float[] {1.5f,3,4 },
		new float[] {1,5,2.5f },
		new float[] {3,1,4.5f },
		new float[] {3,2.5f,3 },
		new float[] {4,1.5f,3 },
		new float[] {3.5f,1.5f,3.5f },
		new float[] {4.5f,2,2 }
	};
	static readonly private string[] MeteoNames = { "fire", "air", "H20", "soil","iron","zap","herb","zoo","glow","dark" };
	static readonly private string[] PlanetNamesEven = { "Geolytian", "Firimian", "Oleanan", "Anasazean", "Grannestian", "Megadomer", "Luna=Lunarian", "Bavoomian", "Freazer","Boggobian","Jeljelian","Mekksian","Forter","Dawndusian","Starriing","Lastaral","Gigagusher","Meteorian" };
	static readonly private string[] PlanetNamesOdd = { "Yoojic", "Hevendorian", "Suburbionite", "Anasazean", "Hottedian", "Vubblian", "Layazeroan", "Floriasian", "Freazer", "Brabbiter", "Wuudite", "Wiralon", "Gravitase", "Caviousian", "Thirnovas", "Globinite", "Gigagusher", "Meteorian" };
    private bool isBatteries = false;
	private bool[] Burned = new bool[8];
	private bool[] VeryBurned = new bool[8];
	private bool[] yeety = new bool[5];
	private bool solving = false;
	static readonly private Vector3[] OgPos =
	{
		new Vector3(0, -0.006274676f, 0.05f),
		new Vector3(0.03535534f, -0.006274676f, 0.03535534f),
		new Vector3(0.05f, -0.006274676f, 0f),
		new Vector3(0.03535534f, -0.006274676f, -0.03535534f),
		new Vector3(0, -0.006274676f, -0.05f),
		new Vector3(-0.03535534f, -0.006274676f, -0.03535534f),
		new Vector3(-0.05f, -0.006274676f,0),
		new Vector3(-0.03535534f, -0.006274676f, 0.03535534f)
	};
	static readonly private Vector3[] AnimPos =
	{
		new Vector3(0, -0.006274676f, 0.05f),
		new Vector3(0.04755283f, -0.006274676f, 0.01545085f),
		new Vector3(0.02938926f, -0.006274676f, -0.04045085f),
		new Vector3(-0.02938926f, -0.006274676f, -0.04045085f),
		new Vector3(-0.04755283f, -0.006274676f, 0.01545085f)
	};
	private int MaterialThreshold = 8, AttemptCount, inputcount=0;
	private int[] IndexesLaunched = new int[5];
	private bool Solvable;
	float duration = 10f;
	// Use this for initialization
	void Awake()
	{
		_moduleId = _moduleIdCounter++;
		for (byte i = 0; i < Buttons.Length; i++)
		{
			byte j = i;
			Buttons[j].OnInteract += delegate
			{
				if(!VeryBurned[j] && inputcount != 5 && !solving)
					HandlePress(j);
				return false;
			};
			Buttons[j].OnHighlight += delegate
			{
				if (!VeryBurned[j] && !solving)
					Burned[j] = true;
				Burn(true, j);
			};
			Buttons[j].OnHighlightEnded += delegate
			{
				if (!VeryBurned[j] && !solving)
					Burned[j] = false;
				Burn(false, j);
			};
		}
	}
	void Start ()
    {
		Audio.PlaySoundAtTransform("land", Module.transform);
		if (Bomb.GetBatteryCount() % 2 != 0)
		{
			isBatteries = true;
		}
		GenerateModule();
	}
	void HandlePress(int index)
    {
		StartCoroutine(DoTheLaunch(index));
    }
	void Burn(bool burning, int index)
    {
		if (!solving)
		{
			if (burning)
			{
				Meteos[index].material = Materials[21];
				Meteos[index].material.mainTextureOffset = new Vector2(MeteoSets[index] / 20f, 0);
			}
			else if (!VeryBurned[index])
			{
				Meteos[index].material = Materials[MeteoSets[index]];
				Meteos[index].material.mainTextureOffset = new Vector2(MeteoIndex[index] / 10f, 0);
			}
		}
	}
	
	void GenerateModule()
    {
		Debug.Log("Attempt " + ++AttemptCount);
		StopAllCoroutines();
		for (int i = 0; i < 8; i++)
		{
			Meteos[i].transform.localScale = new Vector3(-.025f, -.001f, .025f);
			MeteoSets[i] = Rnd.Range(0, 18);
			MeteoIndex[i] = Rnd.Range(0, 10);
			for(int j = 0; j < 3; j++)
            {
				MeteoVals[i][j] = StatsMeteo[MeteoIndex[i]][j];
				if (isBatteries)
					MeteoVals[i][j] *= StatsPlanetOdd[MeteoSets[i]][j];
				else
					MeteoVals[i][j] *= StatsPlanetEven[MeteoSets[i]][j];
			}
		}
		if(RareMetal == 3)
        {
			RareMetal = Rnd.Range(0, 3);
        }
		Stupidface.material = Materials[RareMetal + 18];
        if (CheckValid())
        {
			ValidSolutionFound(Solvable);
			Solvable = true;
        }
        else
        {
			GenerateModule();
        }
	}
	bool CheckValid()
    {
		for (int i = 0; i < 4; i++)
			for (int j = i + 1; j < 5; j++)
				for (int k = j + 1; k < 6; k++)
					for (int l = k + 1; l < 7; l++)
						for (int m = l + 1; m < 8; m++)
							if (TheMathPart(i, j, k, l, m))
							{
								int[] wow = { i, j, k, l, m };
								Debug.LogFormat("[Metalmaking #{0}]: After {1} attempts, found valid solution: {2}. Note that this may not be the only solution.", _moduleId, AttemptCount, wow.Select(x => x+1).Join(", "));
								return true;
							}
		return false;
	}

	bool TheMathPart(int i, int j, int k, int l, int m)
    {
		int[] wow = { i, j, k, l, m };
		var Sum = new Vector3(0,0,0);
		for(int a = 0; a < 5; a++)
        {
			float[] vector = MeteoVals[wow[a]];
			Sum += new Vector3(vector[0], vector[1], vector[2]);
        }
        switch (RareMetal)
        {
			case 0: return (Sum[2] >= Sum[0] + MaterialThreshold && Sum[0] >= Sum[1] + MaterialThreshold);
			case 1: return (Sum[1] >= Sum[2] + MaterialThreshold && Sum[2] >= Sum[0] + MaterialThreshold);
			case 2: return (Sum[0] >= Sum[1] + MaterialThreshold && Sum[1] >= Sum[2] + MaterialThreshold);
        }
		return false;
    }
	void ValidSolutionFound(bool yes)
    {
		Debug.LogFormat("[Metalmaking #{0}]: The rare meteo you need to forge is [{1}].", _moduleId, RareMetal==0 ? "Soul" : RareMetal == 1 ? "Time" : "Chi");
		for(int i = 0; i < 8; i++)
        {
			Debug.LogFormat("[Metalmaking #{0}]: Meteo #{1} is a {2} {3} with a value set of [{4}].", _moduleId, i + 1, (!isBatteries ? PlanetNamesEven[MeteoSets[i]] : PlanetNamesOdd[MeteoSets[i]]), MeteoNames[MeteoIndex[i]], MeteoVals[i].Join(", "));
			StartCoroutine(AnimateMeteo(i));
			if (yes)
			{
				StartCoroutine(Explode(i));
			}
		}
		StartCoroutine(Meteorbit());
		StartCoroutine(AnimateRareMetal());
	}
	IEnumerator Explode(int index)
    {
		yield return null;
		var goal = OgPos[index];
		var fat = new Vector3(0, -0.006274676f, 0);
		float i = 0;
		while (i < 1)
		{
			Meteos[index].transform.localPosition = Vector3.Lerp(fat, goal, easeInBack(i));
			yield return null;
			i += Time.deltaTime;
		}
		Meteos[index].transform.localPosition = goal;
	}

	IEnumerator AnimateRareMetal()
    {
		float timer = 0;
		while (true)
		{
			yield return new WaitForSecondsRealtime(.075f);

				timer += .2f;
				if (timer == 1) timer = 0;
				Stupidface.material.mainTextureOffset = new Vector2(timer, 0);
		}
    }
	IEnumerator AnimateMeteo(int index)
	{
		float timer = 0;
		Meteos[index].material = Materials[MeteoSets[index]];
		Meteos[index].material.mainTextureOffset = new Vector2(MeteoIndex[index] / 10f, 0);
		while (true)
		{
            if (VeryBurned[index])
            {
				break;
			}
			if (Burned[index] || VeryBurned[index])
			{
				Meteos[index].material.mainTextureOffset = new Vector2(MeteoSets[index] / 20f, 0);
				yield return new WaitForSeconds(.05f);
				timer = .5f;
			}
			else if (Meteos[index].material.name == "Flame" && !Burned[index] && !VeryBurned[index])
			{
				Meteos[index].material = Materials[MeteoSets[index]];
				Meteos[index].material.mainTextureOffset = new Vector2(MeteoIndex[index] / 10f, 0);
				yield return null;
				timer = .5f;
			}
			else
			{
				yield return new WaitForSecondsRealtime(.075f);
				if (Materials[MeteoSets[index]].name == "Set17")
					yield return new WaitForSecondsRealtime(.1f);
				else if (Materials[MeteoSets[index]].name == "Set11" || Materials[MeteoSets[index]].name == "Set10")
					yield return new WaitForSecondsRealtime(.025f);
				if (Meteos[index].material.mainTextureScale.y != 1)
				{
					timer += 1 - Meteos[index].material.mainTextureScale.y;
					if (timer >= 1) timer -= 1;
					if (timer == 0f && Meteos[index].material.mainTextureScale.y != .5f && Meteos[index].material.mainTextureScale.y != 0.1666667f) timer += .75f;
					Meteos[index].material.mainTextureOffset = new Vector2(MeteoIndex[index] / 10f, timer);
				}
			}
		}
	}
	IEnumerator Meteorbit()
    {
		//thanks quinn
		while (true)
		{
			var elapsed = 0f;
			while (elapsed < duration)
			{
				GayRotator.transform.localEulerAngles = new Vector3(0f, Mathf.Lerp(0f, 360f, elapsed / duration), 0f);
				for (int i = 0; i < 8; i++)
					Meteos[i].transform.localEulerAngles = new Vector3(0f, Mathf.Lerp(360f, 0f, elapsed / duration), 0f);
				yield return null;
				elapsed += Time.deltaTime;
			}
			GayRotator.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			yield return null;
			for (int i = 0; i < 8; i++)
				Meteos[i].transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
	}
	float easeInBack(float x)
	{
		return 1 - Mathf.Pow(1f - x, 4);
	}
	IEnumerator DoTheLaunch(int index)
    {
		if (!(inputcount == 5))
		{
			Audio.PlaySoundAtTransform("launch", Module.transform);
			IndexesLaunched[inputcount] = index;
			inputcount++;
			Burned[index] = true;
			VeryBurned[index] = true;
			Meteos[index].material = Materials[21];
			Meteos[index].material.mainTextureOffset = new Vector2(MeteoSets[index] / 20f, 0);
			float i = 0;
			var fat = new Vector3(Meteos[index].transform.localPosition.x, -0.006274676f, Meteos[index].transform.localPosition.z);
			var goal = new Vector3(0, -0.006274676f, 0);
			if (inputcount == 5)
			{
				while (i < 1)
				{
					Meteos[index].transform.localPosition = Vector3.Lerp(fat, goal, easeInBack(i));
					yield return null;
					i += Time.deltaTime;
				}
				var goal2 = new Vector3(0, 0, 0);

				Meteos[index].transform.localPosition = goal2;
				Buttons[index].enabled = false;
				Meteos[index].enabled = false;
				if (inputcount == 5 && CheckAnswer(IndexesLaunched))
				{
					StartCoroutine(SolveAnim(true));
				}
				else if (inputcount == 5)
				{
					StartCoroutine(SolveAnim(false));
				}
			}
            else
            {
				while (i < 1)
				{
					Meteos[index].transform.localPosition = Vector3.Lerp(fat, goal, easeInBack(i));
					yield return null;
					i += Time.deltaTime;
				}
			}
		}
	}
	IEnumerator SolveAnim(bool correc)
    {
		solving = true;
		yield return null;
		float j = 0;
		float[] thisisdumb = { 1, 1, 1, 1, 1, 1, 1, 1 };
		for(int k = 0; k < 8; k++) 
		{
            if (IndexesLaunched.Contains(k))
            {
				thisisdumb[k]++;
            }
		}
		int[] indexies = AllIndexes(thisisdumb, 1).ToArray();
		Debug.Log(indexies.Join(" "));
		var fat1 = Meteos[indexies[0]].transform.localPosition;
		var fat2 = Meteos[indexies[1]].transform.localPosition;
		var fat3 = Meteos[indexies[2]].transform.localPosition;
		var goa1 = new Vector3(fat1.x * 50, fat1.y, fat1.z * 50);
		var goa2 = new Vector3(fat2.x * 50, fat2.y, fat2.z * 50);
		var goa3 = new Vector3(fat3.x * 50, fat3.y, fat3.z * 50);
        while (j < 1)
        {
            Meteos[indexies[0]].transform.localPosition = Vector3.Lerp(fat1, goa1, j);
            Meteos[indexies[1]].transform.localPosition = Vector3.Lerp(fat2, goa2, j);
            Meteos[indexies[2]].transform.localPosition = Vector3.Lerp(fat3, goa3, j);
            j += Time.deltaTime / 2f;
			yield return null;
        }
		Meteos[indexies[0]].enabled = false;
		Meteos[indexies[1]].enabled = false;
		Meteos[indexies[2]].enabled = false;
		j = 0;
		for(int i = 0; i < 5; i++)
        {
			VeryBurned[IndexesLaunched[i]] = false;
			Burned[IndexesLaunched[i]] = false;
			Meteos[IndexesLaunched[i]].enabled = true;
			Meteos[IndexesLaunched[i]].material = Materials[MeteoSets[IndexesLaunched[i]]];
			Meteos[IndexesLaunched[i]].material.mainTextureOffset = new Vector2(MeteoIndex[IndexesLaunched[i]] / 10f, 0);
			StartCoroutine(AnimateMeteo(IndexesLaunched[i]));

		}
		duration = 3f;
		while (j < 1)
        {
			for(int i = 0; i < 5; i++)
            {
					Meteos[IndexesLaunched[i]].transform.localPosition = Vector3.Lerp(new Vector3 (0, -0.006274676f, 0), AnimPos[i], easeInBack(j));

			}
			j += Time.deltaTime * 2;
			yield return null;
        }
		yield return new WaitForSeconds(.5f);
		j = 0;
		while (j < 1)
		{
			for (int i = 0; i < 5; i++)
			{
				Meteos[IndexesLaunched[i]].transform.localPosition = Vector3.Lerp(AnimPos[i], new Vector3(0, -0.006274676f, 0), j);

			}
			j += Time.deltaTime / 2f;
			yield return null;
		}
		if (correc)
        {
			Module.HandlePass();
			Audio.PlaySoundAtTransform("SolveSound", Module.transform);
			for(int i = 0; i < 5; i++)
            {
				Meteos[IndexesLaunched[i]].transform.localPosition = AnimPos[i];
			}
			duration = 10f;
		}
        else
        {
			
			Module.HandleStrike();
			for(int i = 0; i < 8; i++)
            {
				VeryBurned[i] = false;
				Burned[i] = false;
				Buttons[i].enabled = true;
				Meteos[i].enabled = true;
			}
			inputcount = 0;
			IndexesLaunched = new int[5];
			duration = 10f;
			AttemptCount = 0;
			Audio.PlaySoundAtTransform("StrikeSound", Module.transform);
			yield return new WaitForSeconds(1f);
			solving = false;
			RareMetal = 3;
			GenerateModule();
		}
    }
	bool CheckAnswer(int[] Indexes)
    {
		//Good luck with the autosolver, Exish.
		for (int i = 0; i < 5; i++)
		{
			yeety[i] = false;
		}
		int[] IgnoreNums = {-1, -1, -1};
		int[] IgnoreNums2Ele3ctricBoogaloo = {-1, -1, -1};
		float[] Heat = { MeteoVals[Indexes[0]][0], MeteoVals[Indexes[1]][0], MeteoVals[Indexes[2]][0], MeteoVals[Indexes[3]][0], MeteoVals[Indexes[4]][0], };
		float[] Mass = { MeteoVals[Indexes[0]][1], MeteoVals[Indexes[1]][1], MeteoVals[Indexes[2]][1], MeteoVals[Indexes[3]][1], MeteoVals[Indexes[4]][1], };
		float[] Volu = { MeteoVals[Indexes[0]][2], MeteoVals[Indexes[1]][2], MeteoVals[Indexes[2]][2], MeteoVals[Indexes[3]][2], MeteoVals[Indexes[4]][2], };
		var Sum = new Vector3(0, 0, 0);
		int bruh = 0;
		for (int a = 0; a < 5; a++)
		{
			float[] vector = MeteoVals[Indexes[a]];
			Sum += new Vector3(vector[0], vector[1], vector[2]);
		}
		switch (RareMetal)
		{
			case 0: yeety[0] = (Sum[2] >= Sum[0] + MaterialThreshold && Sum[0] >= Sum[1] + MaterialThreshold); break;
			case 1: yeety[0] = (Sum[1] >= Sum[2] + MaterialThreshold && Sum[2] >= Sum[0] + MaterialThreshold); break;
			case 2: yeety[0] = (Sum[0] >= Sum[1] + MaterialThreshold && Sum[1] >= Sum[2] + MaterialThreshold); break;
		}

		int[] Wowza = AllIndexes(Heat, Heat.Max()).ToArray();
        if (Wowza.Contains(0))
        {
			IgnoreNums[0] = 0;
			IgnoreNums2Ele3ctricBoogaloo[0] = Indexes[0];
			yeety[1] = true;
        }
		int[] w2owza = AllIndexes(Mass, MaxIgnore(Mass, IgnoreNums)).ToArray();
		if (w2owza.Contains(1))
		{
			IgnoreNums[1] = 1;
			IgnoreNums2Ele3ctricBoogaloo[1] = Indexes[1];
			yeety[2] = true;
		}
		int[] w3owza = AllIndexes(Volu, MaxIgnore(Volu, IgnoreNums)).ToArray();
		if (w3owza.Contains(2))
		{
			IgnoreNums[2] = 2;
			IgnoreNums2Ele3ctricBoogaloo[2] = Indexes[2];
			yeety[3] = true;
		}
		bruh = IgnoreNums[0];
		for (int i = 0; i < 8; i++)
        {
			if (Indexes.Contains(bruh) && !IgnoreNums2Ele3ctricBoogaloo.Contains(bruh))
				break;
			bruh++;
			bruh %= 8;
        }
		Debug.Log(bruh);
		Debug.Log(Indexes[3]);
		yeety[4] = Indexes[3] == bruh;

        if (yeety.All(x => x))
        {
			Debug.LogFormat("[Metalmaking #{0}]: The solution of {1} is valid! Module solved!", _moduleId, Indexes.Select(x=>x+1).Join(", "));
			return true;
        }
		else if (yeety[0])
        {
			Debug.Log(yeety.Join(" "));
			Debug.LogFormat("[Metalmaking #{0}]: The solution of {1} is in the wrong order... :(", _moduleId, Indexes.Select(x => x + 1).Join(", "));
			return false;
		}
        else
        {
			Debug.LogFormat("[Metalmaking #{0}]: The solution of {1} does not sum to an acceptable value...", _moduleId, Indexes.Select(x => x + 1).Join(", "));
			return false;
		}
	}
	float MaxIgnore(float[] Array, int[] Ignore)
    {
		float Maximum = 0;
		for(int i = 0; i < Array.Length; i++)
        {
			if(Array[i] > Maximum && !Ignore.Contains(i))
            {
				Maximum = Array[i];
            }
        }
		return Maximum;
    }
	List<int> AllIndexes(float[] Array, float Number)
    {
        List<int> Index = new List<int>();
        for (int i = 0; i < Array.Length; i++)
        {
			if(Number == Array[i])
            {
				Index.Add(i);
            }
        }
        return Index;
    }
}
