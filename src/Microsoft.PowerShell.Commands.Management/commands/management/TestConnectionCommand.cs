// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// The implementation of the "Test-Connection" cmdlet.
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "Connection", DefaultParameterSetName = DefaultPingParameterSet,
        HelpUri = "https://go.microsoft.com/fwlink/?LinkID=135266")]
    [OutputType(typeof(PingReport), ParameterSetName = new string[] { DefaultPingParameterSet })]
    [OutputType(typeof(PingReply), ParameterSetName = new string[] { RepeatPingParameterSet, MtuSizeDetectParameterSet })]
    [OutputType(typeof(bool), ParameterSetName = new string[] { DefaultPingParameterSet, RepeatPingParameterSet, TcpPortParameterSet })]
    [OutputType(typeof(int), ParameterSetName = new string[] { MtuSizeDetectParameterSet })]
    [OutputType(typeof(TraceRouteReply), ParameterSetName = new string[] { TraceRouteParameterSet })]
    public class TestConnectionCommand : PSCmdlet, IDisposable
    {
        private const string DefaultPingParameterSet = "DefaultPing";
        private const string RepeatPingParameterSet = "RepeatPing";
        private const string TraceRouteParameterSet = "TraceRoute";
        private const string TcpPortParameterSet = "TcpPort";
        private const string MtuSizeDetectParameterSet = "MtuSizeDetect";

        #region Parameters

        /// <summary>
        /// Do ping test.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        public SwitchParameter Ping { get; set; } = true;

        /// <summary>
        /// Force using IPv4 protocol.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        [Parameter(ParameterSetName = TraceRouteParameterSet)]
        [Parameter(ParameterSetName = MtuSizeDetectParameterSet)]
        [Parameter(ParameterSetName = TcpPortParameterSet)]
        public SwitchParameter IPv4 { get; set; }

        /// <summary>
        /// Force using IPv6 protocol.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        [Parameter(ParameterSetName = TraceRouteParameterSet)]
        [Parameter(ParameterSetName = MtuSizeDetectParameterSet)]
        [Parameter(ParameterSetName = TcpPortParameterSet)]
        public SwitchParameter IPv6 { get; set; }

        /// <summary>
        /// Do reverse DNS lookup to get names for IP addresses.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        [Parameter(ParameterSetName = TraceRouteParameterSet)]
        [Parameter(ParameterSetName = MtuSizeDetectParameterSet)]
        [Parameter(ParameterSetName = TcpPortParameterSet)]
        public SwitchParameter ResolveDestination { get; set; }

        /// <summary>
        /// Source from which to do a test (ping, trace route, ...).
        /// The default is Local Host.
        /// Remoting is not yet implemented internally in the cmdlet.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        [Parameter(ParameterSetName = TraceRouteParameterSet)]
        [Parameter(ParameterSetName = TcpPortParameterSet)]
        public string Source { get; } = Dns.GetHostName();

        /// <summary>
        /// The number of times the Ping data packets can be forwarded by routers.
        /// As gateways and routers transmit packets through a network,
        /// they decrement the Time-to-Live (TTL) value found in the packet header.
        /// The default (from Windows) is 128 hops.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        [Parameter(ParameterSetName = TraceRouteParameterSet)]
        [ValidateRange(0, sMaxHops)]
        [Alias("Ttl", "TimeToLive", "Hops")]
        public int MaxHops { get; set; } = sMaxHops;

        private const int sMaxHops = 128;

        /// <summary>
        /// Count of attempts.
        /// The default (from Windows) is 4 times.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [ValidateRange(ValidateRangeKind.Positive)]
        public int Count { get; set; } = 4;

        /// <summary>
        /// Delay between attempts.
        /// The default (from Windows) is 1 second.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        [ValidateRange(ValidateRangeKind.Positive)]
        public int Delay { get; set; } = 1;

        /// <summary>
        /// Buffer size to send.
        /// The default (from Windows) is 32 bites.
        /// Max value is 65500 (limit from Windows API).
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        [Alias("Size", "Bytes", "BS")]
        [ValidateRange(0, 65500)]
        public int BufferSize { get; set; } = DefaultSendBufferSize;

        /// <summary>
        /// Don't fragment ICMP packages.
        /// Currently CoreFX not supports this on Unix.
        /// </summary>
        [Parameter(ParameterSetName = DefaultPingParameterSet)]
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        public SwitchParameter DontFragment { get; set; }

        /// <summary>
        /// Continue ping until user press Ctrl-C
        /// or Int.MaxValue threshold reached.
        /// </summary>
        [Parameter(ParameterSetName = RepeatPingParameterSet)]
        public SwitchParameter Continues { get; set; }

        /// <summary>
        /// Set short output kind ('bool' for Ping, 'int' for MTU size ...).
        /// Default is to return typed result object(s).
        /// </summary>
        [Parameter]
        public SwitchParameter Quiet;

        /// <summary>
        /// Time-out value in seconds.
        /// If a response is not received in this time, no response is assumed.
        /// It is not the cmdlet timeout! It is a timeout for waiting one ping response.
        /// The default (from Windows) is 5 second.
        /// </summary>
        [Parameter]
        [ValidateRange(ValidateRangeKind.Positive)]
        public int TimeoutSeconds { get; set; } = 5;

        /// <summary>
        /// Destination - computer name or IP address.
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        [Alias("ComputerName")]
        public string[] TargetName { get; set; }

        /// <summary>
        /// Detect MTU size.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = MtuSizeDetectParameterSet)]
        public SwitchParameter MTUSizeDetect { get; set; }

        /// <summary>
        /// Do traceroute test.
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = TraceRouteParameterSet)]
        public SwitchParameter Traceroute { get; set; }

        /// <summary>
        /// Do tcp connection test.
        /// </summary>
        [ValidateRange(0, 65535)]
        [Parameter(Mandatory = true, ParameterSetName = TcpPortParameterSet)]
        public int TCPPort { get; set; }

        #endregion Parameters

        /// <summary>
        /// Init the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            switch (ParameterSetName)
            {
                case RepeatPingParameterSet:
                    Count = int.MaxValue;
                    break;
            }
        }

        /// <summary>
        /// Process a connection test.
        /// </summary>
        protected override void ProcessRecord()
        {
            foreach (var targetName in TargetName)
            {
                switch (ParameterSetName)
                {
                    case DefaultPingParameterSet:
                    case RepeatPingParameterSet:
                        ProcessPing(targetName);
                        break;
                    case MtuSizeDetectParameterSet:
                        ProcessMTUSize(targetName);
                        break;
                    case TraceRouteParameterSet:
                        ProcessTraceroute(targetName);
                        break;
                    case TcpPortParameterSet:
                        ProcessConnectionByTCPPort(targetName);
                        break;
                }
            }
        }

        #region ConnectionTest

        private void ProcessConnectionByTCPPort(string targetNameOrAddress)
        {
            string resolvedTargetName;
            IPAddress targetAddress;
            if (!InitProcessPing(targetNameOrAddress, out resolvedTargetName, out targetAddress))
            {
                return;
            }

            TcpClient client = new TcpClient();

            try
            {
                Task connectionTask = client.ConnectAsync(targetAddress, TCPPort);
                string targetString = targetAddress.ToString();

                for (var i = 1; i <= TimeoutSeconds; i++)
                {
                    Task timeoutTask = Task.Delay(millisecondsDelay: 1000);
                    Task.WhenAny(connectionTask, timeoutTask).Result.Wait();

                    if (timeoutTask.Status == TaskStatus.Faulted || timeoutTask.Status == TaskStatus.Canceled)
                    {
                        // Waiting is interrupted by Ctrl-C.
                        WriteObject(false);
                        return;
                    }

                    if (connectionTask.Status == TaskStatus.RanToCompletion)
                    {
                        WriteObject(true);
                        return;
                    }
                }
            }
            catch
            {
                // Silently ignore connection errors.
            }
            finally
            {
                client.Close();
            }

            WriteObject(false);
        }
        #endregion ConnectionTest

        #region TracerouteTest
        private void ProcessTraceroute(string targetNameOrAddress)
        {
            byte[] buffer = GetSendBuffer(BufferSize);

            string resolvedTargetName;
            IPAddress targetAddress;
            if (!InitProcessPing(targetNameOrAddress, out resolvedTargetName, out targetAddress))
            {
                return;
            }

            TraceRouteResult traceRouteResult = new TraceRouteResult(Source, targetAddress, resolvedTargetName);

            int currentHop = 1;
            PingOptions pingOptions = new PingOptions(currentHop, DontFragment.IsPresent);
            PingReply reply = null;
            int timeout = TimeoutSeconds * 1000;

            do
            {
                TraceRouteReply traceRouteReply = new TraceRouteReply();

                pingOptions.Ttl = traceRouteReply.Hop = currentHop;
                currentHop++;

                // In the specific case we don't use 'Count' property.
                // If we change 'DefaultTraceRoutePingCount' we should change 'ConsoleTraceRouteReply' resource string.
                for (int i = 1; i <= DefaultTraceRoutePingCount; i++)
                {
                    try
                    {
                        reply = _sender.Send(targetAddress, timeout, buffer, pingOptions);

                        traceRouteReply.PingReplies.Add(reply);
                    }
                    catch (PingException ex)
                    {
                        string message = StringUtil.Format(
                            TestConnectionResources.NoPingResult,
                            resolvedTargetName,
                            ex.Message);
                        Exception pingException = new PingException(message, ex.InnerException);
                        ErrorRecord errorRecord = new ErrorRecord(
                            pingException,
                            TestConnectionExceptionId,
                            ErrorCategory.ResourceUnavailable,
                            resolvedTargetName);
                        WriteError(errorRecord);

                        continue;
                    }
                    catch
                    {
                        // Ignore host resolve exceptions.
                    }

                    // We use short delay because it is impossible DoS with trace route.
                    Thread.Sleep(200);
                }

                if (ResolveDestination && reply.Status == IPStatus.Success)
                {
                    traceRouteReply.ReplyRouterName = Dns.GetHostEntry(reply.Address).HostName;
                }

                traceRouteReply.ReplyRouterAddress = reply.Address;
                traceRouteResult.Replies.Add(traceRouteReply);
            } while (reply != null
                && currentHop <= sMaxHops
                && (reply.Status == IPStatus.TtlExpired || reply.Status == IPStatus.TimedOut));

            if (Quiet.IsPresent)
            {
                WriteObject(currentHop <= sMaxHops);
            }
            else
            {
                WriteObject(traceRouteResult);
            }
        }

        /// <summary>
        /// The class contains an information about a trace route attempt.
        /// </summary>
        public class TraceRouteReply
        {
            internal TraceRouteReply()
            {
                PingReplies = new List<PingReply>(DefaultTraceRoutePingCount);
            }

            /// <summary>
            /// Number of current hop (router).
            /// </summary>
            public int Hop;

            /// <summary>
            /// List of ping replies for current hop (router).
            /// </summary>
            public List<PingReply> PingReplies;

            /// <summary>
            /// Router IP address.
            /// </summary>
            public IPAddress ReplyRouterAddress;

            /// <summary>
            /// Resolved router name.
            /// </summary>
            public string ReplyRouterName;
        }

        /// <summary>
        /// The class contains an information about the source, the destination and trace route results.
        /// </summary>
        public class TraceRouteResult
        {
            internal TraceRouteResult(string source, IPAddress destinationAddress, string destinationHost)
            {
                Source = source;
                DestinationAddress = destinationAddress;
                DestinationHost = destinationHost;
                Replies = new List<TraceRouteReply>();
            }

            /// <summary>
            /// Source from which to trace route.
            /// </summary>
            public string Source { get; }

            /// <summary>
            /// Destination to which to trace route.
            /// </summary>
            public IPAddress DestinationAddress { get; }

            /// <summary>
            /// Destination to which to trace route.
            /// </summary>
            public string DestinationHost { get; }

            /// <summary>
            /// </summary>
            public List<TraceRouteReply> Replies { get; }
        }

        #endregion TracerouteTest

        #region MTUSizeTest
        private void ProcessMTUSize(string targetNameOrAddress)
        {
            PingReply reply, replyResult = null;
            string resolvedTargetName;
            IPAddress targetAddress;
            if (!InitProcessPing(targetNameOrAddress, out resolvedTargetName, out targetAddress))
            {
                return;
            }

            // Cautious! Algorithm is sensitive to changing boundary values.
            int HighMTUSize = 10000;
            int CurrentMTUSize = 1473;
            int LowMTUSize = targetAddress.AddressFamily == AddressFamily.InterNetworkV6 ? 1280 : 68;
            int timeout = TimeoutSeconds * 1000;

            try
            {
                PingOptions pingOptions = new PingOptions(MaxHops, true);
                int retry = 1;

                while (LowMTUSize < (HighMTUSize - 1))
                {
                    byte[] buffer = GetSendBuffer(CurrentMTUSize);

                    WriteDebug(StringUtil.Format(
                        "LowMTUSize: {0}, CurrentMTUSize: {1}, HighMTUSize: {2}",
                        LowMTUSize,
                        CurrentMTUSize,
                        HighMTUSize));

                    reply = _sender.Send(targetAddress, timeout, buffer, pingOptions);

                    // Cautious! Algorithm is sensitive to changing boundary values.
                    if (reply.Status == IPStatus.PacketTooBig)
                    {
                        HighMTUSize = CurrentMTUSize;
                        retry = 1;
                    }
                    else if (reply.Status == IPStatus.Success)
                    {
                        LowMTUSize = CurrentMTUSize;
                        replyResult = reply;
                        retry = 1;
                    }
                    else
                    {
                        // Target host don't reply - try again up to 'Count'.
                        if (retry >= Count)
                        {
                            string message = StringUtil.Format(
                                TestConnectionResources.NoPingResult,
                                targetAddress,
                                reply.Status.ToString());
                            Exception pingException = new PingException(message);
                            ErrorRecord errorRecord = new ErrorRecord(
                                pingException,
                                TestConnectionExceptionId,
                                ErrorCategory.ResourceUnavailable,
                                targetAddress);
                            WriteError(errorRecord);
                            return;
                        }
                        else
                        {
                            retry++;
                            continue;
                        }
                    }

                    CurrentMTUSize = (LowMTUSize + HighMTUSize) / 2;

                    // Prevent DoS attack.
                    Thread.Sleep(100);
                }
            }
            catch (PingException ex)
            {
                string message = StringUtil.Format(TestConnectionResources.NoPingResult, targetAddress, ex.Message);
                Exception pingException = new PingException(message, ex.InnerException);
                ErrorRecord errorRecord = new ErrorRecord(
                    pingException,
                    TestConnectionExceptionId,
                    ErrorCategory.ResourceUnavailable,
                    targetAddress);
                WriteError(errorRecord);
                return;
            }

            if (Quiet.IsPresent)
            {
                WriteObject(CurrentMTUSize);
            }
            else
            {
                var res = PSObject.AsPSObject(replyResult);

                PSMemberInfo sourceProperty = new PSNoteProperty("Source", Source);
                res.Members.Add(sourceProperty);
                PSMemberInfo destinationProperty = new PSNoteProperty("Destination", targetNameOrAddress);
                res.Members.Add(destinationProperty);
                PSMemberInfo mtuSizeProperty = new PSNoteProperty("MTUSize", CurrentMTUSize);
                res.Members.Add(mtuSizeProperty);
                res.TypeNames.Insert(0, "PingReply#MTUSize");

                WriteObject(res);
            }
        }

        #endregion MTUSizeTest

        #region PingTest

        private void ProcessPing(string targetNameOrAddress)
        {
            string resolvedTargetName;
            IPAddress targetAddress;
            if (!InitProcessPing(targetNameOrAddress, out resolvedTargetName, out targetAddress))
            {
                return;
            }

            bool quietResult = true;
            byte[] buffer = GetSendBuffer(BufferSize);

            PingReply reply;
            PingOptions pingOptions = new PingOptions(MaxHops, DontFragment.IsPresent);
            PingReport pingReport = new PingReport(Source, resolvedTargetName);
            int timeout = TimeoutSeconds * 1000;
            int delay = Delay * 1000;

            for (int i = 1; i <= Count; i++)
            {
                try
                {
                    reply = _sender.Send(targetAddress, timeout, buffer, pingOptions);
                }
                catch (PingException ex)
                {
                    string message = StringUtil.Format(TestConnectionResources.NoPingResult, resolvedTargetName, ex.Message);
                    Exception pingException = new PingException(message, ex.InnerException);
                    ErrorRecord errorRecord = new ErrorRecord(
                        pingException,
                        TestConnectionExceptionId,
                        ErrorCategory.ResourceUnavailable,
                        resolvedTargetName);
                    WriteError(errorRecord);

                    quietResult = false;
                    continue;
                }

                if (Continues.IsPresent)
                {
                    WriteObject(reply);
                }
                else if (Quiet.IsPresent)
                {
                    // Return 'true' only if all pings have completed successfully.
                    quietResult &= reply.Status == IPStatus.Success;
                }
                else
                {
                    pingReport.Replies.Add(reply);
                }

                // Delay between ping but not after last ping.
                if (i < Count && Delay > 0)
                {
                    Thread.Sleep(delay);
                }
            }

            if (Quiet.IsPresent)
            {
                WriteObject(quietResult);
            }
            else
            {
                WriteObject(pingReport);
            }
        }

        /// <summary>
        /// The class contains an information about the source, the destination and ping results.
        /// </summary>
        public class PingReport
        {
            internal PingReport(string source, string destination)
            {
                Source = source;
                Destination = destination;
                Replies = new List<PingReply>();
            }

            /// <summary>
            /// Source from which to ping.
            /// </summary>
            public string Source { get; }

            /// <summary>
            /// Destination to which to ping.
            /// </summary>
            public string Destination { get; }

            /// <summary>
            /// Ping results for every ping attempt.
            /// </summary>
            public List<PingReply> Replies { get; }
        }

        #endregion PingTest

        private bool InitProcessPing(string targetNameOrAddress, out string resolvedTargetName, out IPAddress targetAddress)
        {
            resolvedTargetName = targetNameOrAddress;

            IPHostEntry hostEntry;
            if (IPAddress.TryParse(targetNameOrAddress, out targetAddress))
            {
                if (ResolveDestination)
                {
                    hostEntry = Dns.GetHostEntry(targetNameOrAddress);
                    resolvedTargetName = hostEntry.HostName;
                }
            }
            else
            {
                try
                {
                    hostEntry = Dns.GetHostEntry(targetNameOrAddress);

                    if (ResolveDestination)
                    {
                        resolvedTargetName = hostEntry.HostName;
                        hostEntry = Dns.GetHostEntry(hostEntry.HostName);
                    }
                }
                catch (Exception ex)
                {
                    string message = StringUtil.Format(
                        TestConnectionResources.NoPingResult,
                        resolvedTargetName,
                        TestConnectionResources.CannotResolveTargetName);
                    Exception pingException = new PingException(message, ex);
                    ErrorRecord errorRecord = new ErrorRecord(
                        pingException,
                        TestConnectionExceptionId,
                        ErrorCategory.ResourceUnavailable,
                        resolvedTargetName);
                    WriteError(errorRecord);
                    return false;
                }

                if (IPv6 || IPv4)
                {
                    AddressFamily addressFamily = IPv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork;

                    foreach (var address in hostEntry.AddressList)
                    {
                        if (address.AddressFamily == addressFamily)
                        {
                            targetAddress = address;
                            break;
                        }
                    }

                    if (targetAddress == null)
                    {
                        string message = StringUtil.Format(
                            TestConnectionResources.NoPingResult,
                            resolvedTargetName,
                            TestConnectionResources.TargetAddressAbsent);
                        Exception pingException = new PingException(message, null);
                        ErrorRecord errorRecord = new ErrorRecord(
                            pingException,
                            TestConnectionExceptionId,
                            ErrorCategory.ResourceUnavailable,
                            resolvedTargetName);
                        WriteError(errorRecord);
                        return false;
                    }
                }
                else
                {
                    targetAddress = hostEntry.AddressList[0];
                }
            }

            return true;
        }

        // Users most often use the default buffer size so we cache the buffer.
        // Creates and filles a send buffer. This follows the ping.exe and CoreFX model.
        private byte[] GetSendBuffer(int bufferSize)
        {
            if (bufferSize == DefaultSendBufferSize && s_DefaultSendBuffer != null)
            {
                return s_DefaultSendBuffer;
            }

            byte[] sendBuffer = new byte[bufferSize];

            for (int i = 0; i < bufferSize; i++)
            {
                sendBuffer[i] = (byte)((int)'a' + i % 23);
            }

            if (bufferSize == DefaultSendBufferSize && s_DefaultSendBuffer == null)
            {
                s_DefaultSendBuffer = sendBuffer;
            }

            return sendBuffer;
        }

        /// <summary>
        /// IDisposable implementation, dispose of any disposable resources created by the cmdlet.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implementation of IDisposable for both manual Dispose() and finalizer-called disposal of resources.
        /// </summary>
        /// <param name="disposing">
        /// Specified as true when Dispose() was called, false if this is called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _sender.Dispose();
                }

                _disposed = true;
            }
        }

        // Count of pings sent per each trace route hop.
        // Default = 3 (from Windows).
        // If we change 'DefaultTraceRoutePingCount' we should change 'ConsoleTraceRouteReply' resource string.
        private const int DefaultTraceRoutePingCount = 3;

        /// Create the default send buffer once and cache it.
        private const int DefaultSendBufferSize = 32;
        private static byte[] s_DefaultSendBuffer = null;

        private bool _disposed;

        private readonly Ping _sender = new Ping();

        private const string TestConnectionExceptionId = "TestConnectionException";

        /// <summary>
        /// Finalizes an instance of the <see cref="TestConnectionCommand"/> class.
        /// </summary>
        ~TestConnectionCommand()
        {
            Dispose(disposing: false);
        }
    }
}
