using System;
using Assets.Scripts.Common;
using Assets.Sources.Graphics.Movie;
using Assets.Sources.Scripts.Common;
using UnityEngine;

public class MovieTestScript : MonoBehaviour
{
	private void OnGUI()
	{
		Rect fullscreenRect = DebugGuiSkin.GetFullscreenRect();
		DebugGuiSkin.ApplySkin();
		GUILayout.BeginArea(fullscreenRect);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button("Back", new GUILayoutOption[0]))
		{
			SceneDirector.Replace("MainMenu", SceneTransition.FadeOutToBlack_FadeIn, true);
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Debug", new GUILayoutOption[0]))
		{
			this.isEnableSoundController = !this.isEnableSoundController;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.FlexibleSpace();
		if (this.isEnableSoundController)
		{
			GUILayout.BeginHorizontal("box", new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("File: " + MovieTestScript.MovieFiles[this.currMovieIndex], new GUILayoutOption[0]);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.DrawControlMenu();
			GUILayout.EndVertical();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void DrawControlMenu()
	{
		if (GUILayout.Button("|<", new GUILayoutOption[0]))
		{
			this.PlayPreviousMovie();
		}
		if (GUILayout.Button((!this.isPlaying) ? "Play" : "Pause", new GUILayoutOption[0]))
		{
			if (!this.isPlaying)
			{
				this.movieMaterial.Resume();
			}
			else
			{
				this.movieMaterial.Pause();
			}
			this.isPlaying = !this.isPlaying;
		}
		String text = (this.movieMaterial.FastForward != MovieMaterial.FastForwardMode.HighSpeed) ? "Normal" : "High Speed";
		if (GUILayout.Button(text, new GUILayoutOption[0]))
		{
			if (this.movieMaterial.FastForward == MovieMaterial.FastForwardMode.HighSpeed)
			{
				this.movieMaterial.FastForward = MovieMaterial.FastForwardMode.Normal;
			}
			else if (this.movieMaterial.FastForward == MovieMaterial.FastForwardMode.Normal)
			{
				this.movieMaterial.FastForward = MovieMaterial.FastForwardMode.HighSpeed;
			}
		}
		if (GUILayout.Button(">|", new GUILayoutOption[0]))
		{
			this.PlayNextMovie();
		}
	}

	public void Start()
	{
		this.movieMaterial = MovieMaterial.New(base.gameObject.AddComponent<MovieMaterialProcessor>());
		SoundLib.LoadMovieResources("MovieAudio/", MovieTestScript.MovieFiles);
		GameObject gameObject = GameObject.Find("Plane");
		gameObject.GetComponent<Renderer>().material = this.movieMaterial.Material;
		gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, MovieMaterial.ScaleVector);
		MovieMaterial movieMaterial = this.movieMaterial;
		movieMaterial.OnFinished = (Action)Delegate.Combine(movieMaterial.OnFinished, new Action(delegate
		{
			this.PlayNextMovie();
		}));
		this.PlayNextMovie();
	}

	public void OnDestroy()
	{
		if (this.movieMaterial != null)
		{
			this.movieMaterial.Destroy();
		}
		SoundLib.UnloadMovieResources();
	}

	private void PlayNextMovie()
	{
		this.currMovieIndex = (this.currMovieIndex + 1) % (Int32)MovieTestScript.MovieFiles.Length;
		this.movieMaterial.Load(MovieTestScript.MovieFiles[this.currMovieIndex]);
		this.movieMaterial.Play();
	}

	private void PlayPreviousMovie()
	{
		this.currMovieIndex--;
		if (this.currMovieIndex < 0)
		{
			this.currMovieIndex = (Int32)MovieTestScript.MovieFiles.Length - 1;
		}
		this.movieMaterial.Load(MovieTestScript.MovieFiles[this.currMovieIndex]);
		this.movieMaterial.Play();
	}

	private static readonly String[] MovieFiles = new String[]
	{
		"FMV001",
		"FMV002",
		"FMV003",
		"FMV004",
		"FMV005",
		"FMV006A",
		"FMV006B",
		"FMV008",
		"FMV011",
		"FMV012",
		"FMV013",
		"FMV015",
		"FMV016",
		"FMV017",
		"FMV019",
		"FMV021",
		"FMV023",
		"FMV024",
		"FMV027",
		"FMV029",
		"FMV031",
		"FMV032",
		"FMV033",
		"FMV034",
		"FMV035",
		"FMV036",
		"FMV038",
		"FMV039",
		"FMV040",
		"FMV042",
		"FMV045",
		"FMV046",
		"FMV052",
		"FMV053",
		"FMV055A",
		"FMV055B",
		"FMV055C",
		"FMV055D",
		"FMV056",
		"FMV060",
		"mbg101",
		"mbg102",
		"mbg103",
		"mbg105",
		"mbg106",
		"mbg107",
		"mbg108",
		"mbg109",
		"mbg110",
		"mbg111",
		"mbg112",
		"mbg113",
		"mbg114",
		"mbg115",
		"mbg116",
		"mbg117",
		"mbg118",
		"mbg119"
	};

	private MovieMaterial movieMaterial;

	private Int32 currMovieIndex = -1;

	private Boolean isEnableSoundController = true;

	private Boolean isPlaying = true;
}
