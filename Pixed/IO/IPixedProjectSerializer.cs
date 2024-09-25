using Pixed.Models;
using Pixed.Services;
using System.IO;

namespace Pixed.IO
{
    internal interface IPixedProjectSerializer
    {
        public void Serialize(Stream stream, PixedModel model, bool close);
        public PixedModel Deserialize(Stream stream, ApplicationData applicationData);
    }
}
