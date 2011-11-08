using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using BuildingBlocks.Common.Utils;

namespace BuildingBlocks.Wcf.Helpers
{
    public static class MessageHelpers
    {
        private const string DefaultNamespace = "http://tempuri.org/";

        public static void UpdateHeader(this MessageHeaders messageHeaders, string name, string value, string ns = DefaultNamespace)
        {
            var header = MessageHeader.CreateHeader(name, ns ?? DefaultNamespace, value);

            var index = messageHeaders.FindIndex(h => h.Name == name && h.Namespace == ns);
            if (index >= 0)
            {
                messageHeaders.RemoveAt(index);
                messageHeaders.Insert(index, header);
            }
            else
            {
                messageHeaders.Add(header);
            }
        }

        public static T GetHeaderOrDefault<T>(this MessageHeaders messageHeaders, string name, string ns = null)
        {
            if (ns == null)
            {
                var index = messageHeaders.FindIndex(h => h.Name == name);
                if (index >= 0)
                    return messageHeaders.GetHeader<T>(index);
            }
            else
            {
                try
                {
                    return messageHeaders.GetHeader<T>(name, ns);
                }
                catch (MessageHeaderException)
                {
                }
            }

            return default(T);
        }

        public static T GetHeaderOrDefault<T>(this MessageHeaders messageHeaders, Func<MessageHeaderInfo, bool> condition)
        {
            var index = messageHeaders.FindIndex(condition);
            return index >= 0 ? messageHeaders.GetHeader<T>(index) : default(T);
        }
    }
}