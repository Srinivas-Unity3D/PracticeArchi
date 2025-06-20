namespace CardGame.Core.SaveLoad
{
    public interface ISaveLoadSystem
    {
        void SaveGame(Data.GameData data);
        Data.GameData LoadGame();
        bool SaveExists();
        void DeleteSaveData();
    }
} 