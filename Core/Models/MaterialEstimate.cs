
namespace ThreeDPrintProjectTracker.Engine.Models
{
    public record MaterialEstimate
    {
        public double VolumeMm3 { get; init; }
        public double FilamentLengthMeters { get; init; }
        public double WeightGrams { get; init; }

        public MaterialEstimate(double volumeMm3, double filamentLengthMeters, double weightGrams)
        {
            VolumeMm3 = volumeMm3;
            FilamentLengthMeters = filamentLengthMeters;
            WeightGrams = weightGrams;
        }
    }
}
