using Pixed.Tools.Transform;
using System.Windows.Input;

namespace Pixed.ViewModels
{
    internal class TransformSectionViewModel : PropertyChangedBase
    {
        public ICommand ToolCommand { get; }

        public AbstractTransformTool ToolFlip { get; } = new Flip();
        public AbstractTransformTool ToolRotate { get; } = new Rotate();
        public AbstractTransformTool ToolCenter { get; } = new Center();
        public AbstractTransformTool ToolCrop { get; } = new Crop();

        public TransformSectionViewModel()
        {
            ToolCommand = new ActionCommand<AbstractTransformTool>(ToolAction);
        }

        private void ToolAction(AbstractTransformTool tool)
        {
            tool.ApplyTransformation();
        }
    }
}
