using System.IO;

namespace Pixed.IO;
internal interface IPixedSerializer
{
    public void Serialize(Stream stream);
    public void Deserialize(Stream stream);
}
