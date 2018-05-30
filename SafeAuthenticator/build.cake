#addin Cake.Curl

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var ANDROID_X86  = "android-x86";
var ANDROID_ARMEABI_V7A  = "android-armeabiv7a";
var ANDROID_ARCHITECTURES  = new string[]{ANDROID_X86, ANDROID_ARMEABI_V7A};
var IOS_ARCHITECTURES  = new string[]{"ios"};
var All_ARCHITECTURES = new string[][]{ANDROID_ARCHITECTURES, IOS_ARCHITECTURES};
enum Enviornment {Android,iOS}

// --------------------------------------------------------------------------------
// Native lib directory
// --------------------------------------------------------------------------------

var TAG = "6be5558";
var androidLibDirectory = Directory("SafeAuthenticator.Android/lib/");
var iosLibDirectory = Directory("SafeAuthenticator.iOS/Native References/");
var Native_DIR = Directory(string.Concat(EnvironmentVariable("TEMP"), "/nativeauthlibs"));

// --------------------------------------------------------------------------------
// PREPARATION
// --------------------------------------------------------------------------------

Task("Clean")
    .Does(() =>
{
	CleanDirectories(Native_DIR);
    CleanDirectory(androidLibDirectory);
    CleanDirectory(iosLibDirectory);
});


// --------------------------------------------------------------------------------
// Download Libs
// --------------------------------------------------------------------------------

Task("DownloadTask")
	.IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var item in Enum.GetValues(typeof(Enviornment)))
    {
        string[] targets = null;
        Information(string.Format("\n {0} ",item));
        switch (item)
        {   
            case Enviornment.Android:
                targets = ANDROID_ARCHITECTURES;
                break;
            case Enviornment.iOS:
                targets = IOS_ARCHITECTURES;
                break;
        }

        foreach (var target in targets)
        {   
            var targetdirectory =  string.Format("{0}/{1}/{2}", Native_DIR.Path,item, target);
            var zipdownloadurl = string.Format("https://s3.eu-west-2.amazonaws.com/safe-client-libs/safe_authenticator-{0}-{1}.zip", TAG, target);
			var zipsavepath = string.Format("{0}/{1}/{2}/safe_authenticator-{3}-{4}.zip", Native_DIR.Path,item, target, TAG,target);

            if(!DirectoryExists(targetdirectory))
                CreateDirectory(targetdirectory);

            if(!FileExists(zipsavepath))
            {
                CurlDownloadFiles(
                    new[]
                    {
                        new Uri(zipdownloadurl)
                    },
                new CurlDownloadSettings
                {
                    OutputPaths = new FilePath[]
                    {
                        zipsavepath
                    }
                });
            }
        }
    }
})
.ReportError(exception =>
{  
    Information(exception.Message);
});


Task("UnZipTask")
    .IsDependentOn("DownloadTask")
    .Does(() =>
{
    foreach (var item in Enum.GetValues(typeof(Enviornment)))
    {
        string[] targets = null;
        var outputdirectory = string.Empty;
        Information(string.Format("\n {0} ",item));
        switch (item)
        {
            case Enviornment.Android:
                targets = ANDROID_ARCHITECTURES;
                outputdirectory = androidLibDirectory.Path.FullPath.ToString();
                break;
            case Enviornment.iOS:
                targets = IOS_ARCHITECTURES;
                outputdirectory = iosLibDirectory.Path.FullPath.ToString();
                break;
        }

        CleanDirectories(outputdirectory);

        foreach (var target in targets)
        {
            var zipdirectorysource = Directory(string.Format("{0}/{1}/{2}", Native_DIR.Path,item, target));
            var zipfiles =  GetFiles(string.Format("{0}/*.*" , zipdirectorysource));
            foreach (var zip in zipfiles)
            {
                var filename = zip.GetFilename();
                Information(" Unzipping : " + filename);
                var platformoutputdirecotory = new StringBuilder();
                platformoutputdirecotory.Append(outputdirectory);

                if(target.Equals(ANDROID_X86))
                    platformoutputdirecotory.Append("/x86");
                else if(target.Equals(ANDROID_ARMEABI_V7A))
                    platformoutputdirecotory.Append("/armeabi-v7a");
                
                Unzip(zip, platformoutputdirecotory.ToString());
            }
        }
    }
})
.ReportError(exception =>
{  
    Information(exception.Message);
});


Task("Default")
    .IsDependentOn("UnZipTask")
    .Does(() =>
{
    
});

RunTarget(target);