using System.Linq;

namespace Pixed.Services;
internal class NamingService
{
    private int _index = 0;

    public string GenerateName(string prefix = "Untitled")
    {
        string name = prefix + "-" + _index;
        _index++;

        if (Global.Models.Any(m => m.FileName == name))
        {
            return GenerateName(prefix);
        }

        return name;
    }
}
