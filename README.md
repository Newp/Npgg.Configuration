# Npgg.CsvSerializer
CSV to C# object Serializer

진행중인 프로젝트 입니다.

1. 기존에 지원하던 List, Array 를 임시로 제외하였습니다.
2. string load 를 상속받아야 하던 기능을 임시로 제외하였습니다
3. async/await 을 지원하나 이후 퍼포먼스 테스트를 통하여 변경될 수 있습니다.


///

csv를 활용할 class 를 선언한다
```csharp
    public class CsvSample
    {
        public int Key { get; set; }
        public string Value;
    }
```

csv 를 작성한다
```
Key,Value
1,Value1
2,Value2
3,Value3
```

Load 한다.
```csharp
    public void OnLoad(string csv)
    {
          SimpleLoader loader = new SimpleLoader();

          var loaded =loader.Load<CsvSample>(csv).Result;
          
          //loaded 에는 아래와 같은 아이템들이 들어있다
          // [0] new CsvSample(){ Key=1, Value="Value1"};
          // [1] new CsvSample(){ Key=2, Value="Value2"};
          // [2] new CsvSample(){ Key=2, Value="Value3"};
    }
```

