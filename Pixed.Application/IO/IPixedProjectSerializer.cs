using Pixed.Core.Models;
using System.IO;

namespace Pixed.Application.IO;
internal interface IPixedProjectSerializer
{
    public bool CanSerialize { get; }
    public bool CanDeserialize { get; }
    public void Serialize(Stream stream, PixedModel model, bool close);
    public PixedModel Deserialize(Stream stream, ApplicationData applicationData);
}
