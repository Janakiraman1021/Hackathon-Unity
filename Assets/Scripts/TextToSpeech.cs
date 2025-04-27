using System.Collections;
using System.Collections.Generic; 
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic; 
using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;

public class TextToSpeech : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public async void Start()
    {
        var Credentials = new BasicAWSCredentials("AKIAUPMYMYKVR74DUQ7L", "CFzevfFoQuwawXoxCftVfUYjyCsBEoP5WWz45MoJ"); // Replace these sample credentials with your own AWS credentials
        var Client = new AmazonPollyClient(Credentials, RegionEndpoint.EUCentral1);

        var request = new SynthesizeSpeechRequest()
        {
            Text = "TESTING OF AWS POLLY FROM UNITY IN NAAN MUDHALVAN CLASS ",
            Engine = Engine.Standard,
            VoiceId = VoiceId.Matthew,
            OutputFormat = OutputFormat.Mp3
        };

        var response = await Client.SynthesizeSpeechAsync(request);

        WriteIntoFile(response.AudioStream);

        using (var www = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.persistentDataPath}/audio.mp3", AudioType.MPEG))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone) await Task.Yield();

            var clip = DownloadHandlerAudioClip.GetContent(www);

            audioSource.clip = clip;
            audioSource.Play();
        }
        
     }
       

    private void WriteIntoFile(Stream stream)
    {
        using (var fileStream = new FileStream(path:$"{Application.persistentDataPath}/audio.mp3", FileMode.Create))
        {
            byte[] buffer = new byte[8 * 1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
        }

    }
}

