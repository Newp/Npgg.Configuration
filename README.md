# Npgg.Configuration

## 기능
- CSV, TSV String 을 C# object list로 변환해줍니다.
- 기본타입 (int, string, float...) 을 지원합니다.
- 열거형( List<T>, T[] ) 타입을 지원합니다. 단, 이때 따옴표(")로 값을 감싸줘야 합니다. (ex: "1,2,3")
- TypeDescriptor 에 등록하여 CustomType Convert 가 가능합니다.


///

## Example
1.deserialize 할 클래스를 생성한다.
```csharp
    public class CsvSample
    {
        public int Key { get; set; }
        public string Value;
    }
```
2.csv(또는 tsv) 를 작성하고 string 객체로 read 한다. ( local file, unity TextAsset, CDN download 등 )
```
Key,Value
1,Value1
2,Value2
3,Value3
```
3. Load 한다.
```csharp
    public void OnLoad(string tableString)
    {
          SimpleLoader loader = new SimpleLoader();

          var loaded =loader.Load<CsvSample>(tableString).Result;
          
          //loaded 에는 아래와 같은 아이템들이 들어있다
          // [0] new CsvSample(){ Key=1, Value="Value1"};
          // [1] new CsvSample(){ Key=2, Value="Value2"};
          // [2] new CsvSample(){ Key=2, Value="Value3"};
    }
```


### 주석의 활용
column이나 가장앞열에 // 또는 # 을 통하여 주석처리할 수 있습니다.

아래 경우에는 모든 Value column (Value1,Value2,Value3)값을 무시합니다.
```csv//
Key,//Value
1,Value1
2,Value2
3,Value3
```



아래 경우에는 Key=2 인 row 를 무시합니다
```csv//
Key,//Value
1,Value1
//2,Value2
3,Value3
```



### Attribute 활용
만약 naming convention 등의 이유로 필드 이름과 테이블컬럼 이름을 동일하게 맞출 수 없다면 
ConfigColumnAttribute를 활용하여 이를 맞출 수 있습니다.


```csv
key,value
1,Value1
2,Value2
3,Value3
```

```csharp
    public class CsvSample
    {
        [ConfigColumn("key")]
        public int Key { get; set; }
        
        [ConfigColumn("value")]
        public string Value;
    }

```
