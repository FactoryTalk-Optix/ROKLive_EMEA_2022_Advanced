#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.Core;
using FTOptix.CoreBase;
using FTOptix.NetLogic;
using FTOptix.DataLogger;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Utilities;
using System.Text.RegularExpressions;
using FTOptix.Report;
#endregion

namespace Utilities {
    public class FileSignAndVerifyUtilities {
        public static string GetFormattedKey(string key) {
            string result = "";
            int offset = 0;
            const int LINE_LENGTH = 64;
            while (offset < key.Length) {
                var lineEnd = Math.Min(offset + LINE_LENGTH, key.Length);
                result += key.Substring(offset, lineEnd - offset) + Environment.NewLine;
                offset = lineEnd;
            }

            return result.Remove(result.Length - 1, 1);
        }

        public static string ResourceUriValueToAbsoluteFilePath(UAValue value) {
            var resourceUri = new ResourceUri(value);
            return resourceUri.Uri;
        }

        public static void ExportPrivateKey(RSA rsa, string outputPath) {
            var privateKeyBytes = rsa.ExportRSAPrivateKey();

            const string header = "-----BEGIN RSA PRIVATE KEY-----";
            const string footer = "-----END RSA PRIVATE KEY-----";

            var builder = new StringBuilder();
            builder.AppendLine(header);

            var privateKeyString = Convert.ToBase64String(privateKeyBytes);
            string key = FileSignAndVerifyUtilities.GetFormattedKey(privateKeyString);
            builder.AppendLine(key);
            builder.Append(footer);

            var filePath = outputPath;
            File.WriteAllText(filePath, builder.ToString());
        }

        public static void ExportPublicKey(RSA rsa, string outputPath) {
            const string header = "-----BEGIN PUBLIC KEY-----";
            const string footer = "-----END PUBLIC KEY-----";

            var builder = new StringBuilder();
            builder.AppendLine(header);

            var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
            var publicKeyString = Convert.ToBase64String(publicKeyBytes);
            builder.AppendLine(FileSignAndVerifyUtilities.GetFormattedKey(publicKeyString));
            builder.Append(footer);

            var filePath = outputPath;
            File.WriteAllText(filePath, builder.ToString());
        }

        public static string SpliceText(string text, int lineLength) {
            return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }
    }
}

public class FileSignVerify : BaseNetLogic {
    public override void Start() {
    }

    public override void Stop() {
    }

    [ExportMethod]
    public void SignFile() {
        LogicObject.GetVariable("VerifyResult").Value = 0;
        string filePath = LogicObject.GetVariable("SourceFile").Value;
        LoadPrivateKey();
        if (privateKey == null) {
            Log.Warning("FileSignVerify", "Missing private key. Trying to generate one.");
            GeneratePublicAndPrivateKey();
            LoadPrivateKey();
            if (privateKey == null) {
                Log.Error("FileSignVerify", "Missing private key. Unable to sign file.");
                return;
            }
        }

        var fileAbsolutePath = FileSignAndVerifyUtilities.ResourceUriValueToAbsoluteFilePath(filePath);
        if (!File.Exists(fileAbsolutePath))
            Log.Error("FileSignVerify", "Unable to find file to sign");

        HashAlgorithm hasher = SHA256.Create();
        byte[] hash = hasher.ComputeHash(File.ReadAllBytes(fileAbsolutePath));
        byte[] signature = privateKey.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var base64String = Convert.ToBase64String(signature);
        var signatureFilePath = fileAbsolutePath + ".sign";
        File.WriteAllText(signatureFilePath, FileSignAndVerifyUtilities.SpliceText(base64String, 64));
        Log.Info("FileSignVerify", "Hash file was successfully calculated and saved.");
    }

