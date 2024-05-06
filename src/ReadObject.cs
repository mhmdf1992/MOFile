namespace MO.MOFile{
    public class ReadObject{
        public ReadObject(long pos, int len){
            _position = pos;
            _length = len;
        }

        private long _position;
        private int _length;
        public long Position => _position;
        public int Length => _length;
    }
}