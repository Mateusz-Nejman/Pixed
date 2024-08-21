using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixed.Tools
{
    internal class Tool
    {
        private Dictionary<string, BaseTool> _tools;

        public Tool()
        {
            _tools = [];
            _tools.Add("tool_pen", new ToolPen());
            _tools.Add("tool_eraser", new ToolEraser());
            _tools.Add("tool_paint_bucket", new ToolBucket());
        }
        public BaseTool? GetTool(string name)
        {
            if(_tools.ContainsKey(name))
            {
                return _tools[name];
            }

            return null;
        }
    }
}
