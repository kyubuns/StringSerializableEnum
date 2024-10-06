using StringSerializableEnum;
using UnityEngine;

namespace Sandbox
{
    public class TestMonoBehaviour : MonoBehaviour
    {
        [SerializeField] private StringSerializableTestEnum testEnum;

        public void Start()
        {
            Debug.Log($"Value is {testEnum.Value}");
        }
    }

    [StringSerializable]
    public enum TestEnum
    {
        A,
        B,
        C,
    }
}
