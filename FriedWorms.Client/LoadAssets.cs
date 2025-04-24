using Raylib_cs;
using SpacetimeDB.Types;
using static Raylib_cs.Raylib;
using static System.Net.Mime.MediaTypeNames;

namespace FriedWorms.Client;

partial class Program
{
    //we need to keep a refference to this so the GC doest throw it away
    static byte[] _musicBytes = null!;
    public static Music music;

    static FriedSoundBlock explosions = null!;

    static void LoadAssets()
    {
        InitAudioDevice();

        (var musicExt, _musicBytes) = ResourceLoader.GetMemoryLoader("Assets/Audio/ザハンド.mp3");
        music = LoadMusicStreamFromMemory(musicExt, _musicBytes);
        SetMusicVolume(music, 0.15f);
        PlayMusicStream(music);

        explosions = new FriedSoundBlock("Assets/Audio/explosion.mp3");

        //var (iconExt, _skyBytes) = ResourceLoader.GetMemoryLoader("Assets/Images/FirePowerupAtlas.png");

        //var skyImage = LoadImageFromMemory(iconExt, _skyBytes);
        //if (skyImage.Data == IntPtr.Zero)
        //{
        //    Console.WriteLine("Image loading failed :(");
        //}
        //ExportImage(skyImage, "test_output.png");

        //ImageResize(skyImage, 108, 1);
        //var skyTexture = LoadTextureFromImage(skyImage);

        //sky = new FriedAnimatedTexure(skyTexture, 108, 1);

        //UnloadImage(skyImage);

    }
    public static Sound LoadSound(string path)
    {
        var (fileExt, fileBytes) = ResourceLoader.GetMemoryLoader(path);
        var wave = Raylib.LoadWaveFromMemory(fileExt, fileBytes);
        var sound = Raylib.LoadSoundFromWave(wave);
        Raylib.UnloadWave(wave);
        return sound;
    }

}
