using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Umbra.Data;
using System.Linq;

namespace Umbra.Managers {

	public class MusicManager : Singleton<MusicManager> {
	    protected MusicManager() { }

	    public AudioSource audioS;
	    public AudioClip[] clips;
	    public Dictionary<string, List<int>> playlists;                                                        // tracks to loop through, defaulted to every track
	    public string currentPlaylist;

	    public int currAudio;
	    public bool notPlaying;
	    public bool looping;

		// Use this for initialization
		void Start () {
		    audioS = MusicManager.Instance.GetOrAddComponent<AudioSource>();
			audioS.volume = GameStateManager.Instance.settings.musicVolume;
	        audioS.playOnAwake = false;
	        clips = Resources.LoadAll<AudioClip>("Music");

	        // playlists
	        playlists = new Dictionary<string, List<int>>();
	        playlists["Menu"] = new List<int> { 7, 8, 9 };
	        playlists["StarMap"] = new List<int> { 1, 2 };
	        
	        currentPlaylist = "Default";
			playlists ["Default"] = Enumerable.Range (0, clips.Length).ToList();

	        currAudio = 0;
	        notPlaying = true;
	        looping = false;   
		}

	    // setup and begin playlist
	    public void setPlaylist(string p) {
	        if (currentPlaylist != p)
	        {
	            Stop();
	            currentPlaylist = p;
	            Begin();
	        }
	    }

	    // begin looping music
	    public void Begin()
	    {
	        if (clips.Length > 0 && notPlaying)
	        {
	            looping = true;
	            currAudio = 0;
	            playMusic(playlists[currentPlaylist][0]);
	        }
	    }

	    // stop looping music
	    public void Stop()
	    {
	        looping = false;
	        notPlaying = true;
	        audioS.Stop();
	    }

	    // play a specific track in the list of tracks
	    void playMusic(int m)
	    {
	        audioS.clip = clips[m];
	        audioS.Play();
	        notPlaying = false;
	        Debug.Log("[MusicManager] track " + m + " being played.");
	        StartCoroutine("musicWait");                                // call wait
	    }

	    IEnumerator musicWait()
	    {
	        yield return new WaitForSeconds(audioS.clip.length);        // wait until clip is finished
	        if (looping) currAudio = (currAudio > playlists[currentPlaylist].Count-1) ? 0 : currAudio + 1;
	        notPlaying = true;

	        if (looping)
	        {
	            playMusic(playlists[currentPlaylist][currAudio]);
	        }

	    }
	}
}