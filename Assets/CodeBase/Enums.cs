namespace CodeBase
{
    public enum GameState
    {
        NONE,
        START,
        PLAY,
        FINISH,
        GAMEOVER,
        TUTOR,
        COUNTER,
        BIGTUTOR
    }
    

    public enum JumperType
    {
        light,
        medium,
        super,
        rocket,
        bat,
        oldCell
    }
    

    public enum FloorType
    { 
        cells15x15,
        cells30x30,
        cells25x25,
    }

    public enum CellColor
    {
        green,
        blue,
        darkblue, 
        red,
        violet
    }

    public enum CellType
    { 
        simple,
        empty,
        jumper   
    }

    public enum ioPlayerType
    { 
        simple,
        smarter,
        best
    }

}
