/*
 * EBUninstaller Pro - Application Uninstaller & System Optimization Suite
 * HelperTools Shared Utilities Subsystem
 * Copyright (C) 2026 EBUninstaller Development Team & Contributors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Klocman
{
    /// <summary>
    /// High-throughput Named Pipe Inter-Process Communication (IPC) helper for helper sub-processes.
    /// </summary>
    internal static class InterProcessCommunication
    {
        private const string PipePrefix = "EBUninstaller_IPC_";

        /// <summary>
        /// Sends an object serialized as JSON through a named pipe asynchronously.
        /// </summary>
        public static async Task<bool> SendMessageAsync<T>(string pipeName, T payload, int timeoutMs = 10000, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(pipeName)) return false;

            try
            {
                var fullPipeName = pipeName.StartsWith(PipePrefix, StringComparison.OrdinalIgnoreCase)
                    ? pipeName
                    : PipePrefix + pipeName;

                using var pipeClient = new NamedPipeClientStream(".", fullPipeName, PipeDirection.Out, PipeOptions.Asynchronous);
                await pipeClient.ConnectAsync(timeoutMs, cancellationToken);

                var json = JsonSerializer.Serialize(payload);
                var bytes = Encoding.UTF8.GetBytes(json);

                // Write 4-byte length prefix then body
                var lenBytes = BitConverter.GetBytes(bytes.Length);
                await pipeClient.WriteAsync(lenBytes, 0, 4, cancellationToken);
                await pipeClient.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                await pipeClient.FlushAsync(cancellationToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Reads an object serialized as JSON from an incoming named pipe connection.
        /// </summary>
        public static async Task<T> ReceiveMessageAsync<T>(string pipeName, int timeoutMs = 15000, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(pipeName)) return default;

            var fullPipeName = pipeName.StartsWith(PipePrefix, StringComparison.OrdinalIgnoreCase)
                ? pipeName
                : PipePrefix + pipeName;

            using var pipeServer = new NamedPipeServerStream(fullPipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeoutMs);

            await pipeServer.WaitForConnectionAsync(cts.Token);

            var lenBuf = new byte[4];
            var bytesRead = await pipeServer.ReadAsync(lenBuf, 0, 4, cts.Token);
            if (bytesRead < 4) return default;

            var payloadLength = BitConverter.ToInt32(lenBuf, 0);
            if (payloadLength <= 0 || payloadLength > 50 * 1024 * 1024) return default; // 50 MB safety cap

            var payloadBuf = new byte[payloadLength];
            int totalRead = 0;
            while (totalRead < payloadLength)
            {
                int read = await pipeServer.ReadAsync(payloadBuf, totalRead, payloadLength - totalRead, cts.Token);
                if (read == 0) break;
                totalRead += read;
            }

            var json = Encoding.UTF8.GetString(payloadBuf, 0, totalRead);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
