# StringSerializableEnum

By serializing Enums as Strings instead of Int, you can add or remove values from the Enum without causing issues.  
It also makes it easier to view differences, which is a nice bonus.

![20241006114808](https://github.com/user-attachments/assets/877dafde-1c70-459b-ad6e-93d5404331d5)

## Q&A

### What happens if I change the name?

An error will appear in the Inspector, and if you try to run the game like this, an exception will be thrown at runtime.

![20241006114843](https://github.com/user-attachments/assets/9d690e70-3961-471a-ad3d-9e647b129cc7)


### Isn’t changing the Enum name just as risky as changing its numerical value?

To be honest, I think it’s about the same. It’s more of a philosophical choice.  
That being said, you could statically check it, so it might be a little better in that regard.

### Isn’t converting between String and Enum slow?

It expands into a switch statement, so it’s faster than Enum.TryParse.  
(But optimization isn’t my specialty, so if there’s a better way, I’d love to hear it.)


## Usage

Add the `[StringSerializable]` to any Enum you want to serialize as a String.

```csharp
[StringSerializable]
public enum TestEnum
{
    One,
    Two,
    Three,
}
```

A type named `StringSerializable{EnumName}` will be automatically generated. Use this type for serialization.

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
