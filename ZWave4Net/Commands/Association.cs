﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZWave4Net.Commands
{
    public class Association : CommandClass
    {
        enum associationCmd
        {
            Set = 0x01,
            Get = 0x02,
            Report = 0x03,
            Remove = 0x04,
            GroupingsGet = 0x05,
            GroupingsReport = 0x06
        }

        public Association(Node node) : base(0x85, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(associationCmd)).Cast<Enum>().ToArray(); }
        }

        public async Task<AssociationValue> Get(byte groupID)
        {
            var response = await Invoker.Send(new Command(ClassID, associationCmd.Get, groupID));
            return AssociationValue.Parse(response.Payload);
        }

        public Task Add(byte groupID, byte NodeID)
        {
            return Invoker.Post(new Command(ClassID, associationCmd.Set, groupID, NodeID));
        }

        public Task Remove(byte groupID, byte NodeID)
        {
            return Invoker.Post(new Command(ClassID, associationCmd.Remove, groupID, NodeID));
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            if (object.Equals(request, associationCmd.Get) && object.Equals(response, associationCmd.Report))
                return true;

            return false;
        }

        protected override void OnEvent(Enum @event, byte[] payload)
        {
            var value = AlarmValue.Parse(payload);
            Platform.LogMessage(LogLevel.Debug, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {@event}, {value}"));
        }
    }

}