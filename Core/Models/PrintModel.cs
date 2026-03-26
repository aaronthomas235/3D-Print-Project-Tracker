
namespace ThreeDPrintProjectTracker.Engine.Models
{
    public sealed class PrintModel
    {
        public double VolumeMm3 { get; init; }
        public double SurfaceAreaMm2 { get; init; }
        public double HeightMm { get; init; }
        public MeshDimensions? Dimensions { get; init; }
    }
}
