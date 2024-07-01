using System;
using System.Collections.Generic;

public class SdLibSoundProfileStateGraph
{
    public void Add(SdLibSoundProfileStateGraph.TransitionDelegate transition, SoundProfileState currentState, SoundProfileState nextState)
    {
        if (this.transitionDictionary.ContainsKey(transition))
        {
            this.transitionDictionary[transition].Add(currentState, nextState);
        }
        else
        {
            Dictionary<SoundProfileState, SoundProfileState> dictionary = new Dictionary<SoundProfileState, SoundProfileState>();
            dictionary.Add(currentState, nextState);
            this.transitionDictionary.Add(transition, dictionary);
        }
    }

    public Int32 Transition(SoundProfile soundProfile, SdLibSoundProfileStateGraph.TransitionDelegate transition)
    {
        if (!this.transitionDictionary.ContainsKey(transition))
        {
            SoundLib.Log("Transition not found");
            return 1;
        }
        Dictionary<SoundProfileState, SoundProfileState> dictionary = this.transitionDictionary[transition];
        if (soundProfile == null)
        {
            SoundLib.Log("Sound profile is null");
            return 1;
        }
        if (!dictionary.ContainsKey(soundProfile.SoundProfileState))
        {
            SoundLib.Log("State: " + soundProfile.SoundProfileState + " does not exist for requested transition.");
            return 1;
        }
        transition(soundProfile);
        soundProfile.SoundProfileState = dictionary[soundProfile.SoundProfileState];
        return 0;
    }

    private Dictionary<SdLibSoundProfileStateGraph.TransitionDelegate, Dictionary<SoundProfileState, SoundProfileState>> transitionDictionary = new Dictionary<SdLibSoundProfileStateGraph.TransitionDelegate, Dictionary<SoundProfileState, SoundProfileState>>();

    public delegate void TransitionDelegate(SoundProfile soundProfile);
}
