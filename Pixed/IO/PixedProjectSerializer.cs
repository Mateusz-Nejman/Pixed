using Pixed.Models;
using System.IO;

namespace Pixed.IO
{
    internal sealed class PixedProjectSerializer : IPixedProjectSerializer
    {
        public void Serialize(Stream stream, PixedModel model, bool close = false)
        {
            model.Serialize(stream);

            if (close)
            {
                stream.Dispose();
            }
        }
        public PixedModel Deserialize(Stream stream)
        {
            PixedModel model = new();
            model.Deserialize(stream);

            return model;
        }
    }
}
