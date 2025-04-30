using static Module;

namespace SpacetimeDB;

public static partial class Module
{
    static bool GameIsStable = false;
    static bool PlayerActionComplete = false;
    static bool UserHasControl = true;
    static void ExecuteStateMachine(ReducerContext ctx, Config config)
    {
        switch (config.CurrentState)
        {
            case GameState.Idle:
                break;
            case GameState.Reset:
                {
                    UserHasControl = false;
                    config.NextState = GameState.GenerateTerrain;
                }
                break;
            
            case GameState.GenerateTerrain:
                {
                    UserHasControl = false;
                    CreateMap(config);
                    ClearEntities(ctx);
                    //Entities = gameManager.Conn.Db.Entities.Iter().ToList();
                    config.NextState = GameState.GeneratingTerrain;
                }
                break;
            
            case GameState.GeneratingTerrain:
                {
                    UserHasControl = false;
                    config.NextState = GameState.DeployUnits;
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
                    config.NextState = GameState.DeployingUnits;
                }
                break;
            
            case GameState.DeployingUnits:
                {
                    UserHasControl = false;
                    if (GameIsStable)
                    {
                        PlayerActionComplete = false;
                        config.NextState = GameState.StartPlay;
                    }
                }
                break;
            
            case GameState.StartPlay:
                {
                    UserHasControl = true;
                    if (PlayerActionComplete)
                        config.NextState = GameState.CameraMode;
                }
                break;
            
            case GameState.CameraMode:
                {
                    PlayerActionComplete = false;
                    UserHasControl = false;

                    if (GameIsStable)
                    {
                        config.CameraTrackingId = config.ControlWormId;
                        config.NextState = GameState.StartPlay;
                    }
                }
                break;
            
            default: throw new Exception("Unknown game state:"+config.CurrentState);
        }

        config.CurrentState = config.NextState;
    }
}
