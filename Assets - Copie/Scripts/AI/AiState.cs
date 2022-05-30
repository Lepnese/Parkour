
public enum AiStateId
{
    Idle,
    Shoot,
    Death,
    Pause
}

public interface AiState
{
    AiStateId GetId();
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
    
}