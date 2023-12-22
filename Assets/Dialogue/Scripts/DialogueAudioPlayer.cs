using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DialogueAudioPlayer : MonoBehaviour
{
    public void Initialize()
    {
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip != null)
        {
            if (ServiceManager.Instance != null && ServiceManager.Instance.GameAudio != null)
            {
                ServiceManager.Instance.GameAudio.StopSFXAudio();
                ServiceManager.Instance.GameAudio.Play(clip);
            }
        }
    }

    private IEnumerator PlayAndWaitForAudio(AudioClip clip)
    {
        if (clip != null)
        {
            PlayAudio(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }

    public IEnumerator PlayDialogueAudio(AudioClip clip)
    {
        yield return PlayAndWaitForAudio(clip);
    }

    public IEnumerator PlayChoiceAudio(AudioClip clip)
    {
        yield return PlayAndWaitForAudio(clip);
    }

    public void StopDialogueAudio()
    {
        if (ServiceManager.Instance != null && ServiceManager.Instance.GameAudio != null)
        {
            ServiceManager.Instance.GameAudio.StopSFXAudio();
        }
    }

}
