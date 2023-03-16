using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
public class VoiceController : MonoBehaviour
{
    ///语音队列
    public Queue voiceQueue = new Queue();
    /// <summary>
    /// 语音功能
    /// </summary>
    private Coroutine coroutine;
    private AudioSource audioSource;
    private Wrapper currentWrapper;
    // Start is called before the first frame update
    private void Awake()
    {
        //coroutine = StartCoroutine(voiceCoroutine());
        audioSource = GameObject.FindGameObjectWithTag("Voice").GetComponent<AudioSource>();
        Speaker.Instance.OnSpeakAudioGenerationComplete += speakAudioGenerationCompleteMethod;
        Debug.Log(audioSource);
      
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 语音提示相关函数
    /// </summary>
    private void speakAudio()
    {
        Speaker.Instance.SpeakMarkedWordsWithUID(currentWrapper);
    }
    private void speakAudioGenerationCompleteMethod(Wrapper wrapper)
    {

        currentWrapper = wrapper;

        Invoke(nameof(speakAudio), 0.1f); //needs a small delay
    }
 
    private void OnDestroy()
    {
        voiceQueue.Clear();
        Speaker.Instance.OnSpeakAudioGenerationComplete -= speakAudioGenerationCompleteMethod;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
   /// <summary>
   /// 关闭声音协程
   /// </summary>
    public void stopVoiceCoroutine()
    {
        
        if (coroutine!=null)
        {
            Debug.Log("语音关！");
            StopCoroutine(coroutine);
            coroutine = null;
            voiceQueue.Clear();
        }
    }

    public void startVoiceCoroutine() 
    {
        Debug.Log("语音开启");
        if (coroutine==null)
        {
            coroutine = StartCoroutine(voiceCoroutine());
        }
        
    }
    IEnumerator voiceCoroutine()
    {
        while (true)
        {
         
            while (voiceQueue.Count > 0)
            {
                string currentSpeak = (string)voiceQueue.Dequeue();
                Speaker.Instance.Speak(currentSpeak, audioSource,
                       Speaker.Instance.VoiceForGender(Crosstales.RTVoice.Model.Enum.Gender.FEMALE, "zh", 0, "zh"),
                false, 1.2f);
                yield return new WaitForSeconds(3f);
            }
            yield return new WaitForSeconds(1.5f);
        }

    }
}
