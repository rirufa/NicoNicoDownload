using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicoNicoDownloader.Model
{
    public class Logger
    {
        System.Diagnostics.TraceSource log = new System.Diagnostics.TraceSource("Niconico Download Log", System.Diagnostics.SourceLevels.Information);

        static Logger _instance;

        public static Logger Current
        {
            get
            {
                if (_instance == null)
                    _instance = new Logger();
                return _instance;
            }
        }

        public Logger()
        {
            System.Diagnostics.TextWriterTraceListener listener = new System.Diagnostics.TextWriterTraceListener("nico_download.log", "LogFile");
            this.log.Listeners.Add(listener);
        }

        public void WriteLine(string s)
        {
            this.log.TraceEvent(System.Diagnostics.TraceEventType.Information, 0, s);
            this.log.Flush();
        }
    }
}
