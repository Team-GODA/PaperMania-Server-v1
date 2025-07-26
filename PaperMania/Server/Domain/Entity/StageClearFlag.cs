namespace Server.Domain.Entity;

public class StageClearFlag
{
    private ulong _clearFlags = 0;
    private const int MaxStageCount = 5;
    
    private int GetStageIndex(int stageNum, int stageSubNum)
    {
        return (stageNum - 1) * MaxStageCount + (stageSubNum - 1);
    }
    
    public bool IsCleared(int stageNum, int stageSubNum)
    {
        int index = GetStageIndex(stageNum, stageSubNum);
        if (index < 0 || index >= 64)
            throw new ArgumentOutOfRangeException(nameof(stageNum));

        return (_clearFlags & (1UL << index)) != 0;
    }
    
    public void MarkCleared(int stageNum, int stageSubNum)
    {
        int index = GetStageIndex(stageNum, stageSubNum);
        if (index < 0 || index >= 64)
            throw new ArgumentOutOfRangeException(nameof(stageNum));

        _clearFlags |= (1UL << index);
    }
    
    public void ClearFlag(int stageNum, int stageSubNum)
    {
        int index = GetStageIndex(stageNum, stageSubNum);
        if (index < 0 || index >= 64)
            throw new ArgumentOutOfRangeException(nameof(stageNum));

        _clearFlags &= ~(1UL << index);
    }
    
    public ulong GetFlags() => _clearFlags;
    public void SetFlags(ulong flags) => _clearFlags = flags;
}