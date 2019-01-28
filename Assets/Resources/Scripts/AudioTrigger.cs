using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    private Player myPlayer;
    private AudioManager myAudioManager;
    private AudioSource myHeartBeatSource;
    private bool isMyAudioManagerInitialized;
    private const string mPlayerBreathUnderWater = "PlayerBreathUnderWater";
    private const string mPlayerBreathAboveWater = "PlayerBreathAboveWater";
    private const string mEnvironmentUnderWaterNoise = "EnvironmentUnderWaterNoise";
    private const string mEnvironmentAboveWaterNoise = "EnvironmentAboveWaterNoise";
    private const string mPlayerheartBeat = "PlayerHeartBeat";

    private OxygenMgt myOxygenMgt;


    // Start is called before the first frame update
    void Start()
    {
        myPlayer = GetComponent<Player>();
        myOxygenMgt = GetComponent<OxygenMgt>();
        myAudioManager = GameObject.FindObjectOfType<AudioManager>();
        isMyAudioManagerInitialized = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMyAudioManagerInitialized)
        {
            myAudioManager.PlaySound(mPlayerBreathUnderWater);
            isMyAudioManagerInitialized = true;
            myHeartBeatSource = myAudioManager.GetSource(mPlayerheartBeat);
        }
        
        if (transform.position.y + transform.up.y >= -0.5f)
        {
            myAudioManager.StopSound(mPlayerBreathUnderWater);
            myAudioManager.StopSound(mEnvironmentUnderWaterNoise);
            myAudioManager.PlaySound(mPlayerBreathAboveWater);
            myAudioManager.PlaySound(mEnvironmentAboveWaterNoise);
        }
        else
        {
            myAudioManager.PlaySound(mPlayerBreathUnderWater);
            myAudioManager.PlaySound(mEnvironmentUnderWaterNoise);
            //myAudioManager.StopSound(mPlayerBreathAboveWater);
            myAudioManager.StopSound(mEnvironmentAboveWaterNoise);
        }

        myHeartBeatSource.volume = 0.1f + ((float)myOxygenMgt.MaxOxygen-(float)myOxygenMgt.CurrentOxygen)/(float)myOxygenMgt.MaxOxygen;
        myHeartBeatSource.pitch = 1.0f + ((float)myOxygenMgt.MaxOxygen - (float)myOxygenMgt.CurrentOxygen) / (float)myOxygenMgt.MaxOxygen;
        //myAudioManager.PlaySound(mPlayerheartBeat);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        switch (collision.gameObject.tag)
        {
            
            case "Rock":
                myAudioManager.PlaySound("PlayerHeadHitting");
                break;
            case "Fish":
                myAudioManager.PlaySound("PlayerPain");
                break;
            default:
                break;
               
        }
    }
}
