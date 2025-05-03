using FriedWorms.Common;
using static Module;

namespace SpacetimeDB;

public static partial class Module
{
    static bool GameIsStable = false;
    static bool PlayerActionComplete = false;
    static bool UserHasControl = true;
    static void ExecuteStateMachine(ReducerContext ctx, Config config, Game game)
    {
        switch ((GameState)game.CurrentState)
        {
            case GameState.Idle:
                break;
            case GameState.Reset:
                {
                    UserHasControl = false;
                    game.NextState = (byte)GameState.GenerateTerrain;
                }
                break;
            
            case GameState.GenerateTerrain:
                {
                    UserHasControl = false;
                    CreateMap(ctx);
                    ClearEntities(ctx);
                    //Entities = gameManager.Conn.Db.Entities.Iter().ToList();
                    game.NextState = (byte)GameState.GeneratingTerrain;
                }
                break;
            
            case GameState.GeneratingTerrain:
                {
                    UserHasControl = false;
                    game.NextState = (byte)GameState.DeployUnits;
                }
                break;
            
            case GameState.DeployUnits:
                {
                    UserHasControl = false;
                    var worm = CreateEntityWorm(new (100f, 1.0f));
                    worm.ShootingAngle = 90;
                    ctx.Db.Entities.Insert(worm);
                    config.ControlWormId = worm.Id;
                    config.CameraTrackingId = config.ControlWormId;
                    game.NextState = (byte)GameState.DeployingUnits;
                }
                break;
            
            case GameState.DeployingUnits:
                {
                    UserHasControl = false;
                    if (GameIsStable)
                    {
                        PlayerActionComplete = false;
                        game.NextState = (byte)GameState.StartPlay;
                    }
                }
                break;
            
            case GameState.StartPlay:
                {
                    UserHasControl = true;
                    if (PlayerActionComplete)
                        game.NextState = (byte)GameState.CameraMode;
                }
                break;
            
            case GameState.CameraMode:
                {
                    PlayerActionComplete = false;
                    UserHasControl = false;

                    if (GameIsStable)
                    {
                        config.CameraTrackingId = config.ControlWormId;
                        game.NextState = (byte)GameState.StartPlay;
                    }
                }
                break;
            
            default: throw new Exception("Unknown game state:"+game.CurrentState);
        }

        game.CurrentState = game.NextState;
    }
}
