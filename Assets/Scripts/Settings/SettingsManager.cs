using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Application.runInBackground = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save(Settings config)
    {
        string json = JsonUtility.ToJson(config);

        File.WriteAllText(Application.persistentDataPath + "/settings.json", json);
    }

    public Settings Load()
    {
        if (File.Exists(Application.persistentDataPath + "/settings.json"))
        {
            string loaded = File.ReadAllText(Application.persistentDataPath + "/settings.json");

            Settings loadedSettings = JsonUtility.FromJson<Settings>(loaded);

            GetSounds().SetVolume(loadedSettings);

            return loadedSettings;
        }
        else
        {
            Settings config = new Settings
            {
                fullscreen = true,

                width = 1280,
                height = 720,

                vibration = true,

                musicVolume = 0,
                SFXVolume = 0
            };

            Save(config);

            string loaded = File.ReadAllText(Application.persistentDataPath + "/settings.json");

            Settings loadedSettings = JsonUtility.FromJson<Settings>(loaded);

            GetSounds().SetVolume(loadedSettings);

            return loadedSettings;
        }
    }

    public SoundSettings GetSounds()
    {
        SoundSettings sounds = GetComponent<SoundSettings>();

        return sounds;
    }
}
