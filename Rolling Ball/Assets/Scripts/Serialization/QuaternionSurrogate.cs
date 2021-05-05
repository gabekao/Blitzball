using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

public class QuaternionSurrogate : ISerializationSurrogate
{
    // Serializes quaternion
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        // Cast passed object as quaternion
        Quaternion quaternion = (Quaternion)obj;

        // Add obj values to info
        info.AddValue("x", quaternion.x);
        info.AddValue("y", quaternion.y);
        info.AddValue("z", quaternion.z);
        info.AddValue("w", quaternion.w);
    }

    // Deserializes quaternion
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        // Cast passed object as quaternion
        Quaternion quaternion = (Quaternion)obj;

        // Obtain values from info
        quaternion.x = (float)info.GetValue("x", typeof(float));
        quaternion.y = (float)info.GetValue("y", typeof(float));
        quaternion.z = (float)info.GetValue("z", typeof(float));
        quaternion.w = (float)info.GetValue("w", typeof(float));

        // Return deserialized object
        obj = quaternion;
        return obj;
    }
}