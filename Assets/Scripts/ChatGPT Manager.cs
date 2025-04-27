using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using System.IO;
using System.Threading.Tasks;

public class ChatGPTManager : MonoBehaviour
{
    [System.Serializable]
    private class ChatResponse
    {
        public string conversation_id;
        public string response;
    }

    public Text responseText;
    public AudioSource audioSource; 

    private void WriteIntoFile(Stream stream) 
    {
        using (var fileStream = new FileStream($"{Application.persistentDataPath}/audio.mp3", FileMode.Create, FileAccess.Write))
        {
            stream.CopyTo(fileStream);
        }
    }

    public async void AskChatGPT(string newText)
    {
        string query = UnityWebRequest.EscapeURL(newText);
        string conversationId = "your_conversation_id";

        string url = $"http://localhost:5000/chat?query={query}&conversation_id={conversationId}";

        UnityWebRequest chatRequest = UnityWebRequest.Get(url); 
        chatRequest.downloadHandler = new DownloadHandlerBuffer();
        chatRequest.SetRequestHeader("Content-Type", "application/json");

        var chatOperation = chatRequest.SendWebRequest(); 

        while (!chatOperation.isDone)
            await System.Threading.Tasks.Task.Yield();

        if (chatRequest.result == UnityWebRequest.Result.Success)
        {
            string json = chatRequest.downloadHandler.text;
            ChatResponse chatResponse = JsonUtility.FromJson<ChatResponse>(json);

            Debug.Log("Response: " + chatResponse.response);

            if (responseText != null)
            {
                responseText.text = chatResponse.response;
                var Credentials = new BasicAWSCredentials("AKIAUPMYMYKVR74DUQ7L", "CFzevfFoQuwawXoxCftVfUYjyCsBEoP5WWz45MoJ");
                var Client = new AmazonPollyClient(Credentials, RegionEndpoint.EUCentral1);

                var pollyRequest = new SynthesizeSpeechRequest() 
                {
                    Text = responseText.text,
                    Engine = Engine.Standard,
                    VoiceId = SceneManager.GetActiveScene().name == "ChatG" ? VoiceId.Raveena : VoiceId.Matthew,
                    OutputFormat = OutputFormat.Mp3
                };

                var pollyResponse = await Client.SynthesizeSpeechAsync(pollyRequest);

                WriteIntoFile(pollyResponse.AudioStream);

                using (var audioRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.persistentDataPath}/audio.mp3", AudioType.MPEG))
                {
                    var audioOperation = audioRequest.SendWebRequest(); 

                    while (!audioOperation.isDone) await Task.Yield();

                    var clip = DownloadHandlerAudioClip.GetContent(audioRequest);

                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
        }
        else
        {
            Debug.LogError("Error: " + chatRequest.error);
        }
    }

    public async void resspeak()
    {
        var Credentials = new BasicAWSCredentials("AKIAUPMYMYKVR74DUQ7L", "CFzevfFoQuwawXoxCftVfUYjyCsBEoP5WWz45MoJ");
        var Client = new AmazonPollyClient(Credentials, RegionEndpoint.EUCentral1);

        var pollyRequest = new SynthesizeSpeechRequest() 
        {
            Text = "TESTING OF AWS POLLY FROM UNITY IN NAAN MUDHALVAN CLASS ",
            Engine = Engine.Standard,
            VoiceId = VoiceId.Matthew,
            OutputFormat = OutputFormat.Mp3
        };

        var pollyResponse = await Client.SynthesizeSpeechAsync(pollyRequest);

        WriteIntoFile(pollyResponse.AudioStream);

        using (var audioRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.persistentDataPath}/audio.mp3", AudioType.MPEG))
        {
            var audioOperation = audioRequest.SendWebRequest(); 

            while (!audioOperation.isDone) await Task.Yield();

            var clip = DownloadHandlerAudioClip.GetContent(audioRequest);

            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
    }
}
