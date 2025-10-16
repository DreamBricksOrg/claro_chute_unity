

interface IElementState
{
    void OnBegin();
    void OnPrepare();
    void OnPlay();
    void OnReset();
    void OnCancel();
}

interface IInteract
{
    void OnHit();
}
