using UnityEngine;

public partial class INIParser
{
    public Vector3 ReadValue(string SectionName, string Key, Vector3 DefaultValue)
    {
        string StringValue = ReadValue(SectionName, Key, DefaultValue.ToString());
        Vector3 Value = StringToVector3(StringValue);
        return Value;
    }
    private static Vector3 StringToVector3(string input)
    {
        // Loại bỏ dấu ngoặc và tách chuỗi bằng dấu phẩy
        input = input.Trim('(', ')', ' ').Replace("f", "");
        string[] values = input.Split(',');

        // Chuyển đổi từng giá trị từ string sang float
        float x = float.Parse(values[0]);
        float y = float.Parse(values[1]);
        float z = float.Parse(values[2]);

        return new Vector3(x, y, z);
    }
}
