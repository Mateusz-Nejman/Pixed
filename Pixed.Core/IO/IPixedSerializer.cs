namespace Pixed.Core.IO;
internal interface IPixedSerializer
{
    public void Serialize(Stream stream);
    public void Deserialize(Stream stream);
}
