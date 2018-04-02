﻿using LunaCommon.Enums;
using LunaCommon.Message.Data.Admin;
using LunaCommon.Message.Interface;
using LunaCommon.Message.Server;
using LunaCommon.Message.Types;
using Server.Client;
using Server.Command;
using Server.Context;
using Server.Log;
using Server.Message.Reader.Base;
using Server.Server;
using Server.Settings;
using System;

namespace Server.Message.Reader
{
    public class AdminMsgReader : ReaderBase
    {
        public override void HandleMessage(ClientStructure client, IClientMessageBase message)
        {
            var messageData = (AdminBaseMsgData)message.Data;
            if (!string.IsNullOrEmpty(GeneralSettings.SettingsStore.AdminPassword) && GeneralSettings.SettingsStore.AdminPassword == messageData.AdminPassword)
            {
                var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<AdminReplyMsgData>();
                switch (messageData.AdminMessageType)
                {
                    case AdminMessageType.Ban:
                        LunaLog.Debug($"{client.PlayerName}: Requested a ban against {((AdminBanMsgData)message.Data).PlayerName}");
                        msgData.Response = CommandHandler.Commands["ban"].Func(((AdminBanMsgData)message.Data).PlayerName) ? AdminResponse.Ok : AdminResponse.Error;
                        break;
                    case AdminMessageType.Kick:
                        LunaLog.Debug($"{client.PlayerName}: Requested a kick against {((AdminKickMsgData)message.Data).PlayerName}");
                        msgData.Response = CommandHandler.Commands["kick"].Func(((AdminKickMsgData)message.Data).PlayerName) ? AdminResponse.Ok : AdminResponse.Error;
                        break;
                    case AdminMessageType.Dekessler:
                        LunaLog.Debug($"{client.PlayerName}: Requested a dekessler");
                        CommandHandler.Commands["dekessler"].Func(null);
                        msgData.Response = AdminResponse.Ok;
                        break;
                    case AdminMessageType.Nuke:
                        LunaLog.Debug($"{client.PlayerName}: Requested a nuke");
                        CommandHandler.Commands["nukeksc"].Func(null);
                        msgData.Response = AdminResponse.Ok;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                MessageQueuer.SendToClient<AdminSrvMsg>(client, msgData);
            }
            else
            {
                LunaLog.Warning($"{client.PlayerName}: Tried to run an admin command with an invalid password");

                var msgData = ServerContext.ServerMessageFactory.CreateNewMessageData<AdminReplyMsgData>();
                msgData.Response = AdminResponse.InvalidPassword;
                MessageQueuer.SendToClient<AdminSrvMsg>(client, msgData);
            }
        }
    }
}
