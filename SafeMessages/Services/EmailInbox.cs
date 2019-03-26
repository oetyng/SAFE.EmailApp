using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SafeApp;
using SafeApp.Misc;
using SafeApp.Utilities;
using SafeMessages.Helpers;
using SafeMessages.Models;
using SafeMessages.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(EmailInbox))]

namespace SafeMessages.Services
{
    public class EmailInbox : IDisposable
    {
        readonly Session _session;

        public EmailInbox(Session session) => _session = session;

        public void Dispose() => GC.SuppressFinalize(this);

        public async Task<List<Message>> GetMessagesAsync(string userId)
        {
            var messages = new List<Message>();
            var appCont = await _session.AccessContainer.GetMDataInfoAsync("apps/" + AppService.AppId);
            var userIdByteList = userId.ToUtfBytes();
            var cipherBytes = await _session.MDataInfoActions.EncryptEntryKeyAsync(appCont, userIdByteList);
            var content = await _session.MData.GetValueAsync(appCont, cipherBytes);
            var plainBytes = await _session.MDataInfoActions.DecryptAsync(appCont, content.Item1);

            var msgBox = JsonConvert.DeserializeObject<MessageBox>(plainBytes.ToUtfString());
            var inboxInfo = await _session.MDataInfoActions.DeserialiseAsync(msgBox.Inbox.Data);
            var inboxEntKeys = await _session.MData.ListKeysAsync(inboxInfo);

            using (var inboxPkH = await _session.Crypto.EncPubKeyNewAsync(msgBox.EmailEncPk.ToHexBytes().ToArray()))
            using (var inboxSkH = await _session.Crypto.EncSecretKeyNewAsync(msgBox.EmailEncSk.ToHexBytes().ToArray()))
            {
                foreach (var key in inboxEntKeys)
                {
                    try
                    {
                        var entryKey = key.Key.ToUtfString();
                        if (entryKey == "__email_enc_pk")
                            continue;

                        var value = await _session.MData.GetValueAsync(inboxInfo, key.Key);
                        var iDataNameEncoded =
                            await _session.Crypto.DecryptSealedBoxAsync(value.Item1, inboxPkH, inboxSkH);
                        var iDataNameBytes = iDataNameEncoded.ToUtfString().Split(',')
                            .Select(val => Convert.ToByte(val)).ToArray();
                        using (var msgDataReaderH = await _session.IData.FetchSelfEncryptorAsync(iDataNameBytes))
                        {
                            var msgSize = await _session.IData.SizeAsync(msgDataReaderH);
                            var msgCipher = await _session.IData.ReadFromSelfEncryptorAsync(msgDataReaderH, 0, msgSize);
                            var plainTextMsg = await _session.Crypto.DecryptSealedBoxAsync(msgCipher, inboxPkH, inboxSkH);
                            var jsonMsgData = plainTextMsg.ToUtfString();
                            messages.Add(JsonConvert.DeserializeObject<Message>(jsonMsgData));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Exception: " + e.Message);
                    }
                }
            }

            return messages;
        }

        public async Task SendMessageAsync(string to, Message msg)
        {
            var dstPubIdDigest = await Crypto.Sha3HashAsync(to.ToUtfBytes());
            var dstPubIdMDataInfoH = new MDataInfo { Name = dstPubIdDigest.ToArray(), TypeTag = 15001 };
            var serDstMsgMDataInfo = await _session.MData.GetValueAsync(dstPubIdMDataInfoH, "@email".ToUtfBytes());
            var dstInboxMDataInfoH = await _session.MDataInfoActions.DeserialiseAsync(serDstMsgMDataInfo.Item1);
            var dstInboxPkItem = await _session.MData.GetValueAsync(dstInboxMDataInfoH, "__email_enc_pk".ToUtfBytes());

            using (var dstInboxPkH = await _session.Crypto.EncPubKeyNewAsync(dstInboxPkItem.Item1.ToUtfString().ToHexBytes().ToArray()))
            {
                var plainTxtMsg = JsonConvert.SerializeObject(msg);
                var cipherTxt = await _session.Crypto.EncryptSealedBoxAsync(plainTxtMsg.ToUtfBytes(), dstInboxPkH);
                byte[] msgDataMapNameBytes;

                using (var msgWriterH = await _session.IData.NewSelfEncryptorAsync())
                {
                    await _session.IData.WriteToSelfEncryptorAsync(msgWriterH, cipherTxt);
                    using (var msgCipherOptH = await _session.CipherOpt.NewPlaintextAsync())
                        msgDataMapNameBytes = await _session.IData.CloseSelfEncryptorAsync(msgWriterH, msgCipherOptH);
                }

                var encodedString = string.Join(", ", msgDataMapNameBytes.Select(val => val));
                var msgDataMapNameCipher = await _session.Crypto.EncryptSealedBoxAsync(encodedString.ToUtfBytes(), dstInboxPkH);

                using (var dstMsgEntActH = await _session.MDataEntryActions.NewAsync())
                {
                    await _session.MDataEntryActions.InsertAsync(dstMsgEntActH, RandomGenerator.RandomString(15).ToUtfBytes(), msgDataMapNameCipher);
                    await _session.MData.MutateEntriesAsync(dstInboxMDataInfoH, dstMsgEntActH);
                }
            }
        }
    }
}