using Raylib_cs;
using System;
namespace FriedWorms.Client;

//allow for playing sound multiple times at once
public class FriedSoundBlock
{
    Sound[] sounds;
    int maxSounds;
    int currentSound;
    public FriedSoundBlock(string path, int maxSounds = 10, float volume = 0.5f)
    { 
        sounds = new Sound[maxSounds];
        sounds[0] = Program.LoadSound(path);
        Raylib.SetSoundVolume(sounds[0], volume);

        currentSound = 0;
        this.maxSounds = maxSounds;

        for (int i = 1; i < maxSounds; i++)
        {
            sounds[i] = Raylib.LoadSoundAlias(sounds[0]);        // Load an alias of the sound into slots 1-9. These do not own the sound data, but can be played
            Raylib.SetSoundVolume(sounds[i], volume);
        }
    }

    public void Play()
    {
        Raylib.PlaySound(sounds[currentSound]);            // play the next open sound slot
        currentSound++;                                 // increment the sound slot
        if (currentSound >= maxSounds)                 // if the sound slot is out of bounds, go back to 0
            currentSound = 0;

        // Note: a better way would be to look at the list for the first sound that is not playing and use that slot
    }

    ~FriedSoundBlock()
    {
        for (int i = 1; i < maxSounds; i++)
            Raylib.UnloadSoundAlias(sounds[i]);     // Unload sound aliases
        Raylib.UnloadSound(sounds[0]);              // Unload source sound data
    }
}
