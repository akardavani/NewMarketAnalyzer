using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MarketAnalyzer.Infrastructure.Messaging.Candoo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [ServiceContractAttribute(Namespace = "urn:SMSAPIwsdl", ConfigurationName = "Candoo.SMSAPIPortType")]
    public interface SMSAPIPortType
    {

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#Send", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<MultipleSendResult[]> SendMultipleAsync(string username, string password,
            string[] srcNumbers, string[] bodys, string[] destNos, string[] flashs, long[] checkingMessageIds);

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#Send", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<SendResult[]> SendAsync(string username, string password, string srcNumber,
            string body, string[] destNo, string flash);

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#ViewReceive", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<ReceiveResult[]> ViewReceiveAsync(string username, string password, string number,
            string id);

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#DeliveryReport", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<DeliveryResult[]> DeliveryReportAsync(string username, string password,
            string srcNumber, string id, string fromid);

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#getStatuses", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<string[]> GetStatusesAsync(string username, string password, long[] checkingIds);

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#Acknowledge", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<string[]> AcknowledgeAsync(string username, string password, long[] checkingIds);

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#getStatuses", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<string[]> getStatuses_OriginalIDAsync(string username, string password,
            long[] checkingIds);

        [OperationContractAttribute(Action = "urn:SMSAPIwsdl#return account balance", ReplyAction = "*")]
        [XmlSerializerFormatAttribute(Style = OperationFormatStyle.Rpc, SupportFaults = true,
            Use = OperationFormatUse.Encoded)]
        [ServiceKnownTypeAttribute(typeof(DeliveryResult))]
        [ServiceKnownTypeAttribute(typeof(ReceiveResult))]
        [ServiceKnownTypeAttribute(typeof(SendResult))]
        [ServiceKnownTypeAttribute(typeof(MultipleSendResult))]
        [return: MessageParameterAttribute(Name = "return")]
        System.Threading.Tasks.Task<string> BalanceAsync(string username, string password);
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "urn:SMSAPIwsdl")]
    public class MultipleSendResult
    {
        /// <remarks/>
        public string Id { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "urn:SMSAPIwsdl")]
    public class DeliveryResult
    {
        /// <remarks/>
        public string Id { get; set; }

        /// <remarks/>
        public string Status { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "urn:SMSAPIwsdl")]
    public class ReceiveResult
    {
        /// <remarks/>
        public string Id { get; set; }

        /// <remarks/>
        public string From { get; set; }

        /// <remarks/>
        public string To { get; set; }

        /// <remarks/>
        public string Date { get; set; }

        /// <remarks/>
        public string Body { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace = "urn:SMSAPIwsdl")]
    public class SendResult
    {
        /// <remarks/>
        public string Id { get; set; }

        /// <remarks/>
        public string Mobile { get; set; }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public class SMSAPIPortTypeClient : ClientBase<SMSAPIPortType>, SMSAPIPortType
    {
        public SMSAPIPortTypeClient(Binding binding, EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public System.Threading.Tasks.Task<MultipleSendResult[]> SendMultipleAsync(string username, string password,
            string[] srcNumbers, string[] bodys, string[] destNos, string[] flashs, long[] checkingMessageIds)
        {
            return Channel.SendMultipleAsync(username, password, srcNumbers, bodys, destNos, flashs,
                checkingMessageIds);
        }

        public System.Threading.Tasks.Task<SendResult[]> SendAsync(string username, string password, string srcNumber, string body, string[] destNo, string flash)
        {
            return Channel.SendAsync(username, password, srcNumber, body, destNo, flash);
        }

        public System.Threading.Tasks.Task<ReceiveResult[]> ViewReceiveAsync(string username, string password, string number, string id)
        {
            return Channel.ViewReceiveAsync(username, password, number, id);
        }

        public System.Threading.Tasks.Task<DeliveryResult[]> DeliveryReportAsync(string username, string password, string srcNumber, string id, string fromid)
        {
            return Channel.DeliveryReportAsync(username, password, srcNumber, id, fromid);
        }

        public System.Threading.Tasks.Task<string[]> GetStatusesAsync(string username, string password, long[] checkingIds)
        {
            return Channel.GetStatusesAsync(username, password, checkingIds);
        }

        public System.Threading.Tasks.Task<string[]> AcknowledgeAsync(string username, string password, long[] checkingIds)
        {
            return Channel.AcknowledgeAsync(username, password, checkingIds);
        }

        public System.Threading.Tasks.Task<string[]> getStatuses_OriginalIDAsync(string username, string password, long[] checkingIds)
        {
            return Channel.getStatuses_OriginalIDAsync(username, password, checkingIds);
        }

        public System.Threading.Tasks.Task<string> BalanceAsync(string username, string password)
        {
            return Channel.BalanceAsync(username, password);
        }

        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((ICommunicationObject)(this)).BeginOpen(null, null),
                ((ICommunicationObject)(this)).EndOpen);
        }

        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((ICommunicationObject)(this)).BeginClose(null, null),
                ((ICommunicationObject)(this)).EndClose);
        }

        private static Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SMSAPIPort))
            {
                BasicHttpBinding result = new BasicHttpBinding
                {
                    MaxBufferSize = int.MaxValue,
                    ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                    MaxReceivedMessageSize = int.MaxValue,
                    AllowCookies = true,
                    Security =
                    {
                        Mode = BasicHttpSecurityMode.Transport
                    },
                };
                return result;
            }
            throw new System.InvalidOperationException(
                $"Could not find endpoint with name \'{endpointConfiguration}\'.");
        }

        private static EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.SMSAPIPort))
            {
                return new EndpointAddress("https://my.candoosms.com/services/index.php");
            }
            throw new System.InvalidOperationException(
                $"Could not find endpoint with name \'{endpointConfiguration}\'.");
        }

        public static Binding GetDefaultBinding()
        {
            return GetBindingForEndpoint(EndpointConfiguration.SMSAPIPort);
        }

        public static EndpointAddress GetDefaultEndpointAddress()
        {
            return GetEndpointAddress(EndpointConfiguration.SMSAPIPort);
        }

        public enum EndpointConfiguration
        {

            SMSAPIPort,
        }
    }
}
