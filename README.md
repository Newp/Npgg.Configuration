# Npgg.Configuration

## 기능
- CSV, TSV String 을 C# object list로 변환해줍니다.
- 기본타입 (int, string, float...) 을 지원합니다.
- 열거형( List<T>, T[] ) 타입을 지원합니다. 단, 이때 따옴표(")로 값을 감싸줘야 합니다. (ex: "1,2,3")
- TypeDescriptor 에 등록하여 CustomType Convert 가 가능합니다.


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


## Install Package

Nuget Package Manager
```powershell
PM> Install-Package Npgg.Configuration -Version 1.3.0
```
Dotnet CLI
```powershell
>dotnet add package Npgg.Configuration --version 1.3.0
```

Package Reference
```xml
<PackageReference Include="Npgg.Configuration" Version="1.3.0" />
```
***

## List<T>, T[] 활용
필드안에서 쉼표(,)로 구분하며, CSV의 경우 필드를 따옴표(")로 감싸줍니다.

```csharp
public class ListArraySample<T>
{
	public int Key { get; set; }
	public List<T> Values1 { get; set; }
	public T[] Values2 { get; set; }
}       
```
    
csv, 따옴표로 묶어야 합니다.
```
Key,Values1,Values2
1,"1,2,3","4,5,6"
2,"1,2,3","4,5,6"
``` 
tsv, 따옴표로 묶어도 되고 안묶어도 됩니다.
```
Key,Values1,Values2
1       1,2,3     "4,5,6"
2       "1,2,3"     "4,5,6"
```



### Attribute 활용
만약 naming convention 등의 이유로 필드 이름과 테이블컬럼 이름을 동일하게 맞출 수 없다면 ConfigColumnAttribute를 활용하여 이를 맞출 수 있습니다.
ConfigColumnAttribute가 설정될 경우 기본적으로 Required=true가 됩니다.
(만약, 이름을 바꾸고 Required=false 로 하고싶다면 [ConfigColum("name", false)] 로 지정합니다.)
Required 가 true 임에도 table string에 column 이 존재하지 않는다면 RequiredColumnNotFoundException 이 발생합니다.

```csv
key,value
1,Value1
2,Value2
3,Value3
```

```csharp
public class CsvSample
{
	[ConfigColumn("key")] // default Required value is true
	public int Key { get; set; }

	[ConfigColumn("value", false)] // required = false
	public string Value;
}

```



## CustomConverter 활용

csv에서 row로 사용할 클래스를 선언하고 CustomConverter<T> 를 상속하여 Convert 함수를 정의해줍니다.
```csharp
class Vector3
{
	public float X, Y, Z;
}

class Vector3Converter : CustomConverter<Vector3>
{
	public override Vector3 Convert(string value) //input (1,2,3)
	{
		var splited = value.Split(',')
			.Select(d => d.Trim('(', ')')) // array["1", "2", "3"]
			.Select(d => float.Parse(d)).ToArray(); // array[1, 2, 3]

		return new Vector3() { X = splited[0], Y = splited[1], Z = splited[2] };
	}
}
```

이제 클래스 선언, table string 선언, load 3단계로 마무리 됩니다.

csvString
```
Key,Vector3
1,"(1,2,3)"
```
```csharp

class Vector3Row
{
	public int Key { get; set; }

	public Vector3 Vector3 { get; set; }
}

[Fact]
public void CustomConverterTest()
{

	//CustomConverter<T>.RegistConverter 함수를 활용하여 Custom Converter를 등록합니다.
	CustomConverter<Vector3>.RegistConverter<Vector3Converter>();

	CsvLoader loader = new CsvLoader();

	//Load !
	var list = loader.Load<Vector3Row>(csvString);

	Assert.Single(list);

	var item = list.FirstOrDefault();
	Assert.NotNull(item);
	Assert.Equal(1, item.Key);
	Assert.Equal(1, item.Vector3.X);
	Assert.Equal(2, item.Vector3.Y);
	Assert.Equal(3, item.Vector3.Z);
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


### Convert Error TroubleShooting
만약 필드값(cell value)가 타입과 일치하지 않아 오류가 발생할 경우 ConvertException 이 발생합니다.
제공된 property value들을 통하여 load에서 발생하는 장애를 빠르게 해결하세요.
```csharp
public class ConvertException : Exception
{
	public string ColumnName { get; }
	public string TextValue { get; }
	public int LineNumber { get; }
}
```
