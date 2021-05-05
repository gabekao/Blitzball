using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

public class Vector3Surrogate : ISerializationSurrogate
{
    // Serializes vector3
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        // Cast passed object as vector3
        Vector3 v3 = (Vector3)obj;
        
        // Add obj values to info
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }

    // Deserializes vector3
    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        // Cast passed object as vector3
        Vector3 v3 = (Vector3)obj;
        
        // Obtain values from info
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));

        // Return deserialized object
        obj = v3;
        return obj;
    }
}
