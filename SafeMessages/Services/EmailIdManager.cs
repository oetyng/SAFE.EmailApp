using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SafeApp;
using SafeApp.Misc;
using SafeApp.Utilities;
using SafeMessages.Models;
using SafeMessages.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(EmailInbox))]

namespace SafeMessages.Services
{
    public class EmailIdManager : IDisposable
    {
        readonly Session _session;

        public EmailIdManager(Session session) => _session = session;

        public void Dispose() => GC.SuppressFinalize(this);

        public async Task<List<UserId>> GetIdsAsync()
        {
            var ids = new List<UserId>();
            var appContH = await _session.AccessContainer.GetMDataInfoAsync("apps/" + AppConstants.AppId);
            var appContEntKeys = await _session.MData.ListKeysAsync(appContH);
            foreach (var cipherTextEntKey in appContEntKeys)
            {
                try
                {
                    var plainTextEntKey = await _session.MDataInfoActions.DecryptAsync(appContH, cipherTextEntKey.Key);
                    ids.Add(new UserId(plainTextEntKey.ToUtfString()));
                }
                catch (Exception)
                {
                    // ignore incompatible entries.
                }
            }

            return ids;
        }

        public async Task AddIdAsync(string userId)
        {
            if (userId.Contains(".") || userId.Contains("@"))
                throw new NotSupportedException("Unsupported characters '.' and '@'.");

            // Check if account exits first and return error
            var dstPubIdDigest = await Crypto.Sha3HashAsync(userId.ToUtfBytes());
            var dstPubIdMDataInfoH = new MDataInfo { Name = dstPubIdDigest.ToArray(), TypeTag = 15001 };
            var accountExists = false;
            try
            {
                await _session.MData.ListKeysAsync(dstPubIdMDataInfoH);
                accountExists = true;
            }
            catch (Exception)
            {
                // ignored - acct not found
            }

            if (accountExists)
                throw new Exception("Id already exists.");

            // Create Self Permissions to Inbox and Archive
            using (var inboxSelfPermSetH = await _session.MDataPermissions.NewAsync())
            {
                using (var inboxPermH = await _session.MDataPermissions.NewAsync())
                {
                    using (var appSignPkH = await _session.Crypto.AppPubSignKeyAsync())
                    {
                        await _session.MDataPermissions.InsertAsync(
                            inboxSelfPermSetH,
                            appSignPkH,
                            new PermissionSet
                            { Delete = true, Insert = true, ManagePermissions = true, Read = true, Update = true });
                    }

                    // Create Archive MD
                    var archiveMDataInfoH = await _session.MDataInfoActions.RandomPrivateAsync(15001);
                    await _session.MData.PutAsync(archiveMDataInfoH, inboxPermH, NativeHandle.EmptyMDataEntries);
                    var serArchiveMdInfo = await _session.MDataInfoActions.SerialiseAsync(archiveMDataInfoH);

                    // Update Inbox permisions to allow anyone to insert
                    await _session.MDataPermissions.InsertAsync(inboxPermH, NativeHandle.AnyOne, new PermissionSet { Insert = true });

                    // Create Inbox MD
                    var (inboxEncPk, inboxEncSk) = await _session.Crypto.EncGenerateKeyPairAsync();
                    using (inboxEncSk)
                    using (inboxEncPk)
                    {
                        List<byte> serInboxMdInfo;
                        var inboxEncPkRaw = await _session.Crypto.EncPubKeyGetAsync(inboxEncPk);
                        var inboxEncSkRaw = await _session.Crypto.EncSecretKeyGetAsync(inboxEncSk);

                        var inboxEmailPkEntryKey = "__email_enc_pk".ToUtfBytes();
                        using (var inboxEntriesH = await _session.MDataEntries.NewAsync())
                        {
                            await _session.MDataEntries.InsertAsync(
                                inboxEntriesH,
                                inboxEmailPkEntryKey,
                                inboxEncPkRaw.ToList().ToHexString().ToUtfBytes());
                            var inboxMDataInfoH = await _session.MDataInfoActions.RandomPublicAsync(15001);
                            await _session.MData.PutAsync(inboxMDataInfoH, inboxPermH, inboxEntriesH);
                            serInboxMdInfo = await _session.MDataInfoActions.SerialiseAsync(inboxMDataInfoH);
                        }

                        // Create Public Id MD
                        var idDigest = await Crypto.Sha3HashAsync(userId.ToUtfBytes());
                        var pubIdMDataInfoH = new MDataInfo { Name = idDigest.ToArray(), TypeTag = 15001 };
                        using (var pubIdEntriesH = await _session.MDataEntries.NewAsync())
                        {
                            await _session.MDataEntries.InsertAsync(pubIdEntriesH, "@email".ToUtfBytes(), serInboxMdInfo);
                            await _session.MData.PutAsync(pubIdMDataInfoH, inboxPermH, pubIdEntriesH);
                        }

                        // Update _publicNames Container
                        var publicNamesContH = await _session.AccessContainer.GetMDataInfoAsync("_publicNames");
                        var pubNamesUserIdCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryKeyAsync(publicNamesContH, userId.ToUtfBytes());
                        var pubNamesMsgBoxCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryValueAsync(publicNamesContH, idDigest);
                        using (var pubNamesContEntActH = await _session.MDataEntryActions.NewAsync())
                        {
                            await _session.MDataEntryActions.InsertAsync(pubNamesContEntActH, pubNamesUserIdCipherBytes, pubNamesMsgBoxCipherBytes);
                            await _session.MData.MutateEntriesAsync(publicNamesContH, pubNamesContEntActH);
                        }

                        // Finally update App Container
                        var msgBox = new MessageBox
                        {
                            EmailId = userId,
                            Inbox = new DataArray { Type = "Buffer", Data = serInboxMdInfo },
                            Archive = new DataArray { Type = "Buffer", Data = serArchiveMdInfo },
                            EmailEncPk = inboxEncPkRaw.ToList().ToHexString(),
                            EmailEncSk = inboxEncSkRaw.ToList().ToHexString()
                        };

                        var msgBoxSer = JsonConvert.SerializeObject(msgBox);
                        var appContH = await _session.AccessContainer.GetMDataInfoAsync("apps/" + AppConstants.AppId);
                        var userIdCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryKeyAsync(appContH, userId.ToUtfBytes());
                        var msgBoxCipherBytes =
                            await _session.MDataInfoActions.EncryptEntryValueAsync(appContH, msgBoxSer.ToUtfBytes());
                        using (var appContEntActH = await _session.MDataEntryActions.NewAsync())
                        {
                            await _session.MDataEntryActions.InsertAsync(appContEntActH, userIdCipherBytes, msgBoxCipherBytes);
                            await _session.MData.MutateEntriesAsync(appContH, appContEntActH);
                        }
                    }
                }
            }
        }
    }
}