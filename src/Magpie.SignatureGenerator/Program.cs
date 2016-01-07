using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Magpie.SignatureGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!args.Any())
            {
                Usage();
//                Console.ReadKey();
                return;
            }
            try
            {
                switch (args[0].ToLower())
                {
                    case "generate":
                        GenerateKeys();
                        break;
                    case "sign":
                        Sign(args);
                        break;
                    case "verify":
                        Verify(args);
                        break;
                    default:
                        Usage();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error parsing command: ");
                Console.Error.WriteLine(e.StackTrace);
                Environment.Exit(-1);
            }
        }

        private static void GenerateKeys()
        {
            if (File.Exists(SignatureVerifier.DefaultDSAPrivKeyFileName) || File.Exists(SignatureVerifier.DefaultDSAPubKeyFileName))
            {
                ShowErrorMessage("\tError: Output files already exists!");
            }

            var prv = new DSACryptoServiceProvider();
            using (var sw = new StreamWriter(SignatureVerifier.DefaultDSAPrivKeyFileName))
            {
                sw.Write(prv.ToXmlString(true));
            }

            using (var sw = new StreamWriter(SignatureVerifier.DefaultDSAPubKeyFileName))
            {
                sw.Write(prv.ToXmlString(false));
            }

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\tFinished generating keys");
            Console.ResetColor();
        }

        private static void Sign(IList<string> args)
        {
            if (args.Count != 3)
            {
                Usage();
                Environment.Exit(-1);
            }

            // get parameter
            var artifact = args[1];
            var privKey = args[2];

            if (!File.Exists(artifact))
            {
                ShowErrorMessage(string.Format("\tArtifact {0} doesn't exist", artifact));
            }

            if (!File.Exists(privKey))
            {
                ShowErrorMessage(string.Format("\tPrivate key {0} doesn't exist", privKey));
            }

            string key;
            using (var reader = new StreamReader(privKey))
            {
                key = reader.ReadToEnd();
            }

            var prv = new DSACryptoServiceProvider();
            prv.FromXmlString(key);

            byte[] hash;
            using (Stream inputStream = File.OpenRead(artifact))
            {
                hash = prv.SignData(inputStream);
            }

            var base64Hash = Convert.ToBase64String(hash);
            Console.WriteLine(base64Hash);
        }

        private static void Verify(IList<string> args)
        {
            if (args.Count != 4)
            {
                Usage();
                Environment.Exit(-1);
            }

            // get parameter
            var artifact = args[1];
            var pubKey = args[2];
            var sign = args[3];

            var dsaVerif = new SignatureVerifier(pubKey);
            if (dsaVerif.VerifyDSASignature(sign, artifact))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\tVerified! Artifact's signature is valid!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("\tUnverified: Artifact signature is not valid.");
            }
            Console.ResetColor();
        }

        private static void Usage()
        {
            ShowHeadLine();
            const string cmd = "MagpieVerifier.exe";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Usage:");
            Console.ResetColor();
            Console.Write("\n\t {0}", cmd);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" COMMAND");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  Commands:\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\tgenerate");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\t  e.g. {0} generate", cmd);
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("\t  Generates a public and a private DSA key pair which is stored in the");
            Console.WriteLine("\t  current working directory. The private key will be stored as {0}.", SignatureVerifier.DefaultDSAPrivKeyFileName);
            Console.WriteLine("\t  The public key will be saved as {0}.", SignatureVerifier.DefaultDSAPubKeyFileName);
            Console.WriteLine("\t  Add this public key file as a resource to your application.");
            Console.WriteLine();


            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\tsign <artifact> <private_key_file>");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\t  e.g. {0} sign installer.msi {1}", cmd, SignatureVerifier.DefaultDSAPrivKeyFileName);
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("\t  Signs the given artifact using the given private key.");
            Console.WriteLine("\t  The output is a signature for this artifact that should be added to your");
            Console.WriteLine("\t  appcast.json file.");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\tverify <artifact> <private_key_file> <signature>");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\t  e.g. {0} verify installer.msi {1} MG9289E047A09383...==", cmd, SignatureVerifier.DefaultDSAPrivKeyFileName);
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("\t  Verifies the given artifact using the given private key and a valid signature.");
            Console.WriteLine("\t  The signature is a string created using 'sign' command above.");
            Console.WriteLine();
            Console.ResetColor();
        }

        private static void ShowHeadLine()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("\tMagpie Signature Verifier");
            Console.WriteLine("\t(c) 2015-2016 Ashok Gelal under the terms of MIT license");
            Console.WriteLine();
            Console.WriteLine("\tThis signature verifier is based on:");
            Console.WriteLine("\tNetSparkle DSA Helper");
            Console.WriteLine("\t(c) 2011 Dirk Eisenberg under the terms of MIT license");
            Console.ResetColor();
        }

        private static void ShowErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
            Environment.Exit(-1);
        }
    }
}
