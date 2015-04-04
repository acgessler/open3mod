///////////////////////////////////////////////////////////////////////////////////
// Open 3D Model Viewer (open3mod) (v2.0)
// [RunOnceGuard.cs]
// (c) 2012-2015, Open3Mod Contributors
//
// Licensed under the terms and conditions of the 3-clause BSD license. See
// the LICENSE file in the root folder of the repository for the details.
//
// HIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;

using System.Windows.Forms;


namespace open3mod
{
    public static class RunOnceGuard
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Ensure only one instance of the application is running, communicate files
        /// to be opened to the primary program instance as needed.
        /// </summary>
        /// <param name="mutexName">Globally unique mutex name used to identify other running instances.</param>
        /// <param name="actionPrimary">what to do if this is the first instance of the application</param>
        /// <param name="actionPrimaryReceiveMessage"> what do invoke if this is the first instance of the application,
        /// and another (temporary) instance messages it to open a new tab. </param>
        /// <param name="actionNotifyPrimary">what to send to the first instance of the application if the
        /// current instance is only temporary. No message is send for a null return value.</param>
        public static void Guard(String mutexName, Action actionPrimary, Action<string> actionPrimaryReceiveMessage, Func<object> actionNotifyPrimary)
        {
            Debug.Assert(mutexName != null);
            Debug.Assert(actionPrimary != null);
            Debug.Assert(actionPrimaryReceiveMessage != null);
            Debug.Assert(actionNotifyPrimary != null);

            var pipeName = mutexName + "_pipe";

            // based on http://stackoverflow.com/questions/184084
            bool createdNew;
            using (new Mutex(true, mutexName, out createdNew))
            {
                if (createdNew)
                {
                    using (new ServerRunner(pipeName, actionPrimaryReceiveMessage))
                    {
                        actionPrimary();
                    }
                }
                else
                {
                    // put the real instance to the foreground
                    using (var current = Process.GetCurrentProcess())
                    {
                        Debug.Assert(current != null);
                        foreach (
                            var process in
                                Process.GetProcessesByName(current.ProcessName).Where(
                                    process => process.Id != current.Id))
                        {
                            SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }

                    // see if we have a message for it
                    var message = actionNotifyPrimary();
                    if (message == null)
                    {
                        return;
                    }

                    // attempt to connect to it and notify it
                    Console.WriteLine("Communicating with primary application instance");
                    using (var pipeClient = new NamedPipeClientStream(".", pipeName,PipeDirection.Out, PipeOptions.None))
                    {
                        pipeClient.Connect();

                        using (var sw = new StreamWriter(pipeClient))
                        {
                            sw.Write(message);
                        }
                    }              
                }
            }
        }

        private class ServerRunner : IDisposable
        {
            // note: running multiple servers at the moment to make sure it still works if multiple instances
            // attempt to connect at (almost) same time. This is unreliable though and there must be a
            // better way to handle this.
            private const int ServerCount = 2;

            private readonly Thread[] _threads = new Thread[ServerCount];
            private volatile bool _shutdown = false;

            public ServerRunner(String pipeName, Action<string> actionPrimaryReceiveMessage)
            {
                Debug.Assert(pipeName != null);
                Debug.Assert(actionPrimaryReceiveMessage != null);
                for (uint n = 0; n < ServerCount; ++n)
                {
                    _threads[n] = new Thread(() =>
                    {
                        while (!_shutdown)
                        {
                            try
                            { 
                                using (var server = new NamedPipeServerStream(pipeName, PipeDirection.In, ServerCount, 
                                    PipeTransmissionMode.Byte, 
                                    PipeOptions.Asynchronous))
                                {
                                    var connectEvent = new AutoResetEvent(false);

                                    // note: unfortunately, WaitForConnection() does not put the current
                                    // thread in an interruptible state. This is based on 
                                    // http://stackoverflow.com/questions/607872 and achieves this
                                    // by using the async version, BeginWaitForConnection, in conjunction
                                    // with an event.
                                    server.BeginWaitForConnection(ar => {
                                        // without this guard, unsafe access to a disposed closure can happen
                                        if (_shutdown) 
                                        {
                                            return;
                                        }

                                        // ReSharper disable AccessToDisposedClosure
                                        Debug.Assert(server != null);                                     
                                        server.EndWaitForConnection(ar);

                                        using (var sr = new StreamReader(server))
                                        {
                                            var line = sr.ReadLine();
                                            if (!_shutdown)
                                            {
                                                // note: there is a small window in which the callback
                                                // is called even though the application is likely no longer
                                                // prepared for it. This needs to be checked for in the callback.
                                                actionPrimaryReceiveMessage(line);
                                            }
                                        }
                                        // ReSharper restore AccessToDisposedClosure
                                        connectEvent.Set();                                      
                                    }, null);

                                    connectEvent.WaitOne();
                                }
                            }
                            catch (IOException xc)
                            {
                                // ignore any IO exceptions happening here. We do not
                                // want to interrupt or crash the primary instance.
                                Console.WriteLine("Ignoring IOException in NamedPipe Server: " + xc.ToString());
                            }
                            catch (ThreadInterruptedException)
                            {
                                return;
                            }
                        }
                    });
                    _threads[n].Start();
                }
            }

            public void Dispose()
            {
                _shutdown = true;
                foreach (var thread in _threads)
                {
                    thread.Interrupt();
                    thread.Join();
                }
            }
        }
    }
}

/* vi: set shiftwidth=4 tabstop=4: */ 