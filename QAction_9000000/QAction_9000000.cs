using System;
using System.Collections.Generic;
using MyApi.MyExecutors;
using Skyline.DataMiner.ConnectorAPI.SkylineCommunications.HelloInterAppCalls.InterAppMessages;
using Skyline.DataMiner.ConnectorAPI.SkylineCommunications.HelloInterAppCalls.InterAppMessages.MyTable;
using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
using Skyline.DataMiner.Core.InterAppCalls.Common.Serializing;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class: ProcessInterAppReceived.
/// </summary>
public class QAction
{
    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocolExt protocol)
    {
        try
        {
            string raw = Convert.ToString(protocol.GetParameter(protocol.GetTriggerParameter()));
            protocol.Log($"QA{protocol.QActionID}|Run|Raw message: {raw}");

            var knownTypes = Types.KnownTypes;
            protocol.Log($"QA{protocol.QActionID}|Run|Known Types: {knownTypes}");

            var mySerializer = SerializerFactory.CreateInterAppSerializer(typeof(Message), knownTypes);

            Dictionary<Type, Type> messageToExecutorMapping = new Dictionary<Type, Type>
            {
                {typeof(CreateRow),typeof(CreateRowExecutor)},
            };
            var interAppCall = InterAppCallFactory.CreateFromRawAndAcceptMessage(raw, knownTypes);
            foreach (var message in interAppCall.Messages)
            {
                if (message.TryExecute(protocol, protocol, messageToExecutorMapping, out Message returnMessage) && returnMessage != null)
                {
                    returnMessage.Send(protocol.SLNet.RawConnection, message.ReturnAddress.AgentId, message.ReturnAddress.ElementId, message.ReturnAddress.ParameterId, mySerializer);
                }
            }
        }
        catch (Exception ex)
        {
            protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
        }
    }
}