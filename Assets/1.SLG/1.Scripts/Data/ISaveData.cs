public interface ISaveData<T>
{
    T SaveData();
    void LoadData(T data);
}