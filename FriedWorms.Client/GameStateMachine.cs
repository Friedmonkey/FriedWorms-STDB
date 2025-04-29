
using Raylib_cs;

namespace FriedWorms.Client;

public enum GameState
{ 
    Idle,
    Reset,
    GenerateTerrain,
    GeneratingTerrain,
    DeployUnits,
    DeployingUnits,
    StartPlay,
    CameraMode,
}
static partial class Program
{
    static GameState CurrentState = GameState.Reset;
    static GameState NextState = GameState.Reset;
    static bool GameIsStable = false;
    static bool PlayerActionComplete = false;
    static bool UserHasControl = true;
    static void ExecuteStateMachine()
    {
        switch (CurrentState)
        {
            case GameState.Idle:
                break;
            case GameState.Reset:
                {
                    UserHasControl = false;
                    NextState = GameState.GenerateTerrain;
                }
                break;
            
            case GameState.GenerateTerrain:
                {
                    UserHasControl = false;
                    CreateMap();
                    LoadBackgrounds();
                    Entities.Clear();
                    NextState = GameState.GeneratingTerrain;
                }
                break;
            
            case GameState.GeneratingTerrain:
                {
                    UserHasControl = false;
                    NextState = GameState.DeployUnits;
                }
                break;
            
            case GameState.DeployUnits:
                {
                    UserHasControl = false;
                    var worm = CreateEntityWorm(new (32f, 1.0f));
                    worm.ShootingAngle = 90;
                    Entities.Add(worm);
                    ControlWorm = worm;
                    CameraTracking = ControlWorm;
                    NextState = GameState.DeployingUnits;
                }
                break;
            
            case GameState.DeployingUnits:
                {
                    UserHasControl = false;
                    if (GameIsStable)
                    {
                        PlayerActionComplete = false;
                        NextState = GameState.StartPlay;
                    }
                }
                break;
            
            case GameState.StartPlay:
                {
                    UserHasControl = true;
                    if (PlayerActionComplete)
                        NextState = GameState.CameraMode;
                }
                break;
            
            case GameState.CameraMode:
                {
                    PlayerActionComplete = false;
                    UserHasControl = false;

                    if (GameIsStable)
                    {
                        CameraTracking = ControlWorm;
                        NextState = GameState.StartPlay;
                    }
                }
                break;
            
            default: throw new Exception("Unknown game state:"+CurrentState);
        }

        CurrentState = NextState;
    }
}
