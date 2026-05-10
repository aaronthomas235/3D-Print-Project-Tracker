using Avalonia.Media;
using DrawingColour = System.Drawing.Color;

namespace ThreeDPrintProjectTracker.Avalonia.Services
{
    public static class ColourConverter
    {
        public static Color ToAvalonia(this DrawingColour c) => Color.FromArgb(c.A, c.R, c.G, c.B);

        public static DrawingColour ToDrawing(this Color c) => DrawingColour.FromArgb(c.A, c.R, c.G, c.B);
    }
}
