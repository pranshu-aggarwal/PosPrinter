using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PosPrinter
{
    public class Printer : IDisposable
    {
        private const int printerPort = 9100;
        private readonly string ipAddress;
        private Socket printerSocket;

        public Printer(string idAddress)
        {
            this.ipAddress = idAddress;
            printerSocket = new Socket(SocketType.Stream, ProtocolType.IP);
            printerSocket.SendTimeout = 1500;
        }

        public Task ConnectAsync()
        {
            return printerSocket.ConnectAsync(ipAddress, printerPort);
        }

        public void SetAlignment(PrintAlignment printAlignment)
        {
            printerSocket.Send(CommandFactory.CreateSetAlignmentCommand(printAlignment));
        }

        public void SetUnderline(bool thick = false)
        {
            printerSocket.Send(CommandFactory.CreateUnderlineCommand((byte)(thick ? 2 : 1)));
        }

        public void RemoveUnderline()
        {
            printerSocket.Send(CommandFactory.CreateUnderlineCommand(0));
        }

        public void SetBold()
        {
            printerSocket.Send(CommandFactory.CreateBoldCommand());
        }



        public void RemoveBold()
        {
            printerSocket.Send(CommandFactory.CreateBoldCommand(false));
        }

        public void FeedLines(byte lines)
        {
            printerSocket.Send(CommandFactory.CreateFeedLinesCommand(lines));
        }

        public void PrintLine(string text = "")
        {
            printerSocket.Send(CommandFactory.CreatePrintLineCommand(text));
        }

        public void Print(string text)
        {
            printerSocket.Send(CommandFactory.CreatePrintCommand(text));
        }

        /// <summary>
        /// Cuts the paper.
        /// </summary>
        /// <param name="feedCount">Feed count. CutPosition = CutPositon + (n*verticalmotionunits)</param>
        public void CutPaper(byte feedCount = 50)
        {
            printerSocket.Send(CommandFactory.CreateCutPaperCommand(feedCount));
        }

        public void Reset()
        {
            printerSocket.Send(CommandFactory.CreateResetCommand());
        }

		public void SendBytes(byte[] bytes)
		{
			printerSocket.Send(bytes);
		}

        public void Disconnect()
        {
            printerSocket.Close();
        }

        public void Dispose()
        {
            if(printerSocket != null && printerSocket.Connected)
            {
                printerSocket.Close();
            }
        }
    }
}
