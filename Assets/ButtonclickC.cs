using System.Collections;
using System.Collections.Generic;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonclickC : MonoBehaviour, IPointerClickHandler
{
    public AudioSource SourceA;
    Coroutine coroutine;
    // Start is called before the first frame update
    void OnEnable()
    {
        Speaker.Instance.OnSpeakAudioGenerationComplete += speakAudioGenerationCompleteMethod;

    }
    private void OnDisable()
    {
        Speaker.Instance.OnSpeakAudioGenerationComplete -= speakAudioGenerationCompleteMethod;
    }
    private Wrapper currentWrapper;

    private void speakAudioGenerationCompleteMethod(Wrapper wrapper)
    {

        currentWrapper = wrapper;

        Invoke(nameof(speakAudio), 0.1f); //needs a small delay
    }

    private void speakAudio()
    {
        Speaker.Instance.SpeakMarkedWordsWithUID(currentWrapper);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        coroutine=  StartCoroutine(enumerator());
        print("11");
        //Don't speak the text immediately
       
    }
    IEnumerator enumerator() 
    {
        Speaker.Instance.Speak("敖特根巴雅尔识别完毕！", SourceA,
           Speaker.Instance.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.FEMALE, "zh", 0, "zh"),
           false, 1.25f);
        StopCoroutine(coroutine);
        yield return null;
    }
}