using System.Text;

namespace MOFile.Tests;

public class FileWRTests
{
    IFileWR _fileWR;
    public FileWRTests(){
        _fileWR = new FileWR(Path.Combine(Directory.GetCurrentDirectory(),"test.txt"));
    }
    [Theory]
    [InlineData("hello world")]
    [InlineData("lorem ipsum")]
    [InlineData("banananana")]
    public void WriteReadTest_WriteReturnPosition_ReadPositionEqualsInputText(string input)
    {
        var position = _fileWR.Write(input);
        var res = _fileWR.Read(position, _fileWR.Encoding.GetByteCount(input));
        Assert.Equal(input, res);
    }

    [Fact]
    public void Append_ConcurrentAppend_AllInputsExistsInRead(){
        var arr = new string[]{
            "hello",
            "world",
            "test",
            "me",
            "I"
        };
        var tasks = arr.Select(x => Task.Run(() => _fileWR.Append(x)));
        var res = Task.WhenAll(tasks).Result;
        var all = _fileWR.Read();
        foreach(var item in arr){
            Assert.Contains(item, all);
        }
    }
    
    // [Theory]
    // [InlineData("bahama")]
    // [InlineData("mama")]
    // [InlineData("pijama")]
    // [InlineData("didi")]
    // [InlineData("baba")]
    // public void Append_RetrunedPositionEqualsFindTextAppended(string input)
    // {
    //     var pos = _fileWR.Append(input.ToUpper());
    //     var positions =  _fileWR.Find(input.ToLower(), true);
    //     Assert.Contains(pos, positions);
    // }

    [Fact]
    public void Clear_FileSizeEqualsZero()
    {
        _fileWR.Clear();
        Assert.Equal(0, _fileWR.Size);
    }

    [Fact]
    public void AppendList_ReadReturnedReadObjResultEqualsInputValue()
    {
        _fileWR = new FileWR(Path.Combine(Directory.GetCurrentDirectory(),"db.txt"));
        var hash = new Dictionary<string, string>();
        for(int i = 1; i <= 100000; i++){
            var text = "{" + $"\"Id\":{i},\"Name\":\"User{i}\"" + "}";
            hash.Add($"{i}", text);
        }
        var res = _fileWR.AppendList(hash.ToArray());
        var list = res.Join(
            hash.ToArray(),
            res => res.Key,
            input => input.Key,
            (res, input) => new { Value = input.Value, ReadObj = res.Value }
        );
        foreach(var item in list){
            var value = _fileWR.Read(item.ReadObj.Position, item.ReadObj.Length);
            Assert.Equal(item.Value, value);
        }
    }
}