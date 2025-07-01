using Maize;
using Newtonsoft.Json;
using ReactiveUI;
using System.Reactive;
using System.Text;
using Maize.Services;
using Maize.Models.Responses;
using System.Security.Cryptography;
using LoopringSmartWalletRecoveryPhraseExtractor;
using Nethereum.HdWallet;
using MaizeUI.Helpers;
using NBitcoin;
using System.Numerics;

namespace MaizeUI.ViewModels
{
    public class AppsettingsNoticeWindowViewModel : ViewModelBase
    {
        private readonly AccountService _accountService;
        WalletTypeResponse walletType;
        CounterFactualInfo? isCounterFactual;
        private string _watermark;
        public string Watermark
        {
            get => _watermark;
            set => this.RaiseAndSetIfChanged(ref _watermark, value);
        }
        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }
        public string notice;
        public string location;
        public string eoal1Key;
        public string loopringAppPassCode;
        public LoopringServiceUI loopringService;

        public LoopringServiceUI LoopringService
        {
            get => loopringService;
            set => this.RaiseAndSetIfChanged(ref loopringService, value);
        }
        private string imagePath;
        public string ImagePath
        {
            get => imagePath;
            set => this.RaiseAndSetIfChanged(ref imagePath, value);
        }
        public string Location
        {
            get => location;
            set => this.RaiseAndSetIfChanged(ref location, value);
        }
        public string Notice
        {
            get => notice;
            set => this.RaiseAndSetIfChanged(ref notice, value);
        }
        private bool isEoaTextBoxVisible;
        public bool IsEoaTextBoxVisible
        {
            get => isEoaTextBoxVisible;
            set => this.RaiseAndSetIfChanged(ref isEoaTextBoxVisible, value);
        }
        private bool isLswTextBoxVisible;
        public bool IsLswTextBoxVisible
        {
            get => isLswTextBoxVisible;
            set => this.RaiseAndSetIfChanged(ref isLswTextBoxVisible, value);
        }
        public string EoaL1Key
        {
            get => eoal1Key;
            set => this.RaiseAndSetIfChanged(ref eoal1Key, value);
        }
        public string LoopringAppPassCode
        {
            get => loopringAppPassCode;
            set => this.RaiseAndSetIfChanged(ref loopringAppPassCode, value);
        }
        public string log;
        public string Log
        {
            get => log;
            set
            {
                if (log != value)
                {
                    this.RaiseAndSetIfChanged(ref log, value);
                    UpdateTextBoxVisibilityAsync();
                }
            }
        }
        public bool isEnabled = true;
        public bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }


        private int _inputMode;


        public int InputMode
        {
            get => _inputMode;
            set => this.RaiseAndSetIfChanged(ref _inputMode, value);
        }

        public bool IsSeedPhraseMode => InputMode == 0; // 0 = Seed Phrase
        public bool IsQrCodeMode => InputMode == 1;     // 1 = QR Code and App Passcode

        public ReactiveCommand<Unit, Unit> SetupApsettingsFileCommand { get; }

        public AppsettingsNoticeWindowViewModel(string notice, string location, LoopringServiceUI loopringService, AccountService accountService)
        {
            _accountService = accountService;
            Notice = notice;
            Location = location;
            LoopringService = loopringService;
            Maize.Helpers.Things.OpenUrl("https://loopring.io/#/pro");
            Watermark = $"Paste Account Information from Loopring.io > Avatar > Security > Export Account here and then the L1 Private Key/Image below if needed.\r\n\r\n Example:\r\n{{\r\n    \"address\": \"0x1fdfef87d387e4basdjfhtyghtugh19cff06a982\",\r\n    \"accountId\": 11233,\r\n    \"level\": \"\",\r\n    \"nonce\": 1,\r\n    \"apiKey\": \"miWgX3jDo5zubs1VwYrPShtF5ythgnbhggmczTOwzUrS280AaNtf6v8CuVmwfP4f\",\r\n    \"publicX\": \"0x12167dbhguty675ud3c11bae8a343c138cfc2574349235688ae2d6ce68320ac8\",\r\n    \"publicY\": \"0x1d7e0c7d92b894dc27943a0fghtyghvnfjb0db0dbcc47d42f2914d9b00b84fd3\",\r\n    \"privateKey\": \"0x2ad857be54b8d02badc842ac54e25f5ythgjt0pol50331cc4894509c09f255b\"\r\n}}\r\n";
            IsLswTextBoxVisible = false;
            IsEoaTextBoxVisible = false;
            SetupApsettingsFileCommand = ReactiveCommand.Create(SetupApsettingsFile);

            // React to InputMode changes to update visibility
            this.WhenAnyValue(x => x.InputMode)
                .Subscribe(_ =>
                {
                    this.RaisePropertyChanged(nameof(IsSeedPhraseMode));
                    this.RaisePropertyChanged(nameof(IsQrCodeMode));
                });
        }
        public event Action<string> OnSettingsFileSaved;
        public event Action RequestClose;

        private async void SetupApsettingsFile()
        {
            IsEnabled = false;
            RootObject settings = _accountService.SetupL2(log, location);
            if (string.IsNullOrEmpty(_password))
            {
                Notice = "Please enter in your password";
                IsEnabled = true;
                return;
            }
            if (isCounterFactual == null && walletType.data.isInCounterFactualStatus == false && walletType.data.isContract == false) //EOA
            {
                if (string.IsNullOrEmpty(eoal1Key.Trim()))
                {
                    IsEnabled = true;
                    return;
                }
                var layerOneKey = eoal1Key.Trim();
                settings.Settings.MMorGMEPrivateKey = layerOneKey;
            }
            else if ((isCounterFactual == null || isCounterFactual != null && isCounterFactual.accountId != 0) && walletType.data.isInCounterFactualStatus == false && walletType.data.isContract == true && !string.IsNullOrEmpty(eoal1Key.Trim())) //Typical LSW with layer 1 activated
            {
                if (IsValidBip39Mnemonic(eoal1Key.Trim()))
                {
                    Wallet wallet = new Wallet(eoal1Key.Trim(), null);
                    string walletPrivateKey = BitConverter.ToString(wallet.GetPrivateKey(0)).Replace("-", string.Empty).ToLower();
                    settings.Settings.MMorGMEPrivateKey = walletPrivateKey;
                }
                else if (IsValidEthereumPrivateKey(eoal1Key.Trim()))
                {
                    settings.Settings.MMorGMEPrivateKey = eoal1Key.Trim();
                }
                else
                {
                    Notice = "Please enter a valid seed phrase or private key!";
                    return;
                }
            }
            else if (isCounterFactual != null && walletType.data.isInCounterFactualStatus == true && walletType.data.isContract == true) //Counterfactual
            {
                settings.Settings.MMorGMEPrivateKey = "";
            }
            else if (isCounterFactual != null && isCounterFactual.accountId != 0 && walletType.data.isContract == false) //another counterfactual?
            {
                settings.Settings.MMorGMEPrivateKey = "";
            }
            else
            {
                //if (string.IsNullOrEmpty(loopringAppPassCode) || string.IsNullOrEmpty(imagePath))
                //{
                //    IsEnabled = true;
                //    return;
                //}

                //QrCodeJson qrCodeJson = null;

                //// Read the image from the file
                //using Mat image = Cv2.ImRead(imagePath);

                //// Initialize the QR code detector
                //QRCodeDetector qrCodeDetector = new QRCodeDetector();

                //// Detect the QR code
                //string decodedInfo = qrCodeDetector.DetectAndDecode(image, out Point2f[] points);

                //// Check if the QR code has been detected
                //if (points != null && points.Length > 0)
                //{
                //    // Draw a polygon around the detected QR code
                //    Cv2.Polylines(image, new OpenCvSharp.Point[][] { points.Select(p => p.ToPoint()).ToArray() }, isClosed: true, color: OpenCvSharp.Scalar.Red);

                //    // Display the detected and decoded information
                //    Notice = "QR Code Detected: " + decodedInfo;
                //    if (decodedInfo.Trim().Length > 0)
                //    {
                //        qrCodeJson = JsonConvert.DeserializeObject<QrCodeJson>(decodedInfo);
                //    }
                //}
                //else
                //{
                //    Notice = "QR Code Not Detected. Try a different image...";
                //    IsEnabled = true;
                //    return;
                //}

                //byte[] mnemonic = Encoding.ASCII.GetBytes(qrCodeJson.mnemonic);
                //byte[] iv = Encoding.ASCII.GetBytes(qrCodeJson.iv);
                //byte[] salt = Encoding.ASCII.GetBytes(qrCodeJson.salt);

                //var layerOneKey = QRCodeDecrypt(mnemonic, iv, salt, loopringAppPassCode);

                //if (layerOneKey == "You have entered the wrong passcode... try again!")
                //{
                //    Notice = layerOneKey;
                //    IsEnabled = true;
                //    return;
                //}
                //else
                //{
                //    settings.Settings.MMorGMEPrivateKey = layerOneKey;
                //}
            }

            await _accountService.CreateNewAccountAsync(settings, _password);


            RequestClose?.Invoke();
            OnSettingsFileSaved?.Invoke(settings.Settings.LoopringAddress);

        }
        
        private async void UpdateTextBoxVisibilityAsync()
        {
            RootObject settings = _accountService.SetupL2(log, location);
            if (settings != null)
            {
                IsEnabled = false;
                isCounterFactual = await LoopringService.GetCounterFactualInfo(settings.Settings.LoopringAccountId);
                walletType = await loopringService.GetWalletType(settings.Settings.LoopringAddress);
                try
                {
                    if (isCounterFactual == null && walletType.data.isInCounterFactualStatus == false && walletType.data.isContract == false)
                    {
                        IsEoaTextBoxVisible = true;
                        IsLswTextBoxVisible = false;
                        IsEnabled = true;
                    }
                    else if (isCounterFactual != null && isCounterFactual.accountId != 0 && walletType.data.isContract == false)
                    {
                        IsLswTextBoxVisible = false;
                        IsEoaTextBoxVisible = false;
                        settings.Settings.MMorGMEPrivateKey = "";
                        IsEnabled = true;
                    }
                    else
                    {
                        IsLswTextBoxVisible = true;
                        IsEoaTextBoxVisible = false;
                        IsEnabled = true;
                    }
                }
                catch (Exception e)
                {
                    // old LSW
                    IsLswTextBoxVisible = true;
                    IsEoaTextBoxVisible = false;
                    IsEnabled = true;

                }

            }
        }
        static string QRCodeDecrypt(byte[] mnemonic, byte[] iv, byte[] salt, string passphrase)
        {
            mnemonic = Convert.FromBase64String(Encoding.UTF8.GetString(mnemonic));
            iv = Convert.FromBase64String(Encoding.UTF8.GetString(iv));
            salt = Convert.FromBase64String(Encoding.UTF8.GetString(salt));

            string password = string.Format("0x{0}", BitConverter.ToString(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes("LOOPRING HEBAO Wallet " + passphrase))).Replace("-", "").ToLower());
            byte[] key = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), salt, 4096, HashAlgorithmName.SHA256).GetBytes(32);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(mnemonic, 0, mnemonic.Length);
                    int paddingLength = decryptedBytes[decryptedBytes.Length - 1];
                    try
                    {
                        byte[] result = new byte[decryptedBytes.Length - paddingLength];
                        Array.Copy(decryptedBytes, result, result.Length);
                        string decryptedMnemonic = Encoding.UTF8.GetString(result);
                        Wallet wallet = new Wallet(decryptedMnemonic, null);
                        string walletPrivateKey = BitConverter.ToString(wallet.GetPrivateKey(0)).Replace("-", string.Empty).ToLower();

                        return walletPrivateKey;

                    }
                    catch (Exception)
                    {
                        return "You have entered the wrong passcode... try again!";
                    }
                }
            }
        }

        private bool IsValidBip39Mnemonic(string input)
        {
            try
            {
                var mnemonic = new Mnemonic(input.Trim());
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static bool IsValidEthereumPrivateKey(string privateKey)
        {
            if (string.IsNullOrWhiteSpace(privateKey))
                return false;

            if (privateKey.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                privateKey = privateKey.Substring(2);

            if (privateKey.Length != 64)
                return false;

            foreach (char c in privateKey)
            {
                if (!IsHexDigit(c))
                    return false;
            }

            try
            {
                BigInteger key = BigInteger.Parse("0" + privateKey, System.Globalization.NumberStyles.HexNumber);

                BigInteger maxValue = BigInteger.Parse("0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", System.Globalization.NumberStyles.HexNumber);

                return key > 0 && key < maxValue;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsHexDigit(char c)
        {
            return (c >= '0' && c <= '9') ||
                   (c >= 'A' && c <= 'F') ||
                   (c >= 'a' && c <= 'f');
        }
    }
}
