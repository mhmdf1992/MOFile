using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MO.MOFile{
    public interface IFileWR
    {
        string Read(long startPosition = 0, int? length = null);
        IEnumerable<string> Read(IEnumerable<ReadObject> readObjects);
        Stream GetStreamForRead(long startPosition = 0);
        T OpenStreamForWrite<T>(Func<Stream,T> func);
        long Write(string text, long startPosition = 0);
        long WriteBytes(byte[] bytes, long startPosition = 0);
        long Append(string text);
        IEnumerable<KeyValuePair<string,ReadObject>> AppendList(KeyValuePair<string, string>[] list);
        IEnumerable<KeyValuePair<object,ReadObject>> AppendList(KeyValuePair<object, string>[] list);
        void AppendBytes(byte[][] list);
        long WriteStream(Stream stream, long startPosition = 0);
        long AppendStream(Stream stream);
        long AppendBytes(byte[] bytes);
        void Clear();
        FileInfo FileInfo {get;}
        long Size {get;}
        Encoding Encoding {get;}
    }
}

