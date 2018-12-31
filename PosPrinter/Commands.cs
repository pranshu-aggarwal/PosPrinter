using System;
namespace PosPrinter
{
    public static class Commands
    {
        public const byte Esc = 0x1b;
        public const byte Reset = 0x40;
        public const byte PrintAlignment = 0x61;
        public const byte Line = 0x0A;
        public const byte UnderLine = 0x2D;
        public const int Bold = 0x45;
        public const int FeedLine = 0x64;
        public const int Gs = 0x1D;
        public const int Cut = 0x56;
        public const int PartialCutMode = 0x41;
    }
}
