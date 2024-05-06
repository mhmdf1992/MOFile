using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MO.MOFile{
    public class FileWR : IFileWR, IStack
    {
        protected Encoding _encoding;
        protected string _path;
        public Encoding Encoding => _encoding;
        public FileInfo FileInfo => new FileInfo(_path);
        public long Size => !FileInfo.Exists ? 0 : FileInfo.Length;
        public FileWR(string path, Encoding textEncoding){
            _path = path;
            _encoding = textEncoding ?? Encoding.UTF8;
            if(!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        public FileWR(string path){
            _path = path;
            _encoding = Encoding.UTF8;
            if(!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        public Stream GetStreamForRead(long startPosition = 0)
        {
            if(Size < startPosition )
                throw new Exception($"File size {Size} is less than requested startPosition {startPosition}");
            if(Size == 0)
                return new MemoryStream();
            try{
                var fstream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                fstream.Seek(startPosition, SeekOrigin.Begin);
                return fstream;
            }catch{throw;}
        }

        public string Read(long startPosition = 0, int? length = null)
        {
            using var stream = GetStreamForRead(startPosition);
            var buffer = new byte[length ?? stream.Length - startPosition];
            stream.Read(buffer, 0, buffer.Length);
            return _encoding.GetString(buffer);
        }

        public IEnumerable<string> Read(IEnumerable<ReadObject> readObjects)
        {
            using var stream = GetStreamForRead();
            foreach(var item in readObjects){
                stream.Seek(item.Position, SeekOrigin.Begin);
                var buffer = new byte[item.Length];
                stream.Read(buffer, 0, item.Length);
                yield return _encoding.GetString(buffer);
            }
        }

        public long Write(string text, long startPosition = 0)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                if(fstream.Length < (int) startPosition)
                    throw new Exception("File length is less than requested start position");
                fstream.Seek(startPosition, SeekOrigin.Begin);
                var bytes = _encoding.GetBytes(text);
                fstream.Write(bytes, 0, bytes.Length);
                return startPosition;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public long WriteBytes(byte[] bytes, long startPosition = 0)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                if(fstream.Length < (int) startPosition)
                    throw new Exception("File length is less than requested start position");
                fstream.Seek(startPosition, SeekOrigin.Begin);
                fstream.Write(bytes, 0, bytes.Length);
                return startPosition;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public long Append(string text)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                var position = fstream.Length;
                fstream.Seek(0, SeekOrigin.End);
                var bytes = _encoding.GetBytes(text);
                fstream.Write(bytes, 0, bytes.Length);
                return position;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public long AppendBytes(byte[] bytes)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                var position = fstream.Length;
                fstream.Seek(0, SeekOrigin.End);
                fstream.Write(bytes, 0, bytes.Length);
                return position;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public long WriteStream(Stream stream, long startPosition = 0)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                if(fstream.Length < (int) startPosition)
                    throw new Exception("File length is less than requested start position");
                fstream.Seek(startPosition, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fstream);
                return startPosition;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
                stream?.Dispose();
            }
        }

        public long AppendStream(Stream stream)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                var position = fstream.Length;
                fstream.Seek(0, SeekOrigin.End);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fstream);
                return position;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
                stream?.Dispose();
            }
        }

        public void Clear()
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                fstream.SetLength(0);
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }
    
        protected bool IsBusy(out Stream stream)
        {
            stream = default;
            try
            {
                stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (IOException)
            {
                stream?.Dispose();
                return true;
            }
            return false;
        }

        public IEnumerable<KeyValuePair<string, ReadObject>> AppendList(KeyValuePair<string, string>[] list)
        {
            var res = new List<KeyValuePair<string, ReadObject>>();
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                for(int i = 0; i < list.Length; i ++){
                    var position = fstream.Length;
                    fstream.Seek(0, SeekOrigin.End);
                    var item = list[i];
                    var bytes = _encoding.GetBytes(item.Value);
                    fstream.Write(bytes, 0, bytes.Length);
                    res.Add(new KeyValuePair<string, ReadObject>(item.Key, new ReadObject(position, bytes.Length)));
                }
                return res;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public IEnumerable<KeyValuePair<object, ReadObject>> AppendList(KeyValuePair<object, string>[] list)
        {
            var res = new List<KeyValuePair<object, ReadObject>>();
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                for(int i = 0; i < list.Length; i ++){
                    var position = fstream.Length;
                    fstream.Seek(0, SeekOrigin.End);
                    var item = list[i];
                    var bytes = _encoding.GetBytes(item.Value);
                    fstream.Write(bytes, 0, bytes.Length);
                    res.Add(new KeyValuePair<object, ReadObject>(item.Key, new ReadObject(position, bytes.Length)));
                }
                return res;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public void AppendBytes(byte[][] list){
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                for(int i = 0; i < list.Length; i ++){
                    var position = fstream.Length;
                    fstream.Seek(0, SeekOrigin.End);
                    var bytes = list[i];
                    fstream.Write(bytes, 0, bytes.Length);
                }
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public byte[] Pop(int length)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                if(fstream.Length == 0)
                    return null;
                fstream.Seek(length * -1, SeekOrigin.End);
                var buffer = new byte[length];
                fstream.Read(buffer, 0, length);
                fstream.SetLength(fstream.Length - length);
                return buffer;
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }

        public long Push(byte[] bytes)
        {
            return AppendBytes(bytes);
        }

        public T OpenStreamForWrite<T>(Func<Stream,T> func)
        {
            Stream fstream;
            while(IsBusy(out fstream)){
                Thread.Sleep(1);
            }
            try{
                if(func == null)
                    return default;
                return func(fstream);
            }catch{
                throw;
            }finally{
                fstream?.Dispose();
            }
        }
    }
}