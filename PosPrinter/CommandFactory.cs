using System;
using System.Collections.Generic;

namespace PosPrinter
{
    public static class CommandFactory
    {
        public static byte[] CreateResetCommand()
        {
            var bytes = new List<Byte>();
            bytes.AddRange(new byte[] { Commands.Esc, Commands.Reset });
            return bytes.ToArray();
        }

        public static byte[] CreateSetAlignmentCommand(PrintAlignment printAlignment)
        {
            var bytes = new List<Byte>();
            bytes.AddRange(new byte[] { Commands.Esc, Commands.PrintAlignment, (byte)printAlignment });
            return bytes.ToArray();
        }

        /// <summary>
        /// Creates the underline command.
        /// </summary>
        /// <returns>The underline command.</returns>
        /// <param name="underlineMode">Underline mode. 0: Disabled, 1: LightUnderline, 2: DarkUnderline</param>
        public static byte[] CreateUnderlineCommand(byte underlineMode)
        {
            var bytes = new List<Byte>();
            bytes.AddRange(new byte[] { Commands.Esc, Commands.UnderLine, underlineMode });
            return bytes.ToArray();
        }

        public static byte[] CreateBoldCommand(bool on = true)
        {
            var bytes = new List<Byte>();
            bytes.AddRange(new byte[] { Commands.Esc, Commands.Bold, (byte)(on ? 1 : 0) });
            return bytes.ToArray();
        }

        public static byte[] CreateFeedLinesCommand(byte lines)
        {
            var bytes = new List<Byte>();
            bytes.AddRange(new byte[] { Commands.Esc, Commands.FeedLine, lines });
            return bytes.ToArray();
        }

        public static byte[] CreatePrintLineCommand(string text)
        {
            var bytes = new List<byte>();
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(text));
            bytes.Add(Commands.Line);
            return bytes.ToArray();
        }

        public static byte[] CreatePrintCommand(string text)
        {
            var bytes = new List<byte>();
            bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(text));
            return bytes.ToArray();
        }

        public static byte[] CreateCutPaperCommand(byte feedCount)
        {
            var bytes = new List<Byte>();
            bytes.AddRange(new byte[] { Commands.Gs, Commands.Cut });
            bytes.AddRange(new byte[] { Commands.PartialCutMode, feedCount });
            return bytes.ToArray();
        }
    }
}