    [ExportMethod]
    public void VerifyFileSignature() {
        LogicObject.GetVariable("VerifyResult").Value = 0;
        string filePath = LogicObject.GetVariable("SourceFile").Value;
        string signatureFilePath = LogicObject.GetVariable("HashFile").Value;

        LoadPublicKey();
        if (publicKey == null) {
            LogicObject.GetVariable("VerifyResult").Value = 2;
            return;
        }

        var fileAbsolutePath = FileSignAndVerifyUtilities.ResourceUriValueToAbsoluteFilePath(filePath);
        var signatureFileAbsolutePath = FileSignAndVerifyUtilities.ResourceUriValueToAbsoluteFilePath(signatureFilePath);
        if (!File.Exists(fileAbsolutePath)) {
            Log.Error("FileSignVerify", "Unable to locate file to verify.");
            LogicObject.GetVariable("VerifyResult").Value = 2;
            return;
        }

        if (!File.Exists(signatureFileAbsolutePath)) {
            Log.Error("FileSignVerify", "Unable to locate signature file.");
            LogicObject.GetVariable("VerifyResult").Value = 2;
            return;
        }

        var signature = File.ReadAllText(signatureFileAbsolutePath);
        var result = publicKey.VerifyData(File.ReadAllBytes(fileAbsolutePath),
                                          Convert.FromBase64String(signature),
                                          HashAlgorithmName.SHA256,
                                          RSASignaturePadding.Pkcs1);
        if (result) {
            Log.Info("FileSignVerify", "File was successfully verified");
            LogicObject.GetVariable("VerifyResult").Value = 1;
        } else {
            Log.Info("FileSignVerify", "File cannot be verified");
            LogicObject.GetVariable("VerifyResult").Value = 2;
        }
    }

    [ExportMethod]
    public void GeneratePublicAndPrivateKey() {
        // Check if output directory exists
        bool exists = System.IO.Directory.Exists(Path.GetDirectoryName(GetPrivateKeyPath()));
        if (!exists) {
            try {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(GetPrivateKeyPath()));
            } catch (Exception ex) {
                Log.Error("FileSignVerify", ex.ToString());
                return;
            }
        }
        // Check if output directory exists
        exists = System.IO.Directory.Exists(Path.GetDirectoryName(GetPublicKeyPath()));
        if (!exists) {
            try {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(GetPublicKeyPath()));
            } catch (Exception ex) {
                Log.Error("FileSignVerify", ex.ToString());
                return;
            }
        }

        try {
            RSA rsa = RSA.Create(2048);
            FileSignAndVerifyUtilities.ExportPrivateKey(rsa, GetPrivateKeyPath());
            FileSignAndVerifyUtilities.ExportPublicKey(rsa, GetPublicKeyPath());
            Log.Info("FileSignVerify", "Certificates were successfully generated");
        } catch (Exception ex) {
            Log.Error("FileSignVerify", ex.ToString());
        }
    }

    private void LoadPrivateKey() {
        var privateKeyPath = GetPrivateKeyPath();
        if (!File.Exists(privateKeyPath))
            return;

        privateKey = RSA.Create();
        string privateKeyString = File.ReadAllText(privateKeyPath);
        privateKey.ImportFromPem(privateKeyString.ToCharArray());
    }

    private void LoadPublicKey() {
        var publicKeyPath = GetPublicKeyPath();
        if (!File.Exists(publicKeyPath)) {
            Log.Error("FileSignVerify", "Missing public key. Unable to verify file.");
            return;
        }

        publicKey = RSA.Create();
        string publicKeyString = File.ReadAllText(publicKeyPath);
        try {
            publicKey.ImportFromPem(publicKeyString.ToCharArray());
        } catch (Exception ex) {
            Log.Error("FileSignVerify", "Error: " + ex.ToString());
            return;
        }
    }

    private string GetPrivateKeyPath() {
        return FileSignAndVerifyUtilities.ResourceUriValueToAbsoluteFilePath(LogicObject.GetVariable("PrivateKey").Value);
    }

    private string GetPublicKeyPath() {
        return FileSignAndVerifyUtilities.ResourceUriValueToAbsoluteFilePath(LogicObject.GetVariable("PublicKey").Value);
    }

    private RSA privateKey;
    private RSA publicKey;
}
