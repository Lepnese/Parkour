public static class Tags {
    public const string Player  = "Player";
    public const string Ground = "Ground";
    public const string ClimbArea = "ClimbArea";
    public const string Climbable = "Climbable";
    public const string DefaultClimbable = "Default Climbable";
}
 
public enum Layers {
    Default = 0,
    TransparentFX = 1,
    IgnoreRaycast = 2,
    Water = 4,
    UI = 5,
    Ground = 6,
    Body = 7,
    Grab = 8,
    PhysicsHands = 9,
    ClimbPoint = 10,
    Player  = 11,
    Grappable = 12
}