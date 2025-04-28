using Raylib_cs;
using SpacetimeDB.Types;
using static Raylib_cs.Raylib;

namespace FriedWorms.Client;

partial class Program
{
    //we need to keep a refference to this so the GC doest throw it away
    static byte[] _musicBytes = null!;
    public static Music music;

    static FriedSoundBlock explosions = null!;

    static Image spaceImage;
    static Texture2D spaceTexure;

    static Image cloudImage;
    static Texture2D cloudTexure;

    static Image skyImage;
    static Texture2D skyTexure;
    static FriedAnimatedTexure skygif;
    static FriedAnimatedTexure spacegif;

    static void LoadAssets()
    {
        InitAudioDevice();

        (var musicExt, _musicBytes) = ResourceLoader.GetMemoryLoader("Assets/Audio/main3.mp3");
        music = LoadMusicStreamFromMemory(musicExt, _musicBytes);
        SetMusicVolume(music, 0.15f);
        PlayMusicStream(music);

        explosions = new FriedSoundBlock("Assets/Audio/explode1.mp3");

        var (spaceExt, _spaceBytes) = ResourceLoader.GetMemoryLoader("Assets/Images/space.jpg");
        spaceImage = LoadImageFromMemory(spaceExt, _spaceBytes);
        ImageResize(ref spaceImage, MapWidth * 10, MapHeight * 10);
        spaceTexure = LoadTextureFromImage(spaceImage);
        spacegif = new FriedAnimatedTexure(spaceTexure, 1, 1);

        var (cloudExt, _cloudBytes) = ResourceLoader.GetMemoryLoader("Assets/Images/cloud.png");
        cloudImage = LoadImageFromMemory(cloudExt, _cloudBytes);
        ImageResize(ref cloudImage, 10 * BackgroundScale, 10 * BackgroundScale);
        cloudTexure = LoadTextureFromImage(cloudImage);

        var (skyExt, _skyBytes) = ResourceLoader.GetMemoryLoader("Assets/Images/skyatlas.png");
        skyImage = LoadImageFromMemory(skyExt, _skyBytes);
        ImageResize(ref skyImage, TARGET_WIDTH, TARGET_HEIGHT*108);
        skyTexure = LoadTextureFromImage(skyImage);
        skygif = new FriedAnimatedTexure(skyTexure, 1, 108);

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
