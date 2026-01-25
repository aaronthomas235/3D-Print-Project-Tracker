
namespace Core.Models
{
    public record MeshDimensions
    {
        public double Width { get; init; }
        public double Height { get; init; }
        public double Depth { get; init; }

        public MeshDimensions(double width, double height, double depth)
        {
            Width = width;
            Height = height;
            Depth = depth;
        }
    }
}
