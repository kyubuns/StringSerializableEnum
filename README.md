# StringSerializableEnum

## 使い方

StringとしてシリアライズしたいEnumに `StringSerializableAttribute` をつけます。

```csharp
[StringSerializable]
public enum TestEnum
{
    One,
    Two,
    Three,
}
```

`StringSerializable{Enum名}` という型が自動生成されるので、それをシリアライズしてお使いください。

```csharp
public class TestMonoBehaviour : MonoBehaviour
{
    [SerializeField] private StringSerializableTestEnum testEnum;

    public void Start()
    {
        Debug.Log($"Value is {testEnum.Value}");
    }
}
```

## Installation

### UnityPackageManager

`https://github.com/kyubuns/StringSerializableEnum.git?path=Unity/Assets/StringSerializableEnum`

## Target Environment

- Unity2022.3 or later

## License

MIT License (see [LICENSE](LICENSE))
