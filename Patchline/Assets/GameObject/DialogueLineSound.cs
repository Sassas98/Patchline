using System.Threading;
using TMPro;
using UnityEngine;
using Yarn.Markup;
using Yarn.Unity;

public sealed class DialogueLineSound : ActionMarkupHandler
{
    public AudioSource audioSource;
    public AudioClip lineSound;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    public override void OnPrepareForLine(
        MarkupParseResult line,
        TMP_Text text)
    {
    }

    public override void OnLineDisplayBegin(
        MarkupParseResult line,
        TMP_Text text)
    {
        if (audioSource == null 
            || lineSound == null 
            || audioSource.isPlaying)
        {
            return;
        }

        audioSource.PlayOneShot(lineSound, volume);
    }

    public override YarnTask OnCharacterWillAppear(
        int currentCharacterIndex,
        MarkupParseResult line,
        CancellationToken cancellationToken)
    {
        return YarnTask.CompletedTask;
    }

    public override void OnLineDisplayComplete()
    {
    }

    public override void OnLineWillDismiss()
    {
    }
}