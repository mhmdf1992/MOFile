namespace MO.MOFile{
    public interface IStack{
        byte[] Pop(int length);
        long Push(byte[] bytes);
    }
}