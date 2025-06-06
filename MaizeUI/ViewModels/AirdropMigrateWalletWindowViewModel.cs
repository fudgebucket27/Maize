﻿using Maize;
using Maize.Services;
using Maize.Helpers;
using Maize.Models.ApplicationSpecific;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Text;
using Maize.Models;
using Maize.Models.Responses;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MaizeUI.ViewModels
{
    public class AirdropMigrateWalletWindowViewModel : ViewModelBase 
    {
        List<List<Datum>> allNfts = new List<List<Datum>>();
        bool isWalletActivated = true;
        private bool isNextButtonVisible;
        private bool isStartButtonVisible;
        private bool isPreviewButtonVisible;
        private bool isFeeInfoVisible;
        private bool step1;
        private bool step2;
        private bool step3;
        public string newWallet;
        public string NewWallet
        {
            get => newWallet;
            set => this.RaiseAndSetIfChanged(ref newWallet, value?.Trim());
        }
        public bool Step1
        {
            get => step1;
            set => this.RaiseAndSetIfChanged(ref step1, value);
        }
        public bool Step2
        {
            get => step2;
            set => this.RaiseAndSetIfChanged(ref step2, value);
        }
        public bool Step3
        {
            get => step3;
            set => this.RaiseAndSetIfChanged(ref step3, value);
        }
        public bool IsPreviewButtonVisible
        {
            get => isPreviewButtonVisible;
            set => this.RaiseAndSetIfChanged(ref isPreviewButtonVisible, value);
        }
        public bool IsFeeInfoVisible
        {
            get => isFeeInfoVisible;
            set => this.RaiseAndSetIfChanged(ref isFeeInfoVisible, value);
        }
        public bool IsNextButtonVisible
        {
            get => isNextButtonVisible;
            set => this.RaiseAndSetIfChanged(ref isNextButtonVisible, value);
        }
        public bool IsStartButtonVisible
        {
            get => isStartButtonVisible;
            set => this.RaiseAndSetIfChanged(ref isStartButtonVisible, value);
        }
        private bool isCheckboxVisible;

        public bool IsCheckboxVisible
        {
            get => isCheckboxVisible;
            set => this.RaiseAndSetIfChanged(ref isCheckboxVisible, value);
        }
        private bool isCheckboxVisibleStep1;

        public bool IsCheckboxVisibleStep1
        {
            get => isCheckboxVisibleStep1;
            set => this.RaiseAndSetIfChanged(ref isCheckboxVisibleStep1, value);
        }

        private bool nftIsChecked;
        public bool NftIsChecked
        {
            get => nftIsChecked;
            set => this.RaiseAndSetIfChanged(ref nftIsChecked, value);
        }
        private bool _isChecked;
        private bool isEnabledCheckBox = true;

        public bool IsChecked
        {
            get => _isChecked;
            set => this.RaiseAndSetIfChanged(ref _isChecked, value);
        }        
        public bool IsEnabledCheckBox
        {
            get => isEnabledCheckBox;
            set => this.RaiseAndSetIfChanged(ref isEnabledCheckBox, value);
        }
        public string Notice { get; set; }
        public string Notice2 { get; set; }
        public string Notice3 { get; set; }
        public string Notice4 { get; set; }
        public string Notice5 { get; set; }
        public string Notice6 { get; set; }
        public bool isEnabled = true; 
        public bool isComboBoxEnabled = true; 
        public bool notice5Visible = false; 
        public bool notice6Visible = false; 
        public bool IsComboBoxEnabled
        {
            get => isComboBoxEnabled;
            set => this.RaiseAndSetIfChanged(ref isComboBoxEnabled, value);
        }        
        public bool Notice5Visible
        {
            get => notice5Visible;
            set => this.RaiseAndSetIfChanged(ref notice5Visible, value);
        }
        public bool Notice6Visible
        {
            get => notice6Visible;
            set => this.RaiseAndSetIfChanged(ref notice6Visible, value);
        }
        public bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }

        public string log;

        public string Log
        {
            get => log;
            set => this.RaiseAndSetIfChanged(ref log, value);
        }

        public LoopringServiceUI loopringService;

        public LoopringServiceUI LoopringService
        {
            get => loopringService;
            set => this.RaiseAndSetIfChanged(ref loopringService, value);
        }

        public Settings settings;

        public Settings Settings
        {
            get => settings;
            set => this.RaiseAndSetIfChanged(ref settings, value);
        }
        public Constants.Environment environment;

        public Constants.Environment Environment
        {
            get => environment;
            set => this.RaiseAndSetIfChanged(ref environment, value);
        }
        public List<string> LoopringFeeDropdown { get; } 
        public List<string> MaizeFeeDropdown { get; } 
        public string LoopringFeeSelectedOption { get; set; } 
        public string MaizeFeeSelectedOption { get; set; } 
        public ReactiveCommand<Unit, Unit> NftWalletSwapCommand { get; }
        public ReactiveCommand<Unit, Unit> StartProcessCommand { get; }
        public ReactiveCommand<Unit, Unit> PreviewProcessCommand { get; }

        public AirdropMigrateWalletWindowViewModel()
        {
            IsChecked = false;
            NftIsChecked = false;
            IsComboBoxEnabled = true;
            Step1 = true;
            Step2 = false;
            Step3 = false;
            LoopringFeeDropdown = new List<string> { "Loopring Fee", "ETH", "LRC" };
            MaizeFeeDropdown = new List<string> { "Maize Fee", "ETH", "LRC", "PEPE" };
            LoopringFeeSelectedOption = LoopringFeeDropdown[0];
            MaizeFeeSelectedOption = MaizeFeeDropdown[0];
            IsNextButtonVisible = true;
            IsFeeInfoVisible = false;
            Notice5Visible = false;
            IsStartButtonVisible = false;
            IsCheckboxVisible = false;
            IsCheckboxVisibleStep1 = true;
            IsPreviewButtonVisible = false;
            Log = $"Enter the ENS or wallet address in the above box to transfer all of your NFTs to the desired wallet on Loopring Layer 2.";
            Notice = "Transfer all your NFTs to another Wallet on Loopring Layer 2";
            Notice2 = "👈 Choose";
            Notice3 = "Press Preview to see a summary of your NFT Wallet Swap";
            Notice4 = "Press Start to begin your NFT Wallet Swap";
            Notice5 = "☝️ insufficient funds";
            Notice6 = "NFT Wallet Swap completed! Exit to start a new one.";
            NftWalletSwapCommand = ReactiveCommand.Create(NftWalletSwap);
            StartProcessCommand = ReactiveCommand.Create(StartProcess);
            PreviewProcessCommand = ReactiveCommand.Create(PreviewProcess);

        }
        private async void ViewPreview()
        {
            Step3 = false;
            IsComboBoxEnabled = false;
            Step1 = false;
            Step2 = true;
            IsNextButtonVisible = false;
            IsPreviewButtonVisible = true;
            IsStartButtonVisible = false;
        }
        private async void ViewStart()
        {
            IsComboBoxEnabled = false;
            Notice5Visible = false;
            Step1 = false;
            Step2 = false;
            Step3 = true;
            IsNextButtonVisible = false;
            IsPreviewButtonVisible = false;
            IsStartButtonVisible = true;
        }
        private async void StartProcess()
        {
            //PreviewProcess();
            var sw = new Stopwatch();
            sw.Start();
            IsEnabled = false;
            NftTransferAuditInformation auditInfo = new NftTransferAuditInformation();
            auditInfo.validAddress = new List<string>();
            auditInfo.invalidAddress = new List<string>();
            auditInfo.banishAddress = new List<string>();
            auditInfo.invalidNftData = new List<string>();
            auditInfo.alreadyActivatedAddress = new List<string>();
            auditInfo.gasFeeTotal = 0;
            auditInfo.transactionFeeTotal = 0;
            auditInfo.nftSentTotal = 0;
            int maxFeeTokenId = ("ETH" == LoopringFeeSelectedOption) ? 0 : 1;
            CounterFactualInfo isCounterFactual = new();
            if (settings.MMorGMEPrivateKey == "")
                isCounterFactual = await LoopringService.GetCounterFactualInfo(settings.LoopringAccountId);
            foreach (var item in allNfts.SelectMany(d=>d))
            {
                if (IsChecked == true && allNfts.SelectMany(d => d).ToList().IndexOf(item) == 0)
                {
                    var apiSw = Stopwatch.StartNew();
                    Log = $"Transfering NFT with activation {allNfts.SelectMany(d => d).ToList().IndexOf(item) + 1}/{allNfts.SelectMany(d => d).ToList().Count()} ";
                    // nft transfer
                    var activateAuditInfo = await LoopringService.NftTransfer(
                        LoopringService,
                        settings.Environment,
                        Environment.Url,
                        Environment.Exchange,
                        settings.LoopringApiKey,
                        settings.LoopringPrivateKey,
                        settings.MMorGMEPrivateKey,
                        settings.LoopringAccountId,
                        0,
                        LoopringFeeSelectedOption,
                        maxFeeTokenId,
                        settings.LoopringAddress,
                        Constants.InputFile,
                        Constants.InputFolder,
                        0,
                        item.tokenId,
                        item.total.ToString(),
                        settings.ValidUntil,
                        Constants.LcrTransactionFee,
                        "NFT Wallet Swap via Maize with Activation",
                        item.nftData,
                        newWallet,
                        true,
                        isCounterFactual
                        );
                        auditInfo.validAddress.AddRange(activateAuditInfo.validAddress);
                        auditInfo.invalidAddress.AddRange(activateAuditInfo.invalidAddress);
                        auditInfo.banishAddress.AddRange(activateAuditInfo.banishAddress);

                        if (auditInfo.invalidNftData != null)
                        {
                            auditInfo.invalidNftData.AddRange(activateAuditInfo.invalidNftData);
                        }

                        if (auditInfo.alreadyActivatedAddress != null)
                        {
                            auditInfo.alreadyActivatedAddress.AddRange(activateAuditInfo.alreadyActivatedAddress);
                        }

                        auditInfo.gasFeeTotal += activateAuditInfo.gasFeeTotal;
                        auditInfo.transactionFeeTotal += activateAuditInfo.transactionFeeTotal;
                        auditInfo.nftSentTotal += activateAuditInfo.nftSentTotal;
                        Timers.ApiStopWatchCheck(apiSw);
                }
                else
                {
                    var apiSw = Stopwatch.StartNew();
                    Log = $"Transfering NFT {allNfts.SelectMany(d => d).ToList().IndexOf(item) + 1}/{allNfts.SelectMany(d => d).ToList().Count()}";
                    // nft transfer
                    var newAuditInfo = await LoopringService.NftTransfer(
                        LoopringService,
                        settings.Environment,
                        Environment.Url,
                        Environment.Exchange,
                        settings.LoopringApiKey,
                        settings.LoopringPrivateKey,
                        settings.MMorGMEPrivateKey,
                        settings.LoopringAccountId,
                        0,
                        LoopringFeeSelectedOption,
                        maxFeeTokenId,
                        settings.LoopringAddress,
                        Constants.InputFile,
                        Constants.InputFolder,
                        0, //how many lines...doesnt matter old tech
                        item.tokenId,
                        item.total.ToString(),
                        settings.ValidUntil,
                        Constants.LcrTransactionFee,
                        "NFT Wallet Swap via Maize",
                        item.nftData,
                        newWallet,
                        false,
                        isCounterFactual
                        );
                    auditInfo.validAddress.AddRange(newAuditInfo.validAddress);
                    auditInfo.invalidAddress.AddRange(newAuditInfo.invalidAddress);
                    auditInfo.banishAddress.AddRange(newAuditInfo.banishAddress);

                    if (auditInfo.invalidNftData != null)
                    {
                        auditInfo.invalidNftData.AddRange(newAuditInfo.invalidNftData);
                    }

                    if (auditInfo.alreadyActivatedAddress != null)
                    {
                        auditInfo.alreadyActivatedAddress.AddRange(newAuditInfo.alreadyActivatedAddress);
                    }

                    auditInfo.gasFeeTotal += newAuditInfo.gasFeeTotal;
                    auditInfo.transactionFeeTotal += newAuditInfo.transactionFeeTotal;
                    auditInfo.nftSentTotal += newAuditInfo.nftSentTotal;
                    Timers.ApiStopWatchCheck(apiSw);
                }

            }
            
            maxFeeTokenId = ("ETH" == LoopringFeeSelectedOption) ? 0 : 1;
            var maizeMaxFeeTokenId = ("ETH" == MaizeFeeSelectedOption) ? 0 : 1;
            if (MaizeFeeSelectedOption == "PEPE")
                maizeMaxFeeTokenId = 272;

            var maizeFee = await CalculateMaizeFee(LoopringService, auditInfo.validAddress.Count(), MaizeFeeSelectedOption);

            //uncomment this asap
            var maxFeeVolume = await loopringService.CobTransferTransactionFee(
               settings.Environment,
               Environment.Url,
               Environment.Exchange,
               settings.LoopringApiKey,
               settings.LoopringPrivateKey,
               settings.MMorGMEPrivateKey,
               settings.LoopringAccountId,
               maizeFee,
               auditInfo.nftSentTotal,
               LoopringFeeSelectedOption,
               maxFeeTokenId,
               Environment.MyAccountAddress,
               settings.LoopringAddress,
               0,
               maizeMaxFeeTokenId,
               MaizeFeeSelectedOption,
               isCounterFactual
               );
            auditInfo.gasFeeTotal += maxFeeVolume;

            var fileName = ApplicationUtilitiesUI.ShowAirdropAudit(auditInfo.validAddress, auditInfo.invalidAddress, auditInfo.banishAddress, auditInfo.invalidNftData, auditInfo.alreadyActivatedAddress, null, auditInfo.gasFeeTotal, LoopringFeeSelectedOption, maizeFee, MaizeFeeSelectedOption);
            sw.Stop();
            var swTime = $"This took {(sw.ElapsedMilliseconds > (1 * 60 * 1000) ? Math.Round(Convert.ToDecimal(sw.ElapsedMilliseconds) / 1000m / 60, 3) : Convert.ToDecimal(sw.ElapsedMilliseconds) / 1000m)} {(sw.ElapsedMilliseconds > (1 * 60 * 1000) ? "minutes" : "seconds")} to complete.";
            Log = $"{swTime}\r\n\r\nYour audit file is here:\r\n\r\n{fileName}";
            Step3 = false;
            Notice6Visible = true;
        }
        private async void PreviewProcess()
        {
            if (LoopringFeeSelectedOption == LoopringFeeDropdown[0] || MaizeFeeSelectedOption == MaizeFeeDropdown[0])
            {
                Log = "Select Fees";
                return;
            }
            if (isWalletActivated == false && _isChecked == false)
            {
                Log = "Acknowledge activation to proceed.";
                return;
            }
            string totalTransactions = $"There will be a total of {allNfts.SelectMany(d => d).Count()} transactions with ";
            string totalNfts = $"{allNfts.SelectMany(d => d).Sum(x => int.Parse(x.total))} NFTs sent to ";
            string totalWallets = $"{newWallet} wallet.";
            int totalTransfersNumber;
            int totalActivationsNumber; 
            if (_isChecked == true)
            {
                totalTransfersNumber = allNfts.SelectMany(d => d).Count() - 1;
                totalActivationsNumber = 1;
            }
            else
            {
                totalTransfersNumber = allNfts.SelectMany(d => d).Count();
                totalActivationsNumber = 0;
            }
            string totalTransfers = $"Transfers: {totalTransfersNumber}";
            string totalActivations = $"Transfers with wallet activation: {totalActivationsNumber}";
            var transferFees = (decimal.Parse((await LoopringService.GetNftOffChainFee(settings.LoopringApiKey, settings.LoopringAccountId, 11)).fees.First(x => x.token == LoopringFeeSelectedOption).fee) / 1000000000000000000) * totalTransfersNumber;
            var activationFees = (decimal.Parse((await LoopringService.GetNftOffChainFee(settings.LoopringApiKey, settings.LoopringAccountId, 19)).fees.First(x => x.token == LoopringFeeSelectedOption).fee) / 1000000000000000000) * totalActivationsNumber;
            var maizeFee = await CalculateMaizeFee(LoopringService, allNfts.SelectMany(d => d).Count(), MaizeFeeSelectedOption); 

            var userAssets = await LoopringService.GetUserAssetsForFees(settings.LoopringApiKey, settings.LoopringAccountId);
            var eth = userAssets.FirstOrDefault(asset => asset.tokenId == 0);
            var lrc = userAssets.FirstOrDefault(asset => asset.tokenId == 1);
            var pepe = userAssets.FirstOrDefault(asset => asset.tokenId == 272);
            decimal userLrc = 0;
            decimal userEth = 0;
            decimal userPepe = 0;
            if (lrc != null)
                userLrc = decimal.Parse(lrc.total) / 1000000000000000000;
            if (eth != null)
                userEth = decimal.Parse(eth.total) / 1000000000000000000;
            if (pepe != null)
                userPepe = decimal.Parse(pepe.total) / 1000000000000000000;

            if (_isChecked == true)
                Log = $"Your Assets:\r\n{userEth} ETH | {userLrc} LRC | {userPepe} PEPE\r\n\r\n{totalTransactions}{totalNfts}{totalWallets}\r\n\r\n{totalTransfers}\r\n{totalActivations}\r\n\r\nTransfer Fees: {transferFees} {LoopringFeeSelectedOption}\r\nActivation Fees: {activationFees} {LoopringFeeSelectedOption}\r\nMaize Fee: {maizeFee} {MaizeFeeSelectedOption}";
            else
                Log = $"Your Assets:\r\n{userEth} ETH | {userLrc} LRC | {userPepe} PEPE\r\n\r\n{totalTransactions}{totalNfts}{totalWallets}\r\n\r\n{totalTransfers}\r\nTransfer Fees: {transferFees} {LoopringFeeSelectedOption}\r\nMaize Fee: {maizeFee} {MaizeFeeSelectedOption}";
            switch (MaizeFeeSelectedOption)
            {
                case "ETH":
                    if (LoopringFeeSelectedOption == "ETH")
                    {
                        if (userEth < (transferFees + activationFees + maizeFee))
                        {
                            IsComboBoxEnabled = true;
                            Notice5Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        if ((userEth < maizeFee) || transferFees + activationFees > userLrc)
                        {
                            IsComboBoxEnabled = true;
                            Notice5Visible = true;
                            return;
                        }
                    }
                    break;

                case "LRC":
                    if (LoopringFeeSelectedOption == "LRC")
                    {
                        if (IsChecked == true)
                        {
                            if (userLrc < (transferFees + activationFees + maizeFee))
                            {
                                IsComboBoxEnabled = true;
                                Notice5Visible = true;
                                return;
                            }
                        }
                        else
                        {
                            if (userLrc < (transferFees + maizeFee))
                            {
                                IsComboBoxEnabled = true;
                                Notice5Visible = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (userLrc < maizeFee || transferFees + activationFees > userEth)
                        {
                            IsComboBoxEnabled = true;
                            Notice5Visible = true;
                            return;
                        }
                    }
                    break;

                case "PEPE":
                    if (LoopringFeeSelectedOption == "ETH")
                    {
                        if (userPepe <  maizeFee || transferFees + activationFees > userEth)
                        {
                            IsComboBoxEnabled = true;
                            Notice5Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        if (userPepe < maizeFee || transferFees + activationFees > userLrc)
                        {
                            IsComboBoxEnabled = true;
                            Notice5Visible = true;
                            return;
                        }
                    }
                    break;
            }
            IsEnabledCheckBox = false;
            ViewStart();
        }
        public static async Task<decimal> CurrentTokenPriceInUsd(ILoopringService LoopringService, string maizeFee)
        {
            var culture = new CultureInfo("en-US");
            var varHexAddress = await LoopringService.GetTokens();
            var currentFeePrice = (await LoopringService.GetTokenPrice()).data.FirstOrDefault(x =>x.token == varHexAddress.FirstOrDefault(x=>x.symbol == maizeFee).address).price;
            var convertToOneUsd = 1 / decimal.Parse(currentFeePrice, culture);
            
            return convertToOneUsd;
        }
        public static async Task<decimal> CalculateMaizeFee(ILoopringService LoopringService, decimal totalTransactions, string MaizeFeeSelectedOption)
        {
            return Math.Round((await CurrentTokenPriceInUsd(LoopringService, MaizeFeeSelectedOption)) * (totalTransactions * Constants.LcrTransactionFee), 14);
        }
        private async void NftWalletSwap()
        {
            if (LoopringFeeSelectedOption == LoopringFeeDropdown[0] || MaizeFeeSelectedOption == MaizeFeeDropdown[0] || newWallet == null)
            {
                if (newWallet == null)
                    Log = $"Please enter in an ENS or wallet address...";
                if (LoopringFeeSelectedOption == LoopringFeeDropdown[0] || MaizeFeeSelectedOption == MaizeFeeDropdown[0])
                {
                    IsCheckboxVisible = false;
                    IsFeeInfoVisible = true;
                }
                else
                    IsFeeInfoVisible = false;
                return;
            }
            IsFeeInfoVisible = false;

            //Log = "Checking Input file please give me a moment...";
            IsEnabled = false;
            var sw = new Stopwatch();
            sw.Start();
            NftOffChainFeeResponse activationFees = new();
            IEthereumService ethereumService = new EthereumService();
            INftMetadataService nftMetadataService = new NftMetadataService("https://ipfs.loopring.io/ipfs/");
            //StringBuilder buildinvalidLines = new StringBuilder();
            //StringBuilder buildAttentionLines = new StringBuilder();
            //(transferInfoList, invalidLines, transferInfoCryptoList) = await Files.CheckInputFileVariables(4);
            //if (invalidLines.Contains("Error"))
            //{
            //    Log = $"{invalidLines}\r\nPlease fix the above lines in your Input file and then press Next.";
            //    IsEnabled = true;
            //    return;
            //}
                
            //else
            //    Log = "Input file is formatted correctly!";
            int offset = 0;
            int total = 0;
            while (true)
            {
                var nfts = await LoopringService.GetWalletsNftsOffset(settings.LoopringApiKey, settings.LoopringAccountId, offset);
                if (nfts.Item1.Count > 0)
                {
                    total = nfts.Item2;
                    Log = $"Gathering your NFTs: {allNfts.SelectMany(d => d).Count()}/{total} NFTs retrieved...";
                    allNfts.Add(nfts.Item1);
                    offset += 50;
                }
                else
                {
                    break;
                }
            }
            Log = $"{allNfts.SelectMany(d=>d).Count()} unique NFTs retrieved...";
            //Log = $"Checking data for for issues... ";
            //int validCounter = 0;
            //foreach (var item in transferInfoList.DistinctBy(x => x.NftData))
            //{
            //    Log = $"Checking NFTs: {++validCounter}/{transferInfoList.DistinctBy(x => x.NftData).Count()}";
            //    var nftDataCheck = await LoopringService.GetNftInformationFromNftData(Settings.LoopringApiKey, item.NftData);
            //    if (nftDataCheck.Count == 0)
            //    {
            //        //buildinvalidLines.Append($"Error with NFT Data: Invalid NFT Data.{item.NftData}\r\n");
            //    }
            //    if (allNfts.SelectMany(d => d).Where(x => x.nftData == item.NftData).Count() > 0)
            //    {
            //        if (int.Parse(allNfts.SelectMany(d => d).Where(x => x.nftData == item.NftData).First().total) == 0)
            //        {
            //            //buildinvalidLines.Append($"Error at line {transferInfoList.IndexOf(item) + 1}: You do not own this NFT.\r\n");
            //        }
            //        else
            //        {
            //            //if (int.Parse(allNfts.SelectMany(d => d).Where(x => x.nftData == item.NftData).First().total) < transferInfoList.Where(x => x.NftData == item.NftData).Sum(x => x.Amount))
            //                //buildinvalidLines.Append($"Error at line {transferInfoList.IndexOf(item) + 1}: You own {allNfts.SelectMany(d => d).Where(x => x.nftData == item.NftData).First().total} and are trying to transfer {transferInfoList.Where(x => x.NftData == item.NftData).Sum(x => x.Amount)}.\r\n");
            //        }
            //    }
            //    //else if (allNfts.SelectMany(d => d).Where(x => x.nftData == item.NftData).Count() == 0)
            //        //buildinvalidLines.Append($"Error at line {transferInfoList.IndexOf(item) + 1}: You do not own this NFT.\r\n");
            //}
            //validCounter = 0;
            //foreach (var item in transferInfoList.DistinctBy(x=>x.Memo))
            //{
            //    Log = $"Checking Memos: {++validCounter}/{transferInfoList.DistinctBy(x => x.Memo).Count()}";
            //    if (item.Memo?.Length > 120)
            //    {
            //        //buildinvalidLines.Append($"Error with Memo: Length greater than 120 characters. {item.Memo}\r\n");
            //    }
            //}
            //validCounter = 0;
            var walletAddressCheck = await LoopringService.GetUserAccountInformationFromOwner(await CheckForEthAddress(LoopringService, settings.LoopringApiKey, newWallet));
            if (walletAddressCheck == null || (walletAddressCheck.tags != "FirstUpdateAccountPaid" && walletAddressCheck.nonce == 0))
            {
                isWalletActivated = false;
                Log = $"This wallet does not have an active Loopring account. Please check that its a vaild address. \r\n\r\nCheck the below box to pay its activation fee. \r\n\r\nThe first NFT transfer will pay the activation fee with the remaining to transfer as normal. ";
                IsCheckboxVisible = true;
            }
            //foreach (var item in transferInfoList.DistinctBy(x => x.ToAddress))
            //{
            //    //Log = $"Checking Wallets: {++validCounter}/{transferInfoList.DistinctBy(x => x.ToAddress).Count()}";
            //    var walletAddressCheck = await LoopringService.GetUserAccountInformationFromOwner(await CheckForEthAddress(LoopringService, settings.LoopringApiKey, item.ToAddress));
            //    if (walletAddressCheck == null || (walletAddressCheck.tags != "FirstUpdateAccountPaid" && walletAddressCheck.nonce == 0))
            //    {
            //        //buildAttentionLines.Append($"Attention at line {transferInfoList.IndexOf(item) + 1}: {item.ToAddress} Loopring account is not active.\r\n");
            //        item.Activated = false;
            //    }
            //}
            //if (buildinvalidLines.ToString().Contains("Error"))
            //{
            //    Log = $"{buildinvalidLines}\r\nPlease fix the above errors in your Input file and then press Next.";
            //    IsEnabled = true;
            //    return;
            //}
            //else if (buildAttentionLines.ToString().Contains("Attention"))
            //{
            //    Log = $"Your data is valid!\r\n\r\nThere are wallets without an active Loopring account. Check the below box to include them and pay their activation fee.";
            //    IsCheckboxVisible = true;
            //}
            //else
            //    Log = $"Your data is valid!";

            activationFees = await LoopringService.GetNftOffChainFee(settings.LoopringApiKey, settings.LoopringAccountId, 19);
            
            //if (buildAttentionLines.ToString().Contains("Attention") && _isChecked == true)                
            //    Log = $"{buildAttentionLines}\r\nYou will pay for the above {Regex.Matches(buildAttentionLines.ToString(), "\\b" + Regex.Escape("Attention") + "\\b", RegexOptions.IgnoreCase).Count} wallets activation fees. Total costs in LRC or ETH below.\r\n\r\nLRC: {(decimal.Parse(activationFees.fees.First(x => x.token == "LRC").fee)/1000000000000000000) * Regex.Matches(buildAttentionLines.ToString(), "\\b" + Regex.Escape("Attention") + "\\b", RegexOptions.IgnoreCase).Count}\r\nETH: {(decimal.Parse(activationFees.fees.First(x => x.token == "ETH").fee) / 1000000000000000000) * Regex.Matches(buildAttentionLines.ToString(), "\\b" + Regex.Escape("Attention") + "\\b", RegexOptions.IgnoreCase).Count}";
            ViewPreview();

            IsEnabled = true;
        }

        public static async Task<string> CheckForEthAddress(ILoopringService LoopringService, string apiKey, string address)
        {
            address = address.Trim().ToLower();
            if (address.Contains(".eth"))
            {
                var varHexAddress = await LoopringService.GetHexAddress(apiKey, address);
                if (!String.IsNullOrEmpty(varHexAddress.data))
                    return varHexAddress.data;
                else
                    return null;
            }
            return address;
        }
    }
}
