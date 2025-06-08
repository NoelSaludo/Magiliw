using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DialogueTypewriter))]
public class SoundEffectController : MonoBehaviour
{
    [System.Serializable]
    public class SoundEffect
    {
        [Tooltip("At which dialogue segment this sound effect should trigger")]
        public int segmentIndex;

        [Tooltip("AudioSources to start playing when this segment is active")]
        public List<AudioSource> startSources = new List<AudioSource>();

        [Tooltip("AudioSources to stop playing when this segment is active")]
        public List<AudioSource> stopSources = new List<AudioSource>();
    }

    [Header("Sound Effects")]
    public List<SoundEffect> soundEffects = new List<SoundEffect>();

    private DialogueTypewriter dialogueTypewriter;
    private int lastSegmentIndex = -1;

    void Awake()
    {
        dialogueTypewriter = GetComponent<DialogueTypewriter>();
        if (dialogueTypewriter == null)
        {
            Debug.LogError("SoundEffectController: DialogueTypewriter component not found on the same GameObject.");
            enabled = false;
        }
    }

    void Update()
    {
        if (dialogueTypewriter == null || !dialogueTypewriter.IsTyping)
            return;

        int currentIndex = dialogueTypewriter.CurrentSegmentIndex;

        if (currentIndex != lastSegmentIndex)
        {
            ApplySoundEffectsForSegment(currentIndex);
            lastSegmentIndex = currentIndex;
        }
    }

    private void ApplySoundEffectsForSegment(int segmentIndex)
    {
        foreach (SoundEffect effect in soundEffects)
        {
            if (effect.segmentIndex == segmentIndex)
            {
                // Stop specified sources
                foreach (AudioSource source in effect.stopSources)
                {
                    if (source != null && source.isPlaying)
                        source.Stop();
                }

                // Start specified sources
                foreach (AudioSource source in effect.startSources)
                {
                    if (source != null && !source.isPlaying)
                        source.Play();
                }
            }
        }
    }
}
