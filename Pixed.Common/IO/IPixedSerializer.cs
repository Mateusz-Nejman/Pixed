using System.IO;

namespace Pixed.Common.IO;
internal interface IPixedSerializer
{
    public void Serialize(Stream stream);
    public void Deserialize(Stream stream);
}
