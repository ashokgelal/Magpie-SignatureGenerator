# Magpie DSA Signature Generator
Signature generator is a console application for [Magpie](https://github.com/ashokgelal/Magpie) that can be used to generate DSA key pairs, sign an artifact and verify an artifact all for the sake of secure updating your applications.

**Usage:**

    MagpieVerifier.exe COMMAND

**Commands:**

```shell        
	 generate          e.g. MagpieVerifier.exe generate          Generates a public and a private DSA key pair which is stored in the          current working directory. The private key will be stored as MagpieDSA.priv.          The public key will be saved as MagpieDSA.pub.          Add this public key file as a resource to your application.     sign <artifact> <private_key_file>          e.g. MagpieVerifier.exe sign installer.msi MagpieDSA.priv          Signs the given artifact using the given private key.          The output is a signature for this artifact that should be added to your          appcast.json file.     verify <artifact> <private_key_file> <signature>          e.g. MagpieVerifier.exe verify installer.msi MagpieDSA.priv MG9289E047A09383...==          Verifies the given artifact using the given private key and a valid signature.          The signature is a string created using 'sign' command above.
```

#### .Net Requirement:
This project can be compiled using .net 4.0 client profile.

#### Screenshot:

![](https://raw.githubusercontent.com/ashokgelal/Magpie-SignatureGenerator/master/screenshots/lp_download_screenshot.png)
