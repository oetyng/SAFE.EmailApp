# SAFE.EmailApp 
SAFE.EmailApp is a fork of https://github.com/maidsafe/safe-email-app-csharp - an example of a simple email application. 
SAFE.EmailApp will extend the functionality to become a more capable solution for messaging.

## Features
- First step would be to Authorize the SAFE email app by authenticating it using the [SAFE Authenticator mobile](https://github.com/maidsafe/safe-authenticator-mobile). 
- Create a Public ID which would act as a unique identifier on the SAFE email app. Option to create multiple Public ID's that could be used to create different communication channels. For instance different accounts for personal & official use.
- Compose an email and send it to another user by addressing it to their Public ID.
- Receive & reply to an email from another user.

### Extended functionality:
- Increased message body length to 1500 chars.
- Includes the message body of message being replied to, when opening reply-view, formatting with date and name of the sender.
### Coming up:
- Use [SAFE.AppendOnlyDb](https://github.com/oetyng/SAFE.AppendOnlyDb) IStreamADs for limitless size storage.
- Contact book (based on IStreamAD).
- Conversation streams for each contact (based on IStreamAD).
- Topic threading.
- Paging of inbox / topic threads.
- Searching.

### Application Data Model

*Coming up*

### Legacy Application Data Model
This is how it's implemented in the example, and currently in this fork as well, but it will be changed.

The following diagram depicts how the emails are stored in the SAFE network, as well as how the email app stores email accounts information.

![Email App Data Model](/design/EmailApp-DataModel.png)

## Building

### Pre-requisites
Requires [SAFE Authenticator mobile](https://github.com/maidsafe/safe-authenticator-mobile) to authenticate itself into the SAFE network.

If building on Visual Studio 2017, you will need the following SDKs and workloads installed:

### Workloads required:
- Xamarin

### Required SDK/Tools
- Android 9.0 SDK
- Latest Xcode

### Supported Platforms
- Android 4.2+ (armeabi-v7a, x86_64)
- iOS 8+ (ARM64, x64)

## Further Help
Get your developer related questions clarified on [SAFE Dev Forum](https://forum.safedev.org/). If your looking to share any other ideas or thoughts on the SAFENetwork you can reach out on [SAFE Network Forum](https://safenetforum.org/)

## License
This SAFE Network library is dual-licensed under the Modified BSD ([LICENSE-BSD](LICENSE-BSD) https://opensource.org/licenses/BSD-3-Clause) or the MIT license ([LICENSE-MIT](LICENSE-MIT) https://opensource.org/licenses/MIT) at your option.

## Contribution
Copyrights in the SAFE Network are retained by their contributors. No copyright assignment is required to contribute to this project.
