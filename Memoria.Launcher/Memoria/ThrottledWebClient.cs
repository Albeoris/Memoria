using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Memoria
{
    public class ThrottledWebClient : WebClient
    {
        private Timer _timer;
        private bool _updatePending;

        public ThrottledWebClient()
        {
            _timer = new Timer(500); // Set the interval to 500 milliseconds (or any desired interval)
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        protected override void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (!_updatePending)
            {
                _updatePending = true;
                base.OnDownloadProgressChanged(e);
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _updatePending = false;
        }
    }
}
